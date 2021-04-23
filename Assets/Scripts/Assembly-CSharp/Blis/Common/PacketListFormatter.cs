using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace Blis.Common
{
	
	public class PacketListFormatter : IMessagePackFormatter<List<PacketWrapper>>, IMessagePackFormatter
	{
		// 
		// public int Serialize(ref byte[] bytes, int offset, List<PacketWrapper> value, IFormatterResolver formatterResolver)
		// {
		// 	if (value == null || value.Count == 0)
		// 	{
		// 		return MessagePackBinary.WriteNil(ref bytes, offset);
		// 	}
		// 	int num = offset;
		// 	offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.Count);
		// 	foreach (PacketWrapper packetWrapper in value)
		// 	{
		// 		offset += MessagePackBinary.WriteInt32(ref bytes, offset, (int)packetWrapper.packetType);
		// 		int num2 = MessagePackBinary.WriteBytes(ref bytes, offset, packetWrapper.data);
		// 		offset += num2;
		// 	}
		// 	return offset - num;
		// }
		//
		// 
		// public List<PacketWrapper> Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		// {
		// 	if (MessagePackBinary.IsNil(bytes, offset))
		// 	{
		// 		readSize = 1;
		// 		return new List<PacketWrapper>();
		// 	}
		// 	List<PacketWrapper> list = new List<PacketWrapper>();
		// 	int num = offset;
		// 	int num2 = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
		// 	offset += readSize;
		// 	for (int i = 0; i < num2; i++)
		// 	{
		// 		PacketType packetType = (PacketType)MessagePackBinary.ReadInt32(bytes, offset, out readSize);
		// 		offset += readSize;
		// 		byte[] data = MessagePackBinary.ReadBytes(bytes, offset, out readSize);
		// 		offset += readSize;
		// 		list.Add(new PacketWrapper
		// 		{
		// 			packetType = packetType,
		// 			data = data
		// 		});
		// 	}
		// 	readSize = offset - num;
		// 	return list;
		// }

		public void Serialize(ref MessagePackWriter writer, List<PacketWrapper> value, MessagePackSerializerOptions options)
		{
			if (value == null || value.Count == 0)
			{
				writer.WriteNil();
			}
			else
			{
				writer.WriteInt32(value.Count);

				foreach (PacketWrapper packetWrapper in value)
				{
					writer.WriteInt32((int)packetWrapper.packetType);
					writer.Write(packetWrapper.data);
				}
			}
		}

		public List<PacketWrapper> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
		{
			if (reader.TryReadNil())
			{
				return new List<PacketWrapper>();
			}
			else
			{
				List<PacketWrapper> list = new List<PacketWrapper>();
				int count = reader.ReadInt32();
				
				for (int i = 0; i < count; i++)
				{
					PacketType packetType = (PacketType) reader.ReadInt32();
					var data = reader.ReadBytes();

					if (data != null)
					{
						List<byte> bytes = new List<byte>();
						foreach (ReadOnlyMemory<byte> readByte in data)
						{
							bytes.AddRange(readByte.ToArray());
						}
					
						list.Add(new PacketWrapper()
						{
							packetType = packetType,
							data = bytes.ToArray()
						});	
					}
				}

				return list;
			}
		}
	}
}
