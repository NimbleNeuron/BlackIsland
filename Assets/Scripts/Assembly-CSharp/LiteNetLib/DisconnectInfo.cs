using System.Net.Sockets;

namespace LiteNetLib
{
	
	public struct DisconnectInfo
	{
		
		public DisconnectReason Reason;

		
		public SocketError SocketErrorCode;

		
		public NetPacketReader AdditionalData;
	}
}