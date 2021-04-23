using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Server
{
	public class GameService : MonoBehaviourInstance<GameService>
	{
		public GameServer Server { get; private set; }
		public GameWorld World { get; private set; }
		public AirSupplyService AirSupply { get; private set; }
		public AnnounceService Announce { get; private set; }
		public BotService Bot { get; private set; }
		public LevelService Level { get; private set; }
		public StartConditionService StartCondition { get; private set; }
		public PlayerService Player { get; private set; }
		public PlayerCharacterService PlayerCharacter { get; private set; }
		public SpawnService Spawn { get; private set; }
		public AreaService Area { get; private set; }
		public TutorialService Tutorial { get; private set; }
		public GameStatus GameStatus { get; private set; }
		public LevelData CurrentLevel { get; private set; }
		public BattleResultService BattleResult { get; private set; }
		public IgnoreTargetService IgnoreTargetService { get; private set; }
		
		public bool IsGameStarted()
		{
			return this.GameStatus >= GameStatus.Started;
		}
		
		public bool IsJoinable()
		{
			return this.GameStatus < GameStatus.ReadyToStart;
		}
		
		public bool IsInitialized()
		{
			return this.GameStatus >= GameStatus.Initialized;
		}
		
		public bool HasSomeoneJoined()
		{
			return this.GameStatus >= GameStatus.FirstJoin;
		}
		
		public bool IsLevelLoaded()
		{
			return this.GameStatus >= GameStatus.LevelLoaded;
		}
		
		public MatchingMode MatchingMode { get; private set; }
		public MatchingTeamMode MatchingTeamMode { get; private set; }
		
		public bool IsTeamMode
		{
			get
			{
				return this.MatchingTeamMode.IsTeamMode();
			}
		}
		
		public DateTime GameStartTime { get; private set; }

		public void SetMatchingMode(MatchingMode matchingMode, MatchingTeamMode matchingTeamMode)
		{
			this.MatchingMode = matchingMode;
			this.MatchingTeamMode = matchingTeamMode;
		}
		
		public string BattleTokenKey { get; private set; }
		public Dictionary<int, MatchingTeamToken> BattleTeamMap
		{
			get
			{
				return this.battleTeamMap;
			}
		}
		
		public HideNameSettingDic HideNameSettingDic { get; } = new HideNameSettingDic();
		public Dictionary<long, NicknamePair> TempNicknameDic { get; } = new Dictionary<long, NicknamePair>();
		
		public void SetBattleTokenKey(string battleTokenKey)
		{
			this.BattleTokenKey = battleTokenKey;
		}
		
		public void AddBattleTeam(Dictionary<int, MatchingTeamToken> teamMap)
		{
			if (this.battleTeamMap.Count > 0 && this.MatchingMode != MatchingMode.Dev && this.MatchingMode != MatchingMode.Test)
			{
				return;
			}
			foreach (KeyValuePair<int, MatchingTeamToken> keyValuePair in teamMap)
			{
				if (!keyValuePair.Value.isBotTeam)
				{
					if (this.battleTeamMap.ContainsKey(keyValuePair.Key))
					{
						MatchingTeamToken matchingTeamToken = this.battleTeamMap[keyValuePair.Key];
						using (Dictionary<long, MatchingTeamMemberToken>.Enumerator enumerator2 = keyValuePair.Value.teamMembers.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								KeyValuePair<long, MatchingTeamMemberToken> keyValuePair2 = enumerator2.Current;
								matchingTeamToken.teamMembers.Add(keyValuePair2.Key, keyValuePair2.Value);
							}
							continue;
						}
					}
					this.battleTeamMap.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		public void SetCurrentLevel(LevelData levelData)
		{
			this.CurrentLevel = levelData;
		}
		
		public int IncreaseAndGetItemId()
		{
			int result = this.itemIdIdx + 1;
			this.itemIdIdx = result;
			return result;
		}
		
		public void SetGameStatus(GameStatus gameStatus)
		{
			if (gameStatus < this.GameStatus)
			{
				throw new Exception("Invalid Game Status");
			}
			if (gameStatus == GameStatus.Started)
			{
				this.timeGameStart = Time.realtimeSinceStartup;
			}
			Log.H("GameService Status Changed: {0}", new object[]
			{
				gameStatus
			});
			this.GameStatus = gameStatus;
		}

		public float TimeSinceGameStart
		{
			get
			{
				if (!this.IsGameStarted())
				{
					return 0f;
				}
				return Time.realtimeSinceStartup - this.timeGameStart;
			}
		}
		
		public float CurrentServerFrameTime
		{
			get
			{
				if (this.Server.Seq != this.preServerSeq)
				{
					this.currentServerFrameTime = (float)this.Server.Seq * 0.033333335f;
					this.preServerSeq = this.Server.Seq;
				}
				return this.currentServerFrameTime;
			}
		}
		
		public void Init(GameServer server)
		{
			if (this.IsInitialized())
			{
				Log.W("Already Initialized.");
				return;
			}
			
			this.Server = server;
			Log.H("Init GameService");
			
			this.SetGameStatus(GameStatus.Initialized);
			this.World = GameUtil.TryGetOrAddComponent<GameWorld>(base.gameObject);
			this.AirSupply = GameUtil.TryGetOrAddComponent<AirSupplyService>(base.gameObject);
			this.Announce = GameUtil.TryGetOrAddComponent<AnnounceService>(base.gameObject);
			this.Bot = GameUtil.TryGetOrAddComponent<BotService>(base.gameObject);
			this.Level = GameUtil.TryGetOrAddComponent<LevelService>(base.gameObject);
			this.StartCondition = GameUtil.TryGetOrAddComponent<StartConditionService>(base.gameObject);
			this.Player = GameUtil.TryGetOrAddComponent<PlayerService>(base.gameObject);
			this.PlayerCharacter = GameUtil.TryGetOrAddComponent<PlayerCharacterService>(base.gameObject);
			this.Spawn = GameUtil.TryGetOrAddComponent<SpawnService>(base.gameObject);
			this.Area = GameUtil.TryGetOrAddComponent<AreaService>(base.gameObject);
			this.BattleResult = GameUtil.TryGetOrAddComponent<BattleResultService>(base.gameObject);
			this.Tutorial = GameUtil.TryGetOrAddComponent<TutorialService>(base.gameObject);
			this.IgnoreTargetService = GameUtil.TryGetOrAddComponent<IgnoreTargetService>(base.gameObject);
		}

		
		public void LoadLevel(LevelData levelData, BattleToken battleToken, string spawnPointPath)
		{
			this.CurrentLevel = levelData;
			this.Level.LoadLevel(levelData, spawnPointPath, false);
			if (battleToken.matchingMode == MatchingMode.Custom)
			{
				this.Area.LoadLevel(levelData, battleToken.isOnAcceleration);
			}
			else
			{
				this.Area.LoadLevel(levelData);
			}
			if (battleToken.botCount > 0)
			{
				this.Bot.CreateBotPlayers(battleToken.matchingTeamMode, battleToken.botDifficulty, battleToken.botCount);
			}
			else
			{
				List<MatchingTeamToken> botTeams = (from m in battleToken.matchingTeams
				where m.Value.isBotTeam
				select m.Value).ToList<MatchingTeamToken>();
				this.Bot.CreateBotPlayers(battleToken.botDifficulty, botTeams);
			}
			this.Server.SaveInitialWorldSnapshot(this.Level.LevelSnapshot);
			this.SetGameStatus(GameStatus.LevelLoaded);
		}

		
		public void LoadTutorialLevel(LevelData levelData, int botCount, BotDifficulty botDifficulty, string spawnPointPath)
		{
			this.CurrentLevel = levelData;
			
			this.Level.LoadLevel(levelData, spawnPointPath, true);
			this.Area.LoadLevel(levelData, false);
			this.Bot.CreateTutorialBotPlayers(botDifficulty, botCount);
			this.Server.SaveInitialWorldSnapshot(this.Level.LevelSnapshot);
			this.SetGameStatus(GameStatus.LevelLoaded);
		}

		
		public void StartGame()
		{
			this.GameStartTime = DateTime.Now;
			this.PlayerCharacter.Init();
			
			Log.V("[GameServer] start.");
			
			if (this.MatchingMode.IsTutorialMode())
			{
				this.Player.ApplyTutorialStrategySheet();
				this.Bot.ApplyTutorialStrategySheet();
			}
			else
			{
				this.Player.ApplyStrategySheet();
				this.Bot.ApplyStrategySheet();
			}
			Log.V("[GameServer] end.");
			this.PlayerCharacter.StartMovingDistanceMeasurement();
			this.PlayerCharacter.UpdateMasteryWhenGameStarted();
			this.Bot.InitMMRContext();
			base.StartCoroutine(this._StartGame());
		}

		
		private IEnumerator _StartGame()
		{
			yield return new WaitForSeconds(0.3f);
			if (this.MatchingMode.IsTutorialMode())
			{
				TutorialSettingData tutorialSettingData = GameDB.tutorial.GetTutorialSettingData(this.MatchingMode.ConvertToTutorialType());
				this.Area.StartRestriction(tutorialSettingData.beginDayNight);
				this.Area.StartCheckFirstVisitArea();
				if (!tutorialSettingData.enableBattleTimer)
				{
					this.Area.StopRestriction();
				}
				if (0 < tutorialSettingData.restrictedAreas.Count)
				{
					this.Area.ApplyTutorialRestrictArea(tutorialSettingData);
				}
			}
			else
			{
				this.Area.StartRestriction(DayNight.Day);
				this.Area.StartCheckFirstVisitArea();
				this.Spawn.SpawnMonsterCreateTimeNonZero(this.CurrentLevel);
			}
			this.Spawn.listLifeoftrees.ForEach(delegate(WorldResourceItemBox x)
			{
				x.StartCooldown();
			});
			this.Server.Broadcast(new RpcStartGame
			{
				frameUpdateRate = 0.033333335f
			}, NetChannel.ReliableOrdered);
			this.PlayerCharacter.SendStartEmotion();
			this.SetGameStatus(GameStatus.Started);
		}

		
		public void FinishGame()
		{
			if (this.GameStatus >= GameStatus.Finished)
			{
				return;
			}
			this.World.SightObjectPoolLog();
			this.SetGameStatus(GameStatus.Finished);
			this.Server.OnFinishGame();
		}

		
		public void SettingTeam(Dictionary<int, List<WorldPlayerCharacter>> teamPlayers)
		{
			this.teamPlayers = teamPlayers;
		}

		
		public void SettingBotTeam(Dictionary<int, List<WorldPlayerCharacter>> botTeamPlayers)
		{
			foreach (KeyValuePair<int, List<WorldPlayerCharacter>> keyValuePair in botTeamPlayers)
			{
				this.teamPlayers.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		
		public int GetAliveTeamCount()
		{
			if (this.teamPlayers == null)
			{
				return 0;
			}
			int num = 0;
			foreach (List<WorldPlayerCharacter> list in this.teamPlayers.Values)
			{
				using (List<WorldPlayerCharacter>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.IsAlive)
						{
							num++;
							break;
						}
					}
				}
			}
			return num;
		}

		
		public List<WorldPlayerCharacter> GetAliveTeam()
		{
			foreach (List<WorldPlayerCharacter> list in this.teamPlayers.Values)
			{
				using (List<WorldPlayerCharacter>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.IsAlive)
						{
							return list;
						}
					}
				}
			}
			return null;
		}

		
		public int GetCurrentRank()
		{
			return this.World.FindAll<WorldPlayerCharacter>((WorldPlayerCharacter x) => x.IsAlive).Count + 1;
		}

		
		public int GetCurrentTeamRank(int teamNumber)
		{
			using (List<WorldPlayerCharacter>.Enumerator enumerator = this.teamPlayers[teamNumber].GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsAlive)
					{
						return -1;
					}
				}
			}
			return this.CalcTeamRank();
		}

		
		public int CalcTeamRank()
		{
			int num = 0;
			foreach (int teamNumber in this.battleTeamMap.Keys)
			{
				if (this.Player.IsAnnihilationTeam(teamNumber))
				{
					num++;
				}
			}
			return this.GetAliveTeamCount() + num;
		}

		
		public List<UserInfo> GetPlayerInfosForObserver()
		{
			List<UserInfo> list = new List<UserInfo>();
			foreach (PlayerSession playerSession in this.Player.PlayerSessions)
			{
				list.Add(new UserInfo
				{
					characterSnapshot = this.Player.GetSnapshot(playerSession.Character.ObjectId),
					userId = playerSession.userId,
					nickname = playerSession.nickname,
					startingWeaponCode = playerSession.startingWeaponCode,
					playerSnapshot = playerSession.Character.CreateAllyPlayerSnapshot()
				});
			}
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.Bot.Characters)
			{
				WorldPlayerCharacter worldPlayerCharacter2 = (WorldPlayerCharacter)worldPlayerCharacter;
				list.Add(new UserInfo
				{
					characterSnapshot = this.Bot.GetSnapshot(worldPlayerCharacter2.ObjectId),
					userId = worldPlayerCharacter2.PlayerSession.userId,
					nickname = worldPlayerCharacter2.PlayerSession.nickname,
					startingWeaponCode = worldPlayerCharacter2.PlayerSession.startingWeaponCode,
					playerSnapshot = worldPlayerCharacter2.CreateAllyPlayerSnapshot()
				});
			}
			return list;
		}

		
		public List<SimpleUserInfo> GetPlayerSimpleInfosForObserver()
		{
			List<SimpleUserInfo> list = new List<SimpleUserInfo>();
			foreach (PlayerSession playerSession in this.Player.PlayerSessions)
			{
				list.Add(new SimpleUserInfo
				{
					userId = playerSession.userId,
					playerSnapshot = playerSession.Character.CreateAllyPlayerSnapshot(),
					position = new BlisVector(playerSession.Character.GetPosition()),
					moveAgentSnapshot = playerSession.Character.MoveAgent.CreateSnapshot()
				});
			}
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.Bot.Characters)
			{
				WorldPlayerCharacter worldPlayerCharacter2 = (WorldPlayerCharacter)worldPlayerCharacter;
				list.Add(new SimpleUserInfo
				{
					userId = worldPlayerCharacter2.PlayerSession.userId,
					playerSnapshot = worldPlayerCharacter2.CreateAllyPlayerSnapshot(),
					position = new BlisVector(worldPlayerCharacter2.GetPosition()),
					moveAgentSnapshot = worldPlayerCharacter2.MoveAgent.CreateSnapshot()
				});
			}
			return list;
		}

		
		public List<SimpleSummonInfo> GetSummonSimpleInfosForObserver()
		{
			List<SimpleSummonInfo> list = new List<SimpleSummonInfo>();
			foreach (WorldObject worldObject in this.World.GetCachedObjects(typeof(WorldSummonBase)))
			{
				WorldSummonBase worldSummonBase = worldObject as WorldSummonBase;
				IServerMoveAgentOwner serverMoveAgentOwner = worldSummonBase as IServerMoveAgentOwner;
				if (serverMoveAgentOwner == null)
				{
					list.Add(new SimpleSummonInfo
					{
						objectId = worldSummonBase.ObjectId,
						position = new BlisVector(worldSummonBase.GetPosition())
					});
				}
				else
				{
					list.Add(new SimpleSummonInfo
					{
						objectId = worldSummonBase.ObjectId,
						position = new BlisVector(worldSummonBase.GetPosition()),
						moveAgentSnapshot = serverMoveAgentOwner.MoveAgent.CreateSnapshot()
					});
				}
			}
			return list;
		}

		
		public List<SimpleMonsterInfo> GetMonsterSimpleInfosForObserver()
		{
			List<SimpleMonsterInfo> list = new List<SimpleMonsterInfo>();
			foreach (WorldObject worldObject in this.World.GetCachedObjects(typeof(WorldMonster)))
			{
				WorldMonster worldMonster = worldObject as WorldMonster;
				list.Add(new SimpleMonsterInfo
				{
					objectId = worldMonster.ObjectId,
					position = new BlisVector(worldMonster.GetPosition()),
					moveAgentSnapshot = worldMonster.MoveAgent.CreateSnapshot()
				});
			}
			return list;
		}

		
		public string CheckAndChangeNickname(Session takerUser, Session otherUser)
		{
			if (takerUser == null || otherUser == null)
			{
				return null;
			}
			if (takerUser.userId == otherUser.userId)
			{
				return takerUser.nickname;
			}
			HideNameSetting userHideNameSetting = this.HideNameSettingDic.GetUserHideNameSetting(takerUser.userId);
			HideNameSetting userHideNameSetting2 = this.HideNameSettingDic.GetUserHideNameSetting(otherUser.userId);
			bool flag = false;
			if (!takerUser.IsObserverSession && (!this.IsTeamMode || takerUser.TeamNumber != otherUser.TeamNumber))
			{
				flag = (userHideNameSetting2.hideNameFromEnemy || userHideNameSetting.hideNameFromEnemy);
			}
			if (flag)
			{
				if (!this.TempNicknameDic.ContainsKey(otherUser.userId))
				{
					this.AddTempNicknameDic(otherUser.userId, otherUser.nickname);
				}
				return this.TempNicknameDic[otherUser.userId].temp;
			}
			return otherUser.nickname;
		}

		
		public void AddTempNicknameDic(long userId, string nickname)
		{
			if (!this.TempNicknameDic.ContainsKey(userId))
			{
				this.TempNicknameDic.Add(userId, new NicknamePair(nickname, string.Format("TempNickname/{0}", this.TempNicknameDic.Count + 1)));
			}
		}

		
		public Vector3 GetDropItemPosition(Vector3 center)
		{
			Queue<Vector3> queue = new Queue<Vector3>();
			queue.Enqueue(center);
			int num = 0;
			int num2 = 0;
			int count = this.itemSpawnDirection.Count;
			int num3 = count;
			while (queue.Count > 0)
			{
				Vector3 vector = queue.Dequeue();
				NavMeshHit navMeshHit;
				bool flag = NavMesh.Raycast(center, vector, out navMeshHit, -1);
				int num4 = Physics.OverlapBox(vector, new Vector3(0.2f, 1f, 0.2f), Quaternion.identity, GameConstants.LayerMask.WORLD_OBJECT_LAYER).Count((Collider col) => col.GetComponent<WorldPlayerCharacter>() == null);
				if (!flag && num4 == 0)
				{
					return vector;
				}
				if (num < this.itemSpawnDepth)
				{
					this.itemSpawnDirection.Shuffle<Vector3>();
					for (int i = 0; i < count; i++)
					{
						queue.Enqueue(vector + this.itemSpawnDirection[i]);
					}
					num2++;
					if (num3 == num2)
					{
						num3 *= count;
						num++;
					}
				}
			}
			return center;
		}

		
		public void OnFinalSafeZoneActive()
		{
			this.GetAliveTeam().ForEach(delegate(WorldPlayerCharacter x)
			{
				if (this.Area.IsInFinalSafeZone(x.GetPosition()))
				{
					x.CancelRest();
				}
			});
		}

		
		public IEnumerator StoreChatLogsThroughRestService(string reason)
		{
			if (!MonoBehaviourInstance<RestService>.inst)
			{
				yield break;
			}
			BattleGame battleGame = MonoBehaviourInstance<RestService>.inst.BattleGame;
			long id = battleGame.id;
			string battleTokenKey = battleGame.battleTokenKey;
			List<BattleChatMember> members = (from p in this.Player.PlayerSessions
			select new BattleChatMember
			{
				team = p.TeamNumber,
				userNum = p.userId,
				nickname = p.nickname
			}).ToList<BattleChatMember>();
			
			List<BattleChatMessage> messages = this.PopChatMessages();
			BattleChatContext context = new BattleChatContext(id, battleGame.startDtm, battleGame.endDtm, battleTokenKey, reason, members, messages);
			
			Log.V("Begin");
			yield return base.StartCoroutine(MonoBehaviourInstance<RestService>.inst.LogBattleChat(context));
			Log.V("End");
		}

		
		public void PutChatMessage(BattleChatMessage context)
		{
			this.ChatMessages.Add(context);
		}

		
		private List<BattleChatMessage> PopChatMessages()
		{
			List<BattleChatMessage> result = new List<BattleChatMessage>(this.ChatMessages);
			this.ChatMessages.Clear();
			return result;
		}

		
		private int itemIdIdx;

		
		private float timeGameStart;

		
		private Dictionary<int, List<WorldPlayerCharacter>> teamPlayers;

		
		public readonly List<BattleChatMessage> ChatMessages = new List<BattleChatMessage>();

		
		private readonly Dictionary<int, MatchingTeamToken> battleTeamMap = new Dictionary<int, MatchingTeamToken>();

		
		private float currentServerFrameTime;

		
		private int preServerSeq;

		
		public readonly float ServerFrameDeltaTime = 0.033333335f;

		
		private readonly List<Vector3> itemSpawnDirection = new List<Vector3>
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 0f, -1f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(0f, 0f, 1f)
		};

		
		private readonly int itemSpawnDepth = 4;
	}
}
