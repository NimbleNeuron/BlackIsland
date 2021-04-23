using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;

namespace Blis.Common
{
	public class LiteNetClient : INetClient, INetEventListener
	{
		private int latency;


		private INetClientHandler netClientHandler;


		private NetManager netManager;


		private NetPeer peer;


		private long uniqueId;

		public void Init(INetClientHandler handler, long uniqueId)
		{
			netClientHandler = handler;
			netManager = new NetManager(this);
			netManager.Start();
			netManager.DisconnectTimeout = 10000;
			netManager.UpdateTime = 15;
			this.uniqueId = uniqueId;
		}


		public void SetSimulation(int minLatency, int maxLatency, int packetLoss)
		{
			netManager.SimulateLatency = minLatency > 0 || maxLatency > 0;
			netManager.SimulationMinLatency = minLatency;
			netManager.SimulationMaxLatency = maxLatency;
			netManager.SimulatePacketLoss = packetLoss > 0;
			netManager.SimulationPacketLossChance = packetLoss;
			Log.E("[LiteNetClient] Latency {0}~{1}, PacketLoss: {2}", minLatency, maxLatency, packetLoss);
		}


		public void Open(string ip, int port)
		{
			Log.V("[LiteNetClient] Open {0}:{1}", ip, port);
			peer = netManager.Connect(ip, port, "");
		}


		public void Close()
		{
			if (peer != null && peer.ConnectionState == ConnectionState.Connected)
			{
				peer.Disconnect();
			}

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
				Log.E("[LiteNetClient] Error On Stop: " + ex.Message + "\n" + ex.StackTrace);
			}
		}


		public void Send(byte[] data, NetChannel netChannel)
		{
			peer.Send(data, LiteNetCommon.NetChannelToDeliveryMethod(netChannel));
		}


		public int GetLatency()
		{
			if (peer == null)
			{
				return 0;
			}

			return peer.Ping;
		}


		public void Update()
		{
			if (netManager == null)
			{
				return;
			}

			netManager.PollEvents();
			if (peer == null)
			{
				return;
			}

			if (peer != null)
			{
				ConnectionState connectionState = peer.ConnectionState;
			}
		}


		public void OnPeerConnected(NetPeer peer)
		{
			Log.V("[LiteNetClient] Connected: {0}", peer.EndPoint);
			netClientHandler.OnConnected();
		}


		public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
		{
			Log.E("[LiteNetClient] Error: {0}", socketErrorCode);
			netClientHandler.OnError((int) socketErrorCode);
		}


		public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
		{
			netClientHandler.OnRecv(reader.GetRemainingBytes());
		}


		public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
			UnconnectedMessageType messageType)
		{
			Log.E("[LiteNetClient] received unconnected. remoteEndPoint: {0} | messageType: {1}", remoteEndPoint,
				messageType);
		}


		public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }


		public void OnConnectionRequest(ConnectionRequest request) { }


		public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			Log.E("[LiteNetClient] We disconnected because " + disconnectInfo.Reason);
			netClientHandler.OnDisconnected();
		}
	}
}