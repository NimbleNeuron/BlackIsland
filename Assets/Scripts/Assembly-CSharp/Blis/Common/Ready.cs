using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqReady, true)]
	public class Ready : ReqPacket
	{
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			Log.V(string.Format("[Ready][Player] {0}({1}) request start game", playerSession.nickname,
				playerSession.userId));
			ReadyToGame(gameService, playerSession);
		}


		public override void Action(GameServer gameServer, GameService gameService, ObserverSession observerSession)
		{
			Log.V(string.Format("[Ready][Observer] {0}({1}) request start game", observerSession.nickname,
				observerSession.userId));
			ReadyToGame(gameService, observerSession);
		}


		private void ReadyToGame(GameService gameService, Session session)
		{
			if (session.IsReady)
			{
				return;
			}

			if (gameService.IsGameStarted())
			{
				throw new GameException(ErrorType.GameStartedAlready);
			}

			Log.V(string.Format("[Ready] user {0}({1}) request start game", session.nickname, session.userId));
			session.Ready();
			if (gameService.StartCondition.CheckAllUserReady())
			{
				gameService.StartCondition.SetupGame();
			}
		}
	}
}