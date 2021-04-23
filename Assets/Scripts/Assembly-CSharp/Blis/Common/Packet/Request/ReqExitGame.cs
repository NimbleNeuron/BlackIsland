using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqExitGame, false)]
	public class ReqExitGame : ReqPacket
	{
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			gameService.Player.ExitGame(playerSession);
		}


		public override void Action(GameServer gameServer, GameService gameService, ObserverSession observerSession)
		{
			observerSession.EnqueueCommandPacket(new CmdFinishGame
			{
				rank = -1
			});
		}
	}
}