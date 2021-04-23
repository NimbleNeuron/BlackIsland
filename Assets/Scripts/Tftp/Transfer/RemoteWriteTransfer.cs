using Tftp.Net.Channel;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer
{
	
	internal class RemoteWriteTransfer : TftpTransfer
	{
		
		public RemoteWriteTransfer(ITransferChannel connection, string filename) : base(connection, filename, new StartOutgoingWrite())
		{
		}
	}
}
