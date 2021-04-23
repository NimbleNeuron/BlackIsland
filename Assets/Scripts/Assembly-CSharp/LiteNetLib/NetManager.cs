using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LiteNetLib.Layers;
using LiteNetLib.Utils;

namespace LiteNetLib
{
	
	public class NetManager : INetSocketListener, IEnumerable<NetPeer>, IEnumerable
	{
		
		
		public bool IsRunning
		{
			get
			{
				return this._socket.IsRunning;
			}
		}

		
		
		public int LocalPort
		{
			get
			{
				return this._socket.LocalPort;
			}
		}

		
		
		public NetPeer FirstPeer
		{
			get
			{
				return this._headPeer;
			}
		}

		
		
		
		public byte ChannelsCount
		{
			get
			{
				return this._channelsCount;
			}
			set
			{
				if (value < 1 || value > 64)
				{
					throw new ArgumentException("Channels count must be between 1 and 64");
				}
				this._channelsCount = value;
			}
		}

		
		
		public List<NetPeer> ConnectedPeerList
		{
			get
			{
				this.GetPeersNonAlloc(this._connectedPeerListCache, ConnectionState.Connected);
				return this._connectedPeerListCache;
			}
		}

		
		public NetPeer GetPeerById(int id)
		{
			return this._peersArray[id];
		}

		
		
		public int ConnectedPeersCount
		{
			get
			{
				return this._connectedPeersCount;
			}
		}

		
		
