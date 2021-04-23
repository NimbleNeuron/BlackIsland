using System.Net;

namespace Tftp.Net.Transfer.States
{
	
	internal interface ITransferState
	{
		
		
		
		TftpTransfer Context { get; set; }

		
		void OnStateEnter();

		
		void OnStart();

		
		void OnCancel(TftpErrorPacket reason);

		
		void OnTimer();

		
		void OnCommand(ITftpCommand command, EndPoint endpoint);
	}
}
