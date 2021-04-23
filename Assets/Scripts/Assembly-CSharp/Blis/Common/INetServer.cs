namespace Blis.Common
{
	public interface INetServer
	{
		void Init(INetServerHandler serverHandler, int port);


		void Send(int connectionId, byte[] data, NetChannel netChannel);


		void Broadcast(byte[] data, NetChannel netChannel);


		void Broadcast(int connectionId, byte[] data, NetChannel netChannel);


		void Disconnect(int connectionId);


		void Update();


		void Close();


		int GetRTT(int connectionId);


		int GetMtu(int connectionId);


		void LogConnectionMap();


		int GetPeerTimeSinceLastPacket(int connectionId);
	}
}