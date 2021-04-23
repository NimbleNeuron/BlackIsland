using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Tftp.Net.Channel
{
	
	internal class UdpChannel : ITransferChannel, IDisposable
	{
		
		
		
		public event TftpCommandHandler OnCommandReceived;

		
		
		
		public event TftpChannelErrorHandler OnError;

		
		public UdpChannel(UdpClient client)
		{
			this.client = client;
			this.endpoint = null;
		}

		
		public void Open()
		{
			if (this.client == null)
			{
				throw new ObjectDisposedException("UdpChannel");
			}
			this.client.BeginReceive(new AsyncCallback(this.UdpReceivedCallback), null);
		}

		
		private void UdpReceivedCallback(IAsyncResult result)
		{
			IPEndPoint ipendPoint = new IPEndPoint(0L, 0);
			ITftpCommand tftpCommand = null;
			UdpChannel obj;
			try
			{
				byte[] message = null;
				obj = this;
				lock (obj)
				{
					if (this.client == null)
					{
						return;
					}
					message = this.client.EndReceive(result, ref ipendPoint);
				}
				tftpCommand = this.parser.Parse(message);
			}
			catch (SocketException exception)
			{
				this.RaiseOnError(new NetworkError(exception));
			}
			catch (TftpParserException exception2)
			{
				this.RaiseOnError(new NetworkError(exception2));
			}
			if (tftpCommand != null)
			{
				this.RaiseOnCommand(tftpCommand, ipendPoint);
			}
			obj = this;
			lock (obj)
			{
				if (this.client != null)
				{
					this.client.BeginReceive(new AsyncCallback(this.UdpReceivedCallback), null);
				}
			}
		}

		
		private void RaiseOnCommand(ITftpCommand command, IPEndPoint endpoint)
		{
			if (this.OnCommandReceived != null)
			{
				this.OnCommandReceived(command, endpoint);
			}
		}

		
		private void RaiseOnError(TftpTransferError error)
		{
			if (this.OnError != null)
			{
				this.OnError(error);
			}
		}

		
		public void Send(ITftpCommand command)
		{
			if (this.client == null)
			{
				throw new ObjectDisposedException("UdpChannel");
			}
			if (this.endpoint == null)
			{
				throw new InvalidOperationException("RemoteEndpoint needs to be set before you can send TFTP commands.");
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.serializer.Serialize(command, memoryStream);
				byte[] buffer = memoryStream.GetBuffer();
				this.client.Send(buffer, (int)memoryStream.Length, this.endpoint);
			}
		}

		
		public void Dispose()
		{
			lock (this)
			{
				if (this.client != null)
				{
					this.client.Close();
					this.client = null;
				}
			}
		}

		
		
		
		public EndPoint RemoteEndpoint
		{
			get
			{
				return this.endpoint;
			}
			set
			{
				if (!(value is IPEndPoint))
				{
					throw new NotSupportedException("UdpChannel can only connect to IPEndPoints.");
				}
				if (this.client == null)
				{
					throw new ObjectDisposedException("UdpChannel");
				}
				this.endpoint = (IPEndPoint)value;
			}
		}

		
		private IPEndPoint endpoint;

		
		private UdpClient client;

		
		private readonly CommandSerializer serializer = new CommandSerializer();

		
		private readonly CommandParser parser = new CommandParser();
	}
}
