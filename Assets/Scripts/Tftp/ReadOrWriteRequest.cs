using System.Collections.Generic;

namespace Tftp.Net
{
	
	internal abstract class ReadOrWriteRequest
	{
		
		
		
		public string Filename { get; private set; }

		
		
		
		public TftpTransferMode Mode { get; private set; }

		
		
		
		public IEnumerable<TransferOption> Options { get; private set; }

		
		protected ReadOrWriteRequest(ushort opCode, string filename, TftpTransferMode mode, IEnumerable<TransferOption> options)
		{
			this.opCode = opCode;
			this.Filename = filename;
			this.Mode = mode;
			this.Options = options;
		}

		
		private readonly ushort opCode;
	}
}
