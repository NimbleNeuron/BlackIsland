using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResPickUpItem, false)]
	public class ResPickUpItem : ResPacket
	{
		[Key(1)] public int errorCode = -1;
	}
}