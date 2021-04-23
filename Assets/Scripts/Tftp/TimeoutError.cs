using System;

namespace Tftp.Net
{
	
	public class TimeoutError : TftpTransferError
	{
		
		public TimeoutError(TimeSpan retryTimeout, int retryCount)
		{
			this.RetryTimeout = retryTimeout;
			this.RetryCount = retryCount;
		}

		
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Timeout error. RetryTimeout (",
				this.RetryCount,
				") violated more than ",
				this.RetryCount,
				" times in a row"
			});
		}

		
		private readonly TimeSpan RetryTimeout;

		
		private readonly int RetryCount;
	}
}
