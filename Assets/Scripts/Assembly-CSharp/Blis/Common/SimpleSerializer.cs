using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace Blis.Common
{
	public class SimpleSerializer : ISerializer
	{
		public byte[] Serialize<T>(T obj)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
				{
					new BinaryFormatter().Serialize(deflateStream, obj);
				}

				result = memoryStream.ToArray();
			}

			return result;
		}


		public T Deserialize<T>(byte[] data)
		{
			T result = default;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
				{
					result = (T) new BinaryFormatter().Deserialize(deflateStream);
				}
			}

			return result;
		}
	}
}