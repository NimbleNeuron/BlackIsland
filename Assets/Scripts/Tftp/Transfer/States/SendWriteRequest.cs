using System.Net;

namespace Tftp.Net.Transfer.States
{
	
	internal class SendWriteRequest : StateWithNetworkTimeout
	{
		
		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.SendRequest();
		}

		
		private void SendRequest()
		{
			WriteRequest command = new WriteRequest(base.Context.Filename, base.Context.TransferMode, base.Context.ProposedOptions.ToOptionList());
			base.SendAndRepeat(command);
		}

		
		public override void OnCommand(ITftpCommand command, EndPoint endpoint)
		{
			if (command is OptionAcknowledgement)
			{
				TransferOptionSet negotiated = new TransferOptionSet((command as OptionAcknowledgement).Options);
				base.Context.FinishOptionNegotiation(negotiated);
				this.BeginSendingTo(endpoint);
				return;
			}
			if (command is Acknowledgement && (command as Acknowledgement).BlockNumber == 0)
			{
				base.Context.FinishOptionNegotiation(TransferOptionSet.NewEmptySet());
				this.BeginSendingTo(endpoint);
				return;
			}
			if (command is Error)
			{
				Error error = (Error)command;
				base.Context.SetState(new ReceivedError(error));
				return;
			}
			base.OnCommand(command, endpoint);
		}

		
		private void BeginSendingTo(EndPoint endpoint)
		{
			base.Context.GetConnection().RemoteEndpoint = endpoint;
			base.Context.SetState(new Sending());
		}

		
		public override void OnCancel(TftpErrorPacket reason)
		{
			base.Context.SetState(new CancelledByUser(reason));
		}
	}
}
