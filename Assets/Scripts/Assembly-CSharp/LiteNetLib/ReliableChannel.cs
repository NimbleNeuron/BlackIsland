using System;
using System.Collections.Generic;

namespace LiteNetLib
{
	
	internal sealed class ReliableChannel : BaseChannel
	{
		
		public ReliableChannel(NetPeer peer, bool ordered, byte id) : base(peer)
		{
			this._id = id;
			this._windowSize = 64;
			this._ordered = ordered;
			this._pendingPackets = new ReliableChannel.PendingPacket[this._windowSize];
			for (int i = 0; i < this._pendingPackets.Length; i++)
			{
				this._pendingPackets[i] = default(ReliableChannel.PendingPacket);
			}
			if (this._ordered)
			{
				this._deliveryMethod = DeliveryMethod.ReliableOrdered;
				this._receivedPackets = new NetPacket[this._windowSize];
			}
			else
			{
				this._deliveryMethod = DeliveryMethod.ReliableUnordered;
				this._earlyReceived = new bool[this._windowSize];
			}
			this._localWindowStart = 0;
			this._localSeqence = 0;
			this._remoteSequence = 0;
			this._remoteWindowStart = 0;
			this._outgoingAcks = new NetPacket(PacketProperty.Ack, (this._windowSize - 1) / 8 + 2)
			{
				ChannelId = id
			};
		}

		
		private void ProcessAck(NetPacket packet)
		{
			if (packet.Size != this._outgoingAcks.Size)
			{
				return;
			}
			ushort sequence = packet.Sequence;
			int num = NetUtils.RelativeSequenceNumber(this._localWindowStart, (int)sequence);
			if (sequence >= 32768 || num < 0)
			{
				return;
			}
			if (num >= this._windowSize)
			{
				return;
			}
			byte[] rawData = packet.RawData;
			ReliableChannel.PendingPacket[] pendingPackets = this._pendingPackets;
			lock (pendingPackets)
			{
				for (int num2 = this._localWindowStart; num2 != this._localSeqence; num2 = (num2 + 1) % 32768)
				{
					if (NetUtils.RelativeSequenceNumber(num2, (int)sequence) >= this._windowSize)
					{
						break;
					}
					int num3 = num2 % this._windowSize;
					int num4 = 4 + num3 / 8;
					int num5 = num3 % 8;
					if (((int)rawData[num4] & 1 << num5) == 0)
					{
						if (this.Peer.NetManager.EnableStatistics)
						{
							this.Peer.Statistics.PacketLoss += 1UL;
						}
					}
					else
					{
						if (num2 == this._localWindowStart)
						{
							this._localWindowStart = (this._localWindowStart + 1) % 32768;
						}
						this._pendingPackets[num3].Clear(this.Peer);
					}
				}
			}
		}

		
		public override void SendNextPackets()
		{
			if (this._mustSendAcks)
			{
				this._mustSendAcks = false;
				NetPacket outgoingAcks = this._outgoingAcks;
				lock (outgoingAcks)
				{
					this.Peer.SendUserData(this._outgoingAcks);
				}
			}
			long ticks = DateTime.UtcNow.Ticks;
			ReliableChannel.PendingPacket[] pendingPackets = this._pendingPackets;
			lock (pendingPackets)
			{
				Queue<NetPacket> outgoingQueue = this.OutgoingQueue;
				lock (outgoingQueue)
				{
					while (this.OutgoingQueue.Count > 0)
					{
						if (NetUtils.RelativeSequenceNumber(this._localSeqence, this._localWindowStart) >= this._windowSize)
						{
							break;
						}
						NetPacket netPacket = this.OutgoingQueue.Dequeue();
						netPacket.Sequence = (ushort)this._localSeqence;
						netPacket.ChannelId = this._id;
						this._pendingPackets[this._localSeqence % this._windowSize].Init(netPacket);
						this._localSeqence = (this._localSeqence + 1) % 32768;
					}
				}
				for (int num = this._localWindowStart; num != this._localSeqence; num = (num + 1) % 32768)
				{
					this._pendingPackets[num % this._windowSize].TrySend(ticks, this.Peer);
				}
			}
		}

		
		public override bool ProcessPacket(NetPacket packet)
		{
			if (packet.Property == PacketProperty.Ack)
			{
				this.ProcessAck(packet);
				return false;
			}
			int sequence = (int)packet.Sequence;
			if (sequence >= 32768)
			{
				return false;
			}
			int num = NetUtils.RelativeSequenceNumber(sequence, this._remoteWindowStart);
			if (NetUtils.RelativeSequenceNumber(sequence, this._remoteSequence) > this._windowSize)
			{
				return false;
			}
			if (num < 0)
			{
				return false;
			}
			if (num >= this._windowSize * 2)
			{
				return false;
			}
			NetPacket outgoingAcks = this._outgoingAcks;
			int num3;
			lock (outgoingAcks)
			{
				int num4;
				int num5;
				if (num >= this._windowSize)
				{
					int num2 = (this._remoteWindowStart + num - this._windowSize + 1) % 32768;
					this._outgoingAcks.Sequence = (ushort)num2;
					while (this._remoteWindowStart != num2)
					{
						num3 = this._remoteWindowStart % this._windowSize;
						num4 = 4 + num3 / 8;
						num5 = num3 % 8;
						byte[] rawData = this._outgoingAcks.RawData;
						int num6 = num4;
						rawData[num6] &= (byte)(~(byte)(1 << num5));
						this._remoteWindowStart = (this._remoteWindowStart + 1) % 32768;
					}
				}
				this._mustSendAcks = true;
				num3 = sequence % this._windowSize;
				num4 = 4 + num3 / 8;
				num5 = num3 % 8;
				if (((int)this._outgoingAcks.RawData[num4] & 1 << num5) != 0)
				{
					return false;
				}
				byte[] rawData2 = this._outgoingAcks.RawData;
				int num7 = num4;
				rawData2[num7] |= (byte)(1 << num5);
			}
			if (sequence == this._remoteSequence)
			{
				this.Peer.AddReliablePacket(this._deliveryMethod, packet);
				this._remoteSequence = (this._remoteSequence + 1) % 32768;
				if (this._ordered)
				{
					NetPacket p;
					while ((p = this._receivedPackets[this._remoteSequence % this._windowSize]) != null)
					{
						this._receivedPackets[this._remoteSequence % this._windowSize] = null;
						this.Peer.AddReliablePacket(this._deliveryMethod, p);
						this._remoteSequence = (this._remoteSequence + 1) % 32768;
					}
				}
				else
				{
					while (this._earlyReceived[this._remoteSequence % this._windowSize])
					{
						this._earlyReceived[this._remoteSequence % this._windowSize] = false;
						this._remoteSequence = (this._remoteSequence + 1) % 32768;
					}
				}
				return true;
			}
			if (this._ordered)
			{
				this._receivedPackets[num3] = packet;
			}
			else
			{
				this._earlyReceived[num3] = true;
				this.Peer.AddReliablePacket(this._deliveryMethod, packet);
			}
			return true;
		}

		
		private readonly NetPacket _outgoingAcks;

		
		private readonly ReliableChannel.PendingPacket[] _pendingPackets;

		
		private readonly NetPacket[] _receivedPackets;

		
		private readonly bool[] _earlyReceived;

		
		private int _localSeqence;

		
		private int _remoteSequence;

		
		private int _localWindowStart;

		
		private int _remoteWindowStart;

		
		private bool _mustSendAcks;

		
		private readonly DeliveryMethod _deliveryMethod;

		
		private readonly bool _ordered;

		
		private readonly int _windowSize;

		
		private const int BitsInByte = 8;

		
		private readonly byte _id;

		
		private struct PendingPacket
		{
			
			public override string ToString()
			{
				if (this._packet != null)
				{
					return this._packet.Sequence.ToString();
				}
				return "Empty";
			}

			
			public void Init(NetPacket packet)
			{
				this._packet = packet;
				this._isSent = false;
			}

			
			public void TrySend(long currentTime, NetPeer peer)
			{
				if (this._packet == null)
				{
					return;
				}
				if (this._isSent)
				{
					double num = peer.ResendDelay * 10000.0;
					if ((double)(currentTime - this._timeStamp) < num)
					{
						return;
					}
				}
				this._timeStamp = currentTime;
				this._isSent = true;
				peer.SendUserData(this._packet);
			}

			
			public bool Clear(NetPeer peer)
			{
				if (this._packet != null)
				{
					peer.RecycleAndDeliver(this._packet);
					this._packet = null;
					return true;
				}
				return false;
			}

			
			private NetPacket _packet;

			
			private long _timeStamp;

			
			private bool _isSent;
		}
	}
}
