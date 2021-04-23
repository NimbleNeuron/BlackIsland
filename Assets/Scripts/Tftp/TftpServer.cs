using System;
using System.Net;
using Tftp.Net.Channel;
using Tftp.Net.Transfer;

namespace Tftp.Net
{
	
	public class TftpServer : IDisposable
	{
		
		
		
		public event TftpServerEventHandler OnReadRequest;

		
		
		
		public event TftpServerEventHandler OnWriteRequest;

		
		
		
		public event TftpServerErrorHandler OnError;

		
		public TftpServer(IPEndPoint localAddress)
		{
			if (localAddress == null)
			{
				throw new ArgumentNullException("localAddress");
			}
			this.serverSocket = TransferChannelFactory.CreateServer(localAddress);
			this.serverSocket.OnCommandReceived += this.serverSocket_OnCommandReceived;
			this.serverSocket.OnError += this.serverSocket_OnError;
		}

		
		public TftpServer(IPAddress localAddress) : this(localAddress, 69)
		{
		}

		
		public TftpServer(IPAddress localAddress, int port) : this(new IPEndPoint(localAddress, port))
		{
		}

		
		public TftpServer(int port) : this(new IPEndPoint(IPAddress.Any, port))
		{
		}

		
		public TftpServer() : this(69)
		{
		}

		
		public void Start()
		{
			this.serverSocket.Open();
		}

		
		private void serverSocket_OnError(TftpTransferError error)
		{
			this.RaiseOnError(error);
		}

		
		private void serverSocket_OnCommandReceived(ITftpCommand command, EndPoint endpoint)
		{
			if (!(command is ReadOrWriteRequest))
			{
				return;
			}
			ITransferChannel connection = TransferChannelFactory.CreateConnection(endpoint);
			ReadOrWriteRequest readOrWriteRequest = (ReadOrWriteRequest)command;
			ITftpTransfer tftpTransfer2;
			if (!(readOrWriteRequest is ReadRequest))
			{
				ITftpTransfer tftpTransfer = new LocalWriteTransfer(connection, readOrWriteRequest.Filename, readOrWriteRequest.Options);
				tftpTransfer2 = tftpTransfer;
			}
			else
			{
				ITftpTransfer tftpTransfer = new LocalReadTransfer(connection, readOrWriteRequest.Filename, readOrWriteRequest.Options);
				tftpTransfer2 = tftpTransfer;
			}
			ITftpTransfer transfer = tftpTransfer2;
			if (command is ReadRequest)
			{
				this.RaiseOnReadRequest(transfer, endpoint);
				return;
			}
			if (command is WriteRequest)
			{
				this.RaiseOnWriteRequest(transfer, endpoint);
				return;
			}
			throw new Exception("Unexpected tftp transfer request: " + command);
		}

		
		public void Dispose()
		{
			this.serverSocket.Dispose();
		}

		
		private void RaiseOnError(TftpTransferError error)
		{
			if (this.OnError != null)
			{
				this.OnError(error);
			}
		}

		
		private void RaiseOnWriteRequest(ITftpTransfer transfer, EndPoint client)
		{
			if (this.OnWriteRequest != null)
			{
				this.OnWriteRequest(transfer, client);
				return;
			}
			transfer.Cancel(new TftpErrorPacket(0, "Server did not provide a OnWriteRequest handler."));
		}

		
		private void RaiseOnReadRequest(ITftpTransfer transfer, EndPoint client)
		{
			if (this.OnReadRequest != null)
			{
				this.OnReadRequest(transfer, client);
				return;
			}
			transfer.Cancel(new TftpErrorPacket(0, "Server did not provide a OnReadRequest handler."));
		}

		
		public const int DEFAULT_SERVER_PORT = 69;

		
		private readonly ITransferChannel serverSocket;
	}
}
