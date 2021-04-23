namespace Blis.Common
{
	public interface ISerializer
	{
		byte[] Serialize<T>(T obj);


		T Deserialize<T>(byte[] data);
	}
}