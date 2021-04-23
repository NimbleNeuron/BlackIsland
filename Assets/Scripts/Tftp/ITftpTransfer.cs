using System;
using System.IO;

namespace Tftp.Net
{
	
	public interface ITftpTransfer : IDisposable
	{
		
		
		
		event TftpProgressHandler OnProgress;

		
		
		
		event TftpEventHandler OnFinished;

		
		
		
		event TftpErrorHandler OnError;

		
		
		
		TftpTransferMode TransferMode { get; set; }

		
		
		
		int BlockSize { get; set; }

		
		
		
		TimeSpan RetryTimeout { get; set; }

		
		
		
		int RetryCount { get; set; }

		
		
		
		BlockCounterWrapAround BlockCounterWrapping { get; set; }

		
		
		
		long ExpectedSize { get; set; }

		
		
		string Filename { get; }

		
		
		
		object UserContext { get; set; }

		
		void Start(Stream data);

		
		void Cancel(TftpErrorPacket reason);
	}
}
