using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;

namespace Blis.Common
{
	public class LiteNetServer : INetServer, INetEventListener
	{
		private readonly Dictionary<int, NetPeer> connectionMap = new Dictionary<int, NetPeer>();


		private NetManager netManager;


		private INetServerHandler netServerHandler;


		public void OnPeerConnected(NetPeer peer)
		{
			connectionMap.Add(peer.Id, peer);
			netServerHandler.OnConnected(peer.Id);
			Log.V("[LiteNetServer] Connected: {0}", peer.EndPoint);
		}


		public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
		{
			Log.E("[LiteNetServer] Error: {0}", socketErrorCode);
		}


		public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
			UnconnectedMessageType messageType) { }


		public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }


		public void OnConnectionRequest(ConnectionRequest request)
		{
			request.Accept();
		}


		public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			Log.V("[LiteNetServer] Disconnected: {0}, Reason: {1}, SocketError: {2}", peer.EndPoint,
				disconnectInfo.Reason, disconnectInfo.SocketErrorCode);
			connectionMap.Remove(peer.Id);
			netServerHandler.OnDisconnected(peer.Id);
		}


		public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
		{
			INetServerHandler netServerHandler = this.netServerHandler;
			if (netServerHandler == null)
			{
				return;
			}

			netServerHandler.OnRecv(peer.Id, reader.GetRemainingBytes());
		}


		public void Init(INetServerHandler netServerHandler, int port)
		{
			this.netServerHandler = netServerHandler;
			netManager = new NetManager(this);
			if (!netManager.Start(port))
			{
				throw new SocketException(10053);
			}

			netManager.UpdateTime = 15;
			netManager.DisconnectTimeout = 10000;
			Log.H("[LiteNetServer] Start Listen: {0}", port);
			netServerHandler.OnStartServer();
		}


		public int GetPeerTimeSinceLastPacket(int connectionId)
		{
			int result = 0;
			NetPeer netPeer;
			if (connectionMap.TryGetValue(connectionId, out netPeer))
			{
				result = netPeer.TimeSinceLastPacket;
			}

			return result;
		}


		public void Send(int connectionId, byte[] data, NetChannel netChannel)
		{
			NetPeer peer;
			if (connectionMap.TryGetValue(connectionId, out peer))
			{
				Send(peer, data, netChannel);
			}
		}


		public void Broadcast(byte[] data, NetChannel netChannel)
		{
			foreach (NetPeer peer in connectionMap.Values)
			{
				Send(peer, data, netChannel);
			}
		}


		public void Broadcast(int connectionId, byte[] data, NetChannel netChannel)
		{
			foreach (NetPeer netPeer in connectionMap.Values)
			{
				if (netPeer.Id != connectionId)
				{
					Send(netPeer, data, netChannel);
				}
			}
		}


		public void Disconnect(int connectionId)
		{
			NetPeer netPeer;
			if (connectionMap.TryGetValue(connectionId, out netPeer))
			{
				netPeer.Disconnect();
			}
		}


		public void Update()
		{
			netManager.PollEvents();
		}


		public void Close()
		{
			try
			{
				NetManager netManager = this.netManager;
				if (netManager != null)
				{
					netManager.Stop();
				}
			}
			catch (Exception ex)
			{
				Log.E("[LiteNetServer] Error On Stop: " + ex.Message + "\n" + ex.StackTrace);
			}
		}


		public int GetRTT(int connectionId)
		{
			NetPeer netPeer;
			if (connectionMap.TryGetValue(connectionId, out netPeer))
			{
				return netPeer.Ping * 2;
			}

			return -1;
		}


		public int GetMtu(int connectionId)
		{
			NetPeer netPeer;
			if (connectionMap.TryGetValue(connectionId, out netPeer))
			{
				return netPeer.Mtu;
			}

			return -1;
		}


		public void LogConnectionMap()
		{
			foreach (KeyValuePair<int, NetPeer> keyValuePair in connectionMap)
			{
				Log.V(string.Format("[LiteNetServer] ConnectionId : {0}, {1}", keyValuePair.Key,
					keyValuePair.Value.Id));
			}
		}


		public void Send(NetPeer peer, byte[] data, NetChannel netChannel)
		{
			try
			{
				peer.Send(data, LiteNetCommon.NetChannelToDeliveryMethod(netChannel));
			}
			catch (TooBigPacketException)
			{
				Log.W("Failed to send packet because it's too big. Resend it by reliable. (Size: {0})", data.Length);
				peer.Send(data, DeliveryMethod.ReliableUnordered);
			}
		}
	}
}