namespace Neptune.WebSocket
{
	
	public enum WebSocketStatusCodes : ushort
	{
		
		Normal = 1000,
		
		GoingAway,
		
		ProtocolError,
		
		Unsupported,
		
		NoStatus = 1005,
		
		Abnormal,
		
		InconsistentData,
		
		ViolatePolicy,
		
		TooLarge,
		
		ClientError = 4000,
		
		UncleanClose
	}
}
