using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject(false)]
	public class ServerPacketWrapper : PacketWrapper
	{
		
		public ServerPacketWrapper()
		{
		}

		
		public ServerPacketWrapper(int seq, int serverTime, IPacket packet) : base(packet)
		{
			this.serverTime = serverTime;
		}

		
		[Key(2)]
		public int serverTime;
	}
}
