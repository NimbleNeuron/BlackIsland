namespace LiteNetLib
{
	
	internal enum PacketProperty : byte
	{
		
		Unreliable,

		
		Channeled,

		
		Ack,

		
		Ping,

		
		Pong,

		
		ConnectRequest,

		
		ConnectAccept,

		
		Disconnect,

		
		UnconnectedMessage,

		
		MtuCheck,

		
		MtuOk,

		
		Broadcast,

		
		Merged,

		
		ShutdownOk,

		
		PeerNotFound,

		
		InvalidProtocol,

		
		NatMessage,

		
		Empty
	}
}