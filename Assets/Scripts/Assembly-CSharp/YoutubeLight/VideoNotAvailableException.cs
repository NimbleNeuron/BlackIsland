using System;

namespace YoutubeLight
{
	
	public class VideoNotAvailableException : Exception
	{
		
		public VideoNotAvailableException() { }

		
		public VideoNotAvailableException(string message) : base(message) { }
	}
}