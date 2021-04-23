using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqExitTeamGame, false)]
	public class ReqExitTeamGame : ReqPacket
	{
		[Key(0)] public bool openGameResult;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession) { }
	}
}