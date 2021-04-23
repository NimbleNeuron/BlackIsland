namespace Tftp.Net.Transfer.States
{
	
	internal class StartOutgoingRead : BaseState
	{
		
		public override void OnStart()
		{
			base.Context.SetState(new SendReadRequest());
		}

		
		public override void OnCancel(TftpErrorPacket reason)
		{
			base.Context.SetState(new Closed());
		}
	}
}
