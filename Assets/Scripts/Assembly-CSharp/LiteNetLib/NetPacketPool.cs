using System;
using System.Threading;

namespace LiteNetLib
{
	
	internal sealed class NetPacketPool
	{
		
		public NetPacket GetWithData(PacketProperty property, byte[] data, int start, int length)
		{
			int headerSize = NetPacket.GetHeaderSize(property);
			NetPacket packet = this.GetPacket(length + headerSize);
			packet.Property = property;
			Buffer.BlockCopy(data, start, packet.RawData, headerSize, length);
			return packet;
		}

		
		public NetPacket GetWithProperty(PacketProperty property, int size)
		{
			NetPacket packet = this.GetPacket(size + NetPacket.GetHeaderSize(property));
			packet.Property = property;
			return packet;
		}

		
		public NetPacket GetWithProperty(PacketProperty property)
		{
			NetPacket packet = this.GetPacket(NetPacket.GetHeaderSize(property));
			packet.Property = property;
			return packet;
		}

		
		public NetPacket GetPacket(int size)
		{
			if (size <= NetConstants.MaxPacketSize)
			{
				NetPacket netPacket = null;
				this._lock.EnterUpgradeableReadLock();
				if (this._count > 0)
				{
					this._lock.EnterWriteLock();
					this._count--;
					netPacket = this._pool[this._count];
					this._pool[this._count] = null;
					this._lock.ExitWriteLock();
				}
				this._lock.ExitUpgradeableReadLock();
				if (netPacket != null)
				{
					netPacket.Size = size;
					if (netPacket.RawData.Length < size)
					{
						netPacket.RawData = new byte[size];
					}
					return netPacket;
				}
			}
			return new NetPacket(size);
		}

		
		public void Recycle(NetPacket packet)
		{
			if (packet.RawData.Length > NetConstants.MaxPacketSize)
			{
				return;
			}
			packet.RawData[0] = 0;
			this._lock.EnterUpgradeableReadLock();
			if (this._count == 1000)
			{
				this._lock.ExitUpgradeableReadLock();
				return;
			}
			this._lock.EnterWriteLock();
			this._pool[this._count] = packet;
			this._count++;
			this._lock.ExitWriteLock();
			this._lock.ExitUpgradeableReadLock();
		}

		
		private readonly NetPacket[] _pool = new NetPacket[1000];

		
		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

		
		private int _count;
	}
}
