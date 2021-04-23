using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.ReqChangeToObserver, false)]
	public class ReqChangeToObserver : ReqPacket
	{
		
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (playerSession.Character.IsAlive)
			{
				return;
			}

			if (gameService.MatchingMode != MatchingMode.Custom)
			{
				return;
			}

			if (!playerSession.Character.IsTeamAnnihilation())
			{
				return;
			}

			if (GameStatus.Finished <= gameService.GameStatus)
			{
				return;
			}

			if (gameService.Player.IsObserver(playerSession.userId))
			{
				return;
			}

			gameServer.ChangeToObserver(playerSession);
		}
	}
}