using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Blis.Client
{
	public static class TextDataUnidirectionalDocumentSerializerStorage
	{
		private static readonly Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();


		private static readonly byte[] memoryStreamData = new byte[32768];


		public static object DeserializeFromXmlDefaultSerializer(XmlReader reader, Type type)
		{
			XmlSerializer xmlSerializer;
			if (!serializers.TryGetValue(type, out xmlSerializer))
			{
				xmlSerializer = new XmlSerializer(type);
				serializers.Add(type, xmlSerializer);
			}

			return xmlSerializer.Deserialize(reader);
		}


		public static object DeserializeFromNetSerializer(XmlReader reader, Type type)
		{
			MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(reader.Value));
			object result = HunterSerializer.Deserialize(memoryStream, type);
			memoryStream.Close();
			return result;
		}


		public static object DeserializeBase64FromNetSerializer(string base64String, Type type)
		{
			MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(base64String));
			object result = HunterSerializer.Deserialize(memoryStream, type);
			memoryStream.Close();
			return result;
		}


		public static string SerializeBase64FromNetSerializer(object obj)
		{
			MemoryStream memoryStream = new MemoryStream(memoryStreamData);
			HunterSerializer.Serialize(memoryStream, obj);
			byte[] inArray = memoryStreamData.Take((int) memoryStream.Position).ToArray<byte>();
			memoryStream.Close();
			return Convert.ToBase64String(inArray);
		}


		public static List<DataType> ListDeserializeStreamFromNetSerializer<DataType>(MemoryStream dataStream)
		{
			Type typeFromHandle = typeof(DataType);
			int num;
			HunterSerializer.Primitives.ReadPrimitive(dataStream, out num);
			List<DataType> list = new List<DataType>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add((DataType) HunterSerializer.Deserialize(dataStream, typeFromHandle));
			}

			return list;
		}


		public static byte[] ListSerializeStreamFromNetSerializer<DataType>(List<DataType> datas)
		{
			MemoryStream memoryStream = new MemoryStream(32768);
			HunterSerializer.Primitives.WritePrimitive(memoryStream, datas.Count);
			foreach (DataType dataType in datas)
			{
				HunterSerializer.Serialize(memoryStream, dataType);
			}

			return memoryStream.GetBuffer().Take((int) memoryStream.Position).ToArray<byte>();
		}
	}
}