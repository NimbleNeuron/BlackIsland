using System.Collections.Generic;

namespace LiteNetLib
{
	
	internal abstract class BaseChannel
	{
		
		protected BaseChannel(NetPeer peer)
		{
			this.Peer = peer;
			this.OutgoingQueue = new Queue<NetPacket>(64);
		}

		
		
		public int PacketsInQueue
		{
			get
			{
				return this.OutgoingQueue.Count;
			}
		}

		
		public void AddToQueue(NetPacket packet)
		{
			Queue<NetPacket> outgoingQueue = this.OutgoingQueue;
			lock (outgoingQueue)
			{
				this.OutgoingQueue.Enqueue(packet);
			}
		}

		
		public abstract void SendNextPackets();

		
		public abstract bool ProcessPacket(NetPacket packet);

		
		public BaseChannel Next;

		
		protected readonly NetPeer Peer;

		
		protected readonly Queue<NetPacket> OutgoingQueue;
	}
}
