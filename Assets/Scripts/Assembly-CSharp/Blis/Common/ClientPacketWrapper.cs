using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class ClientPacketWrapper : PacketWrapper
	{
		
		[Key(2)] public long userId;

		
		public ClientPacketWrapper() { }

		
		public ClientPacketWrapper(long userId, IPacket packet) : base(packet)
		{
			this.userId = userId;
		}
	}
}