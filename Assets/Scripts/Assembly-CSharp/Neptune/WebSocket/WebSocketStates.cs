namespace Neptune.WebSocket
{
	
	public enum WebSocketStates : ushort
	{
		
		Connecting,
		
		Handshaking,
		
		Open,
		
		Closing,
		
		Closed
	}
}
