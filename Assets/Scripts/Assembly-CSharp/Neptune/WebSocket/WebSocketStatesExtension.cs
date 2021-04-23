namespace Neptune.WebSocket
{
	
	public static class WebSocketStatesExtension
	{
		
		public static bool IsOpen(this WebSocketStates state)
		{
			return state == WebSocketStates.Open;
		}
	}
}
