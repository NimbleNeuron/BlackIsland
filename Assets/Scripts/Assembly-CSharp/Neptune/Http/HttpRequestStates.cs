namespace Neptune.Http
{
	public enum HttpRequestStates : ushort
	{
		Unsent,
		Opened,
		Sending,
		Sent,
		HeaderReceived,
		Loading,
		Retrying,
		Done,
		Aborted,
		Error,
		TimedOut
	}
}
