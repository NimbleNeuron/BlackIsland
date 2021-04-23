namespace Neptune.Http
{
	
	public static class HttpResultStatesExtensions
	{
		public static bool IsOK(this HttpResultStates state)
		{
			return state == HttpResultStates.OK;
		}
	}
}
