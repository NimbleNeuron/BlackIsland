using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResAskGameStart, false)]
	public class ResAskGameStart : ResPacket
	{
		[Key(1)] public bool gameStarted;
	}
}