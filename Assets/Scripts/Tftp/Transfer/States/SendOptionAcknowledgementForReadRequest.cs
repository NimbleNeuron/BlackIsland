namespace Tftp.Net.Transfer.States
{
	
	internal class SendOptionAcknowledgementForReadRequest : SendOptionAcknowledgementBase
	{
		
		public override void OnAcknowledgement(Acknowledgement command)
		{
			if (command.BlockNumber == 0)
			{
				base.Context.SetState(new Sending());
			}
		}
	}
}
