using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer
{
	
	internal class SendOptionAcknowledgementBase : StateThatExpectsMessagesFromDefaultEndPoint
	{
		
		public override void OnStateEnter()
		{
			base.OnStateEnter();
			base.SendAndRepeat(new OptionAcknowledgement(base.Context.NegotiatedOptions.ToOptionList()));
		}

		
		public override void OnError(Error command)
		{
			base.Context.SetState(new ReceivedError(command));
		}

		
		public override void OnCancel(TftpErrorPacket reason)
		{
			base.Context.SetState(new CancelledByUser(reason));
		}
	}
}
