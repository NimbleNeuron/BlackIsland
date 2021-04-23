using System;

namespace Tftp.Net
{
	
	[Serializable]
	internal class TftpParserException : Exception
	{
		
		public TftpParserException(string message) : base(message)
		{
		}

		
		public TftpParserException(Exception e) : base("Error while parsing message.", e)
		{
		}
	}
}
