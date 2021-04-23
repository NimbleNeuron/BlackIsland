using System;
using System.Net;
using System.Net.Sockets;

namespace Tftp.Net.Channel
{
	
	internal static class TransferChannelFactory
	{
		
		public static ITransferChannel CreateServer(EndPoint localAddress)
		{
			if (localAddress is IPEndPoint)
			{
				return TransferChannelFactory.CreateServerUdp((IPEndPoint)localAddress);
			}
			throw new NotSupportedException("Unsupported endpoint type.");
		}

		
		public static ITransferChannel CreateConnection(EndPoint remoteAddress)
		{
			if (remoteAddress is IPEndPoint)
			{
				return TransferChannelFactory.CreateConnectionUdp((IPEndPoint)remoteAddress);
			}
			throw new NotSupportedException("Unsupported endpoint type.");
		}

		
		private static ITransferChannel CreateServerUdp(IPEndPoint localAddress)
		{
			return new UdpChannel(new UdpClient(localAddress));
		}

		
		private static ITransferChannel CreateConnectionUdp(IPEndPoint remoteAddress)
		{
			return new UdpChannel(new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
			{
				RemoteEndpoint = remoteAddress
			};
		}
	}
}
