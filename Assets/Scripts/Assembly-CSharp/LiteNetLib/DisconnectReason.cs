namespace LiteNetLib
{
	
	public enum DisconnectReason
	{
		
		ConnectionFailed,

		
		Timeout,

		
		HostUnreachable,

		
		NetworkUnreachable,

		
		RemoteConnectionClose,

		
		DisconnectPeerCalled,

		
		ConnectionRejected,

		
		InvalidProtocol,

		
		UnknownHost,

		
		Reconnect,

		
		PeerToPeerConnection
	}
}