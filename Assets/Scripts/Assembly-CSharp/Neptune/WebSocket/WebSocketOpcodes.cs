namespace Neptune.WebSocket
{
	
	public enum WebSocketOpcodes : byte
	{
		
		Continuation,
		
		Text,
		
		Binary,
		
		ConnectionClose = 8,
		
		Ping,
		
		Pong
	}
}
