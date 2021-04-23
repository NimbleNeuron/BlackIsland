using System.Collections.Generic;

namespace Tftp.Net
{
	
	internal class OptionAcknowledgement : ITftpCommand
	{
		
		
		
		public IEnumerable<TransferOption> Options { get; private set; }

		
		public OptionAcknowledgement(IEnumerable<TransferOption> options)
		{
			this.Options = options;
		}

		
		public void Visit(ITftpCommandVisitor visitor)
		{
			visitor.OnOptionAcknowledgement(this);
		}

		
		public const ushort OpCode = 6;
	}
}
