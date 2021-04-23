using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResIgnorePlayer, false)]
	public class ResIgnorePlayer : ResPacket
	{
		[Key(1)] public bool isIgnore;
	}
}