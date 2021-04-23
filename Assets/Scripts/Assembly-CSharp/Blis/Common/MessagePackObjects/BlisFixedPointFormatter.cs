using MessagePack;
using MessagePack.Formatters;

namespace Blis.Common
{
	
	public class BlisFixedPointFormatter : IMessagePackFormatter<BlisFixedPoint>, IMessagePackFormatter
	{
		// co : outdated
		// 
		// public int Serialize(ref byte[] bytes, int offset, BlisFixedPoint value, IFormatterResolver formatterResolver)
		// {
		// 	if (value == null)
		// 	{
		// 		return MessagePackBinary.WriteNil(ref bytes, offset);
		// 	}
		// 	int num = offset;
		// 	int num2 = MessagePackBinary.WriteInt32(ref bytes, offset, value.InternalValue);
		// 	offset += num2;
		// 	return offset - num;
		// }
		//
		// 
		// public BlisFixedPoint Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		// {
		// 	if (MessagePackBinary.IsNil(bytes, offset))
		// 	{
		// 		readSize = 1;
		// 		return new BlisFixedPoint();
		// 	}
		// 	BlisFixedPoint blisFixedPoint = new BlisFixedPoint();
		// 	int internalValue = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
		// 	blisFixedPoint.InternalValue = internalValue;
		// 	return blisFixedPoint;
		// }
		
		public void Serialize(ref MessagePackWriter writer, BlisFixedPoint value, MessagePackSerializerOptions options)
		{
			if (value == null)
			{
				writer.WriteNil();
			}
			else
			{
				writer.Write(value.InternalValue);
			}
		}

		public BlisFixedPoint Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
		{
			if (reader.TryReadNil())
			{
				return default;
			}
			else
			{
				BlisFixedPoint blisFixedPoint = new BlisFixedPoint();
				int internalValue = reader.ReadInt32();
				blisFixedPoint.InternalValue = internalValue;

				return blisFixedPoint;
			}
		}
	}
}
