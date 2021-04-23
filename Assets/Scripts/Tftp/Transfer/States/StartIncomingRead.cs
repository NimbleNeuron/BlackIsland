using System.Collections.Generic;

namespace Tftp.Net.Transfer.States
{
	
	internal class StartIncomingRead : BaseState
	{
		
		public StartIncomingRead(IEnumerable<TransferOption> optionsRequestedByClient)
		{
			this.optionsRequestedByClient = optionsRequestedByClient;
		}

		
		public override void OnStateEnter()
		{
			base.Context.ProposedOptions = new TransferOptionSet(this.optionsRequestedByClient);
		}

		
		public override void OnStart()
		{
			base.Context.FillOrDisableTransferSizeOption();
			base.Context.FinishOptionNegotiation(base.Context.ProposedOptions);
			if (base.Context.NegotiatedOptions.ToOptionList().Count > 0)
			{
				base.Context.SetState(new SendOptionAcknowledgementForReadRequest());
				return;
			}
			base.Context.SetState(new Sending());
		}

		
		public override void OnCancel(TftpErrorPacket reason)
		{
			base.Context.SetState(new CancelledByUser(reason));
		}

		
		private readonly IEnumerable<TransferOption> optionsRequestedByClient;
	}
}