		public int ExtraPacketSizeForLayer
		{
			get
			{
				if (this._extraPacketLayer == null)
				{
					return 0;
				}
				return this._extraPacketLayer.ExtraPacketSizeForLayer;
			}
		}

		
		private bool TryGetPeer(IPEndPoint endPoint, out NetPeer peer)
		{
			this._peersLock.EnterReadLock();
			bool result = this._peersDict.TryGetValue(endPoint, out peer);
			this._peersLock.ExitReadLock();
			return result;
		}

		
		private void AddPeer(NetPeer peer)
		{
			this._peersLock.EnterWriteLock();
			if (this._headPeer != null)
			{
				peer.NextPeer = this._headPeer;
				this._headPeer.PrevPeer = peer;
			}
			this._headPeer = peer;
			this._peersDict.Add(peer.EndPoint, peer);
			if (peer.Id >= this._peersArray.Length)
			{
				int num = this._peersArray.Length * 2;
				while (peer.Id >= num)
				{
					num *= 2;
				}
				Array.Resize<NetPeer>(ref this._peersArray, num);
			}
			this._peersArray[peer.Id] = peer;
			this._peersLock.ExitWriteLock();
		}

		
		private void RemovePeer(NetPeer peer)
		{
			this._peersLock.EnterWriteLock();
			this.RemovePeerInternal(peer);
			this._peersLock.ExitWriteLock();
		}

		
		private void RemovePeerInternal(NetPeer peer)
		{
			if (!this._peersDict.Remove(peer.EndPoint))
			{
				return;
			}
			if (peer == this._headPeer)
			{
				this._headPeer = peer.NextPeer;
			}
			if (peer.PrevPeer != null)
			{
				peer.PrevPeer.NextPeer = peer.NextPeer;
			}
			if (peer.NextPeer != null)
			{
				peer.NextPeer.PrevPeer = peer.PrevPeer;
			}
			peer.PrevPeer = null;
			this._peersArray[peer.Id] = null;
			Queue<int> peerIds = this._peerIds;
			lock (peerIds)
			{
				this._peerIds.Enqueue(peer.Id);
			}
		}

		
		public NetManager(INetEventListener listener, PacketLayerBase extraPacketLayer = null)
		{
			this._socket = new NetSocket(this);
			this._netEventListener = listener;
			this._deliveryEventListener = (listener as IDeliveryEventListener);
			this._netEventsQueue = new Queue<NetEvent>();
			this._netEventsPool = new Stack<NetEvent>();
			this.NetPacketPool = new NetPacketPool();
			this.NatPunchModule = new NatPunchModule(this._socket);
			this.Statistics = new NetStatistics();
			this._connectedPeerListCache = new List<NetPeer>();
			this._peersDict = new Dictionary<IPEndPoint, NetPeer>(new NetManager.IPEndPointComparer());
			this._requestsDict = new Dictionary<IPEndPoint, ConnectionRequest>(new NetManager.IPEndPointComparer());
			this._peersLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			this._peerIds = new Queue<int>();
			this._peersArray = new NetPeer[32];
			this._extraPacketLayer = extraPacketLayer;
		}

		
		internal void ConnectionLatencyUpdated(NetPeer fromPeer, int latency)
		{
			this.CreateEvent(NetEvent.EType.ConnectionLatencyUpdated, fromPeer, null, SocketError.Success, latency, DisconnectReason.ConnectionFailed, null, DeliveryMethod.Unreliable, null, null);
		}

		
		internal void MessageDelivered(NetPeer fromPeer, object userData)
		{
			if (this._deliveryEventListener != null)
			{
				this.CreateEvent(NetEvent.EType.MessageDelivered, fromPeer, null, SocketError.Success, 0, DisconnectReason.ConnectionFailed, null, DeliveryMethod.Unreliable, null, userData);
			}
		}

		
		internal int SendRawAndRecycle(NetPacket packet, IPEndPoint remoteEndPoint)
		{
			int result = this.SendRaw(packet.RawData, 0, packet.Size, remoteEndPoint);
			this.NetPacketPool.Recycle(packet);
			return result;
		}

		
		internal int SendRaw(NetPacket packet, IPEndPoint remoteEndPoint)
		{
			return this.SendRaw(packet.RawData, 0, packet.Size, remoteEndPoint);
		}

		
		internal int SendRaw(byte[] message, int start, int length, IPEndPoint remoteEndPoint)
		{
			if (!this._socket.IsRunning)
			{
				return 0;
			}
			SocketError socketError = SocketError.Success;
			int num;
			if (this._extraPacketLayer != null)
			{
				NetPacket packet = this.NetPacketPool.GetPacket(length + this._extraPacketLayer.ExtraPacketSizeForLayer);
				Buffer.BlockCopy(message, start, packet.RawData, 0, length);
				int offset = 0;
				this._extraPacketLayer.ProcessOutBoundPacket(ref packet.RawData, ref offset, ref length);
				num = this._socket.SendTo(packet.RawData, offset, length, remoteEndPoint, ref socketError);
				this.NetPacketPool.Recycle(packet);
			}
			else
			{
				num = this._socket.SendTo(message, start, length, remoteEndPoint, ref socketError);
			}
			if (socketError == SocketError.MessageSize)
			{
				return -1;
			}
			if (socketError == SocketError.NetworkUnreachable)
			{
				NetPeer peer;
				if (this.TryGetPeer(remoteEndPoint, out peer))
				{
					this.DisconnectPeerForce(peer, DisconnectReason.NetworkUnreachable, socketError, null);
				}
				this.CreateEvent(NetEvent.EType.Error, null, remoteEndPoint, socketError, 0, DisconnectReason.ConnectionFailed, null, DeliveryMethod.Unreliable, null, null);
				return -1;
			}
			if (socketError == SocketError.HostUnreachable)
			{
				NetPeer peer;
				if (this.TryGetPeer(remoteEndPoint, out peer))
				{
					this.DisconnectPeerForce(peer, DisconnectReason.HostUnreachable, socketError, null);
				}
				this.CreateEvent(NetEvent.EType.Error, null, remoteEndPoint, socketError, 0, DisconnectReason.ConnectionFailed, null, DeliveryMethod.Unreliable, null, null);
				return -1;
			}
			if (num <= 0)
			{
				return 0;
			}
			if (this.EnableStatistics)
			{
				this.Statistics.PacketsSent += 1UL;
				this.Statistics.BytesSent += (ulong)length;
			}
			return num;
		}

		
		internal void DisconnectPeerForce(NetPeer peer, DisconnectReason reason, SocketError socketErrorCode, NetPacket eventData)
		{
			this.DisconnectPeer(peer, reason, socketErrorCode, true, null, 0, 0, eventData);
		}

		
		private void DisconnectPeer(NetPeer peer, DisconnectReason reason, SocketError socketErrorCode, bool force, byte[] data, int start, int count, NetPacket eventData)
		{
			ShutdownResult shutdownResult = peer.Shutdown(data, start, count, force);
			if (shutdownResult == ShutdownResult.None)
			{
				return;
			}
			if (shutdownResult == ShutdownResult.WasConnected)
			{
				this._connectedPeersCount--;
			}
			this.CreateEvent(NetEvent.EType.Disconnect, peer, null, socketErrorCode, 0, reason, null, DeliveryMethod.Unreliable, eventData, null);
		}

		
		private void CreateEvent(NetEvent.EType type, NetPeer peer = null, IPEndPoint remoteEndPoint = null, SocketError errorCode = SocketError.Success, int latency = 0, DisconnectReason disconnectReason = DisconnectReason.ConnectionFailed, ConnectionRequest connectionRequest = null, DeliveryMethod deliveryMethod = DeliveryMethod.Unreliable, NetPacket readerSource = null, object userData = null)
		{
			bool flag = this.UnsyncedEvents;
			if (type == NetEvent.EType.Connect)
			{
				this._connectedPeersCount++;
			}
			else if (type == NetEvent.EType.MessageDelivered)
			{
				flag = this.UnsyncedDeliveryEvent;
			}
			Stack<NetEvent> netEventsPool = this._netEventsPool;
			NetEvent netEvent;
			lock (netEventsPool)
			{
				netEvent = ((this._netEventsPool.Count > 0) ? this._netEventsPool.Pop() : new NetEvent(this));
			}
			netEvent.Type = type;
			netEvent.DataReader.SetSource(readerSource);
			netEvent.Peer = peer;
			netEvent.RemoteEndPoint = remoteEndPoint;
			netEvent.Latency = latency;
			netEvent.ErrorCode = errorCode;
			netEvent.DisconnectReason = disconnectReason;
			netEvent.ConnectionRequest = connectionRequest;
			netEvent.DeliveryMethod = deliveryMethod;
			netEvent.UserData = userData;
			if (flag)
			{
				this.ProcessEvent(netEvent);
				return;
			}
			Queue<NetEvent> netEventsQueue = this._netEventsQueue;
			lock (netEventsQueue)
			{
				this._netEventsQueue.Enqueue(netEvent);
			}
		}

		
		private void ProcessEvent(NetEvent evt)
		{
			bool isNull = evt.DataReader.IsNull;
			switch (evt.Type)
			{
			case NetEvent.EType.Connect:
				this._netEventListener.OnPeerConnected(evt.Peer);
				break;
			case NetEvent.EType.Disconnect:
			{
				DisconnectInfo disconnectInfo = new DisconnectInfo
				{
					Reason = evt.DisconnectReason,
					AdditionalData = evt.DataReader,
					SocketErrorCode = evt.ErrorCode
				};
				this._netEventListener.OnPeerDisconnected(evt.Peer, disconnectInfo);
				break;
			}
			case NetEvent.EType.Receive:
				this._netEventListener.OnNetworkReceive(evt.Peer, evt.DataReader, evt.DeliveryMethod);
				break;
			case NetEvent.EType.ReceiveUnconnected:
				this._netEventListener.OnNetworkReceiveUnconnected(evt.RemoteEndPoint, evt.DataReader, UnconnectedMessageType.BasicMessage);
				break;
			case NetEvent.EType.Error:
				this._netEventListener.OnNetworkError(evt.RemoteEndPoint, evt.ErrorCode);
				break;
			case NetEvent.EType.ConnectionLatencyUpdated:
				this._netEventListener.OnNetworkLatencyUpdate(evt.Peer, evt.Latency);
				break;
			case NetEvent.EType.Broadcast:
				this._netEventListener.OnNetworkReceiveUnconnected(evt.RemoteEndPoint, evt.DataReader, UnconnectedMessageType.Broadcast);
				break;
			case NetEvent.EType.ConnectionRequest:
				this._netEventListener.OnConnectionRequest(evt.ConnectionRequest);
				break;
			case NetEvent.EType.MessageDelivered:
				this._deliveryEventListener.OnMessageDelivered(evt.Peer, evt.UserData);
				break;
			}
			if (isNull)
			{
				this.RecycleEvent(evt);
				return;
			}
			if (this.AutoRecycle)
			{
				evt.DataReader.RecycleInternal();
			}
		}

		
		internal void RecycleEvent(NetEvent evt)
		{
			evt.Peer = null;
			evt.ErrorCode = SocketError.Success;
			evt.RemoteEndPoint = null;
			evt.ConnectionRequest = null;
			Stack<NetEvent> netEventsPool = this._netEventsPool;
			lock (netEventsPool)
			{
				this._netEventsPool.Push(evt);
			}
		}

		
		private void UpdateLogic()
		{
			List<NetPeer> list = new List<NetPeer>();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while (this._socket.IsRunning)
			{
				ulong num = 0UL;
				int num2 = (int)stopwatch.ElapsedMilliseconds;
				num2 = ((num2 <= 0) ? 1 : num2);
				stopwatch.Reset();
				stopwatch.Start();
				for (NetPeer netPeer = this._headPeer; netPeer != null; netPeer = netPeer.NextPeer)
				{
					if (netPeer.ConnectionState == ConnectionState.Disconnected && netPeer.TimeSinceLastPacket > this.DisconnectTimeout)
					{
						list.Add(netPeer);
					}
					else
					{
						netPeer.Update(num2);
						if (this.EnableStatistics)
						{
							num += netPeer.Statistics.PacketLoss;
						}
					}
				}
				if (list.Count > 0)
				{
					this._peersLock.EnterWriteLock();
					for (int i = 0; i < list.Count; i++)
					{
						this.RemovePeerInternal(list[i]);
					}
					this._peersLock.ExitWriteLock();
					list.Clear();
				}
				if (this.EnableStatistics)
				{
					this.Statistics.PacketLoss = num;
				}
				int num3 = this.UpdateTime - (int)stopwatch.ElapsedMilliseconds;
				if (num3 > 0)
				{
					Thread.Sleep(num3);
				}
			}
			stopwatch.Stop();
		}

		
		void INetSocketListener.OnMessageReceived(byte[] data, int length, SocketError errorCode, IPEndPoint remoteEndPoint)
		{
			if (errorCode != SocketError.Success)
			{
				this.CreateEvent(NetEvent.EType.Error, null, null, errorCode, 0, DisconnectReason.ConnectionFailed, null, DeliveryMethod.Unreliable, null, null);
				NetDebug.WriteError("[NM] Receive error: {0}", new object[]
				{
					errorCode
				});
				return;
			}
			try
			{
				this.DataReceived(data, length, remoteEndPoint);
			}
			catch (Exception arg)
			{
				NetDebug.WriteError("[NM] SocketReceiveThread error: " + arg, Array.Empty<object>());
			}
		}

		
		internal NetPeer OnConnectionSolved(ConnectionRequest request, byte[] rejectData, int start, int length)
		{
			NetPeer netPeer = null;
			if (request.Result == ConnectionRequestResult.RejectForce)
			{
				if (rejectData != null && length > 0)
				{
					NetPacket withProperty = this.NetPacketPool.GetWithProperty(PacketProperty.Disconnect, length);
					withProperty.ConnectionNumber = request.ConnectionNumber;
					FastBitConverter.GetBytes(withProperty.RawData, 1, request.ConnectionTime);
					if (withProperty.Size >= NetConstants.PossibleMtu[0])
					{
						NetDebug.WriteError("[Peer] Disconnect additional data size more than MTU!", Array.Empty<object>());
					}
					else
					{
						Buffer.BlockCopy(rejectData, start, withProperty.RawData, 9, length);
					}
					this.SendRawAndRecycle(withProperty, request.RemoteEndPoint);
				}
			}
			else
			{
				this._peersLock.EnterUpgradeableReadLock();
				if (this._peersDict.TryGetValue(request.RemoteEndPoint, out netPeer))
				{
					this._peersLock.ExitUpgradeableReadLock();
				}
				else if (request.Result == ConnectionRequestResult.Reject)
				{
					netPeer = new NetPeer(this, request.RemoteEndPoint, this.GetNextPeerId());
					netPeer.Reject(request.ConnectionTime, request.ConnectionNumber, rejectData, start, length);
					this.AddPeer(netPeer);
					this._peersLock.ExitUpgradeableReadLock();
				}
				else
				{
					netPeer = new NetPeer(this, request.RemoteEndPoint, this.GetNextPeerId(), request.ConnectionTime, request.ConnectionNumber);
					this.AddPeer(netPeer);
					this._peersLock.ExitUpgradeableReadLock();
					this.CreateEvent(NetEvent.EType.Connect, netPeer, null, SocketError.Success, 0, DisconnectReason.ConnectionFailed, null, DeliveryMethod.Unreliable, null, null);
				}
			}
			Dictionary<IPEndPoint, ConnectionRequest> requestsDict = this._requestsDict;
			lock (requestsDict)
			{
				this._requestsDict.Remove(request.RemoteEndPoint);
			}
			return netPeer;
		}

		
		private int GetNextPeerId()
		{
			Queue<int> peerIds = this._peerIds;
			int num2;
			lock (peerIds)
			{
				int num;
				if (this._peerIds.Count != 0)
				{
					num = this._peerIds.Dequeue();
				}
				else
				{
					num2 = this._lastPeerId;
					this._lastPeerId = num2 + 1;
					num = num2;
				}
				num2 = num;
			}
			return num2;
		}

		
		private void ProcessConnectRequest(IPEndPoint remoteEndPoint, NetPeer netPeer, NetConnectRequestPacket connRequest)
		{
			byte connectionNumber = connRequest.ConnectionNumber;
			if (netPeer != null)
			{
				ConnectRequestResult connectRequestResult = netPeer.ProcessConnectRequest(connRequest);
				switch (connectRequestResult)
				{
				case ConnectRequestResult.P2PLose:
					this.DisconnectPeerForce(netPeer, DisconnectReason.PeerToPeerConnection, SocketError.Success, null);
					this.RemovePeer(netPeer);
					break;
				case ConnectRequestResult.Reconnection:
					this.DisconnectPeerForce(netPeer, DisconnectReason.Reconnect, SocketError.Success, null);
					this.RemovePeer(netPeer);
					break;
				case ConnectRequestResult.NewConnection:
					this.RemovePeer(netPeer);
					break;
				default:
					return;
				}
				if (connectRequestResult != ConnectRequestResult.P2PLose)
				{
					// co: dotPeek
					connectionNumber = (byte) (((int) netPeer.ConnectionNum + 1) % 4);
				}
			}
			Dictionary<IPEndPoint, ConnectionRequest> requestsDict = this._requestsDict;
			ConnectionRequest connectionRequest;
			lock (requestsDict)
			{
				if (this._requestsDict.TryGetValue(remoteEndPoint, out connectionRequest))
				{
					connectionRequest.UpdateRequest(connRequest);
					return;
				}
				connectionRequest = new ConnectionRequest(connRequest.ConnectionTime, connectionNumber, connRequest.Data, remoteEndPoint, this);
				this._requestsDict.Add(remoteEndPoint, connectionRequest);
			}
			this.CreateEvent(NetEvent.EType.ConnectionRequest, null, null, SocketError.Success, 0, DisconnectReason.ConnectionFailed, connectionRequest, DeliveryMethod.Unreliable, null, null);
		}

		
		private void DataReceived(byte[] reusableBuffer, int count, IPEndPoint remoteEndPoint)
		{
			if (this.EnableStatistics)
			{
				this.Statistics.PacketsReceived += 1UL;
				this.Statistics.BytesReceived += (ulong)count;
			}
			if (this._extraPacketLayer != null)
			{
				this._extraPacketLayer.ProcessInboundPacket(ref reusableBuffer, ref count);
				if (count == 0)
				{
					return;
				}
			}
			if (reusableBuffer[0] == 17)
			{
				return;
			}
			NetPacket packet = this.NetPacketPool.GetPacket(count);
			if (!packet.FromBytes(reusableBuffer, 0, count))
			{
				this.NetPacketPool.Recycle(packet);
				return;
			}
			PacketProperty property = packet.Property;
			if (property <= PacketProperty.UnconnectedMessage)
			{
				if (property != PacketProperty.ConnectRequest)
				{
					if (property == PacketProperty.UnconnectedMessage)
					{
						if (!this.UnconnectedMessagesEnabled)
						{
							return;
						}
						this.CreateEvent(NetEvent.EType.ReceiveUnconnected, null, remoteEndPoint, SocketError.Success, 0, DisconnectReason.ConnectionFailed, null, DeliveryMethod.Unreliable, packet, null);
						return;
					}
				}
				else if (NetConnectRequestPacket.GetProtocolId(packet) != 11)
				{
					this.SendRawAndRecycle(this.NetPacketPool.GetWithProperty(PacketProperty.InvalidProtocol), remoteEndPoint);
					return;
				}
			}
			else if (property != PacketProperty.Broadcast)
			{
				if (property == PacketProperty.NatMessage)
				{
					if (this.NatPunchEnabled)
					{
						this.NatPunchModule.ProcessMessage(remoteEndPoint, packet);
					}
					return;
				}
			}
			else
			{
				if (!this.BroadcastReceiveEnabled)
				{
					return;
				}
				this.CreateEvent(NetEvent.EType.Broadcast, null, remoteEndPoint, SocketError.Success, 0, DisconnectReason.ConnectionFailed, null, DeliveryMethod.Unreliable, packet, null);
				return;
			}
			this._peersLock.EnterReadLock();
			NetPeer netPeer;
			bool flag = this._peersDict.TryGetValue(remoteEndPoint, out netPeer);
			this._peersLock.ExitReadLock();
			property = packet.Property;
			switch (property)
			{
			case PacketProperty.ConnectRequest:
			{
				NetConnectRequestPacket netConnectRequestPacket = NetConnectRequestPacket.FromData(packet);
				if (netConnectRequestPacket != null)
				{
					this.ProcessConnectRequest(remoteEndPoint, netPeer, netConnectRequestPacket);
				}
				break;
			}
			case PacketProperty.ConnectAccept:
			{
				if (!flag)
				{
					return;
				}
				NetConnectAcceptPacket netConnectAcceptPacket = NetConnectAcceptPacket.FromData(packet);
				if (netConnectAcceptPacket != null && netPeer.ProcessConnectAccept(netConnectAcceptPacket))
				{
					this.CreateEvent(NetEvent.EType.Connect, netPeer, null, SocketError.Success, 0, DisconnectReason.ConnectionFailed, null, DeliveryMethod.Unreliable, null, null);
				}
				break;
			}
			case PacketProperty.Disconnect:
				if (flag)
				{
					DisconnectResult disconnectResult = netPeer.ProcessDisconnect(packet);
					if (disconnectResult == DisconnectResult.None)
					{
						this.NetPacketPool.Recycle(packet);
						return;
					}
					this.DisconnectPeerForce(netPeer, (disconnectResult == DisconnectResult.Disconnect) ? DisconnectReason.RemoteConnectionClose : DisconnectReason.ConnectionRejected, SocketError.Success, packet);
				}
				else
				{
					this.NetPacketPool.Recycle(packet);
				}
				this.SendRawAndRecycle(this.NetPacketPool.GetWithProperty(PacketProperty.ShutdownOk), remoteEndPoint);
				return;
			default:
				if (property != PacketProperty.PeerNotFound)
				{
					if (property != PacketProperty.InvalidProtocol)
					{
						if (flag)
						{
							netPeer.ProcessPacket(packet);
							return;
						}
						this.SendRawAndRecycle(this.NetPacketPool.GetWithProperty(PacketProperty.PeerNotFound), remoteEndPoint);
					}
					else if (flag && netPeer.ConnectionState == ConnectionState.Outgoing)
					{
						this.DisconnectPeerForce(netPeer, DisconnectReason.InvalidProtocol, SocketError.Success, null);
					}
				}
				else if (flag)
				{
					if (netPeer.ConnectionState != ConnectionState.Connected)
					{
						return;
					}
					if (packet.Size == 1)
					{
						NetPacket withProperty = this.NetPacketPool.GetWithProperty(PacketProperty.PeerNotFound, 9);
						withProperty.RawData[1] = 0;
						FastBitConverter.GetBytes(withProperty.RawData, 2, netPeer.ConnectTime);
						this.SendRawAndRecycle(withProperty, remoteEndPoint);
						return;
					}
					if (packet.Size == 10 && packet.RawData[1] == 1 && BitConverter.ToInt64(packet.RawData, 2) == netPeer.ConnectTime)
					{
						this.DisconnectPeerForce(netPeer, DisconnectReason.RemoteConnectionClose, SocketError.Success, null);
					}
				}
				else if (packet.Size == 10 && packet.RawData[1] == 0)
				{
					packet.RawData[1] = 1;
					this.SendRawAndRecycle(packet, remoteEndPoint);
				}
				break;
			}
		}

		
		internal void CreateReceiveEvent(NetPacket packet, DeliveryMethod method, NetPeer fromPeer)
		{
			Stack<NetEvent> netEventsPool = this._netEventsPool;
			NetEvent netEvent;
			lock (netEventsPool)
			{
				netEvent = ((this._netEventsPool.Count > 0) ? this._netEventsPool.Pop() : new NetEvent(this));
			}
			netEvent.Type = NetEvent.EType.Receive;
			netEvent.DataReader.SetSource(packet);
			netEvent.Peer = fromPeer;
			netEvent.DeliveryMethod = method;
			if (this.UnsyncedEvents)
			{
				this.ProcessEvent(netEvent);
				return;
			}
			Queue<NetEvent> netEventsQueue = this._netEventsQueue;
			lock (netEventsQueue)
			{
				this._netEventsQueue.Enqueue(netEvent);
			}
		}

		
		public void SendToAll(NetDataWriter writer, DeliveryMethod options)
		{
			this.SendToAll(writer.Data, 0, writer.Length, options);
		}

		
		public void SendToAll(byte[] data, DeliveryMethod options)
		{
			this.SendToAll(data, 0, data.Length, options);
		}

		
		public void SendToAll(byte[] data, int start, int length, DeliveryMethod options)
		{
			this.SendToAll(data, start, length, 0, options);
		}

		
		public void SendToAll(NetDataWriter writer, byte channelNumber, DeliveryMethod options)
		{
			this.SendToAll(writer.Data, 0, writer.Length, channelNumber, options);
		}

		
		public void SendToAll(byte[] data, byte channelNumber, DeliveryMethod options)
		{
			this.SendToAll(data, 0, data.Length, channelNumber, options);
		}

		
		public void SendToAll(byte[] data, int start, int length, byte channelNumber, DeliveryMethod options)
		{
			try
			{
				this._peersLock.EnterReadLock();
				for (NetPeer netPeer = this._headPeer; netPeer != null; netPeer = netPeer.NextPeer)
				{
					netPeer.Send(data, start, length, channelNumber, options);
				}
			}
			finally
			{
				this._peersLock.ExitReadLock();
			}
		}

		
		public void SendToAll(NetDataWriter writer, DeliveryMethod options, NetPeer excludePeer)
		{
			this.SendToAll(writer.Data, 0, writer.Length, 0, options, excludePeer);
		}

		
		public void SendToAll(byte[] data, DeliveryMethod options, NetPeer excludePeer)
		{
			this.SendToAll(data, 0, data.Length, 0, options, excludePeer);
		}

		
		public void SendToAll(byte[] data, int start, int length, DeliveryMethod options, NetPeer excludePeer)
		{
			this.SendToAll(data, start, length, 0, options, excludePeer);
		}

		
		public void SendToAll(NetDataWriter writer, byte channelNumber, DeliveryMethod options, NetPeer excludePeer)
		{
			this.SendToAll(writer.Data, 0, writer.Length, channelNumber, options, excludePeer);
		}

		
		public void SendToAll(byte[] data, byte channelNumber, DeliveryMethod options, NetPeer excludePeer)
		{
			this.SendToAll(data, 0, data.Length, channelNumber, options, excludePeer);
		}

		
		public void SendToAll(byte[] data, int start, int length, byte channelNumber, DeliveryMethod options, NetPeer excludePeer)
		{
			try
			{
				this._peersLock.EnterReadLock();
				for (NetPeer netPeer = this._headPeer; netPeer != null; netPeer = netPeer.NextPeer)
				{
					if (netPeer != excludePeer)
					{
						netPeer.Send(data, start, length, channelNumber, options);
					}
				}
			}
			finally
			{
				this._peersLock.ExitReadLock();
			}
		}

		
		public bool Start()
		{
			return this.Start(0);
		}

		
		public bool Start(IPAddress addressIPv4, IPAddress addressIPv6, int port)
		{
			if (!this._socket.Bind(addressIPv4, addressIPv6, port, this.ReuseAddress, this.IPv6Enabled))
			{
				return false;
			}
			this._logicThread = new Thread(new ThreadStart(this.UpdateLogic))
			{
				Name = "LogicThread",
				IsBackground = true
			};
			this._logicThread.Start();
			return true;
		}

		
		public bool Start(string addressIPv4, string addressIPv6, int port)
		{
			IPAddress addressIPv7 = NetUtils.ResolveAddress(addressIPv4);
			IPAddress addressIPv8 = NetUtils.ResolveAddress(addressIPv6);
			return this.Start(addressIPv7, addressIPv8, port);
		}

		
		public bool Start(int port)
		{
			return this.Start(IPAddress.Any, IPAddress.IPv6Any, port);
		}

		
		public bool SendUnconnectedMessage(byte[] message, IPEndPoint remoteEndPoint)
		{
			return this.SendUnconnectedMessage(message, 0, message.Length, remoteEndPoint);
		}

		
		public bool SendUnconnectedMessage(NetDataWriter writer, IPEndPoint remoteEndPoint)
		{
			return this.SendUnconnectedMessage(writer.Data, 0, writer.Length, remoteEndPoint);
		}

		
		public bool SendUnconnectedMessage(byte[] message, int start, int length, IPEndPoint remoteEndPoint)
		{
			NetPacket withData = this.NetPacketPool.GetWithData(PacketProperty.UnconnectedMessage, message, start, length);
			return this.SendRawAndRecycle(withData, remoteEndPoint) > 0;
		}

		
		public bool SendBroadcast(NetDataWriter writer, int port)
		{
			return this.SendBroadcast(writer.Data, 0, writer.Length, port);
		}

		
		public bool SendBroadcast(byte[] data, int port)
		{
			return this.SendBroadcast(data, 0, data.Length, port);
		}

		
		public bool SendBroadcast(byte[] data, int start, int length, int port)
		{
			NetPacket netPacket;
			if (this._extraPacketLayer != null)
			{
				int headerSize = NetPacket.GetHeaderSize(PacketProperty.Broadcast);
				netPacket = this.NetPacketPool.GetPacket(headerSize + length + this._extraPacketLayer.ExtraPacketSizeForLayer);
				netPacket.Property = PacketProperty.Broadcast;
				Buffer.BlockCopy(data, start, netPacket.RawData, headerSize, length);
				int num = 0;
				int num2 = length + headerSize;
				this._extraPacketLayer.ProcessOutBoundPacket(ref netPacket.RawData, ref num, ref num2);
			}
			else
			{
				netPacket = this.NetPacketPool.GetWithData(PacketProperty.Broadcast, data, start, length);
			}
			bool result = this._socket.SendBroadcast(netPacket.RawData, 0, netPacket.Size, port);
			this.NetPacketPool.Recycle(netPacket);
			return result;
		}

		
		public void Flush()
		{
			for (NetPeer netPeer = this._headPeer; netPeer != null; netPeer = netPeer.NextPeer)
			{
				netPeer.Flush();
			}
		}

		
		public void PollEvents()
		{
			if (this.UnsyncedEvents)
			{
				return;
			}
			int count = this._netEventsQueue.Count;
			for (int i = 0; i < count; i++)
			{
				Queue<NetEvent> netEventsQueue = this._netEventsQueue;
				NetEvent evt;
				lock (netEventsQueue)
				{
					if (this._netEventsQueue.Count <= 0)
					{
						break;
					}
					evt = this._netEventsQueue.Dequeue();
				}
				this.ProcessEvent(evt);
			}
		}

		
		public NetPeer Connect(string address, int port, string key)
		{
			return this.Connect(address, port, NetDataWriter.FromString(key));
		}

		
		public NetPeer Connect(string address, int port, NetDataWriter connectionData)
		{
			IPEndPoint target;
			try
			{
				target = NetUtils.MakeEndPoint(address, port);
			}
			catch
			{
				this.CreateEvent(NetEvent.EType.Disconnect, null, null, SocketError.Success, 0, DisconnectReason.UnknownHost, null, DeliveryMethod.Unreliable, null, null);
				return null;
			}
			return this.Connect(target, connectionData);
		}

		
		public NetPeer Connect(IPEndPoint target, string key)
		{
			return this.Connect(target, NetDataWriter.FromString(key));
		}

		
		public NetPeer Connect(IPEndPoint target, NetDataWriter connectionData)
		{
			if (!this._socket.IsRunning)
			{
				throw new InvalidOperationException("Client is not running");
			}
			byte connectNum = 0;
			if (this._requestsDict.ContainsKey(target))
			{
				return null;
			}
			this._peersLock.EnterUpgradeableReadLock();
			NetPeer netPeer;
			if (this._peersDict.TryGetValue(target, out netPeer))
			{
				ConnectionState connectionState = netPeer.ConnectionState;
				if (connectionState == ConnectionState.Outgoing || connectionState == ConnectionState.Connected)
				{
					this._peersLock.ExitUpgradeableReadLock();
					return netPeer;
				}
				
				// co: dotPeek
				connectNum = (byte) (((int) netPeer.ConnectionNum + 1) % 4);
				this.RemovePeer(netPeer);
			}
			netPeer = new NetPeer(this, target, this.GetNextPeerId(), connectNum, connectionData);
			this.AddPeer(netPeer);
			this._peersLock.ExitUpgradeableReadLock();
			return netPeer;
		}

		
		public void Stop()
		{
			this.Stop(true);
		}

		
		public void Stop(bool sendDisconnectMessages)
		{
			if (!this._socket.IsRunning)
			{
				return;
			}
			for (NetPeer netPeer = this._headPeer; netPeer != null; netPeer = netPeer.NextPeer)
			{
				netPeer.Shutdown(null, 0, 0, !sendDisconnectMessages);
			}
			this._socket.Close(false);
			this._logicThread.Join();
			this._logicThread = null;
			this._peersLock.EnterWriteLock();
			this._headPeer = null;
			this._peersDict.Clear();
			this._peersArray = new NetPeer[32];
			this._peersLock.ExitWriteLock();
			Queue<int> peerIds = this._peerIds;
			lock (peerIds)
			{
				this._peerIds.Clear();
			}
			this._connectedPeersCount = 0;
			Queue<NetEvent> netEventsQueue = this._netEventsQueue;
			lock (netEventsQueue)
			{
				this._netEventsQueue.Clear();
			}
		}

		
		public int GetPeersCount(ConnectionState peerState)
		{
			int num = 0;
			this._peersLock.EnterReadLock();
			for (NetPeer netPeer = this._headPeer; netPeer != null; netPeer = netPeer.NextPeer)
			{
				if ((netPeer.ConnectionState & peerState) != (ConnectionState)0)
				{
					num++;
				}
			}
			this._peersLock.ExitReadLock();
			return num;
		}

		
		public void GetPeersNonAlloc(List<NetPeer> peers, ConnectionState peerState)
		{
			peers.Clear();
			this._peersLock.EnterReadLock();
			for (NetPeer netPeer = this._headPeer; netPeer != null; netPeer = netPeer.NextPeer)
			{
				if ((netPeer.ConnectionState & peerState) != (ConnectionState)0)
				{
					peers.Add(netPeer);
				}
			}
			this._peersLock.ExitReadLock();
		}

		
		public void DisconnectAll()
		{
			this.DisconnectAll(null, 0, 0);
		}

		
		public void DisconnectAll(byte[] data, int start, int count)
		{
			this._peersLock.EnterReadLock();
			for (NetPeer netPeer = this._headPeer; netPeer != null; netPeer = netPeer.NextPeer)
			{
				this.DisconnectPeer(netPeer, DisconnectReason.DisconnectPeerCalled, SocketError.Success, false, data, start, count, null);
			}
			this._peersLock.ExitReadLock();
		}

		
		public void DisconnectPeerForce(NetPeer peer)
		{
			this.DisconnectPeerForce(peer, DisconnectReason.DisconnectPeerCalled, SocketError.Success, null);
		}

		
		public void DisconnectPeer(NetPeer peer)
		{
			this.DisconnectPeer(peer, null, 0, 0);
		}

		
		public void DisconnectPeer(NetPeer peer, byte[] data)
		{
			this.DisconnectPeer(peer, data, 0, data.Length);
		}

		
		public void DisconnectPeer(NetPeer peer, NetDataWriter writer)
		{
			this.DisconnectPeer(peer, writer.Data, 0, writer.Length);
		}

		
		public void DisconnectPeer(NetPeer peer, byte[] data, int start, int count)
		{
			this.DisconnectPeer(peer, DisconnectReason.DisconnectPeerCalled, SocketError.Success, false, data, start, count, null);
		}

		
		public NetManager.NetPeerEnumerator GetEnumerator()
		{
			return new NetManager.NetPeerEnumerator(this._headPeer);
		}

		
		IEnumerator<NetPeer> IEnumerable<NetPeer>.GetEnumerator()
		{
			return new NetManager.NetPeerEnumerator(this._headPeer);
		}

		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new NetManager.NetPeerEnumerator(this._headPeer);
		}

		
		private readonly NetSocket _socket;

		
		private Thread _logicThread;

		
		private readonly Queue<NetEvent> _netEventsQueue;

		
		private readonly Stack<NetEvent> _netEventsPool;

		
		private readonly INetEventListener _netEventListener;

		
		private readonly IDeliveryEventListener _deliveryEventListener;

		
		private readonly Dictionary<IPEndPoint, NetPeer> _peersDict;

		
		private readonly Dictionary<IPEndPoint, ConnectionRequest> _requestsDict;

		
		private readonly ReaderWriterLockSlim _peersLock;

		
		private volatile NetPeer _headPeer;

		
		private volatile int _connectedPeersCount;

		
		private readonly List<NetPeer> _connectedPeerListCache;

		
		private NetPeer[] _peersArray;

		
		private readonly PacketLayerBase _extraPacketLayer;

		
		private int _lastPeerId;

		
		private readonly Queue<int> _peerIds;

		
		private byte _channelsCount = 1;

		
		internal readonly NetPacketPool NetPacketPool;

		
		public bool UnconnectedMessagesEnabled;

		
		public bool NatPunchEnabled;

		
		public int UpdateTime = 15;

		
		public int PingInterval = 1000;

		
		public int DisconnectTimeout = 5000;

		
		public bool SimulatePacketLoss;

		
		public bool SimulateLatency;

		
		public int SimulationPacketLossChance = 10;

		
		public int SimulationMinLatency = 30;

		
		public int SimulationMaxLatency = 100;

		
		public bool UnsyncedEvents;

		
		public bool UnsyncedDeliveryEvent;

		
		public bool BroadcastReceiveEnabled;

		
		public int ReconnectDelay = 500;

		
		public int MaxConnectAttempts = 10;

		
		public bool ReuseAddress = true;

		
		public readonly NetStatistics Statistics;

		
		public bool EnableStatistics;

		
		public readonly NatPunchModule NatPunchModule;

		
		public bool AutoRecycle;

		
		public IPv6Mode IPv6Enabled = IPv6Mode.SeparateSocket;

		
		private class IPEndPointComparer : IEqualityComparer<IPEndPoint>
		{
			
			public bool Equals(IPEndPoint x, IPEndPoint y)
			{
				return x.Address.Equals(y.Address) && x.Port == y.Port;
			}

			
			public int GetHashCode(IPEndPoint obj)
			{
				return obj.GetHashCode();
			}
		}

		
		public struct NetPeerEnumerator : IEnumerator<NetPeer>, IEnumerator, IDisposable
		{
			
			public NetPeerEnumerator(NetPeer p)
			{
				this._initialPeer = p;
				this._p = null;
			}

			
			public void Dispose()
			{
			}

			
			public bool MoveNext()
			{
				this._p = ((this._p == null) ? this._initialPeer : this._p.NextPeer);
				return this._p != null;
			}

			
			public void Reset()
			{
				throw new NotSupportedException();
			}

			
			
			public NetPeer Current
			{
				get
				{
					return this._p;
				}
			}

			
			
			object IEnumerator.Current
			{
				get
				{
					return this._p;
				}
			}

			
			private readonly NetPeer _initialPeer;

			
			private NetPeer _p;
		}
	}
}
