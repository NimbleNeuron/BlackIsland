using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States
{
	
	internal class ReceivedError : BaseState
	{
		
		public ReceivedError(Error error) : this(new TftpErrorPacket(error.ErrorCode, ReceivedError.GetNonEmptyErrorMessage(error)))
		{
		}

		
		private static string GetNonEmptyErrorMessage(Error error)
		{
			if (!string.IsNullOrEmpty(error.Message))
			{
				return error.Message;
			}
			return "(No error message provided)";
		}

		
		public ReceivedError(TftpTransferError error)
		{
			this.error = error;
		}

		
		public override void OnStateEnter()
		{
			TftpTrace.Trace("Received error: " + this.error, base.Context);
			base.Context.RaiseOnError(this.error);
			base.Context.SetState(new Closed());
		}

		
		private readonly TftpTransferError error;
	}
}
