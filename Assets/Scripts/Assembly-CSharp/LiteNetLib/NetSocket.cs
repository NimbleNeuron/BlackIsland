using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LiteNetLib
{
	
	internal sealed class NetSocket
	{
		
		
		
		public int LocalPort { get; private set; }

		
		
		
		public short Ttl
		{
			get
			{
				if (this._udpSocketv4.AddressFamily == AddressFamily.InterNetworkV6)
				{
					return (short)this._udpSocketv4.GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.HopLimit);
				}
				return this._udpSocketv4.Ttl;
			}
			set
			{
				if (this._udpSocketv4.AddressFamily == AddressFamily.InterNetworkV6)
				{
					this._udpSocketv4.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.HopLimit, (int)value);
					return;
				}
				this._udpSocketv4.Ttl = value;
			}
		}

		
		public NetSocket(INetSocketListener listener)
		{
			this._listener = listener;
		}

		
		private bool IsActive()
		{
			return this.IsRunning;
		}

		
		private void ReceiveLogic(object state)
		{
			Socket socket = (Socket)state;
			EndPoint endPoint = new IPEndPoint((socket.AddressFamily == AddressFamily.InterNetwork) ? IPAddress.Any : IPAddress.IPv6Any, 0);
			byte[] array = new byte[NetConstants.MaxPacketSize];
			while (this.IsActive())
			{
				int length;
				try
				{
					if (socket.Available == 0 && !socket.Poll(500000, SelectMode.SelectRead))
					{
						continue;
					}
					length = socket.ReceiveFrom(array, 0, array.Length, SocketFlags.None, ref endPoint);
				}
				catch (SocketException ex)
				{
					SocketError socketErrorCode = ex.SocketErrorCode;
					if (socketErrorCode <= SocketError.NotSocket)
					{
						if (socketErrorCode == SocketError.Interrupted || socketErrorCode == SocketError.NotSocket)
						{
							break;
						}
					}
					else if (socketErrorCode == SocketError.MessageSize || socketErrorCode == SocketError.ConnectionReset || socketErrorCode == SocketError.TimedOut)
					{
						continue;
					}
					NetDebug.WriteError("[R]Error code: {0} - {1}", new object[]
					{
						(int)ex.SocketErrorCode,
						ex.ToString()
					});
					this._listener.OnMessageReceived(null, 0, ex.SocketErrorCode, (IPEndPoint)endPoint);
					continue;
				}
				catch (ObjectDisposedException)
				{
					break;
				}
				this._listener.OnMessageReceived(array, length, SocketError.Success, (IPEndPoint)endPoint);
			}
		}

		
		public bool Bind(IPAddress addressIPv4, IPAddress addressIPv6, int port, bool reuseAddress, IPv6Mode ipv6Mode)
		{
			if (this.IsActive())
			{
				return false;
			}
			bool flag = ipv6Mode == IPv6Mode.DualMode && NetSocket.IPv6Support;
			this._udpSocketv4 = new Socket(flag ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			if (!this.BindSocket(this._udpSocketv4, new IPEndPoint(flag ? addressIPv6 : addressIPv4, port), reuseAddress, ipv6Mode))
			{
				return false;
			}
			this.LocalPort = ((IPEndPoint)this._udpSocketv4.LocalEndPoint).Port;
			if (flag)
			{
				this._udpSocketv6 = this._udpSocketv4;
			}
			this.IsRunning = true;
			this._threadv4 = new Thread(new ParameterizedThreadStart(this.ReceiveLogic))
			{
				Name = "SocketThreadv4(" + this.LocalPort + ")",
				IsBackground = true
			};
			this._threadv4.Start(this._udpSocketv4);
			if (!NetSocket.IPv6Support || ipv6Mode != IPv6Mode.SeparateSocket)
			{
				return true;
			}
			this._udpSocketv6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
			if (this.BindSocket(this._udpSocketv6, new IPEndPoint(addressIPv6, this.LocalPort), reuseAddress, ipv6Mode))
			{
				this._threadv6 = new Thread(new ParameterizedThreadStart(this.ReceiveLogic))
				{
					Name = "SocketThreadv6(" + this.LocalPort + ")",
					IsBackground = true
				};
				this._threadv6.Start(this._udpSocketv6);
			}
			return true;
		}

		
		private bool BindSocket(Socket socket, IPEndPoint ep, bool reuseAddress, IPv6Mode ipv6Mode)
		{
			socket.ReceiveTimeout = 500;
			socket.SendTimeout = 500;
			socket.ReceiveBufferSize = 1048576;
			socket.SendBufferSize = 1048576;
			try
			{
				socket.IOControl(-1744830452, new byte[1], null);
			}
			catch
			{
			}
			try
			{
				socket.ExclusiveAddressUse = !reuseAddress;
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, reuseAddress);
			}
			catch
			{
			}
			if (socket.AddressFamily == AddressFamily.InterNetwork)
			{
				this.Ttl = 255;
				try
				{
					socket.DontFragment = true;
				}
				catch (SocketException ex)
				{
					NetDebug.WriteError("[B]DontFragment error: {0}", new object[]
					{
						ex.SocketErrorCode
					});
				}
				try
				{
					socket.EnableBroadcast = true;
					goto IL_F8;
				}
				catch (SocketException ex2)
				{
					NetDebug.WriteError("[B]Broadcast error: {0}", new object[]
					{
						ex2.SocketErrorCode
					});
					goto IL_F8;
				}
			}
			if (ipv6Mode == IPv6Mode.DualMode)
			{
				try
				{
					socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
				}
				catch (Exception ex3)
				{
					NetDebug.WriteError("[B]Bind exception (dualmode setting): {0}", new object[]
					{
						ex3.ToString()
					});
				}
			}
			IL_F8:
			try
			{
				socket.Bind(ep);
				AddressFamily addressFamily = socket.AddressFamily;
			}
			catch (SocketException ex4)
			{
				SocketError socketErrorCode = ex4.SocketErrorCode;
				if (socketErrorCode == SocketError.AddressFamilyNotSupported)
				{
					return true;
				}
				if (socketErrorCode == SocketError.AddressAlreadyInUse && socket.AddressFamily == AddressFamily.InterNetworkV6 && ipv6Mode != IPv6Mode.DualMode)
				{
					try
					{
						socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, true);
						socket.Bind(ep);
					}
					catch (SocketException ex5)
					{
						NetDebug.WriteError("[B]Bind exception: {0}, errorCode: {1}", new object[]
						{
							ex5.ToString(),
							ex5.SocketErrorCode
						});
						return false;
					}
					return true;
				}
				NetDebug.WriteError("[B]Bind exception: {0}, errorCode: {1}", new object[]
				{
					ex4.ToString(),
					ex4.SocketErrorCode
				});
				return false;
			}
			return true;
		}

		
		public bool SendBroadcast(byte[] data, int offset, int size, int port)
		{
			if (!this.IsActive())
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			try
			{
				flag = (this._udpSocketv4.SendTo(data, offset, size, SocketFlags.None, new IPEndPoint(IPAddress.Broadcast, port)) > 0);
				if (this._udpSocketv6 != null)
				{
					flag2 = (this._udpSocketv6.SendTo(data, offset, size, SocketFlags.None, new IPEndPoint(NetSocket.MulticastAddressV6, port)) > 0);
				}
			}
			catch (Exception arg)
			{
				NetDebug.WriteError("[S][MCAST]" + arg, Array.Empty<object>());
				return flag;
			}
			return flag || flag2;
		}

		
		public int SendTo(byte[] data, int offset, int size, IPEndPoint remoteEndPoint, ref SocketError errorCode)
		{
			if (!this.IsActive())
			{
				return 0;
			}
			int result;
			try
			{
				Socket socket = this._udpSocketv4;
				if (remoteEndPoint.AddressFamily == AddressFamily.InterNetworkV6 && NetSocket.IPv6Support)
				{
					socket = this._udpSocketv6;
				}
				result = socket.SendTo(data, offset, size, SocketFlags.None, remoteEndPoint);
			}
			catch (SocketException ex)
			{
				SocketError socketErrorCode = ex.SocketErrorCode;
				if (socketErrorCode != SocketError.Interrupted)
				{
					if (socketErrorCode != SocketError.MessageSize)
					{
						if (socketErrorCode == SocketError.NoBufferSpaceAvailable)
						{
							goto IL_5A;
						}
						NetDebug.WriteError("[S]" + ex, Array.Empty<object>());
					}
					errorCode = ex.SocketErrorCode;
					return -1;
				}
				IL_5A:
				result = 0;
			}
			catch (Exception arg)
			{
				NetDebug.WriteError("[S]" + arg, Array.Empty<object>());
				result = -1;
			}
			return result;
		}

		
		public void Close(bool suspend)
		{
			if (!suspend)
			{
				this.IsRunning = false;
			}
			if (this._udpSocketv4 == this._udpSocketv6)
			{
				this._udpSocketv6 = null;
			}
			if (this._udpSocketv4 != null)
			{
				this._udpSocketv4.Close();
			}
			if (this._udpSocketv6 != null)
			{
				this._udpSocketv6.Close();
			}
			this._udpSocketv4 = null;
			this._udpSocketv6 = null;
			if (this._threadv4 != null && this._threadv4 != Thread.CurrentThread)
			{
				this._threadv4.Join();
			}
			if (this._threadv6 != null && this._threadv6 != Thread.CurrentThread)
			{
				this._threadv6.Join();
			}
			this._threadv4 = null;
			this._threadv6 = null;
		}

		
		public const int ReceivePollingTime = 500000;

		
		private Socket _udpSocketv4;

		
		private Socket _udpSocketv6;

		
		private Thread _threadv4;

		
		private Thread _threadv6;

		
		private readonly INetSocketListener _listener;

		
		private const int SioUdpConnreset = -1744830452;

		
		private static readonly IPAddress MulticastAddressV6 = IPAddress.Parse("ff02::1");

		
		internal static readonly bool IPv6Support = Socket.OSSupportsIPv6;

		
		public volatile bool IsRunning;
	}
}
