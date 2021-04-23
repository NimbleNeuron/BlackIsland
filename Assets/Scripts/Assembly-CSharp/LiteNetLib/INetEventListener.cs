using System.Net;
using System.Net.Sockets;

namespace LiteNetLib
{
	
	public interface INetEventListener
	{
		
		void OnPeerConnected(NetPeer peer);

		
		void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo);

		
		void OnNetworkError(IPEndPoint endPoint, SocketError socketError);

		
		void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);

		
		void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType);

		
		void OnNetworkLatencyUpdate(NetPeer peer, int latency);

		
		void OnConnectionRequest(ConnectionRequest request);
	}
}
