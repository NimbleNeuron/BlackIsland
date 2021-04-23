using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace LiteNetLib.Utils
{
	
	public class NetSerializer
	{
		
		public void RegisterNestedType<T>() where T : struct, INetSerializable
		{
			this._registeredTypes.Add(typeof(T), new NetSerializer.CustomTypeStruct<T>());
		}

		
		public void RegisterNestedType<T>(Func<T> constructor) where T : class, INetSerializable
		{
			this._registeredTypes.Add(typeof(T), new NetSerializer.CustomTypeClass<T>(constructor));
		}

		
		public void RegisterNestedType<T>(Action<NetDataWriter, T> writer, Func<NetDataReader, T> reader)
		{
			this._registeredTypes.Add(typeof(T), new NetSerializer.CustomTypeStatic<T>(writer, reader));
		}

		
		public NetSerializer() : this(0)
		{
		}

		
		public NetSerializer(int maxStringLength)
		{
			this._maxStringLength = maxStringLength;
		}

		
		private NetSerializer.ClassInfo<T> RegisterInternal<T>()
		{
			if (NetSerializer.ClassInfo<T>.Instance != null)
			{
				return NetSerializer.ClassInfo<T>.Instance;
			}
			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
			List<NetSerializer.FastCall<T>> list = new List<NetSerializer.FastCall<T>>();
			foreach (PropertyInfo propertyInfo in properties)
			{
				Type propertyType = propertyInfo.PropertyType;
				Type type = propertyType.IsArray ? propertyType.GetElementType() : propertyType;
				NetSerializer.CallType type2 = propertyType.IsArray ? NetSerializer.CallType.Array : NetSerializer.CallType.Basic;
				if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
				{
					type = propertyType.GetGenericArguments()[0];
					type2 = NetSerializer.CallType.List;
				}
				MethodInfo getMethod = propertyInfo.GetGetMethod();
				MethodInfo setMethod = propertyInfo.GetSetMethod();
				if (!(getMethod == null) && !(setMethod == null))
				{
					NetSerializer.FastCall<T> fastCall = null;
					if (propertyType.IsEnum)
					{
						Type underlyingType = Enum.GetUnderlyingType(propertyType);
						if (underlyingType == typeof(byte))
						{
							fastCall = new NetSerializer.EnumByteSerializer<T>(propertyInfo, propertyType);
						}
						else
						{
							if (!(underlyingType == typeof(int)))
							{
								throw new InvalidTypeException("Not supported enum underlying type: " + underlyingType.Name);
							}
							fastCall = new NetSerializer.EnumIntSerializer<T>(propertyInfo, propertyType);
						}
					}
					else if (type == typeof(string))
					{
						fastCall = new NetSerializer.StringSerializer<T>(this._maxStringLength);
					}
					else if (type == typeof(bool))
					{
						fastCall = new NetSerializer.BoolSerializer<T>();
					}
					else if (type == typeof(byte))
					{
						fastCall = new NetSerializer.ByteSerializer<T>();
					}
					else if (type == typeof(sbyte))
					{
						fastCall = new NetSerializer.SByteSerializer<T>();
					}
					else if (type == typeof(short))
					{
						fastCall = new NetSerializer.ShortSerializer<T>();
					}
					else if (type == typeof(ushort))
					{
						fastCall = new NetSerializer.UShortSerializer<T>();
					}
					else if (type == typeof(int))
					{
						fastCall = new NetSerializer.IntSerializer<T>();
					}
					else if (type == typeof(uint))
					{
						fastCall = new NetSerializer.UIntSerializer<T>();
					}
					else if (type == typeof(long))
					{
						fastCall = new NetSerializer.LongSerializer<T>();
					}
					else if (type == typeof(ulong))
					{
						fastCall = new NetSerializer.ULongSerializer<T>();
					}
					else if (type == typeof(float))
					{
						fastCall = new NetSerializer.FloatSerializer<T>();
					}
					else if (type == typeof(double))
					{
						fastCall = new NetSerializer.DoubleSerializer<T>();
					}
					else if (type == typeof(char))
					{
						fastCall = new NetSerializer.CharSerializer<T>();
					}
					else if (type == typeof(IPEndPoint))
					{
						fastCall = new NetSerializer.IPEndPointSerializer<T>();
					}
					else
					{
						NetSerializer.CustomType customType;
						this._registeredTypes.TryGetValue(type, out customType);
						if (customType != null)
						{
							fastCall = customType.Get<T>();
						}
					}
					if (fastCall == null)
					{
						throw new InvalidTypeException("Unknown property type: " + propertyType.FullName);
					}
					fastCall.Init(getMethod, setMethod, type2);
					list.Add(fastCall);
				}
			}
			NetSerializer.ClassInfo<T>.Instance = new NetSerializer.ClassInfo<T>(list);
			return NetSerializer.ClassInfo<T>.Instance;
		}

		
		public void Register<T>()
		{
			this.RegisterInternal<T>();
		}

		
		public T Deserialize<T>(NetDataReader reader) where T : class, new()
		{
			NetSerializer.ClassInfo<T> classInfo = this.RegisterInternal<T>();
			T t = Activator.CreateInstance<T>();
			try
			{
				classInfo.Read(t, reader);
			}
			catch
			{
				return default(T);
			}
			return t;
		}

		
		public bool Deserialize<T>(NetDataReader reader, T target) where T : class, new()
		{
			NetSerializer.ClassInfo<T> classInfo = this.RegisterInternal<T>();
			try
			{
				classInfo.Read(target, reader);
			}
			catch
			{
				return false;
			}
			return true;
		}

		
		public void Serialize<T>(NetDataWriter writer, T obj) where T : class, new()
		{
			this.RegisterInternal<T>().Write(obj, writer);
		}

		
		public byte[] Serialize<T>(T obj) where T : class, new()
		{
			if (this._writer == null)
			{
				this._writer = new NetDataWriter();
			}
			this._writer.Reset();
			this.Serialize<T>(this._writer, obj);
			return this._writer.CopyData();
		}

		
		private NetDataWriter _writer;

		
		private readonly int _maxStringLength;

		
		private readonly Dictionary<Type, NetSerializer.CustomType> _registeredTypes = new Dictionary<Type, NetSerializer.CustomType>();

		
		private enum CallType
		{
			
			Basic,
			
			Array,
			
			List
		}

		
		private abstract class FastCall<T>
		{
			
			public virtual void Init(MethodInfo getMethod, MethodInfo setMethod, NetSerializer.CallType type)
			{
				this.Type = type;
			}

			
			public abstract void Read(T inf, NetDataReader r);

			
			public abstract void Write(T inf, NetDataWriter w);

			
			public abstract void ReadArray(T inf, NetDataReader r);

			
			public abstract void WriteArray(T inf, NetDataWriter w);

			
			public abstract void ReadList(T inf, NetDataReader r);

			
			public abstract void WriteList(T inf, NetDataWriter w);

			
			public NetSerializer.CallType Type;
		}

		
		private abstract class FastCallSpecific<TClass, TProperty> : NetSerializer.FastCall<TClass>
		{
			
			public override void ReadArray(TClass inf, NetDataReader r)
			{
				throw new InvalidTypeException("Unsupported type: " + typeof(TProperty) + "[]");
			}

			
			public override void WriteArray(TClass inf, NetDataWriter w)
			{
				throw new InvalidTypeException("Unsupported type: " + typeof(TProperty) + "[]");
			}

			
			public override void ReadList(TClass inf, NetDataReader r)
			{
				throw new InvalidTypeException("Unsupported type: List<" + typeof(TProperty) + ">");
			}

			
			public override void WriteList(TClass inf, NetDataWriter w)
			{
				throw new InvalidTypeException("Unsupported type: List<" + typeof(TProperty) + ">");
			}

			
			protected TProperty[] ReadArrayHelper(TClass inf, NetDataReader r)
			{
				ushort @ushort = r.GetUShort();
				TProperty[] array = this.GetterArr(inf);
				array = ((array == null || array.Length != (int)@ushort) ? new TProperty[(int)@ushort] : array);
				this.SetterArr(inf, array);
				return array;
			}

			
			protected TProperty[] WriteArrayHelper(TClass inf, NetDataWriter w)
			{
				TProperty[] array = this.GetterArr(inf);
				w.Put((ushort)array.Length);
				return array;
			}

			
			protected List<TProperty> ReadListHelper(TClass inf, NetDataReader r, out int len)
			{
				len = (int)r.GetUShort();
				List<TProperty> list = this.GetterList(inf);
				if (list == null)
				{
					list = new List<TProperty>(len);
					this.SetterList(inf, list);
				}
				return list;
			}

			
			protected List<TProperty> WriteListHelper(TClass inf, NetDataWriter w, out int len)
			{
				List<TProperty> list = this.GetterList(inf);
				if (list == null)
				{
					len = 0;
					w.Put(0);
					return null;
				}
				len = list.Count;
				w.Put((ushort)len);
				return list;
			}

			
			public override void Init(MethodInfo getMethod, MethodInfo setMethod, NetSerializer.CallType type)
			{
				base.Init(getMethod, setMethod, type);
				if (type == NetSerializer.CallType.Array)
				{
					this.GetterArr = (Func<TClass, TProperty[]>)Delegate.CreateDelegate(typeof(Func<TClass, TProperty[]>), getMethod);
					this.SetterArr = (Action<TClass, TProperty[]>)Delegate.CreateDelegate(typeof(Action<TClass, TProperty[]>), setMethod);
					return;
				}
				if (type != NetSerializer.CallType.List)
				{
					this.Getter = (Func<TClass, TProperty>)Delegate.CreateDelegate(typeof(Func<TClass, TProperty>), getMethod);
					this.Setter = (Action<TClass, TProperty>)Delegate.CreateDelegate(typeof(Action<TClass, TProperty>), setMethod);
					return;
				}
				this.GetterList = (Func<TClass, List<TProperty>>)Delegate.CreateDelegate(typeof(Func<TClass, List<TProperty>>), getMethod);
				this.SetterList = (Action<TClass, List<TProperty>>)Delegate.CreateDelegate(typeof(Action<TClass, List<TProperty>>), setMethod);
			}

			
			protected Func<TClass, TProperty> Getter;

			
			protected Action<TClass, TProperty> Setter;

			
			protected Func<TClass, TProperty[]> GetterArr;

			
			protected Action<TClass, TProperty[]> SetterArr;

			
			protected Func<TClass, List<TProperty>> GetterList;

			
			protected Action<TClass, List<TProperty>> SetterList;
		}

		
		private abstract class FastCallSpecificAuto<TClass, TProperty> : NetSerializer.FastCallSpecific<TClass, TProperty>
		{
			
			protected abstract void ElementRead(NetDataReader r, out TProperty prop);

			
			protected abstract void ElementWrite(NetDataWriter w, ref TProperty prop);

			
			public override void Read(TClass inf, NetDataReader r)
			{
				TProperty arg;
				this.ElementRead(r, out arg);
				this.Setter(inf, arg);
			}

			
			public override void Write(TClass inf, NetDataWriter w)
			{
				TProperty tproperty = this.Getter(inf);
				this.ElementWrite(w, ref tproperty);
			}

			
			public override void ReadArray(TClass inf, NetDataReader r)
			{
				TProperty[] array = base.ReadArrayHelper(inf, r);
				for (int i = 0; i < array.Length; i++)
				{
					this.ElementRead(r, out array[i]);
				}
			}

			
			public override void WriteArray(TClass inf, NetDataWriter w)
			{
				TProperty[] array = base.WriteArrayHelper(inf, w);
				for (int i = 0; i < array.Length; i++)
				{
					this.ElementWrite(w, ref array[i]);
				}
			}
		}

		
		private sealed class FastCallStatic<TClass, TProperty> : NetSerializer.FastCallSpecific<TClass, TProperty>
		{
			
			public FastCallStatic(Action<NetDataWriter, TProperty> write, Func<NetDataReader, TProperty> read)
			{
				this._writer = write;
				this._reader = read;
			}

			
			public override void Read(TClass inf, NetDataReader r)
			{
				this.Setter(inf, this._reader(r));
			}

			
			public override void Write(TClass inf, NetDataWriter w)
			{
				this._writer(w, this.Getter(inf));
			}

			
			public override void ReadList(TClass inf, NetDataReader r)
			{
				int num;
				List<TProperty> list = base.ReadListHelper(inf, r, out num);
				int count = list.Count;
				if (num > count)
				{
					for (int i = 0; i < count; i++)
					{
						list[i] = this._reader(r);
					}
					for (int j = count; j < num; j++)
					{
						list.Add(this._reader(r));
					}
					return;
				}
				if (num < count)
				{
					list.RemoveRange(num, count - num);
				}
				for (int k = 0; k < num; k++)
				{
					list[k] = this._reader(r);
				}
			}

			
			public override void WriteList(TClass inf, NetDataWriter w)
			{
				int num;
				List<TProperty> list = base.WriteListHelper(inf, w, out num);
				for (int i = 0; i < num; i++)
				{
					this._writer(w, list[i]);
				}
			}

			
			public override void ReadArray(TClass inf, NetDataReader r)
			{
				TProperty[] array = base.ReadArrayHelper(inf, r);
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					array[i] = this._reader(r);
				}
			}

			
			public override void WriteArray(TClass inf, NetDataWriter w)
			{
				TProperty[] array = base.WriteArrayHelper(inf, w);
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					this._writer(w, array[i]);
				}
			}

			
			private readonly Action<NetDataWriter, TProperty> _writer;

			
			private readonly Func<NetDataReader, TProperty> _reader;
		}

		
		private sealed class FastCallStruct<TClass, TProperty> : NetSerializer.FastCallSpecific<TClass, TProperty> where TProperty : struct, INetSerializable
		{
			
			public override void Read(TClass inf, NetDataReader r)
			{
				this._p.Deserialize(r);
				this.Setter(inf, this._p);
			}

			
			public override void Write(TClass inf, NetDataWriter w)
			{
				this._p = this.Getter(inf);
				this._p.Serialize(w);
			}

			
			public override void ReadList(TClass inf, NetDataReader r)
			{
				int num;
				List<TProperty> list = base.ReadListHelper(inf, r, out num);
				int count = list.Count;
				if (num > count)
				{
					for (int i = 0; i < count; i++)
					{
						TProperty tproperty = list[i];
						tproperty.Deserialize(r);
					}
					for (int j = count; j < num; j++)
					{
						TProperty item = default(TProperty);
						item.Deserialize(r);
						list.Add(item);
					}
					return;
				}
				if (num < count)
				{
					list.RemoveRange(num, count - num);
				}
				for (int k = 0; k < num; k++)
				{
					TProperty tproperty = list[k];
					tproperty.Deserialize(r);
				}
			}

			
			public override void WriteList(TClass inf, NetDataWriter w)
			{
				int num;
				List<TProperty> list = base.WriteListHelper(inf, w, out num);
				for (int i = 0; i < num; i++)
				{
					TProperty tproperty = list[i];
					tproperty.Serialize(w);
				}
			}

			
			public override void ReadArray(TClass inf, NetDataReader r)
			{
				TProperty[] array = base.ReadArrayHelper(inf, r);
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					array[i].Deserialize(r);
				}
			}

			
			public override void WriteArray(TClass inf, NetDataWriter w)
			{
				TProperty[] array = base.WriteArrayHelper(inf, w);
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					array[i].Serialize(w);
				}
			}

			
			private TProperty _p;
		}

		
		private sealed class FastCallClass<TClass, TProperty> : NetSerializer.FastCallSpecific<TClass, TProperty> where TProperty : class, INetSerializable
		{
			
			public FastCallClass(Func<TProperty> constructor)
			{
				this._constructor = constructor;
			}

			
			public override void Read(TClass inf, NetDataReader r)
			{
				TProperty tproperty = this._constructor();
				tproperty.Deserialize(r);
				this.Setter(inf, tproperty);
			}

			
			public override void Write(TClass inf, NetDataWriter w)
			{
				TProperty tproperty = this.Getter(inf);
				if (tproperty != null)
				{
					tproperty.Serialize(w);
				}
			}

			
			public override void ReadList(TClass inf, NetDataReader r)
			{
				int num;
				List<TProperty> list = base.ReadListHelper(inf, r, out num);
				int count = list.Count;
				if (num > count)
				{
					for (int i = 0; i < count; i++)
					{
						list[i].Deserialize(r);
					}
					for (int j = count; j < num; j++)
					{
						TProperty tproperty = this._constructor();
						tproperty.Deserialize(r);
						list.Add(tproperty);
					}
					return;
				}
				if (num < count)
				{
					list.RemoveRange(num, count - num);
				}
				for (int k = 0; k < num; k++)
				{
					list[k].Deserialize(r);
				}
			}

			
			public override void WriteList(TClass inf, NetDataWriter w)
			{
				int num;
				List<TProperty> list = base.WriteListHelper(inf, w, out num);
				for (int i = 0; i < num; i++)
				{
					list[i].Serialize(w);
				}
			}

			
			public override void ReadArray(TClass inf, NetDataReader r)
			{
				TProperty[] array = base.ReadArrayHelper(inf, r);
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					array[i] = this._constructor();
					array[i].Deserialize(r);
				}
			}

			
			public override void WriteArray(TClass inf, NetDataWriter w)
			{
				TProperty[] array = base.WriteArrayHelper(inf, w);
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					array[i].Serialize(w);
				}
			}

			
			private readonly Func<TProperty> _constructor;
		}

		
		private class IntSerializer<T> : NetSerializer.FastCallSpecific<T, int>
		{
			
			public override void Read(T inf, NetDataReader r)
			{
				this.Setter(inf, r.GetInt());
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put(this.Getter(inf));
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				this.SetterArr(inf, r.GetIntArray());
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				w.PutArray(this.GetterArr(inf));
			}
		}

		
		private class UIntSerializer<T> : NetSerializer.FastCallSpecific<T, uint>
		{
			
			public override void Read(T inf, NetDataReader r)
			{
				this.Setter(inf, r.GetUInt());
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put(this.Getter(inf));
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				this.SetterArr(inf, r.GetUIntArray());
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				w.PutArray(this.GetterArr(inf));
			}
		}

		
		private class ShortSerializer<T> : NetSerializer.FastCallSpecific<T, short>
		{
			
			public override void Read(T inf, NetDataReader r)
			{
				this.Setter(inf, r.GetShort());
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put(this.Getter(inf));
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				this.SetterArr(inf, r.GetShortArray());
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				w.PutArray(this.GetterArr(inf));
			}
		}

		
		private class UShortSerializer<T> : NetSerializer.FastCallSpecific<T, ushort>
		{
			
			public override void Read(T inf, NetDataReader r)
			{
				this.Setter(inf, r.GetUShort());
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put(this.Getter(inf));
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				this.SetterArr(inf, r.GetUShortArray());
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				w.PutArray(this.GetterArr(inf));
			}
		}

		
		private class LongSerializer<T> : NetSerializer.FastCallSpecific<T, long>
		{
			
			public override void Read(T inf, NetDataReader r)
			{
				this.Setter(inf, r.GetLong());
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put(this.Getter(inf));
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				this.SetterArr(inf, r.GetLongArray());
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				w.PutArray(this.GetterArr(inf));
			}
		}

		
		private class ULongSerializer<T> : NetSerializer.FastCallSpecific<T, ulong>
		{
			
			public override void Read(T inf, NetDataReader r)
			{
				this.Setter(inf, r.GetULong());
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put(this.Getter(inf));
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				this.SetterArr(inf, r.GetULongArray());
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				w.PutArray(this.GetterArr(inf));
			}
		}

		
		private class ByteSerializer<T> : NetSerializer.FastCallSpecific<T, byte>
		{
			
			public override void Read(T inf, NetDataReader r)
			{
				this.Setter(inf, r.GetByte());
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put(this.Getter(inf));
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				this.SetterArr(inf, r.GetBytesWithLength());
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				w.PutBytesWithLength(this.GetterArr(inf));
			}
		}

		
		private class SByteSerializer<T> : NetSerializer.FastCallSpecific<T, sbyte>
		{
			
			public override void Read(T inf, NetDataReader r)
			{
				this.Setter(inf, r.GetSByte());
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put(this.Getter(inf));
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				this.SetterArr(inf, r.GetSBytesWithLength());
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				w.PutSBytesWithLength(this.GetterArr(inf));
			}
		}

		
		private class FloatSerializer<T> : NetSerializer.FastCallSpecific<T, float>
		{
			
			public override void Read(T inf, NetDataReader r)
			{
				this.Setter(inf, r.GetFloat());
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put(this.Getter(inf));
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				this.SetterArr(inf, r.GetFloatArray());
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				w.PutArray(this.GetterArr(inf));
			}
		}

		
		private class DoubleSerializer<T> : NetSerializer.FastCallSpecific<T, double>
		{
			
			public override void Read(T inf, NetDataReader r)
			{
				this.Setter(inf, r.GetDouble());
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put(this.Getter(inf));
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				this.SetterArr(inf, r.GetDoubleArray());
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				w.PutArray(this.GetterArr(inf));
			}
		}

		
		private class BoolSerializer<T> : NetSerializer.FastCallSpecific<T, bool>
		{
			
			public override void Read(T inf, NetDataReader r)
			{
				this.Setter(inf, r.GetBool());
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put(this.Getter(inf));
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				this.SetterArr(inf, r.GetBoolArray());
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				w.PutArray(this.GetterArr(inf));
			}
		}

		
		private class CharSerializer<T> : NetSerializer.FastCallSpecificAuto<T, char>
		{
			
			protected override void ElementWrite(NetDataWriter w, ref char prop)
			{
				w.Put(prop);
			}

			
			protected override void ElementRead(NetDataReader r, out char prop)
			{
				prop = r.GetChar();
			}
		}

		
		private class IPEndPointSerializer<T> : NetSerializer.FastCallSpecificAuto<T, IPEndPoint>
		{
			
			protected override void ElementWrite(NetDataWriter w, ref IPEndPoint prop)
			{
				w.Put(prop);
			}

			
			protected override void ElementRead(NetDataReader r, out IPEndPoint prop)
			{
				prop = r.GetNetEndPoint();
			}
		}

		
		private class StringSerializer<T> : NetSerializer.FastCallSpecific<T, string>
		{
			
			public StringSerializer(int maxLength)
			{
				this._maxLength = ((maxLength > 0) ? maxLength : 32767);
			}

			
			public override void Read(T inf, NetDataReader r)
			{
				this.Setter(inf, r.GetString(this._maxLength));
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put(this.Getter(inf), this._maxLength);
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				this.SetterArr(inf, r.GetStringArray(this._maxLength));
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				w.PutArray(this.GetterArr(inf), this._maxLength);
			}

			
			private readonly int _maxLength;
		}

		
		private class EnumByteSerializer<T> : NetSerializer.FastCall<T>
		{
			
			public EnumByteSerializer(PropertyInfo property, Type propertyType)
			{
				this.Property = property;
				this.PropertyType = propertyType;
			}

			
			public override void Read(T inf, NetDataReader r)
			{
				this.Property.SetValue(inf, Enum.ToObject(this.PropertyType, r.GetByte()), null);
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put((byte)this.Property.GetValue(inf, null));
			}

			
			public override void ReadArray(T inf, NetDataReader r)
			{
				throw new InvalidTypeException("Unsupported type: Enum[]");
			}

			
			public override void WriteArray(T inf, NetDataWriter w)
			{
				throw new InvalidTypeException("Unsupported type: Enum[]");
			}

			
			public override void ReadList(T inf, NetDataReader r)
			{
				throw new InvalidTypeException("Unsupported type: List<Enum>");
			}

			
			public override void WriteList(T inf, NetDataWriter w)
			{
				throw new InvalidTypeException("Unsupported type: List<Enum>");
			}

			
			protected readonly PropertyInfo Property;

			
			protected readonly Type PropertyType;
		}

		
		private class EnumIntSerializer<T> : NetSerializer.EnumByteSerializer<T>
		{
			
			public EnumIntSerializer(PropertyInfo property, Type propertyType) : base(property, propertyType)
			{
			}

			
			public override void Read(T inf, NetDataReader r)
			{
				this.Property.SetValue(inf, Enum.ToObject(this.PropertyType, r.GetInt()), null);
			}

			
			public override void Write(T inf, NetDataWriter w)
			{
				w.Put((int)this.Property.GetValue(inf, null));
			}
		}

		
		private sealed class ClassInfo<T>
		{
			
			public ClassInfo(List<NetSerializer.FastCall<T>> serializers)
			{
				this._membersCount = serializers.Count;
				this._serializers = serializers.ToArray();
			}

			
			public void Write(T obj, NetDataWriter writer)
			{
				for (int i = 0; i < this._membersCount; i++)
				{
					NetSerializer.FastCall<T> fastCall = this._serializers[i];
					if (fastCall.Type == NetSerializer.CallType.Basic)
					{
						fastCall.Write(obj, writer);
					}
					else if (fastCall.Type == NetSerializer.CallType.Array)
					{
						fastCall.WriteArray(obj, writer);
					}
					else
					{
						fastCall.WriteList(obj, writer);
					}
				}
			}

			
			public void Read(T obj, NetDataReader reader)
			{
				for (int i = 0; i < this._membersCount; i++)
				{
					NetSerializer.FastCall<T> fastCall = this._serializers[i];
					if (fastCall.Type == NetSerializer.CallType.Basic)
					{
						fastCall.Read(obj, reader);
					}
					else if (fastCall.Type == NetSerializer.CallType.Array)
					{
						fastCall.ReadArray(obj, reader);
					}
					else
					{
						fastCall.ReadList(obj, reader);
					}
				}
			}

			
			public static NetSerializer.ClassInfo<T> Instance;

			
			private readonly NetSerializer.FastCall<T>[] _serializers;

			
			private readonly int _membersCount;
		}

		
		private abstract class CustomType
		{
			
			public abstract NetSerializer.FastCall<T> Get<T>();
		}

		
		private sealed class CustomTypeStruct<TProperty> : NetSerializer.CustomType where TProperty : struct, INetSerializable
		{
			
			public override NetSerializer.FastCall<T> Get<T>()
			{
				return new NetSerializer.FastCallStruct<T, TProperty>();
			}
		}

		
		private sealed class CustomTypeClass<TProperty> : NetSerializer.CustomType where TProperty : class, INetSerializable
		{
			
			public CustomTypeClass(Func<TProperty> constructor)
			{
				this._constructor = constructor;
			}

			
			public override NetSerializer.FastCall<T> Get<T>()
			{
				return new NetSerializer.FastCallClass<T, TProperty>(this._constructor);
			}

			
			private readonly Func<TProperty> _constructor;
		}

		
		private sealed class CustomTypeStatic<TProperty> : NetSerializer.CustomType
		{
			
			public CustomTypeStatic(Action<NetDataWriter, TProperty> writer, Func<NetDataReader, TProperty> reader)
			{
				this._writer = writer;
				this._reader = reader;
			}

			
			public override NetSerializer.FastCall<T> Get<T>()
			{
				return new NetSerializer.FastCallStatic<T, TProperty>(this._writer, this._reader);
			}

			
			private readonly Action<NetDataWriter, TProperty> _writer;

			
			private readonly Func<NetDataReader, TProperty> _reader;
		}
	}
}
