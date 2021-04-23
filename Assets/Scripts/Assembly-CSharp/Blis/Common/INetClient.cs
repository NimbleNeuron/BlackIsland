namespace Blis.Common
{
	public interface INetClient
	{
		void Init(INetClientHandler handler, long uniqueId);


		void Open(string ip, int port);


		void Send(byte[] data, NetChannel netChannel);


		void Update();


		void SetSimulation(int minLatency, int maxLatency, int packetLoss);


		int GetLatency();


		void Close();
	}
}