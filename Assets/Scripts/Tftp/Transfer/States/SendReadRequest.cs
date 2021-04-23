using System.Net;

namespace Tftp.Net.Transfer.States
{
	
	internal class SendReadRequest : StateWithNetworkTimeout
	{
		
		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.SendRequest();
		}

		
		private void SendRequest()
		{
			ReadRequest command = new ReadRequest(base.Context.Filename, base.Context.TransferMode, base.Context.ProposedOptions.ToOptionList());
			base.SendAndRepeat(command);
		}

		
		public override void OnCommand(ITftpCommand command, EndPoint endpoint)
		{
			if (command is Data || command is OptionAcknowledgement)
			{
				base.Context.GetConnection().RemoteEndpoint = endpoint;
			}
			if (command is Data)
			{
				if (base.Context.NegotiatedOptions == null)
				{
					base.Context.FinishOptionNegotiation(TransferOptionSet.NewEmptySet());
				}
				ITransferState transferState = new Receiving();
				base.Context.SetState(transferState);
				transferState.OnCommand(command, endpoint);
				return;
			}
			if (command is OptionAcknowledgement)
			{
				base.Context.FinishOptionNegotiation(new TransferOptionSet((command as OptionAcknowledgement).Options));
				base.SendAndRepeat(new Acknowledgement(0));
				return;
			}
			if (command is Error)
			{
				base.Context.SetState(new ReceivedError((Error)command));
				return;
			}
			base.OnCommand(command, endpoint);
		}

		
		public override void OnCancel(TftpErrorPacket reason)
		{
			base.Context.SetState(new CancelledByUser(reason));
		}
	}
}
