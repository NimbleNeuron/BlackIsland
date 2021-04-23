using System.Collections.Generic;

namespace Tftp.Net
{
	
	internal class ReadRequest : ReadOrWriteRequest, ITftpCommand
	{
		
		public ReadRequest(string filename, TftpTransferMode mode, IEnumerable<TransferOption> options) : base(1, filename, mode, options)
		{
		}

		
		public void Visit(ITftpCommandVisitor visitor)
		{
			visitor.OnReadRequest(this);
		}

		
		public const ushort OpCode = 1;
	}
}
