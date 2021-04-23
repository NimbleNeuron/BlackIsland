using System.Net;
using Tftp.Net.Transfer;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Trace
{
	
	internal class LoggingStateDecorator : ITransferState
	{
		
		
		
		public TftpTransfer Context
		{
			get
			{
				return this.decoratee.Context;
			}
			set
			{
				this.decoratee.Context = value;
			}
		}

		
		public LoggingStateDecorator(ITransferState decoratee, TftpTransfer transfer)
		{
			this.decoratee = decoratee;
			this.transfer = transfer;
		}

		
		public string GetStateName()
		{
			return "[" + this.decoratee.GetType().Name + "]";
		}

		
		public void OnStateEnter()
		{
			TftpTrace.Trace(this.GetStateName() + " OnStateEnter", this.transfer);
			this.decoratee.OnStateEnter();
		}

		
		public void OnStart()
		{
			TftpTrace.Trace(this.GetStateName() + " OnStart", this.transfer);
			this.decoratee.OnStart();
		}

		
		public void OnCancel(TftpErrorPacket reason)
		{
			TftpTrace.Trace(this.GetStateName() + " OnCancel: " + reason, this.transfer);
			this.decoratee.OnCancel(reason);
		}

		
		public void OnCommand(ITftpCommand command, EndPoint endpoint)
		{
			TftpTrace.Trace(string.Concat(new object[]
			{
				this.GetStateName(),
				" OnCommand: ",
				command,
				" from ",
				endpoint
			}), this.transfer);
			this.decoratee.OnCommand(command, endpoint);
		}

		
		public void OnTimer()
		{
			this.decoratee.OnTimer();
		}

		
		private readonly ITransferState decoratee;

		
		private readonly TftpTransfer transfer;
	}
}
