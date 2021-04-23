namespace Tftp.Net.Transfer.States
{
	
	internal class CancelledByUser : BaseState
	{
		
		public CancelledByUser(TftpErrorPacket reason)
		{
			this.reason = reason;
		}

		
		public override void OnStateEnter()
		{
			Error command = new Error(this.reason.ErrorCode, this.reason.ErrorMessage);
			base.Context.GetConnection().Send(command);
			base.Context.SetState(new Closed());
		}

		
		private readonly TftpErrorPacket reason;
	}
}
