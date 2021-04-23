using MessagePack;

namespace Blis.Common
{
	
	public class LZ4MsgPackSerializer : ISerializer
	{
		
		public byte[] Serialize<T>(T obj)
		{
			MessagePackSerializerOptions lz4Options =
				MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);
			return MessagePackSerializer.Serialize<T>(obj, lz4Options);
			// return LZ4MessagePackSerializer.Serialize<T>(obj);
		}

		
		public T Deserialize<T>(byte[] data)
		{
			MessagePackSerializerOptions lz4Options =
				MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);
			return MessagePackSerializer.Deserialize<T>(data, lz4Options);
			// return LZ4MessagePackSerializer.Deserialize<T>(data);
		}
	}
}