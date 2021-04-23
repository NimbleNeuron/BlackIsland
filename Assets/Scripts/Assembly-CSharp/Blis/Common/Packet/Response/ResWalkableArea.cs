using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResWalkableArea, false)]
	public class ResWalkableArea : ResPacket
	{
		[Key(1)] public int walkableNavMask;
	}
}