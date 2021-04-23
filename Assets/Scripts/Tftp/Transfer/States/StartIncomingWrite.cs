using System.Collections.Generic;

namespace Tftp.Net.Transfer.States
{
	
	internal class StartIncomingWrite : BaseState
	{
		
		public StartIncomingWrite(IEnumerable<TransferOption> optionsRequestedByClient)
		{
			this.optionsRequestedByClient = optionsRequestedByClient;
		}

		
		public override void OnStateEnter()
		{
			base.Context.ProposedOptions = new TransferOptionSet(this.optionsRequestedByClient);
		}

		
		public override void OnStart()
		{
			base.Context.FinishOptionNegotiation(base.Context.ProposedOptions);
			if (base.Context.NegotiatedOptions.ToOptionList().Count > 0)
			{
				base.Context.SetState(new SendOptionAcknowledgementForWriteRequest());
				return;
			}
			base.Context.SetState(new AcknowledgeWriteRequest());
		}

		
		public override void OnCancel(TftpErrorPacket reason)
		{
			base.Context.SetState(new CancelledByUser(reason));
		}

		
		private readonly IEnumerable<TransferOption> optionsRequestedByClient;
	}
}
