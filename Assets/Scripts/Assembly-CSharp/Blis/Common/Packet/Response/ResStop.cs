using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResStop, false)]
	public class ResStop : ResPacket
	{
		[Key(1)] public bool cancelNormalAttack;
	}
}