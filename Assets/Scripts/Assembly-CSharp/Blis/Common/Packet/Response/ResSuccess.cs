using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResSuccess, false)]
	public class ResSuccess : ResPacket { }
}