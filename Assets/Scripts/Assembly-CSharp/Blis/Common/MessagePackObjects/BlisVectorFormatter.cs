using MessagePack;
using MessagePack.Formatters;

namespace Blis.Common
{
	public class BlisVectorFormatter : IMessagePackFormatter<BlisVector>, IMessagePackFormatter
	{
		// 
		// public int Serialize(ref byte[] bytes, int offset, BlisVector value, IFormatterResolver formatterResolver)
		// {
		// 	if (value == null)
		// 	{
		// 		return MessagePackBinary.WriteNil(ref bytes, offset);
		// 	}
		// 	int num = offset;
		// 	offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.x);
		// 	offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.y);
		// 	offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.z);
		// 	return offset - num;
		// }
		//
		// 
		// BlisVector IMessagePackFormatter<BlisVector>.Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		// {
		// 	if (MessagePackBinary.IsNil(bytes, offset))
		// 	{
		// 		readSize = 1;
		// 		return null;
		// 	}
		// 	int num = offset;
		// 	int x = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
		// 	offset += readSize;
		// 	int y = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
		// 	offset += readSize;
		// 	int z = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
		// 	offset += readSize;
		// 	readSize = offset - num;
		// 	return new BlisVector(x, y, z);
		// }
		//
		public void Serialize(ref MessagePackWriter writer, BlisVector value, MessagePackSerializerOptions options)
		{
			if (value == null)
			{
				writer.WriteNil();
			}
			else
			{
				writer.WriteInt32(value.x);
				writer.WriteInt32(value.y);
				writer.WriteInt32(value.z);
			}
		}

		public BlisVector Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
		{
			if (reader.TryReadNil())
			{
				return default;
			}

			int x = reader.ReadInt32();
			int y = reader.ReadInt32();
			int z = reader.ReadInt32();

			return new BlisVector(x, y, z);
		}
	}
}