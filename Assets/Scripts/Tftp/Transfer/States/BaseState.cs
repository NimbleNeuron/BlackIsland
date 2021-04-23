using System.Net;

namespace Tftp.Net.Transfer.States
{
	
	internal class BaseState : ITransferState
	{
		
		
		
		public TftpTransfer Context { get; set; }

		
		public virtual void OnStateEnter()
		{
		}

		
		public virtual void OnStart()
		{
		}

		
		public virtual void OnCancel(TftpErrorPacket reason)
		{
		}

		
		public virtual void OnCommand(ITftpCommand command, EndPoint endpoint)
		{
		}

		
		public virtual void OnTimer()
		{
		}
	}
}
