using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResUseSkill, false)]
	public class ResUseSkill : ResPacket
	{
		[Key(1)] public int errorCode;
	}
}