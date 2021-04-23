namespace Blis.Common
{
	public interface INetServerHandler
	{
		void OnStartServer();


		void OnConnected(int connectionId);


		void OnDisconnected(int connectionId);


		void OnRecv(int connectionId, byte[] data);
	}
}