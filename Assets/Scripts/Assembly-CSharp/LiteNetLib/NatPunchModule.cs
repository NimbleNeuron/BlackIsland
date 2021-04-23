using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib.Utils;

namespace LiteNetLib
{
	
	public sealed class NatPunchModule
	{
		
		internal NatPunchModule(NetSocket socket)
		{
			this._socket = socket;
			this._netPacketProcessor.SubscribeReusable<NatPunchModule.NatIntroduceResponsePacket>(new Action<NatPunchModule.NatIntroduceResponsePacket>(this.OnNatIntroductionResponse));
			this._netPacketProcessor.SubscribeReusable<NatPunchModule.NatIntroduceRequestPacket, IPEndPoint>(new Action<NatPunchModule.NatIntroduceRequestPacket, IPEndPoint>(this.OnNatIntroductionRequest));
			this._netPacketProcessor.SubscribeReusable<NatPunchModule.NatPunchPacket, IPEndPoint>(new Action<NatPunchModule.NatPunchPacket, IPEndPoint>(this.OnNatPunch));
		}

		
		internal void ProcessMessage(IPEndPoint senderEndPoint, NetPacket packet)
		{
			NetDataReader cacheReader = this._cacheReader;
			lock (cacheReader)
			{
				this._cacheReader.SetSource(packet.RawData, 1, packet.Size);
				this._netPacketProcessor.ReadAllPackets(this._cacheReader, senderEndPoint);
			}
		}

		
		public void Init(INatPunchListener listener)
		{
			this._natPunchListener = listener;
		}

		
		private void Send<T>(T packet, IPEndPoint target) where T : class, new()
		{
			SocketError socketError = SocketError.Success;
			this._cacheWriter.Reset();
			this._cacheWriter.Put(16);
			this._netPacketProcessor.Write<T>(this._cacheWriter, packet);
			this._socket.SendTo(this._cacheWriter.Data, 0, this._cacheWriter.Length, target, ref socketError);
		}

		
		public void NatIntroduce(IPEndPoint hostInternal, IPEndPoint hostExternal, IPEndPoint clientInternal, IPEndPoint clientExternal, string additionalInfo)
		{
			NatPunchModule.NatIntroduceResponsePacket natIntroduceResponsePacket = new NatPunchModule.NatIntroduceResponsePacket
			{
				Token = additionalInfo
			};
			natIntroduceResponsePacket.Internal = hostInternal;
			natIntroduceResponsePacket.External = hostExternal;
			this.Send<NatPunchModule.NatIntroduceResponsePacket>(natIntroduceResponsePacket, clientExternal);
			natIntroduceResponsePacket.Internal = clientInternal;
			natIntroduceResponsePacket.External = clientExternal;
			this.Send<NatPunchModule.NatIntroduceResponsePacket>(natIntroduceResponsePacket, hostExternal);
		}

		
		public void PollEvents()
		{
			if (this._natPunchListener == null || (this._successEvents.Count == 0 && this._requestEvents.Count == 0))
			{
				return;
			}
			Queue<NatPunchModule.SuccessEventData> successEvents = this._successEvents;
			lock (successEvents)
			{
				while (this._successEvents.Count > 0)
				{
					NatPunchModule.SuccessEventData successEventData = this._successEvents.Dequeue();
					this._natPunchListener.OnNatIntroductionSuccess(successEventData.TargetEndPoint, successEventData.Type, successEventData.Token);
				}
			}
			Queue<NatPunchModule.RequestEventData> requestEvents = this._requestEvents;
			lock (requestEvents)
			{
				while (this._requestEvents.Count > 0)
				{
					NatPunchModule.RequestEventData requestEventData = this._requestEvents.Dequeue();
					this._natPunchListener.OnNatIntroductionRequest(requestEventData.LocalEndPoint, requestEventData.RemoteEndPoint, requestEventData.Token);
				}
			}
		}

		
		public void SendNatIntroduceRequest(string host, int port, string additionalInfo)
		{
			this.SendNatIntroduceRequest(NetUtils.MakeEndPoint(host, port), additionalInfo);
		}

		
		public void SendNatIntroduceRequest(IPEndPoint masterServerEndPoint, string additionalInfo)
		{
			string localIp = NetUtils.GetLocalIp(LocalAddrType.IPv4);
			if (string.IsNullOrEmpty(localIp))
			{
				localIp = NetUtils.GetLocalIp(LocalAddrType.IPv6);
			}
			this.Send<NatPunchModule.NatIntroduceRequestPacket>(new NatPunchModule.NatIntroduceRequestPacket
			{
				Internal = NetUtils.MakeEndPoint(localIp, this._socket.LocalPort),
				Token = additionalInfo
			}, masterServerEndPoint);
		}

		
		private void OnNatIntroductionRequest(NatPunchModule.NatIntroduceRequestPacket req, IPEndPoint senderEndPoint)
		{
			Queue<NatPunchModule.RequestEventData> requestEvents = this._requestEvents;
			lock (requestEvents)
			{
				this._requestEvents.Enqueue(new NatPunchModule.RequestEventData
				{
					LocalEndPoint = req.Internal,
					RemoteEndPoint = senderEndPoint,
					Token = req.Token
				});
			}
		}

		
		private void OnNatIntroductionResponse(NatPunchModule.NatIntroduceResponsePacket req)
		{
			NatPunchModule.NatPunchPacket natPunchPacket = new NatPunchModule.NatPunchPacket
			{
				Token = req.Token
			};
			this.Send<NatPunchModule.NatPunchPacket>(natPunchPacket, req.Internal);
			SocketError socketError = SocketError.Success;
			this._socket.Ttl = 2;
			this._socket.SendTo(new byte[]
			{
				17
			}, 0, 1, req.External, ref socketError);
			this._socket.Ttl = 255;
			natPunchPacket.IsExternal = true;
			this.Send<NatPunchModule.NatPunchPacket>(natPunchPacket, req.External);
		}

		
		private void OnNatPunch(NatPunchModule.NatPunchPacket req, IPEndPoint senderEndPoint)
		{
			Queue<NatPunchModule.SuccessEventData> successEvents = this._successEvents;
			lock (successEvents)
			{
				this._successEvents.Enqueue(new NatPunchModule.SuccessEventData
				{
					TargetEndPoint = senderEndPoint,
					Type = (req.IsExternal ? NatAddressType.External : NatAddressType.Internal),
					Token = req.Token
				});
			}
		}

		
		private readonly NetSocket _socket;

		
		private readonly Queue<NatPunchModule.RequestEventData> _requestEvents = new Queue<NatPunchModule.RequestEventData>();

		
		private readonly Queue<NatPunchModule.SuccessEventData> _successEvents = new Queue<NatPunchModule.SuccessEventData>();

		
		private readonly NetDataReader _cacheReader = new NetDataReader();

		
		private readonly NetDataWriter _cacheWriter = new NetDataWriter();

		
		private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor(256);

		
		private INatPunchListener _natPunchListener;

		
		public const int MaxTokenLength = 256;

		
		private struct RequestEventData
		{
			
			public IPEndPoint LocalEndPoint;

			
			public IPEndPoint RemoteEndPoint;

			
			public string Token;
		}

		
		private struct SuccessEventData
		{
			
			public IPEndPoint TargetEndPoint;

			
			public NatAddressType Type;

			
			public string Token;
		}

		
		private class NatIntroduceRequestPacket
		{
			
			
			
			public IPEndPoint Internal { get; set; }

			
			
			
			public string Token { get; set; }
		}

		
		private class NatIntroduceResponsePacket
		{
			
			
			
			public IPEndPoint Internal { get; set; }

			
			
			
			public IPEndPoint External { get; set; }

			
			
			
			public string Token { get; set; }
		}

		
		private class NatPunchPacket
		{
			
			
			
			public string Token { get; set; }

			
			
			
			public bool IsExternal { get; set; }
		}
	}
}
