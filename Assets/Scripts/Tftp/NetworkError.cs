using System;

namespace Tftp.Net
{
	
	public class NetworkError : TftpTransferError
	{
		
		
		
		public Exception Exception { get; private set; }

		
		public NetworkError(Exception exception)
		{
			this.Exception = exception;
		}

		
		public override string ToString()
		{
			return this.Exception.ToString();
		}
	}
}
