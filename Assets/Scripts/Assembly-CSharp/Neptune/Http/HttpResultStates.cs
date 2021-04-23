namespace Neptune.Http
{
	
	public enum HttpResultStates : ushort
	{
		NotFinished,
		OK = 2,
		Redirected,
		ClientError,
		ServerError,
		Aborted,
		TimedOut,
		Exception
	}
}
