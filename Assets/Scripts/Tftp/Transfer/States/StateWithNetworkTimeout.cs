using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States
{
	
	internal class StateWithNetworkTimeout : BaseState
	{
		
		public override void OnStateEnter()
		{
			this.timer = new SimpleTimer(base.Context.RetryTimeout);
		}

		
		public override void OnTimer()
		{
			if (this.timer.IsTimeout())
			{
				TftpTrace.Trace("Network timeout.", base.Context);
				this.timer.Restart();
				int num = this.retriesUsed;
				this.retriesUsed = num + 1;
				if (num >= base.Context.RetryCount)
				{
					TftpTransferError error = new TimeoutError(base.Context.RetryTimeout, base.Context.RetryCount);
					base.Context.SetState(new ReceivedError(error));
					return;
				}
				this.HandleTimeout();
			}
		}

		
		private void HandleTimeout()
		{
			if (this.lastCommand != null)
			{
				base.Context.GetConnection().Send(this.lastCommand);
			}
		}

		
		protected void SendAndRepeat(ITftpCommand command)
		{
			base.Context.GetConnection().Send(command);
			this.lastCommand = command;
			this.ResetTimeout();
		}

		
		protected void ResetTimeout()
		{
			this.timer.Restart();
			this.retriesUsed = 0;
		}

		
		private SimpleTimer timer;

		
		private ITftpCommand lastCommand;

		
		private int retriesUsed;
	}
}
