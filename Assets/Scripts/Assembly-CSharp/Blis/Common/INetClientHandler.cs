namespace Blis.Common
{
	public interface INetClientHandler
	{
		void OnConnected();


		void OnDisconnected();


		void OnError(int errorCode);


		void OnRecv(byte[] buffer);
	}
}