using System.Net;
using System.Net.Sockets;

namespace LiteNetLib
{
	
	internal sealed class NetEvent
	{
		
		public enum EType
		{
			
			Connect,

			
			Disconnect,

			
			Receive,

			
			ReceiveUnconnected,

			
			Error,

			
			ConnectionLatencyUpdated,

			
			Broadcast,

			
			ConnectionRequest,

			
			MessageDelivered
		}

		
		public readonly NetPacketReader DataReader;

		
		public ConnectionRequest ConnectionRequest;

		
		public DeliveryMethod DeliveryMethod;

		
		public DisconnectReason DisconnectReason;

		
		public SocketError ErrorCode;

		
		public int Latency;

		
		public NetPeer Peer;

		
		public IPEndPoint RemoteEndPoint;

		
		public EType Type;

		
		public object UserData;

		
		public NetEvent(NetManager manager)
		{
			DataReader = new NetPacketReader(manager, this);
		}
	}
}