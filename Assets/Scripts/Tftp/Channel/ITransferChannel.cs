using System;
using System.Net;

namespace Tftp.Net.Channel
{
	
	internal interface ITransferChannel : IDisposable
	{
		
		
		
		event TftpCommandHandler OnCommandReceived;

		
		
		
		event TftpChannelErrorHandler OnError;

		
		
		
		EndPoint RemoteEndpoint { get; set; }

		
		void Open();

		
		void Send(ITftpCommand command);
	}
}
