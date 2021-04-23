using System;
using System.Collections.Generic;

namespace LiteNetLib
{
	
	internal sealed class SequencedChannel : BaseChannel
	{
		
		public SequencedChannel(NetPeer peer, bool reliable, byte id) : base(peer)
		{
			this._id = id;
			this._reliable = reliable;
			if (this._reliable)
			{
				this._ackPacket = new NetPacket(PacketProperty.Ack, 0)
				{
					ChannelId = id
				};
			}
		}

		
		public override void SendNextPackets()
		{
			if (this._reliable && this.OutgoingQueue.Count == 0)
			{
				long ticks = DateTime.UtcNow.Ticks;
				if ((double)(ticks - this._lastPacketSendTime) < this.Peer.ResendDelay * 10000.0)
				{
					return;
				}
				NetPacket lastPacket = this._lastPacket;
				if (lastPacket != null)
				{
					this._lastPacketSendTime = ticks;
					this.Peer.SendUserData(lastPacket);
				}
			}
			else
			{
				Queue<NetPacket> outgoingQueue = this.OutgoingQueue;
				lock (outgoingQueue)
				{
					while (this.OutgoingQueue.Count > 0)
					{
						NetPacket netPacket = this.OutgoingQueue.Dequeue();
						this._localSequence = (this._localSequence + 1) % 32768;
						netPacket.Sequence = (ushort)this._localSequence;
						netPacket.ChannelId = this._id;
						this.Peer.SendUserData(netPacket);
						if (this._reliable && this.OutgoingQueue.Count == 0)
						{
							this._lastPacketSendTime = DateTime.UtcNow.Ticks;
							this._lastPacket = netPacket;
						}
						else
						{
							this.Peer.NetManager.NetPacketPool.Recycle(netPacket);
						}
					}
				}
			}
			if (this._reliable && this._mustSendAck)
			{
				this._mustSendAck = false;
				this._ackPacket.Sequence = this._remoteSequence;
				this.Peer.SendUserData(this._ackPacket);
			}
		}

		
		public override bool ProcessPacket(NetPacket packet)
		{
			if (packet.IsFragmented)
			{
				return false;
			}
			if (packet.Property == PacketProperty.Ack)
			{
				if (this._reliable && this._lastPacket != null && packet.Sequence == this._lastPacket.Sequence)
				{
					this._lastPacket = null;
				}
				return false;
			}
			int num = NetUtils.RelativeSequenceNumber((int)packet.Sequence, (int)this._remoteSequence);
			bool result = false;
			if (packet.Sequence < 32768 && num > 0)
			{
				this.Peer.Statistics.PacketLoss += (ulong)((long)(num - 1));
				this._remoteSequence = packet.Sequence;
				this.Peer.NetManager.CreateReceiveEvent(packet, this._reliable ? DeliveryMethod.ReliableSequenced : DeliveryMethod.Sequenced, this.Peer);
				result = true;
			}
			this._mustSendAck = true;
			return result;
		}

		
		private int _localSequence;

		
		private ushort _remoteSequence;

		
		private readonly bool _reliable;

		
		private NetPacket _lastPacket;

		
		private readonly NetPacket _ackPacket;

		
		private bool _mustSendAck;

		
		private readonly byte _id;

		
		private long _lastPacketSendTime;
	}
}
