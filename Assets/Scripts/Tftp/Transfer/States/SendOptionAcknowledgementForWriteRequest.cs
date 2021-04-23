namespace Tftp.Net.Transfer.States
{
	
	internal class SendOptionAcknowledgementForWriteRequest : SendOptionAcknowledgementBase
	{
		
		public override void OnData(Data command)
		{
			if (command.BlockNumber == 1)
			{
				ITransferState transferState = new Receiving();
				base.Context.SetState(transferState);
				transferState.OnCommand(command, base.Context.GetConnection().RemoteEndpoint);
			}
		}
	}
}
