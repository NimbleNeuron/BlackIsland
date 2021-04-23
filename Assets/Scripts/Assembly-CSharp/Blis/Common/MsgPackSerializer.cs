using MessagePack;

namespace Blis.Common
{
	public class MsgPackSerializer : ISerializer
	{
		public byte[] Serialize<T>(T obj)
		{
			return MessagePackSerializer.Serialize<T>(obj);
		}


		public T Deserialize<T>(byte[] data)
		{
			return MessagePackSerializer.Deserialize<T>(data);
		}
	}
}