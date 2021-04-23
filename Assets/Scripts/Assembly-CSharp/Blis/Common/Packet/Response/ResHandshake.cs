using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResHandshake, false)]
	public class ResHandshake : ResPacket
	{
		[Key(1)] public string hostname;
	}
}