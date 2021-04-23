using System.Net;
using System.Net.Sockets;

namespace LiteNetLib
{
	
	public class EventBasedNetListener : INetEventListener, IDeliveryEventListener
	{
		
		public delegate void OnConnectionRequest(ConnectionRequest request);

		
		public delegate void OnDeliveryEvent(NetPeer peer, object userData);

		
		public delegate void OnNetworkError(IPEndPoint endPoint, SocketError socketError);

		
		public delegate void OnNetworkLatencyUpdate(NetPeer peer, int latency);

		
		public delegate void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);

		
		public delegate void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
			UnconnectedMessageType messageType);

		
		public delegate void OnPeerConnected(NetPeer peer);

		
		public delegate void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo);

		
		void IDeliveryEventListener.OnMessageDelivered(NetPeer peer, object userData)
		{
			if (DeliveryEvent != null)
			{
				DeliveryEvent(peer, userData);
			}
		}

		
		void INetEventListener.OnPeerConnected(NetPeer peer)
		{
			if (PeerConnectedEvent != null)
			{
				PeerConnectedEvent(peer);
			}
		}

		
		void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			if (PeerDisconnectedEvent != null)
			{
				PeerDisconnectedEvent(peer, disconnectInfo);
			}
		}

		
		void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
		{
			if (NetworkErrorEvent != null)
			{
				NetworkErrorEvent(endPoint, socketErrorCode);
			}
		}

		
		void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
		{
			if (NetworkReceiveEvent != null)
			{
				NetworkReceiveEvent(peer, reader, deliveryMethod);
			}
		}

		
		void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
			UnconnectedMessageType messageType)
		{
			if (NetworkReceiveUnconnectedEvent != null)
			{
				NetworkReceiveUnconnectedEvent(remoteEndPoint, reader, messageType);
			}
		}

		
		void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
		{
			if (NetworkLatencyUpdateEvent != null)
			{
				NetworkLatencyUpdateEvent(peer, latency);
			}
		}

		
		void INetEventListener.OnConnectionRequest(ConnectionRequest request)
		{
			if (ConnectionRequestEvent != null)
			{
				ConnectionRequestEvent(request);
			}
		}

		
		
		
		public event OnPeerConnected PeerConnectedEvent;

		
		
		
		public event OnPeerDisconnected PeerDisconnectedEvent;

		
		
		
		public event OnNetworkError NetworkErrorEvent;

		
		
		
		public event OnNetworkReceive NetworkReceiveEvent;

		
		
		
		public event OnNetworkReceiveUnconnected NetworkReceiveUnconnectedEvent;

		
		
		
		public event OnNetworkLatencyUpdate NetworkLatencyUpdateEvent;

		
		
		
		public event OnConnectionRequest ConnectionRequestEvent;

		
		
		
		public event OnDeliveryEvent DeliveryEvent;

		
		public void ClearPeerConnectedEvent()
		{
			PeerConnectedEvent = null;
		}

		
		public void ClearPeerDisconnectedEvent()
		{
			PeerDisconnectedEvent = null;
		}

		
		public void ClearNetworkErrorEvent()
		{
			NetworkErrorEvent = null;
		}

		
		public void ClearNetworkReceiveEvent()
		{
			NetworkReceiveEvent = null;
		}

		
		public void ClearNetworkReceiveUnconnectedEvent()
		{
			NetworkReceiveUnconnectedEvent = null;
		}

		
		public void ClearNetworkLatencyUpdateEvent()
		{
			NetworkLatencyUpdateEvent = null;
		}

		
		public void ClearConnectionRequestEvent()
		{
			ConnectionRequestEvent = null;
		}

		
		public void ClearDeliveryEvent()
		{
			DeliveryEvent = null;
		}
	}
}