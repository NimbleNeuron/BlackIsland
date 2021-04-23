using System;
using System.Net;
using System.Net.Sockets;

namespace LiteNetLib.Utils
{
	
	public sealed class NtpRequest : INetSocketListener
	{
		
		private NtpRequest(IPEndPoint endPoint, Action<NtpPacket> onRequestComplete)
		{
			this._ntpEndPoint = endPoint;
			this._onRequestComplete = onRequestComplete;
			this._socket = new NetSocket(this);
			this._socket.Bind(IPAddress.Any, IPAddress.IPv6Any, 0, false, (endPoint.AddressFamily == AddressFamily.InterNetworkV6) ? IPv6Mode.SeparateSocket : IPv6Mode.Disabled);
		}

		
		public static NtpRequest Create(IPEndPoint endPoint, Action<NtpPacket> onRequestComplete)
		{
			return new NtpRequest(endPoint, onRequestComplete);
		}

		
		public static NtpRequest Create(IPAddress ipAddress, Action<NtpPacket> onRequestComplete)
		{
			return NtpRequest.Create(new IPEndPoint(ipAddress, 123), onRequestComplete);
		}

		
		public static NtpRequest Create(string ntpServerAddress, int port, Action<NtpPacket> onRequestComplete)
		{
			return NtpRequest.Create(NetUtils.MakeEndPoint(ntpServerAddress, port), onRequestComplete);
		}

		
		public static NtpRequest Create(string ntpServerAddress, Action<NtpPacket> onRequestComplete)
		{
			return NtpRequest.Create(NetUtils.MakeEndPoint(ntpServerAddress, 123), onRequestComplete);
		}

		
		public void Send()
		{
			SocketError socketError = SocketError.Success;
			NtpPacket ntpPacket = new NtpPacket();
			ntpPacket.ValidateRequest();
			byte[] bytes = ntpPacket.Bytes;
			int num = this._socket.SendTo(bytes, 0, bytes.Length, this._ntpEndPoint, ref socketError);
			if (socketError != SocketError.Success || num != bytes.Length)
			{
				this._onRequestComplete(null);
			}
		}

		
		public void Close()
		{
			this._socket.Close(false);
		}

		
		void INetSocketListener.OnMessageReceived(byte[] data, int length, SocketError errorCode, IPEndPoint remoteEndPoint)
		{
			DateTime utcNow = DateTime.UtcNow;
			if (!remoteEndPoint.Equals(this._ntpEndPoint))
			{
				return;
			}
			if (length < 48)
			{
				this._onRequestComplete(null);
				return;
			}
			NtpPacket ntpPacket = NtpPacket.FromServerResponse(data, utcNow);
			try
			{
				ntpPacket.ValidateReply();
			}
			catch (InvalidOperationException)
			{
				ntpPacket = null;
			}
			this._onRequestComplete(ntpPacket);
		}

		
		public const int DefaultPort = 123;

		
		private readonly NetSocket _socket;

		
		private readonly Action<NtpPacket> _onRequestComplete;

		
		private readonly IPEndPoint _ntpEndPoint;
	}
}
