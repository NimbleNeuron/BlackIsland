using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResAskGameSetup, false)]
	public class ResAskGameSetup : ResPacket
	{
		[Key(1)] public bool readyToStart;


		[Key(2)] public int standByTime;
	}
}