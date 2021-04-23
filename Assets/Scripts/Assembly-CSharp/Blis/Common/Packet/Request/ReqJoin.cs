using System.Collections.Generic;
using Blis.Common.Utils;
using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqJoin, true)]
	public class ReqJoin : ReqPacketForResponse
	{
		[Key(1)] public string identifier;


		[Key(2)] public string language;


		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			playerSession.Identifier = identifier;
			playerSession.SetLanguage(language);
			
			SnapshotWrapper snapshot = gameService.Player.GetSnapshot(playerSession.Character.ObjectId);
			gameService.AddTempNicknameDic(playerSession.userId, playerSession.nickname);
			gameServer.BroadcastJoinUser(playerSession, snapshot);
			ResJoin resJoin = new ResJoin();
			resJoin.userId = playerSession.userId;
			resJoin.nickname = playerSession.nickname;
			resJoin.isObserver = false;
			resJoin.startingWeaponCode = playerSession.startingWeaponCode;
			resJoin.userList = GetPlayerInfos(playerSession, gameService.Player, gameService.Bot);
			resJoin.character = snapshot;
			resJoin.snapshot = gameService.Level.LevelSnapshot;
			resJoin.playerSnapshot = playerSession.Character.CreateMyPlayerSnapshot();
			RestService inst = MonoBehaviourInstance<RestService>.inst;
			long? num;
			if (inst == null)
			{
				num = null;
			}
			else
			{
				BattleGame battleGame = inst.BattleGame;
				num = battleGame != null ? new long?(battleGame.id) : null;
			}

			resJoin.gameId = num ?? 0L;
			return resJoin;
		}


		public override ResPacket Action(GameServer gameServer, GameService gameService,
			ObserverSession observerSession)
		{
			SnapshotWrapper snapshot = gameService.Player.GetSnapshot(observerSession.Observer.ObjectId);
			gameServer.BroadcastJoinUser(observerSession, snapshot);
			ResJoin resJoin = new ResJoin();
			resJoin.userId = observerSession.userId;
			resJoin.nickname = observerSession.nickname;
			resJoin.isObserver = true;
			resJoin.userList = gameService.GetPlayerInfosForObserver();
			resJoin.character = snapshot;
			resJoin.snapshot = gameService.Level.LevelSnapshot;
			resJoin.playerSnapshot = observerSession.Observer.CreateMyPlayerSnapshot();
			RestService inst = MonoBehaviourInstance<RestService>.inst;
			long? num;
			if (inst == null)
			{
				num = null;
			}
			else
			{
				BattleGame battleGame = inst.BattleGame;
				num = battleGame != null ? new long?(battleGame.id) : null;
			}

			resJoin.gameId = num ?? 0L;
			return resJoin;
		}


		private List<UserInfo> GetPlayerInfos(PlayerSession userPlayerSession, PlayerService playerService,
			BotService botService)
		{
			List<UserInfo> list = new List<UserInfo>();
			foreach (PlayerSession playerSession in playerService.PlayerSessions)
			{
				if (userPlayerSession.userId != playerSession.userId)
				{
					list.Add(new UserInfo
					{
						characterSnapshot = playerService.GetSnapshot(playerSession.Character.ObjectId),
						userId = playerSession.userId,
						nickname = MonoBehaviourInstance<GameService>.inst.CheckAndChangeNickname(userPlayerSession,
							playerSession),
						startingWeaponCode = playerSession.startingWeaponCode,
						playerSnapshot = playerSession.Character.CreatePlayerSnapshot()
					});
				}
			}

			foreach (WorldPlayerCharacter worldPlayerCharacter in botService.Characters)
			{
				WorldPlayerCharacter worldPlayerCharacter2 = worldPlayerCharacter;
				list.Add(new UserInfo
				{
					characterSnapshot = botService.GetSnapshot(worldPlayerCharacter2.ObjectId),
					userId = worldPlayerCharacter2.PlayerSession.userId,
					nickname = MonoBehaviourInstance<GameService>.inst.CheckAndChangeNickname(userPlayerSession,
						worldPlayerCharacter2.PlayerSession),
					startingWeaponCode = worldPlayerCharacter2.PlayerSession.startingWeaponCode
				});
			}

			return list;
		}
	}
}