using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResExitTeamGame, false)]
	public class ResExitTeamGame : ResPacket { }
}