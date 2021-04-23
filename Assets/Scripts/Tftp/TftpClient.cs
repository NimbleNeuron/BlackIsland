using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Tftp.Net.Channel;
using Tftp.Net.Transfer;

namespace Tftp.Net
{
	
	public class TftpClient
	{
		
		public TftpClient(IPEndPoint remoteAddress)
		{
			this.remoteAddress = remoteAddress;
		}

		
		public TftpClient(IPAddress ip, int port) : this(new IPEndPoint(ip, port))
		{
		}

		
		public TftpClient(IPAddress ip) : this(new IPEndPoint(ip, 69))
		{
		}

		
		public TftpClient(string host) : this(host, 69)
		{
		}

		
		public TftpClient(string host, int port)
		{
			IPAddress ipaddress = Dns.GetHostAddresses(host).FirstOrDefault((IPAddress x) => x.AddressFamily == AddressFamily.InterNetwork);
			if (ipaddress == null)
			{
				throw new ArgumentException("Could not convert '" + host + "' to an IPv4 address.", "host");
			}
			this.remoteAddress = new IPEndPoint(ipaddress, port);
		}

		
		public ITftpTransfer Download(string filename)
		{
			return new RemoteReadTransfer(TransferChannelFactory.CreateConnection(this.remoteAddress), filename);
		}

		
		public ITftpTransfer Upload(string filename)
		{
			return new RemoteWriteTransfer(TransferChannelFactory.CreateConnection(this.remoteAddress), filename);
		}

		
		private const int DEFAULT_SERVER_PORT = 69;

		
		private readonly IPEndPoint remoteAddress;
	}
}
