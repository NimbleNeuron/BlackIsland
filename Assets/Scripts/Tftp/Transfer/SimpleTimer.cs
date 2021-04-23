using System;

namespace Tftp.Net.Transfer
{
	
	internal class SimpleTimer
	{
		
		public SimpleTimer(TimeSpan timeout)
		{
			this.timeout = timeout;
			this.Restart();
		}

		
		public void Restart()
		{
			this.nextTimeout = DateTime.Now.Add(this.timeout);
		}

		
		public bool IsTimeout()
		{
			return DateTime.Now >= this.nextTimeout;
		}

		
		private DateTime nextTimeout;

		
		private readonly TimeSpan timeout;
	}
}
