namespace Neptune.Http
{
	public static class HttpRequestStatesExtensions
	{
		public static bool IsFinal(this HttpRequestStates state)
		{
			return state >= HttpRequestStates.Done;
		}
		
		public static bool IsDone(this HttpRequestStates state)
		{
			return state == HttpRequestStates.Done;
		}
	}
}
