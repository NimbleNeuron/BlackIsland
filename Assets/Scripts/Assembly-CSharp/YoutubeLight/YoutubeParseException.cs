using System;

namespace YoutubeLight
{
	
	public class YoutubeParseException : Exception
	{
		
		public YoutubeParseException(string message, Exception innerException) : base(message, innerException) { }
	}
}