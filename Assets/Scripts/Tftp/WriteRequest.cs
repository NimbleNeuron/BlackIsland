using System.Collections.Generic;

namespace Tftp.Net
{
	
	internal class WriteRequest : ReadOrWriteRequest, ITftpCommand
	{
		
		public WriteRequest(string filename, TftpTransferMode mode, IEnumerable<TransferOption> options) : base(2, filename, mode, options)
		{
		}

		
		public void Visit(ITftpCommandVisitor visitor)
		{
			visitor.OnWriteRequest(this);
		}

		
		public const ushort OpCode = 2;
	}
}
