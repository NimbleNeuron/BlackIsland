using System.Collections.Generic;
using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqGameSnapshot, true)]
	public class ReqGameSnapshot : ReqPacketForResponse
	{
		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (!gameService.IsGameStarted())
			{
				return new ResGameSnapshot
				{
					frameUpdateRate = 0.033333335f,
					currentSeq = gameServer.Seq
				};
			}

			if (gameService.GameStatus == GameStatus.Finished)
			{
				throw new GameException(ErrorType.UserGameFinished);
			}

			if (gameService.Player.IsBattleResultProcessed(playerSession.Character))
			{
				throw new GameException(ErrorType.UserGameFinished);
			}

			gameServer.EnqueueCommand(new CmdUserConnected
			{
				connectedObjectId = playerSession.ObjectId
			});
			playerSession.ResendFinishGameTeamAlive();
			return new ResGameSnapshot
			{
				frameUpdateRate = 0.033333335f,
				currentSeq = gameServer.Seq,
				gameSnapshot = CreateGameSnapshot(gameServer, gameService, false),
				userSnapshot = CreateMyPlayerSnapshot(playerSession),
				outSightCharacterIds = playerSession.Character.GetOutSightCharacterIds(),
				invisibleCharacterIds = playerSession.Character.GetInvisibleCharacterIds()
			};
		}


		public override ResPacket Action(GameServer gameServer, GameService gameService,
			ObserverSession observerSession)
		{
			if (gameService.IsGameStarted())
			{
				return new ResGameSnapshot
				{
					frameUpdateRate = 0.033333335f,
					currentSeq = gameServer.Seq,
					gameSnapshot = CreateGameSnapshot(gameServer, gameService, true),
					userSnapshot = CreateObserverSnapshot(observerSession)
				};
			}

			return new ResGameSnapshot
			{
				frameUpdateRate = 0.033333335f,
				currentSeq = gameServer.Seq
			};
		}


		private GameSnapshot CreateGameSnapshot(GameServer gameServer, GameService gameService, bool forObserver)
		{
			List<UserSnapshot> list = new List<UserSnapshot>();
			Dictionary<int, MoveAgentSnapshot> dictionary = new Dictionary<int, MoveAgentSnapshot>();
			foreach (PlayerSession playerSession in gameService.Player.PlayerSessions)
			{
				UserSnapshot item = forObserver
					? CreateAllyPlayerSnapshot(playerSession, playerSession.Character)
					: CreatePlayerSnapshot(playerSession, playerSession.Character);
				list.Add(item);
			}

			foreach (WorldPlayerCharacter worldPlayerCharacter in gameService.Bot.Characters)
			{
				WorldPlayerCharacter worldPlayerCharacter2 = worldPlayerCharacter;
				UserSnapshot item2 = forObserver
					? CreateAllyPlayerSnapshot(worldPlayerCharacter2.PlayerSession, worldPlayerCharacter2)
					: CreatePlayerSnapshot(worldPlayerCharacter2.PlayerSession, worldPlayerCharacter2);
				list.Add(item2);
			}

			foreach (WorldObject worldObject in gameService.World.AllObjectAlloc())
			{
				IServerMoveAgentOwner serverMoveAgentOwner = worldObject as IServerMoveAgentOwner;
				if (serverMoveAgentOwner != null)
				{
					dictionary.Add(worldObject.ObjectId, serverMoveAgentOwner.MoveAgent.CreateSnapshot());
				}
			}

			return new GameSnapshot
			{
				userList = list,
				worldSnapshot = gameService.World.GetWorldSnapshot(gameServer.Seq),
				moveAgentSnapshots = dictionary,
				areaRestrictionRemainTime = new BlisFixedPoint(gameService.Area.AreaRestrictionRemainTime),
				areaStateMap = gameService.Area.AreaStateMap,
				dayNight = gameService.Area.DayNight,
				day = gameService.Area.Day,
				isStopAreaRestriction = gameService.Area.IsStopRestrictedArea,
				wicklineRespawnRemainTime = gameService.Spawn.WicklineRespawnTime != null
					? new BlisFixedPoint(gameService.Spawn.WicklineRespawnTime.Value)
					: null,
				sights = CreateSightSnapshots(gameService)
			};
		}


		private UserSnapshot CreatePlayerSnapshot(Session session, WorldPlayerCharacter character,
			byte[] playerSnapshot = null)
		{
			UserSnapshot userSnapshot = new UserSnapshot();
			userSnapshot.userId = session.userId;
			userSnapshot.characterSnapshot = character.CreateSnapshotWrapper();
			userSnapshot.playerSnapshot = playerSnapshot ?? character.CreatePlayerSnapshot();
			userSnapshot.equips = character.Equipment.GetEquips()
				.ConvertAll<EquipItem>(x => new EquipItem(x.GetEquipSlotType(), x));
			userSnapshot.walkableNavMask = character.WalkableNavMask;
			return userSnapshot;
		}


		private UserSnapshot CreateAllyPlayerSnapshot(Session session, WorldPlayerCharacter character)
		{
			return CreatePlayerSnapshot(session, character, character.CreateAllyPlayerSnapshot());
		}


		private UserSnapshot CreateMyPlayerSnapshot(PlayerSession playerSession)
		{
			WorldPlayerCharacter character = playerSession.Character;
			UserSnapshot userSnapshot =
				CreatePlayerSnapshot(playerSession, character, character.CreateMyPlayerSnapshot());
			userSnapshot.playerSnapshot = character.CreateMyPlayerSnapshot();
			userSnapshot.exp = character.Exp;
			userSnapshot.survivalTime = character.SurvivableTime;
			userSnapshot.inventoryItems = character.Inventory.CreateSnapshot();
			userSnapshot.bulletItems = character.CreateBulletItemSnapshot();
			return userSnapshot;
		}


		private UserSnapshot CreateObserverSnapshot(ObserverSession observerSession)
		{
			WorldObserver observer = observerSession.Observer;
			return new UserSnapshot
			{
				userId = observerSession.userId,
				characterSnapshot = observer.CreateSnapshotWrapper(),
				playerSnapshot = observer.CreateMyPlayerSnapshot()
			};
		}


		private void CollectSights(WorldObject worldObject, List<SightSnapshot> list)
		{
			foreach (ServerSightAgent serverSightAgent in worldObject.GetComponents<ServerSightAgent>())
			{
				if (serverSightAgent.GetOwner() != null)
				{
					list.Add(new SightSnapshot
					{
						attachSightId = serverSightAgent.AttachSightId,
						targetId = worldObject.ObjectId,
						sightRange = new BlisFixedPoint(serverSightAgent.SightRange),
						ownerId = serverSightAgent.SightOwnerId
					});
				}
			}
		}


		private List<SightSnapshot> CreateSightSnapshots(GameService gameService)
		{
			List<SightSnapshot> snapshots = new List<SightSnapshot>();
			gameService.World.FindAllDoAction<WorldSecurityCamera>(delegate(WorldSecurityCamera x)
			{
				CollectSights(x, snapshots);
			});
			gameService.World.FindAllDoAction<WorldSightObject>(delegate(WorldSightObject x)
			{
				CollectSights(x, snapshots);
			});
			return snapshots;
		}
	}
}