using System;
using LiteNetLib.Utils;

namespace LiteNetLib
{
	
	public sealed class NetPacketReader : NetDataReader
	{
		
		private readonly NetEvent _evt;

		
		private readonly NetManager _manager;

		
		private NetPacket _packet;

		
		internal NetPacketReader(NetManager manager, NetEvent evt)
		{
			_manager = manager;
			_evt = evt;
		}

		
		internal void SetSource(NetPacket packet)
		{
			if (packet == null)
			{
				return;
			}

			_packet = packet;
			base.SetSource(packet.RawData, packet.GetHeaderSize(), packet.Size);
		}

		
		internal void RecycleInternal()
		{
			Clear();
			if (_packet != null)
			{
				_manager.NetPacketPool.Recycle(_packet);
			}

			_packet = null;
			_manager.RecycleEvent(_evt);
		}

		
		public void Recycle()
		{
			if (_manager.AutoRecycle)
			{
				throw new Exception("Recycle called with AutoRecycle enabled");
			}

			RecycleInternal();
		}
	}
}