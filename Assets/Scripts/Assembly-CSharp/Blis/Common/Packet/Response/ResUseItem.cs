using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResUseItem, false)]
	public class ResUseItem : ResPacket
	{
		[Key(1)] public bool success;
	}
}