using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LiteNetLib.Utils;

namespace LiteNetLib
{
	
	public class NetPeer
	{
		
		
		
		internal byte ConnectionNum
		{
			get
			{
				return this._connectNum;
			}
			private set
			{
				this._connectNum = value;
				this._mergeData.ConnectionNumber = value;
				this._pingPacket.ConnectionNumber = value;
				this._pongPacket.ConnectionNumber = value;
			}
		}

		
		
		public ConnectionState ConnectionState
		{
			get
			{
				return this._connectionState;
			}
		}

		
		
		internal long ConnectTime
		{
			get
			{
				return this._connectTime;
			}
		}

		
		
		public int Ping
		{
			get
			{
				return this._avgRtt / 2;
			}
		}

		
		
		public int Mtu
		{
			get
			{
				return this._mtu;
			}
		}

		
		
		public long RemoteTimeDelta
		{
			get
			{
				return this._remoteDelta;
			}
		}

		
		
		public DateTime RemoteUtcTime
		{
			get
			{
				return new DateTime(DateTime.UtcNow.Ticks + this._remoteDelta);
			}
		}

		
		
		public int TimeSinceLastPacket
		{
			get
			{
				return this._timeSinceLastPacket;
			}
		}

		
		
		internal double ResendDelay
		{
			get
			{
				return this._resendDelay;
			}
		}

		
		internal NetPeer(NetManager netManager, IPEndPoint remoteEndPoint, int id)
		{
			this.Id = id;
			this.Statistics = new NetStatistics();
			this._packetPool = netManager.NetPacketPool;
			this.NetManager = netManager;
			this.SetMtu(0);
			this.EndPoint = remoteEndPoint;
			this._connectionState = ConnectionState.Connected;
			this._mergeData = new NetPacket(PacketProperty.Merged, NetConstants.MaxPacketSize);
			this._pongPacket = new NetPacket(PacketProperty.Pong, 0);
			this._pingPacket = new NetPacket(PacketProperty.Ping, 0)
			{
				Sequence = 1
			};
			this._unreliableChannel = new Queue<NetPacket>(64);
			this._headChannel = null;
			this._holdedFragments = new Dictionary<ushort, NetPeer.IncomingFragments>();
			this._deliveredFramgnets = new Dictionary<ushort, ushort>();
			this._channels = new BaseChannel[(int)(netManager.ChannelsCount * 4)];
		}

		
		private void SetMtu(int mtuIdx)
		{
			this._mtu = NetConstants.PossibleMtu[mtuIdx] - this.NetManager.ExtraPacketSizeForLayer;
		}

		
		public int GetPacketsCountInReliableQueue(byte channelNumber, bool ordered)
		{
			int num = (int)(channelNumber * 4 + (ordered ? 2 : 0));
			BaseChannel baseChannel = this._channels[num];
			if (baseChannel == null)
			{
				return 0;
			}
			return ((ReliableChannel)baseChannel).PacketsInQueue;
		}

		
		private BaseChannel CreateChannel(byte idx)
		{
			BaseChannel baseChannel = this._channels[(int)idx];
			if (baseChannel != null)
			{
				return baseChannel;
			}
			switch (idx % 4)
			{
			case 0:
				baseChannel = new ReliableChannel(this, false, idx);
				break;
			case 1:
				baseChannel = new SequencedChannel(this, false, idx);
				break;
			case 2:
				baseChannel = new ReliableChannel(this, true, idx);
				break;
			case 3:
				baseChannel = new SequencedChannel(this, true, idx);
				break;
			}
			this._channels[(int)idx] = baseChannel;
			baseChannel.Next = this._headChannel;
			this._headChannel = baseChannel;
			return baseChannel;
		}

		
		internal NetPeer(NetManager netManager, IPEndPoint remoteEndPoint, int id, byte connectNum, NetDataWriter connectData) : this(netManager, remoteEndPoint, id)
		{
			this._connectTime = DateTime.UtcNow.Ticks;
			this._connectionState = ConnectionState.Outgoing;
			this.ConnectionNum = connectNum;
			this._connectRequestPacket = NetConnectRequestPacket.Make(connectData, remoteEndPoint.Serialize(), this._connectTime);
			this._connectRequestPacket.ConnectionNumber = connectNum;
			this.NetManager.SendRaw(this._connectRequestPacket, this.EndPoint);
		}

		
		internal NetPeer(NetManager netManager, IPEndPoint remoteEndPoint, int id, long connectId, byte connectNum) : this(netManager, remoteEndPoint, id)
		{
			this._connectTime = connectId;
			this._connectionState = ConnectionState.Connected;
			this.ConnectionNum = connectNum;
			this._connectAcceptPacket = NetConnectAcceptPacket.Make(this._connectTime, connectNum, false);
			this.NetManager.SendRaw(this._connectAcceptPacket, this.EndPoint);
		}

		
		internal void Reject(long connectionId, byte connectionNumber, byte[] data, int start, int length)
		{
			this._connectTime = connectionId;
			this._connectNum = connectionNumber;
			this.Shutdown(data, start, length, false);
		}

		
		internal bool ProcessConnectAccept(NetConnectAcceptPacket packet)
		{
			if (this._connectionState != ConnectionState.Outgoing)
			{
				return false;
			}
			if (packet.ConnectionId != this._connectTime)
			{
				return false;
			}
			this.ConnectionNum = packet.ConnectionNumber;
			Interlocked.Exchange(ref this._timeSinceLastPacket, 0);
			this._connectionState = ConnectionState.Connected;
			return true;
		}

		
		public int GetMaxSinglePacketSize(DeliveryMethod options)
		{
			return this._mtu - NetPacket.GetHeaderSize((options == DeliveryMethod.Unreliable) ? PacketProperty.Unreliable : PacketProperty.Channeled);
		}

		
		public void SendWithDeliveryEvent(byte[] data, byte channelNumber, DeliveryMethod deliveryMethod, object userData)
		{
			if (deliveryMethod != DeliveryMethod.ReliableOrdered && deliveryMethod != DeliveryMethod.ReliableUnordered)
			{
				throw new ArgumentException("Delivery event will work only for ReliableOrdered/Unordered packets");
			}
			this.SendInternal(data, 0, data.Length, channelNumber, deliveryMethod, userData);
		}

		
		public void SendWithDeliveryEvent(byte[] data, int start, int length, byte channelNumber, DeliveryMethod deliveryMethod, object userData)
		{
			if (deliveryMethod != DeliveryMethod.ReliableOrdered && deliveryMethod != DeliveryMethod.ReliableUnordered)
			{
				throw new ArgumentException("Delivery event will work only for ReliableOrdered/Unordered packets");
			}
			this.SendInternal(data, start, length, channelNumber, deliveryMethod, userData);
		}

		
		public void SendWithDeliveryEvent(NetDataWriter dataWriter, byte channelNumber, DeliveryMethod deliveryMethod, object userData)
		{
			if (deliveryMethod != DeliveryMethod.ReliableOrdered && deliveryMethod != DeliveryMethod.ReliableUnordered)
			{
				throw new ArgumentException("Delivery event will work only for ReliableOrdered/Unordered packets");
			}
			this.SendInternal(dataWriter.Data, 0, dataWriter.Length, channelNumber, deliveryMethod, userData);
		}

		
		public void Send(byte[] data, DeliveryMethod deliveryMethod)
		{
			this.SendInternal(data, 0, data.Length, 0, deliveryMethod, null);
		}

		
		public void Send(NetDataWriter dataWriter, DeliveryMethod deliveryMethod)
		{
			this.SendInternal(dataWriter.Data, 0, dataWriter.Length, 0, deliveryMethod, null);
		}

		
		public void Send(byte[] data, int start, int length, DeliveryMethod options)
		{
			this.SendInternal(data, start, length, 0, options, null);
		}

		
		public void Send(byte[] data, byte channelNumber, DeliveryMethod deliveryMethod)
		{
			this.SendInternal(data, 0, data.Length, channelNumber, deliveryMethod, null);
		}

		
		public void Send(NetDataWriter dataWriter, byte channelNumber, DeliveryMethod deliveryMethod)
		{
			this.SendInternal(dataWriter.Data, 0, dataWriter.Length, channelNumber, deliveryMethod, null);
		}

		
		public void Send(byte[] data, int start, int length, byte channelNumber, DeliveryMethod deliveryMethod)
		{
			this.SendInternal(data, start, length, channelNumber, deliveryMethod, null);
		}

		
		private void SendInternal(byte[] data, int start, int length, byte channelNumber, DeliveryMethod deliveryMethod, object userData)
		{
			if (this._connectionState != ConnectionState.Connected || (int)channelNumber >= this._channels.Length)
			{
				return;
			}
			BaseChannel baseChannel = null;
			PacketProperty property;
			if (deliveryMethod == DeliveryMethod.Unreliable)
			{
				property = PacketProperty.Unreliable;
			}
			else
			{
				property = PacketProperty.Channeled;
				// co: dotPeek
				baseChannel = this.CreateChannel((byte) ((byte) ((int) channelNumber * 4) + deliveryMethod));
			}
			int headerSize = NetPacket.GetHeaderSize(property);
			int mtu = this._mtu;
			if (length + headerSize <= mtu)
			{
				NetPacket packet = this._packetPool.GetPacket(headerSize + length);
				packet.Property = property;
				Buffer.BlockCopy(data, start, packet.RawData, headerSize, length);
				packet.UserData = userData;
				if (baseChannel == null)
				{
					Queue<NetPacket> unreliableChannel = this._unreliableChannel;
					lock (unreliableChannel)
					{
						this._unreliableChannel.Enqueue(packet);
						return;
					}
				}
				baseChannel.AddToQueue(packet);
				return;
			}
			if (deliveryMethod != DeliveryMethod.ReliableOrdered && deliveryMethod != DeliveryMethod.ReliableUnordered)
			{
				throw new TooBigPacketException("Unreliable packet size exceeded maximum of " + (mtu - headerSize) + " bytes");
			}
			int num = mtu - headerSize - 6;
			int num2 = length / num + ((length % num == 0) ? 0 : 1);
			if (num2 > 65535)
			{
				throw new TooBigPacketException(string.Concat(new object[]
				{
					"Data was split in ",
					num2,
					" fragments, which exceeds ",
					ushort.MaxValue
				}));
			}
			object sendLock = this._sendLock;
			ushort fragmentId;
			lock (sendLock)
			{
				fragmentId = this._fragmentId;
				this._fragmentId += 1;
			}
			ushort num3 = 0;
			while ((int)num3 < num2)
			{
				int num4 = (length > num) ? num : length;
				NetPacket packet2 = this._packetPool.GetPacket(headerSize + num4 + 6);
				packet2.Property = property;
				packet2.UserData = userData;
				packet2.FragmentId = fragmentId;
				packet2.FragmentPart = num3;
				packet2.FragmentsTotal = (ushort)num2;
				packet2.MarkFragmented();
				Buffer.BlockCopy(data, (int)num3 * num, packet2.RawData, 10, num4);
				baseChannel.AddToQueue(packet2);
				length -= num4;
				num3 += 1;
			}
		}

		
		public void Disconnect(byte[] data)
		{
			this.NetManager.DisconnectPeer(this, data);
		}

		
		public void Disconnect(NetDataWriter writer)
		{
			this.NetManager.DisconnectPeer(this, writer);
		}

		
		public void Disconnect(byte[] data, int start, int count)
		{
			this.NetManager.DisconnectPeer(this, data, start, count);
		}

		
		public void Disconnect()
		{
			this.NetManager.DisconnectPeer(this);
		}

		
		internal DisconnectResult ProcessDisconnect(NetPacket packet)
		{
			if ((this._connectionState != ConnectionState.Connected && this._connectionState != ConnectionState.Outgoing) || packet.Size < 9 || BitConverter.ToInt64(packet.RawData, 1) != this._connectTime || packet.ConnectionNumber != this._connectNum)
			{
				return DisconnectResult.None;
			}
			if (this._connectionState != ConnectionState.Connected)
			{
				return DisconnectResult.Reject;
			}
			return DisconnectResult.Disconnect;
		}

		
		internal ShutdownResult Shutdown(byte[] data, int start, int length, bool force)
		{
			object shutdownLock = this._shutdownLock;
			ShutdownResult result;
			lock (shutdownLock)
			{
				if (this._connectionState == ConnectionState.Disconnected || this._connectionState == ConnectionState.ShutdownRequested)
				{
					result = ShutdownResult.None;
				}
				else
				{
					ShutdownResult shutdownResult = (this._connectionState == ConnectionState.Connected) ? ShutdownResult.WasConnected : ShutdownResult.Success;
					if (force)
					{
						this._connectionState = ConnectionState.Disconnected;
						result = shutdownResult;
					}
					else
					{
						Interlocked.Exchange(ref this._timeSinceLastPacket, 0);
						this._shutdownPacket = new NetPacket(PacketProperty.Disconnect, length)
						{
							ConnectionNumber = this._connectNum
						};
						FastBitConverter.GetBytes(this._shutdownPacket.RawData, 1, this._connectTime);
						if (this._shutdownPacket.Size >= this._mtu)
						{
							NetDebug.WriteError("[Peer] Disconnect additional data size more than MTU - 8!", Array.Empty<object>());
						}
						else if (data != null && length > 0)
						{
							Buffer.BlockCopy(data, start, this._shutdownPacket.RawData, 9, length);
						}
						this._connectionState = ConnectionState.ShutdownRequested;
						this.NetManager.SendRaw(this._shutdownPacket, this.EndPoint);
						result = shutdownResult;
					}
				}
			}
			return result;
		}

		
		private void UpdateRoundTripTime(int roundTripTime)
		{
			this._rtt += roundTripTime;
			this._rttCount++;
			this._avgRtt = this._rtt / this._rttCount;
			this._resendDelay = 25.0 + (double)this._avgRtt * 2.1;
		}

		
		internal void AddReliablePacket(DeliveryMethod method, NetPacket p)
		{
			if (!p.IsFragmented)
			{
				this.NetManager.CreateReceiveEvent(p, method, this);
				return;
			}
			ushort fragmentId = p.FragmentId;
			NetPeer.IncomingFragments incomingFragments;
			if (!this._holdedFragments.TryGetValue(fragmentId, out incomingFragments))
			{
				incomingFragments = new NetPeer.IncomingFragments
				{
					Fragments = new NetPacket[(int)p.FragmentsTotal],
					ChannelId = p.ChannelId
				};
				this._holdedFragments.Add(fragmentId, incomingFragments);
			}
			NetPacket[] fragments = incomingFragments.Fragments;
			if ((int)p.FragmentPart >= fragments.Length || fragments[(int)p.FragmentPart] != null || p.ChannelId != incomingFragments.ChannelId)
			{
				this._packetPool.Recycle(p);
				NetDebug.WriteError("Invalid fragment packet", Array.Empty<object>());
				return;
			}
			fragments[(int)p.FragmentPart] = p;
			incomingFragments.ReceivedCount++;
			incomingFragments.TotalSize += p.Size - 10;
			if (incomingFragments.ReceivedCount != fragments.Length)
			{
				return;
			}
			NetPacket withProperty = this._packetPool.GetWithProperty(PacketProperty.Unreliable, incomingFragments.TotalSize);
			int num = fragments[0].Size - 10;
			for (int i = 0; i < incomingFragments.ReceivedCount; i++)
			{
				NetPacket netPacket = fragments[i];
				Buffer.BlockCopy(netPacket.RawData, 10, withProperty.RawData, 1 + num * i, netPacket.Size - 10);
				this._packetPool.Recycle(netPacket);
			}
			Array.Clear(fragments, 0, incomingFragments.ReceivedCount);
			this.NetManager.CreateReceiveEvent(withProperty, method, this);
			this._holdedFragments.Remove(fragmentId);
		}

		
		private void ProcessMtuPacket(NetPacket packet)
		{
			if (packet.Size < NetConstants.PossibleMtu[0])
			{
				return;
			}
			int num = BitConverter.ToInt32(packet.RawData, 1);
			int num2 = BitConverter.ToInt32(packet.RawData, packet.Size - 4);
			if (num != packet.Size || num != num2 || num > NetConstants.MaxPacketSize)
			{
				NetDebug.WriteError("[MTU] Broken packet. RMTU {0}, EMTU {1}, PSIZE {2}", new object[]
				{
					num,
					num2,
					packet.Size
				});
				return;
			}
			if (packet.Property == PacketProperty.MtuCheck)
			{
				this._mtuCheckAttempts = 0;
				packet.Property = PacketProperty.MtuOk;
				this.NetManager.SendRawAndRecycle(packet, this.EndPoint);
				return;
			}
			if (num > this._mtu && !this._finishMtu)
			{
				if (num != NetConstants.PossibleMtu[this._mtuIdx + 1])
				{
					return;
				}
				object mtuMutex = this._mtuMutex;
				lock (mtuMutex)
				{
					this._mtuIdx++;
					this.SetMtu(this._mtuIdx);
				}
				if (this._mtuIdx == NetConstants.PossibleMtu.Length - 1)
				{
					this._finishMtu = true;
				}
			}
		}

		
		private void UpdateMtuLogic(int deltaTime)
		{
			if (this._finishMtu)
			{
				return;
			}
			this._mtuCheckTimer += deltaTime;
			if (this._mtuCheckTimer < 1000)
			{
				return;
			}
			this._mtuCheckTimer = 0;
			this._mtuCheckAttempts++;
			if (this._mtuCheckAttempts >= 4)
			{
				this._finishMtu = true;
				return;
			}
			object mtuMutex = this._mtuMutex;
			lock (mtuMutex)
			{
				if (this._mtuIdx < NetConstants.PossibleMtu.Length - 1)
				{
					int num = NetConstants.PossibleMtu[this._mtuIdx + 1];
					NetPacket packet = this._packetPool.GetPacket(num);
					packet.Property = PacketProperty.MtuCheck;
					FastBitConverter.GetBytes(packet.RawData, 1, num);
					FastBitConverter.GetBytes(packet.RawData, packet.Size - 4, num);
					if (this.NetManager.SendRawAndRecycle(packet, this.EndPoint) <= 0)
					{
						this._finishMtu = true;
					}
				}
			}
		}

		
		internal ConnectRequestResult ProcessConnectRequest(NetConnectRequestPacket connRequest)
		{
			ConnectionState connectionState = this._connectionState;
			if (connectionState <= ConnectionState.Connected)
			{
				if (connectionState != ConnectionState.Outgoing)
				{
					if (connectionState == ConnectionState.Connected)
					{
						if (connRequest.ConnectionTime == this._connectTime)
						{
							this.NetManager.SendRaw(this._connectAcceptPacket, this.EndPoint);
						}
						else if (connRequest.ConnectionTime > this._connectTime)
						{
							return ConnectRequestResult.Reconnection;
						}
					}
				}
				else
				{
					if (connRequest.ConnectionTime < this._connectTime)
					{
						return ConnectRequestResult.P2PLose;
					}
					if (connRequest.ConnectionTime == this._connectTime)
					{
						SocketAddress socketAddress = this.EndPoint.Serialize();
						byte[] targetAddress = connRequest.TargetAddress;
						for (int i = socketAddress.Size - 1; i >= 0; i--)
						{
							byte b = socketAddress[i];
							if (b != targetAddress[i] && b < targetAddress[i])
							{
								return ConnectRequestResult.P2PLose;
							}
						}
					}
				}
			}
			else if (connectionState == ConnectionState.ShutdownRequested || connectionState == ConnectionState.Disconnected)
			{
				if (connRequest.ConnectionTime >= this._connectTime)
				{
					return ConnectRequestResult.NewConnection;
				}
			}
			return ConnectRequestResult.None;
		}

		
		internal void ProcessPacket(NetPacket packet)
		{
			if (this._connectionState == ConnectionState.Outgoing)
			{
				this._packetPool.Recycle(packet);
				return;
			}
			if (packet.ConnectionNumber != this._connectNum && packet.Property != PacketProperty.ShutdownOk)
			{
				this._packetPool.Recycle(packet);
				return;
			}
			Interlocked.Exchange(ref this._timeSinceLastPacket, 0);
			switch (packet.Property)
			{
			case PacketProperty.Unreliable:
				this.NetManager.CreateReceiveEvent(packet, DeliveryMethod.Unreliable, this);
				return;
			case PacketProperty.Channeled:
			case PacketProperty.Ack:
			{
				if ((int)packet.ChannelId > this._channels.Length)
				{
					this._packetPool.Recycle(packet);
					return;
				}
				BaseChannel baseChannel = this._channels[(int)packet.ChannelId] ?? ((packet.Property == PacketProperty.Ack) ? null : this.CreateChannel(packet.ChannelId));
				if (baseChannel != null && !baseChannel.ProcessPacket(packet))
				{
					this._packetPool.Recycle(packet);
					return;
				}
				return;
			}
			case PacketProperty.Ping:
				if (NetUtils.RelativeSequenceNumber((int)packet.Sequence, (int)this._pongPacket.Sequence) > 0)
				{
					FastBitConverter.GetBytes(this._pongPacket.RawData, 3, DateTime.UtcNow.Ticks);
					this._pongPacket.Sequence = packet.Sequence;
					this.NetManager.SendRaw(this._pongPacket, this.EndPoint);
				}
				this._packetPool.Recycle(packet);
				return;
			case PacketProperty.Pong:
				if (packet.Sequence == this._pingPacket.Sequence)
				{
					this._pingTimer.Stop();
					int num = (int)this._pingTimer.ElapsedMilliseconds;
					this._remoteDelta = BitConverter.ToInt64(packet.RawData, 3) + (long)num * 10000L / 2L - DateTime.UtcNow.Ticks;
					this.UpdateRoundTripTime(num);
					this.NetManager.ConnectionLatencyUpdated(this, num / 2);
				}
				this._packetPool.Recycle(packet);
				return;
			case PacketProperty.MtuCheck:
			case PacketProperty.MtuOk:
				this.ProcessMtuPacket(packet);
				return;
			case PacketProperty.Merged:
			{
				int i = 1;
				while (i < packet.Size)
				{
					ushort num2 = BitConverter.ToUInt16(packet.RawData, i);
					i += 2;
					NetPacket packet2 = this._packetPool.GetPacket((int)num2);
					if (!packet2.FromBytes(packet.RawData, i, (int)num2))
					{
						this._packetPool.Recycle(packet);
						return;
					}
					i += (int)num2;
					this.ProcessPacket(packet2);
				}
				return;
			}
			case PacketProperty.ShutdownOk:
				if (this._connectionState == ConnectionState.ShutdownRequested)
				{
					this._connectionState = ConnectionState.Disconnected;
				}
				this._packetPool.Recycle(packet);
				return;
			}
			NetDebug.WriteError("Error! Unexpected packet type: " + packet.Property, Array.Empty<object>());
		}

		
		private void SendMerged()
		{
			if (this._mergeCount == 0)
			{
				return;
			}
			int num;
			if (this._mergeCount > 1)
			{
				num = this.NetManager.SendRaw(this._mergeData.RawData, 0, 1 + this._mergePos, this.EndPoint);
			}
			else
			{
				num = this.NetManager.SendRaw(this._mergeData.RawData, 3, this._mergePos - 2, this.EndPoint);
			}
			if (this.NetManager.EnableStatistics)
			{
				this.Statistics.PacketsSent += 1UL;
				this.Statistics.BytesSent += (ulong)((long)num);
			}
			this._mergePos = 0;
			this._mergeCount = 0;
		}

		
		internal void SendUserData(NetPacket packet)
		{
			packet.ConnectionNumber = this._connectNum;
			int num = 1 + packet.Size + 2;
			if (num + 20 >= this._mtu)
			{
				int num2 = this.NetManager.SendRaw(packet, this.EndPoint);
				if (this.NetManager.EnableStatistics)
				{
					this.Statistics.PacketsSent += 1UL;
					this.Statistics.BytesSent += (ulong)((long)num2);
				}
				return;
			}
			if (this._mergePos + num > this._mtu)
			{
				this.SendMerged();
			}
			FastBitConverter.GetBytes(this._mergeData.RawData, this._mergePos + 1, (ushort)packet.Size);
			Buffer.BlockCopy(packet.RawData, 0, this._mergeData.RawData, this._mergePos + 1 + 2, packet.Size);
			this._mergePos += packet.Size + 2;
			this._mergeCount++;
		}

		
		public void Flush()
		{
			if (this._connectionState != ConnectionState.Connected)
			{
				return;
			}
			object flushLock = this._flushLock;
			lock (flushLock)
			{
				for (BaseChannel baseChannel = this._headChannel; baseChannel != null; baseChannel = baseChannel.Next)
				{
					baseChannel.SendNextPackets();
				}
				Queue<NetPacket> unreliableChannel = this._unreliableChannel;
				lock (unreliableChannel)
				{
					while (this._unreliableChannel.Count > 0)
					{
						NetPacket packet = this._unreliableChannel.Dequeue();
						this.SendUserData(packet);
						this.NetManager.NetPacketPool.Recycle(packet);
					}
				}
				this.SendMerged();
			}
		}

		
		internal void Update(int deltaTime)
		{
			Interlocked.Add(ref this._timeSinceLastPacket, deltaTime);
			ConnectionState connectionState = this._connectionState;
			if (connectionState <= ConnectionState.Connected)
			{
				if (connectionState == ConnectionState.Outgoing)
				{
					this._connectTimer += deltaTime;
					if (this._connectTimer > this.NetManager.ReconnectDelay)
					{
						this._connectTimer = 0;
						this._connectAttempts++;
						if (this._connectAttempts > this.NetManager.MaxConnectAttempts)
						{
							this.NetManager.DisconnectPeerForce(this, DisconnectReason.ConnectionFailed, SocketError.Success, null);
							return;
						}
						this.NetManager.SendRaw(this._connectRequestPacket, this.EndPoint);
					}
					return;
				}
				if (connectionState == ConnectionState.Connected)
				{
					if (this._timeSinceLastPacket > this.NetManager.DisconnectTimeout)
					{
						this.NetManager.DisconnectPeerForce(this, DisconnectReason.Timeout, SocketError.Success, null);
						return;
					}
				}
			}
			else if (connectionState != ConnectionState.ShutdownRequested)
			{
				if (connectionState == ConnectionState.Disconnected)
				{
					return;
				}
			}
			else
			{
				if (this._timeSinceLastPacket > this.NetManager.DisconnectTimeout)
				{
					this._connectionState = ConnectionState.Disconnected;
					return;
				}
				this._shutdownTimer += deltaTime;
				if (this._shutdownTimer >= 300)
				{
					this._shutdownTimer = 0;
					this.NetManager.SendRaw(this._shutdownPacket, this.EndPoint);
				}
				return;
			}
			this._pingSendTimer += deltaTime;
			if (this._pingSendTimer >= this.NetManager.PingInterval)
			{
				this._pingSendTimer = 0;
				NetPacket pingPacket = this._pingPacket;
				// co: dotPeek
				++pingPacket.Sequence;
				if (this._pingTimer.IsRunning)
				{
					this.UpdateRoundTripTime((int)this._pingTimer.ElapsedMilliseconds);
				}
				this._pingTimer.Reset();
				this._pingTimer.Start();
				this.NetManager.SendRaw(this._pingPacket, this.EndPoint);
			}
			this._rttResetTimer += deltaTime;
			if (this._rttResetTimer >= this.NetManager.PingInterval * 3)
			{
				this._rttResetTimer = 0;
				this._rtt = this._avgRtt;
				this._rttCount = 1;
			}
			this.UpdateMtuLogic(deltaTime);
			this.Flush();
		}

		
		internal void RecycleAndDeliver(NetPacket packet)
		{
			if (packet.UserData != null)
			{
				if (packet.IsFragmented)
				{
					ushort num;
					this._deliveredFramgnets.TryGetValue(packet.FragmentId, out num);
					num += 1;
					if (num == packet.FragmentsTotal)
					{
						this.NetManager.MessageDelivered(this, packet.UserData);
						this._deliveredFramgnets.Remove(packet.FragmentId);
					}
					else
					{
						this._deliveredFramgnets[packet.FragmentId] = num;
					}
				}
				else
				{
					this.NetManager.MessageDelivered(this, packet.UserData);
				}
				packet.UserData = null;
			}
			this._packetPool.Recycle(packet);
		}

		
		private int _rtt;

		
		private int _avgRtt;

		
		private int _rttCount;

		
		private double _resendDelay = 27.0;

		
		private int _pingSendTimer;

		
		private int _rttResetTimer;

		
		private readonly Stopwatch _pingTimer = new Stopwatch();

		
		private int _timeSinceLastPacket;

		
		private long _remoteDelta;

		
		private readonly NetPacketPool _packetPool;

		
		private readonly object _flushLock = new object();

		
		private readonly object _sendLock = new object();

		
		private readonly object _shutdownLock = new object();

		
		internal volatile NetPeer NextPeer;

		
		internal NetPeer PrevPeer;

		
		private readonly Queue<NetPacket> _unreliableChannel;

		
		private readonly BaseChannel[] _channels;

		
		private BaseChannel _headChannel;

		
		private int _mtu;

		
		private int _mtuIdx;

		
		private bool _finishMtu;

		
		private int _mtuCheckTimer;

		
		private int _mtuCheckAttempts;

		
		private const int MtuCheckDelay = 1000;

		
		private const int MaxMtuCheckAttempts = 4;

		
		private readonly object _mtuMutex = new object();

		
		private ushort _fragmentId;

		
		private readonly Dictionary<ushort, NetPeer.IncomingFragments> _holdedFragments;

		
		private readonly Dictionary<ushort, ushort> _deliveredFramgnets;

		
		private readonly NetPacket _mergeData;

		
		private int _mergePos;

		
		private int _mergeCount;

		
		private int _connectAttempts;

		
		private int _connectTimer;

		
		private long _connectTime;

		
		private byte _connectNum;

		
		private ConnectionState _connectionState;

		
		private NetPacket _shutdownPacket;

		
		private const int ShutdownDelay = 300;

		
		private int _shutdownTimer;

		
		private readonly NetPacket _pingPacket;

		
		private readonly NetPacket _pongPacket;

		
		private readonly NetPacket _connectRequestPacket;

		
		private readonly NetPacket _connectAcceptPacket;

		
		public readonly IPEndPoint EndPoint;

		
		public readonly NetManager NetManager;

		
		public readonly int Id;

		
		public object Tag;

		
		public readonly NetStatistics Statistics;

		
		private class IncomingFragments
		{
			
			public NetPacket[] Fragments;

			
			public int ReceivedCount;

			
			public int TotalSize;

			
			public byte ChannelId;
		}
	}
}
