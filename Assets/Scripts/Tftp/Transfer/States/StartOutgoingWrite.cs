namespace Tftp.Net.Transfer.States
{
	
	internal class StartOutgoingWrite : BaseState
	{
		
		public override void OnStart()
		{
			base.Context.FillOrDisableTransferSizeOption();
			base.Context.SetState(new SendWriteRequest());
		}

		
		public override void OnCancel(TftpErrorPacket reason)
		{
			base.Context.SetState(new Closed());
		}
	}
}
