using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Blis.Client
{
	public class HunterSerializer
	{
		private static readonly Dictionary<Type, FieldDeserializer[]> typeToClassDeserializers =
			new Dictionary<Type, FieldDeserializer[]>();


		private static readonly Dictionary<Type, FieldReaderWriter> typeToFieldReaderWriters =
			new Dictionary<Type, FieldReaderWriter>();


		static HunterSerializer()
		{
			AddToInitialElement(typeof(string), new FieldReaderWriterString());
			AddToInitialElement(typeof(decimal), new FieldReaderWriterDecimal());
			AddToInitialElement(typeof(double), new FieldReaderWriterDouble());
			AddToInitialElement(typeof(long), new FieldReaderWriterLong());
			AddToInitialElement(typeof(ulong), new FieldReaderWriterULong());
			AddToInitialElement(typeof(float), new FieldReaderWriterFloat());
			AddToInitialElement(typeof(int), new FieldReaderWriterInt());
			AddToInitialElement(typeof(uint), new FieldReaderWriterUInt());
			AddToInitialElement(typeof(short), new FieldReaderWriterShort());
			AddToInitialElement(typeof(ushort), new FieldReaderWriterUShort());
			AddToInitialElement(typeof(sbyte), new FieldReaderWriterSByte());
			AddToInitialElement(typeof(byte), new FieldReaderWriterByte());
			AddToInitialElement(typeof(bool), new FieldReaderWriterBool());
			AddToInitialElement(typeof(char), new FieldReaderWriterChar());
			AddToInitialElement(typeof(Enum), new FieldReaderWriterEnum());
			AddToInitialElement(typeof(Quaternion), new FieldReaderWriterQuaternion());
			AddToInitialElement(typeof(Vector4), new FieldReaderWriterVector4());
			AddToInitialElement(typeof(Vector3), new FieldReaderWriterVector3());
			AddToInitialElement(typeof(Vector2), new FieldReaderWriterVector2());
			AddToInitialElement(typeof(Color), new FieldReaderWriterColor());
			AddToInitialElement(typeof(Array), new FieldReaderWriterArray());
			AddToInitialElement(typeof(IList), new FieldReaderWriterIList());
		}

		private static FieldReaderWriter GetFieldReaderWriter(Type type)
		{
			FieldReaderWriter result;
			if (typeToFieldReaderWriters.TryGetValue(type, out result))
			{
				return result;
			}

			if (type.BaseType == typeof(Enum))
			{
				result = typeToFieldReaderWriters[typeof(Enum)];
			}
			else if (type.IsArray)
			{
				result = typeToFieldReaderWriters[typeof(Array)];
			}
			else
			{
				if (!(type is IList) || !type.GetType().IsGenericType)
				{
					throw new NotImplementedException(type + " is not support serialize");
				}

				result = typeToFieldReaderWriters[typeof(IList)];
			}

			return result;
		}


		public static void Serialize(MemoryStream memoryStream, object targetObject)
		{
			foreach (FieldInfo fieldInfo in targetObject.GetType()
				.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				GetFieldReaderWriter(fieldInfo.FieldType).Write(memoryStream, fieldInfo.GetValue(targetObject));
			}
		}


		public static object Deserialize(MemoryStream memoryStream, Type targetType)
		{
			FieldDeserializer[] array;
			if (!typeToClassDeserializers.TryGetValue(targetType, out array))
			{
				FieldInfo[] fields =
					targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				array = new FieldDeserializer[fields.Length];
				for (int i = 0; i < array.Length; i++)
				{
					FieldInfo fieldInfo = fields[i];
					FieldReaderWriter fieldReaderWriter = GetFieldReaderWriter(fieldInfo.FieldType);
					array[i] = new FieldDeserializer(fieldInfo, fieldReaderWriter);
				}

				typeToClassDeserializers.Add(targetType, array);
			}

			object obj = Activator.CreateInstance(targetType);
			foreach (FieldDeserializer fieldDeserializer in array)
			{
				fieldDeserializer.field.SetValue(obj,
					fieldDeserializer.deserializer.Read(memoryStream, fieldDeserializer.fieldType));
			}

			return obj;
		}


		private static void AddToInitialElement(Type type, FieldReaderWriter readerWriter)
		{
			typeToFieldReaderWriters.Add(type, readerWriter);
		}


		private class FieldDeserializer
		{
			public readonly FieldReaderWriter deserializer;


			public readonly FieldInfo field;


			public readonly Type fieldType;

			public FieldDeserializer(FieldInfo field, FieldReaderWriter deserializer)
			{
				this.field = field;
				fieldType = field.FieldType;
				this.deserializer = deserializer;
			}
		}


		private class FieldReaderWriterArray : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Array array = (Array) value;
				Primitives.WritePrimitive(memoryStream, array.Length);
				Type elementType = array.GetType().GetElementType();
				FieldReaderWriter fieldReaderWriter = GetFieldReaderWriter(elementType);
				if (elementType.IsValueType && array.Length >= 8 && elementType.IsPrimitive)
				{
					try
					{
						Primitives.WritePrimitiveArray(memoryStream, array);
						return;
					}
					catch (ArgumentException innerException)
					{
						throw new Exception(array + " is not array", innerException);
					}
				}

				for (int i = 0; i < array.Length; i++)
				{
					fieldReaderWriter.Write(memoryStream, array.GetValue(i));
				}
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				int length;
				Primitives.ReadPrimitive(memoryStream, out length);
				Type elementType = type.GetElementType();
				FieldReaderWriter fieldReaderWriter = GetFieldReaderWriter(elementType);
				Array array = Array.CreateInstance(elementType, length);
				if (elementType.IsValueType && array.Length >= 8 && elementType.IsPrimitive)
				{
					try
					{
						Primitives.ReadPrimitiveArray(memoryStream, array);
						return array;
					}
					catch (ArgumentException innerException)
					{
						throw new Exception(array + " is not array", innerException);
					}
				}

				for (int i = 0; i < array.Length; i++)
				{
					array.SetValue(fieldReaderWriter.Read(memoryStream, elementType), i);
				}

				return array;
			}
		}


		private class FieldReaderWriterIList : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				IList list = (IList) value;
				Array array = new Array[list.Count];
				list.CopyTo(array, 0);
				typeToFieldReaderWriters[typeof(Array)].Write(memoryStream, array);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				Type type2 = type.GetType().GetGenericArguments()[0];
				Type type3 = Array.CreateInstance(type2, 0).GetType();
				Array array = (Array) typeToFieldReaderWriters[typeof(Array)].Read(memoryStream, type3);
				IList list = CreateList(type2);
				foreach (object value in array)
				{
					list.Add(value);
				}

				return list;
			}


			private IList CreateList(Type myType)
			{
				return (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(myType));
			}
		}


		private class FieldReaderWriterQuaternion : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Quaternion quaternion = (Quaternion) value;
				Primitives.WritePrimitive(memoryStream, quaternion.w);
				Primitives.WritePrimitive(memoryStream, quaternion.x);
				Primitives.WritePrimitive(memoryStream, quaternion.y);
				Primitives.WritePrimitive(memoryStream, quaternion.z);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				Quaternion quaternion;
				Primitives.ReadPrimitive(memoryStream, out quaternion.w);
				Primitives.ReadPrimitive(memoryStream, out quaternion.x);
				Primitives.ReadPrimitive(memoryStream, out quaternion.y);
				Primitives.ReadPrimitive(memoryStream, out quaternion.z);
				return quaternion;
			}
		}


		private class FieldReaderWriterVector4 : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Vector4 vector = (Vector4) value;
				Primitives.WritePrimitive(memoryStream, vector.w);
				Primitives.WritePrimitive(memoryStream, vector.x);
				Primitives.WritePrimitive(memoryStream, vector.y);
				Primitives.WritePrimitive(memoryStream, vector.z);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				Vector4 vector;
				Primitives.ReadPrimitive(memoryStream, out vector.w);
				Primitives.ReadPrimitive(memoryStream, out vector.x);
				Primitives.ReadPrimitive(memoryStream, out vector.y);
				Primitives.ReadPrimitive(memoryStream, out vector.z);
				return vector;
			}
		}


		private class FieldReaderWriterVector3 : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Vector3 vector = (Vector3) value;
				Primitives.WritePrimitive(memoryStream, vector.x);
				Primitives.WritePrimitive(memoryStream, vector.y);
				Primitives.WritePrimitive(memoryStream, vector.z);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				Vector3 vector;
				Primitives.ReadPrimitive(memoryStream, out vector.x);
				Primitives.ReadPrimitive(memoryStream, out vector.y);
				Primitives.ReadPrimitive(memoryStream, out vector.z);
				return vector;
			}
		}


		private class FieldReaderWriterVector2 : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Vector2 vector = (Vector2) value;
				Primitives.WritePrimitive(memoryStream, vector.x);
				Primitives.WritePrimitive(memoryStream, vector.y);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				Vector2 vector;
				Primitives.ReadPrimitive(memoryStream, out vector.x);
				Primitives.ReadPrimitive(memoryStream, out vector.y);
				return vector;
			}
		}


		private class FieldReaderWriterColor : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Color color = (Color) value;
				Primitives.WritePrimitive(memoryStream, color.a);
				Primitives.WritePrimitive(memoryStream, color.r);
				Primitives.WritePrimitive(memoryStream, color.g);
				Primitives.WritePrimitive(memoryStream, color.b);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				Color color;
				Primitives.ReadPrimitive(memoryStream, out color.a);
				Primitives.ReadPrimitive(memoryStream, out color.r);
				Primitives.ReadPrimitive(memoryStream, out color.g);
				Primitives.ReadPrimitive(memoryStream, out color.b);
				return color;
			}
		}


		private class FieldReaderWriterEnum : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (int) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				int num;
				Primitives.ReadPrimitive(memoryStream, out num);
				return num;
			}
		}


		private class FieldReaderWriterInt : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (int) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				int num;
				Primitives.ReadPrimitive(memoryStream, out num);
				return num;
			}
		}


		private class FieldReaderWriterChar : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (char) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				char c;
				Primitives.ReadPrimitive(memoryStream, out c);
				return c;
			}
		}


		private class FieldReaderWriterBool : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (bool) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				bool flag;
				Primitives.ReadPrimitive(memoryStream, out flag);
				return flag;
			}
		}


		private class FieldReaderWriterByte : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (byte) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				byte b;
				Primitives.ReadPrimitive(memoryStream, out b);
				return b;
			}
		}


		private class FieldReaderWriterSByte : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (sbyte) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				sbyte b;
				Primitives.ReadPrimitive(memoryStream, out b);
				return b;
			}
		}


		private class FieldReaderWriterUShort : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (ushort) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				ushort num;
				Primitives.ReadPrimitive(memoryStream, out num);
				return num;
			}
		}


		private class FieldReaderWriterShort : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (short) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				short num;
				Primitives.ReadPrimitive(memoryStream, out num);
				return num;
			}
		}


		private class FieldReaderWriterUInt : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (uint) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				uint num;
				Primitives.ReadPrimitive(memoryStream, out num);
				return num;
			}
		}


		private class FieldReaderWriterFloat : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (float) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				float num;
				Primitives.ReadPrimitive(memoryStream, out num);
				return num;
			}
		}


		private class FieldReaderWriterULong : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (ulong) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				ulong num;
				Primitives.ReadPrimitive(memoryStream, out num);
				return num;
			}
		}


		private class FieldReaderWriterLong : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (long) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				long num;
				Primitives.ReadPrimitive(memoryStream, out num);
				return num;
			}
		}


		private class FieldReaderWriterDouble : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (double) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				double num;
				Primitives.ReadPrimitive(memoryStream, out num);
				return num;
			}
		}


		private class FieldReaderWriterDecimal : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (decimal) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				decimal num;
				Primitives.ReadPrimitive(memoryStream, out num);
				return num;
			}
		}


		private class FieldReaderWriterString : FieldReaderWriter
		{
			public void Write(MemoryStream memoryStream, object value)
			{
				Primitives.WritePrimitive(memoryStream, (string) value);
			}


			public object Read(MemoryStream memoryStream, Type type)
			{
				string result;
				Primitives.ReadPrimitive(memoryStream, out result);
				return result;
			}
		}


		public interface FieldReaderWriter
		{
			void Write(MemoryStream memoryStream, object value);


			object Read(MemoryStream memoryStream, Type type);
		}


		public static class Primitives
		{
			[ThreadStatic] private static int[] s_decimalBitsArray;


			private static readonly byte[] s_emptyByteArray = new byte[0];


			private static byte[] ArrayCopyBuffer = new byte[64];

			public static MethodInfo GetWritePrimitive(Type type)
			{
				return typeof(Primitives).GetMethod("WritePrimitive",
					BindingFlags.Static | BindingFlags.Public | BindingFlags.ExactBinding, null, new[]
					{
						typeof(Stream),
						type
					}, null);
			}


			public static MethodInfo GetReaderPrimitive(Type type)
			{
				return typeof(Primitives).GetMethod("ReadPrimitive",
					BindingFlags.Static | BindingFlags.Public | BindingFlags.ExactBinding, null, new[]
					{
						typeof(Stream),
						type.MakeByRefType()
					}, null);
			}


			private static uint EncodeZigZag32(int n)
			{
				return (uint) ((n << 1) ^ (n >> 31));
			}


			private static ulong EncodeZigZag64(long n)
			{
				return (ulong) ((n << 1) ^ (n >> 63));
			}


			private static int DecodeZigZag32(uint n)
			{
				return (int) ((n >> 1) ^ -(int) (n & 1U));
			}


			private static long DecodeZigZag64(ulong n)
			{
				return (long) (n >> 1) ^ -((long) n & 1L);
			}

			// co: dotPeek
			// private static long DecodeZigZag64(ulong n)
			// {
			// 	return (long)(n >> 1 ^ -(long)(n & 1UL));
			// }


			private static uint ReadVarint32(Stream stream)
			{
				int num = 0;
				for (int i = 0; i < 32; i += 7)
				{
					int num2 = stream.ReadByte();
					if (num2 == -1)
					{
						throw new EndOfStreamException();
					}

					num |= (num2 & 127) << i;
					if ((num2 & 128) == 0)
					{
						return (uint) num;
					}
				}

				throw new InvalidOperationException();
			}


			private static void WriteVarint32(Stream stream, uint value)
			{
				while (value >= 128U)
				{
					stream.WriteByte((byte) (value | 128U));
					value >>= 7;
				}

				stream.WriteByte((byte) value);
			}


			private static ulong ReadVarint64(Stream stream)
			{
				long num = 0L;
				for (int i = 0; i < 64; i += 7)
				{
					int num2 = stream.ReadByte();
					if (num2 == -1)
					{
						throw new EndOfStreamException();
					}

					num |= (long) (num2 & 127) << i;
					if ((num2 & 128) == 0)
					{
						return (ulong) num;
					}
				}

				throw new InvalidOperationException();
			}


			private static void WriteVarint64(Stream stream, ulong value)
			{
				while (value >= 128UL)
				{
					stream.WriteByte((byte) (value | 128UL));
					value >>= 7;
				}

				stream.WriteByte((byte) value);
			}


			public static void WritePrimitive(Stream stream, bool value)
			{
				stream.WriteByte(value ? (byte) 1 : (byte) 0);
			}
			// co: dotPeek
			// public static void WritePrimitive(Stream stream, bool value)
			// {
			// 	stream.WriteByte(value ? 1 : 0);
			// }


			public static void ReadPrimitive(Stream stream, out bool value)
			{
				int num = stream.ReadByte();
				value = num != 0;
			}


			public static void WritePrimitive(Stream stream, byte value)
			{
				stream.WriteByte(value);
			}


			public static void ReadPrimitive(Stream stream, out byte value)
			{
				value = (byte) stream.ReadByte();
			}


			public static void WritePrimitive(Stream stream, sbyte value)
			{
				stream.WriteByte((byte) value);
			}


			public static void ReadPrimitive(Stream stream, out sbyte value)
			{
				value = (sbyte) stream.ReadByte();
			}


			public static void WritePrimitive(Stream stream, char value)
			{
				WriteVarint32(stream, value);
			}


			public static void ReadPrimitive(Stream stream, out char value)
			{
				value = (char) ReadVarint32(stream);
			}


			public static void WritePrimitive(Stream stream, ushort value)
			{
				WriteVarint32(stream, value);
			}


			public static void ReadPrimitive(Stream stream, out ushort value)
			{
				value = (ushort) ReadVarint32(stream);
			}


			public static void WritePrimitive(Stream stream, short value)
			{
				WriteVarint32(stream, EncodeZigZag32(value));
			}


			public static void ReadPrimitive(Stream stream, out short value)
			{
				value = (short) DecodeZigZag32(ReadVarint32(stream));
			}


			public static void WritePrimitive(Stream stream, uint value)
			{
				WriteVarint32(stream, value);
			}


			public static void ReadPrimitive(Stream stream, out uint value)
			{
				value = ReadVarint32(stream);
			}


			public static void WritePrimitive(Stream stream, int value)
			{
				WriteVarint32(stream, EncodeZigZag32(value));
			}


			public static void ReadPrimitive(Stream stream, out int value)
			{
				value = DecodeZigZag32(ReadVarint32(stream));
			}


			public static void WritePrimitive(Stream stream, ulong value)
			{
				WriteVarint64(stream, value);
			}


			public static void ReadPrimitive(Stream stream, out ulong value)
			{
				value = ReadVarint64(stream);
			}


			public static void WritePrimitive(Stream stream, long value)
			{
				WriteVarint64(stream, EncodeZigZag64(value));
			}


			public static void ReadPrimitive(Stream stream, out long value)
			{
				value = DecodeZigZag64(ReadVarint64(stream));
			}


			public static void WritePrimitive(Stream stream, float value)
			{
				WritePrimitive(stream, (double) value);
			}


			public static void ReadPrimitive(Stream stream, out float value)
			{
				double num;
				ReadPrimitive(stream, out num);
				value = (float) num;
			}


			public static void WritePrimitive(Stream stream, double value)
			{
				ulong value2 = (ulong) BitConverter.DoubleToInt64Bits(value);
				WriteVarint64(stream, value2);
			}


			public static void ReadPrimitive(Stream stream, out double value)
			{
				ulong value2 = ReadVarint64(stream);
				value = BitConverter.Int64BitsToDouble((long) value2);
			}


			public static void WritePrimitive(Stream stream, DateTime value)
			{
				long value2 = value.ToBinary();
				WritePrimitive(stream, value2);
			}


			public static void ReadPrimitive(Stream stream, out DateTime value)
			{
				long dateData;
				ReadPrimitive(stream, out dateData);
				value = DateTime.FromBinary(dateData);
			}


			public static void WritePrimitive(Stream stream, decimal value)
			{
				int[] bits = decimal.GetBits(value);
				ulong num = (ulong) bits[0];
				ulong num2 = (ulong) bits[1] << 32;
				ulong value2 = num | num2;
				uint value3 = (uint) bits[2];
				uint num3 = ((uint) bits[3] >> 15) & 510U;
				uint num4 = (uint) bits[3] >> 31;
				uint value4 = num3 | num4;
				WritePrimitive(stream, value2);
				WritePrimitive(stream, value3);
				WritePrimitive(stream, value4);
			}


			public static void ReadPrimitive(Stream stream, out decimal value)
			{
				ulong num;
				ReadPrimitive(stream, out num);
				uint num2;
				ReadPrimitive(stream, out num2);
				uint num3;
				ReadPrimitive(stream, out num3);
				int num4 = (int) (num3 & 18446744073709551614UL) << 15;
				int num5 = (int) (num3 & 1U) << 31;
				int[] array = s_decimalBitsArray;
				if (array == null)
				{
					array = s_decimalBitsArray = new int[4];
				}

				array[0] = (int) num;
				array[1] = (int) (num >> 32);
				array[2] = (int) num2;
				array[3] = num4 | num5;
				value = new decimal(array);
			}


			public static void WritePrimitive(Stream stream, string value)
			{
				if (value == null)
				{
					WritePrimitive(stream, 0U);
					return;
				}

				UTF8Encoding utf8Encoding = new UTF8Encoding(false, true);
				int byteCount = utf8Encoding.GetByteCount(value);
				WritePrimitive(stream, (uint) (byteCount + 1));
				byte[] array = new byte[byteCount];
				utf8Encoding.GetBytes(value, 0, value.Length, array, 0);
				stream.Write(array, 0, byteCount);
			}


			public static void ReadPrimitive(Stream stream, out string value)
			{
				uint num;
				ReadPrimitive(stream, out num);
				if (num == 0U)
				{
					value = null;
					return;
				}

				if (num == 1U)
				{
					value = string.Empty;
					return;
				}

				num -= 1U;
				UTF8Encoding utf8Encoding = new UTF8Encoding(false, true);
				byte[] array = new byte[num];
				int num2 = 0;
				while (num2 < num)
				{
					int num3 = stream.Read(array, num2, (int) (num - (uint) num2));
					if (num3 == 0)
					{
						throw new EndOfStreamException();
					}

					num2 += num3;
				}

				value = utf8Encoding.GetString(array);
			}


			public static void WritePrimitive(Stream stream, byte[] value)
			{
				if (value == null)
				{
					WritePrimitive(stream, 0U);
					return;
				}

				WritePrimitive(stream, (uint) (value.Length + 1));
				stream.Write(value, 0, value.Length);
			}


			public static void ReadPrimitive(Stream stream, out byte[] value)
			{
				uint num;
				ReadPrimitive(stream, out num);
				if (num == 0U)
				{
					value = null;
					return;
				}

				if (num == 1U)
				{
					value = s_emptyByteArray;
					return;
				}

				num -= 1U;
				value = new byte[num];
				int num2 = 0;
				while (num2 < num)
				{
					int num3 = stream.Read(value, num2, (int) (num - (uint) num2));
					if (num3 == 0)
					{
						throw new EndOfStreamException();
					}

					num2 += num3;
				}
			}


			private static byte[] GetArrayCopyBuffer(Array array)
			{
				int num = Buffer.ByteLength(array);
				if (num > 65536)
				{
					return new byte[num];
				}

				if (num > ArrayCopyBuffer.Length)
				{
					ArrayCopyBuffer = new byte[num];
				}

				return ArrayCopyBuffer;
			}


			public static void WritePrimitiveArray(MemoryStream memoryStream, Array array)
			{
				int count = Buffer.ByteLength(array);
				byte[] arrayCopyBuffer = GetArrayCopyBuffer(array);
				Buffer.BlockCopy(array, 0, arrayCopyBuffer, 0, count);
				memoryStream.Write(arrayCopyBuffer, 0, count);
			}


			public static void ReadPrimitiveArray(MemoryStream memoryStream, Array array)
			{
				int count = Buffer.ByteLength(array);
				byte[] arrayCopyBuffer = GetArrayCopyBuffer(array);
				memoryStream.Read(arrayCopyBuffer, 0, count);
				Buffer.BlockCopy(arrayCopyBuffer, 0, array, 0, count);
			}
		}
	}
}