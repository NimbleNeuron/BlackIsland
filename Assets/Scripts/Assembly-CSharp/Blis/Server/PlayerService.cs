using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class PlayerService : ServiceBase
	{
		
		
		public IEnumerable<PlayerSession> PlayerSessions
		{
			get
			{
				return this.playerMap.Values;
			}
		}

		
		
		public IEnumerable<ObserverSession> ObserverSessions
		{
			get
			{
				return this.observerMap.Values;
			}
		}

		
		
		public int PlayerCount
		{
			get
			{
				return this.playerCount;
			}
		}

		
		
		public int TotalUserCount
		{
			get
			{
				return this.playerCount + this.observerCount;
			}
		}

		
		
		public int TotalSessionCount
		{
			get
			{
				return this.playerMap.Count + this.observerMap.Count;
			}
		}

		
		
		public int TotalCharacterCount
		{
			get
			{
				return this.playerCount + this.game.Bot.Characters.Count<WorldPlayerCharacter>();
			}
		}

		
		public bool IsObserver(long userId)
		{
			return this.observerMap.Any((KeyValuePair<long, ObserverSession> x) => x.Key == userId);
		}

		
		public void AddObserver(ObserverSession observerSession)
		{
			WorldObserver worldObserver = this.game.Spawn.SpawnObserver();
			observerSession.SetObserver(worldObserver);
			this.observerMap.Add(observerSession.userId, observerSession);
			this.observerCharacterSnapshots.Add(observerSession.ObjectId, worldObserver.CreateSnapshotWrapper());
		}

		
		public void AddPlayer(PlayerSession playerSession)
		{
			WorldPlayerCharacter worldPlayerCharacter = this.game.Spawn.SpawnPlayerCharacter(playerSession.characterId, playerSession.skinCode, playerSession.TeamNumber, SpecialSkillId.None);
			playerSession.SetCharacter(worldPlayerCharacter);
			this.playerMap.Add(playerSession.userId, playerSession);
			this.playerCharacterSnapshots.Add(worldPlayerCharacter.ObjectId, worldPlayerCharacter.CreateSnapshotWrapper());
		}

		
		public void AddTutorialPlayer(TutorialType tutorialType, PlayerSession playerSession)
		{
			WorldPlayerCharacter worldPlayerCharacter = this.game.Spawn.SpawnPlayerCharacter(playerSession.characterId, playerSession.skinCode, playerSession.TeamNumber, SpecialSkillId.None);
			TutorialSettingData tutorialSettingData = GameDB.tutorial.GetTutorialSettingData(tutorialType);
			worldPlayerCharacter.SetCharacterSettingCode(tutorialSettingData.playerSettingDataCode);
			playerSession.SetCharacter(worldPlayerCharacter);
			this.playerMap.Add(playerSession.userId, playerSession);
			this.playerCharacterSnapshots.Add(worldPlayerCharacter.ObjectId, worldPlayerCharacter.CreateSnapshotWrapper());
		}

		
		public void UserDisconnected(Session session)
		{
		}

		
		public void DropSession(PlayerSession playerSession)
		{
			if (this.game.IsGameStarted() && playerSession.Character.IsAlive)
			{
				playerSession.Character.Dead(DamageType.RedZone);
			}
		}

		
		public SnapshotWrapper GetSnapshot(int objectId)
		{
			SnapshotWrapper result;
			if (this.observerCharacterSnapshots.TryGetValue(objectId, out result))
			{
				return result;
			}
			if (this.playerCharacterSnapshots.TryGetValue(objectId, out result))
			{
				return result;
			}
			throw new GameException(ErrorType.Internal);
		}

		
		public void RemovePlayer(PlayerSession playerSession)
		{
			this.playerMap.Remove(playerSession.userId);
		}

		
		public void ClearDeadPlayerInfo()
		{
			this.curFrameDeadPlayers.Clear();
		}

		
		public int GetCurFrameDeadPlayerLowestMmr()
		{
			if (this.curFrameDeadPlayers.Count <= 0)
			{
				return 0;
			}
			return this.curFrameDeadPlayers.Min((PlayerService.DeadInfo d) => d.deadPlayer.MMRContext.mmrBefore);
		}

		
		public void DecisionPlayerRaking()
		{
			if (this.curFrameDeadPlayers.Count == 0)
			{
				return;
			}
			WorldPlayerCharacter worldPlayerCharacter = null;
			int aliveTeamCount = MonoBehaviourInstance<GameService>.inst.GetAliveTeamCount();
			int num = (from d in this.curFrameDeadPlayers
			where d.deadPlayer.IsTeamAnnihilation()
			select d.deadPlayer.TeamNumber).Distinct<int>().Count<int>();
			if (aliveTeamCount == 0)
			{
				worldPlayerCharacter = (from d in this.curFrameDeadPlayers
				select d.deadPlayer into c
				orderby c.MMRContext.mmrBefore
				select c).FirstOrDefault<WorldPlayerCharacter>();
			}
			foreach (PlayerService.DeadInfo deadInfo in this.curFrameDeadPlayers)
			{
				WorldPlayerCharacter deadPlayer = deadInfo.deadPlayer;
				int rank = aliveTeamCount + num;
				List<WorldPlayerCharacter> teamCharacters = deadPlayer.PlayerSession.GetTeamCharacters();
				bool flag = teamCharacters.Exists((WorldPlayerCharacter x) => x.IsAlive);
				deadPlayer.rank = rank;
				if (flag)
				{
					deadPlayer.SendResultScoreBoardRanking(-1);
					if (deadPlayer.IsObserving)
					{
						Log.V("[OBSERVING BUG] isTeamMemberAlive && DeadCharacter IsObserving is true");
					}
					deadPlayer.PlayerSession.EnqueueCommandPacket(new CmdFinishGameTeamAlive
					{
						attackerObjectType = deadInfo.attackerObjectType,
						attackerObjectId = deadInfo.attackerObjectId,
						attackerDataCode = deadInfo.attackerDataCode,
						attackerNickName = deadInfo.attackerNickName,
						attackerTempNickname = MonoBehaviourInstance<GameService>.inst.CheckAndChangeNickname(deadPlayer.PlayerSession, this.GetPlayerSession(deadInfo.attackerUserId))
					});
					if (deadInfo.deadPlayer.PlayerSession.IsSurrender)
					{
						deadPlayer.SetRank(-1);
						deadPlayer.MMRContext.OnDeath(null);
						this.ProcessBattleResult(deadPlayer, rank, deadInfo);
					}
				}
				else
				{
					if (deadInfo.attackerUserId == 0L)
					{
						this.noKillerDeathPlayerList.Add(deadPlayer);
					}
					else if (worldPlayerCharacter == null)
					{
						worldPlayerCharacter = this.world.Find<WorldPlayerCharacter>(deadInfo.attackerObjectId);
					}
					deadPlayer.SetRank(rank);
					deadPlayer.SendResultScoreBoardRanking(rank);
					deadPlayer.MMRContext.OnDeath(worldPlayerCharacter);
					this.ProcessBattleResult(deadPlayer, rank, deadInfo);
					if (deadPlayer.IsObserving)
					{
						Log.V("[OBSERVING BUG] DeadCharacter IsObserving is true");
					}
					this.SendFinishGame(deadPlayer.PlayerSession, rank, deadInfo);
					using (List<WorldPlayerCharacter>.Enumerator enumerator2 = teamCharacters.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							WorldPlayerCharacter teamMember = enumerator2.Current;
							if (deadPlayer.ObjectId != teamMember.ObjectId && !this.curFrameDeadPlayers.Any((PlayerService.DeadInfo x) => x.deadPlayer.ObjectId == teamMember.ObjectId))
							{
								teamMember.SetRank(rank);
								teamMember.SendResultScoreBoardRanking(rank);
								teamMember.MMRContext.OnDeath(worldPlayerCharacter);
								this.ProcessBattleResult(teamMember, rank, deadInfo);
								this.SendFinishGame(teamMember.PlayerSession, rank, deadInfo);
							}
						}
					}
				}
			}
			if (aliveTeamCount == 1)
			{
				WorldPlayerCharacter worldPlayerCharacter2 = null;
				foreach (WorldPlayerCharacter worldPlayerCharacter3 in MonoBehaviourInstance<GameService>.inst.GetAliveTeam())
				{
					foreach (WorldPlayerCharacter targetPlayerCharacter in this.noKillerDeathPlayerList)
					{
						worldPlayerCharacter3.MMRContext.OnKill(targetPlayerCharacter);
					}
					if (worldPlayerCharacter3.IsAlive && worldPlayerCharacter2 == null)
					{
						worldPlayerCharacter2 = worldPlayerCharacter3;
					}
					worldPlayerCharacter3.SetRank(1);
					worldPlayerCharacter3.SendResultScoreBoardRanking(1);
					this.ProcessBattleResult(worldPlayerCharacter3, 1, null);
					this.SendFinishGame(worldPlayerCharacter3.PlayerSession, 1, null);
				}
				foreach (ObserverSession session in MonoBehaviourInstance<GameService>.inst.Player.ObserverSessions)
				{
					this.SendObserverFinishGame(session, 1, null, worldPlayerCharacter2);
				}
				this.game.FinishGame();
			}
			else if (!this.playerMap.Any((KeyValuePair<long, PlayerSession> pair) => pair.Value.Character.IsAlive))
			{
				foreach (ObserverSession observerSession in this.observerMap.Values)
				{
					observerSession.EnqueueCommandPacket(new PacketWrapper(new CmdFinishGame
					{
						finishGame = true,
						rank = -1
					}));
				}
				this.game.FinishGame();
			}
			this.curFrameDeadPlayers.Clear();
		}

		
		public void SendFinishGame(Session session, int rank, PlayerService.DeadInfo deadPlayerInfo)
		{
			ObjectType objectType = ObjectType.None;
			int num = 0;
			int dataCode = 0;
			string nickName = "";
			if (deadPlayerInfo != null)
			{
				objectType = deadPlayerInfo.attackerObjectType;
				num = deadPlayerInfo.attackerObjectId;
				dataCode = deadPlayerInfo.attackerDataCode;
				nickName = deadPlayerInfo.attackerNickName;
			}
			this.SendFinishGame(session, rank, objectType, num, dataCode, nickName, MonoBehaviourInstance<GameService>.inst.CheckAndChangeNickname(session, this.GetPlayerSession((long)num)));
		}

		
		public void SendObserverFinishGame(Session session, int rank, PlayerService.DeadInfo deadPlayerInfo, WorldPlayerCharacter finalSurvivor)
		{
			ObjectType objectType = ObjectType.None;
			int objectId = 0;
			int dataCode = 0;
			string nickName = "";
			string tempNickname = "";
			if (deadPlayerInfo != null)
			{
				objectType = deadPlayerInfo.attackerObjectType;
				objectId = deadPlayerInfo.attackerObjectId;
				dataCode = deadPlayerInfo.attackerDataCode;
				nickName = deadPlayerInfo.attackerNickName;
				tempNickname = MonoBehaviourInstance<GameService>.inst.TempNicknameDic[deadPlayerInfo.attackerUserId].temp;
			}
			if (finalSurvivor != null)
			{
				objectType = finalSurvivor.ObjectType;
				objectId = finalSurvivor.ObjectId;
				dataCode = finalSurvivor.CharacterCode;
				nickName = finalSurvivor.PlayerSession.nickname;
				tempNickname = MonoBehaviourInstance<GameService>.inst.TempNicknameDic[finalSurvivor.PlayerSession.userId].temp;
			}
			this.SendFinishGame(session, rank, objectType, objectId, dataCode, nickName, tempNickname);
		}

		
		private void SendFinishGame(Session session, int rank, ObjectType objectType, int objectId, int dataCode, string nickName, string tempNickname)
		{
			if (this.game.MatchingMode.IsTutorialMode())
			{
				this.server.Send(session, new RpcFinishTutorial
				{
					rank = rank,
					attackerObjectType = objectType,
					attackerObjectId = objectId,
					attackerDataCode = dataCode,
					attackerNickName = nickName
				}, NetChannel.ReliableOrdered);
				return;
			}
			if (rank == -1)
			{
				session.EnqueueCommandPacket(new CmdFinishGameTeamAlive
				{
					attackerObjectType = objectType,
					attackerObjectId = objectId,
					attackerDataCode = dataCode,
					attackerNickName = nickName,
					attackerTempNickname = tempNickname
				});
				return;
			}
			bool flag = rank == 1;
			if (!flag)
			{
				flag = !this.playerMap.Any((KeyValuePair<long, PlayerSession> pair) => pair.Value.Character.IsAlive);
			}
			session.EnqueueCommandPacket(new CmdFinishGame
			{
				finishGame = flag,
				rank = rank,
				attackerObjectType = objectType,
				attackerObjectId = objectId,
				attackerDataCode = dataCode,
				attackerNickName = nickName,
				attackerTempNickname = tempNickname,
				nicknamePairDic = MonoBehaviourInstance<GameService>.inst.TempNicknameDic
			});
		}

		
		public void OnPlayerDead(WorldPlayerCharacter deadCharacter, long attackerUserId, ObjectType attackerObjectType, int attackerObjectId, int attackerDataCode, string attackerNickName, DamageType damageType, DamageSubType damageSubType, int damageDataCode, List<int> assistants, int? forceToSetRank = null)
		{
			if (!this.game.IsGameStarted())
			{
				return;
			}
			this.game.Announce.DeadAnnounce(damageType, deadCharacter, attackerObjectType, attackerObjectId, attackerDataCode, damageSubType, assistants);
			this.curFrameDeadPlayers.Add(new PlayerService.DeadInfo(deadCharacter, attackerUserId, attackerObjectType, attackerObjectId, attackerDataCode, attackerNickName, damageType, damageSubType, damageDataCode, forceToSetRank));
		}

		
		public bool IsBattleResultProcessed(WorldPlayerCharacter character)
		{
			return this.BattleResultDoneObjectIds.Contains(character.ObjectId);
		}

		
		public void ProcessBattleResult(WorldPlayerCharacter character, int rank, PlayerService.DeadInfo deadInfo)
		{
			if (this.BattleResultDoneObjectIds.Contains(character.ObjectId))
			{
				return;
			}
			this.BattleResultDoneObjectIds.Add(character.ObjectId);
			if (this.IsGettableReward())
			{
				this.GetRewardACoin(character, rank);
			}
			int num = this.game.MatchingTeamMode.MaxTeamCount();
			if (rank > num || rank <= 0)
			{
				Log.E(string.Format("Error Occured!! Invalid Rank: {0}", rank));
				rank = Mathf.Clamp(rank, 1, num);
			}
			string battleResultTokenKey = this.game.BattleResult.GetBattleResultTokenKey(character.PlayerSession.userId);
			this.server.Send(character.PlayerSession, new RpcBattleResultKey
			{
				battleResultKey = battleResultTokenKey
			}, NetChannel.ReliableOrdered);
			PlayerSession playerSession;
			if (0 < rank && (playerSession = character.PlayerSession) != null)
			{
				playerSession.Character.teamRank = rank;
				playerSession.CalculatePlayTime(this.game.GameStartTime);
			}
		}

		
		private int GetRewardACoin(WorldPlayerCharacter character, int rank)
		{
			return (int)(10f + Mathf.Pow((float)(character.Status.Level - 1), 1.6f));
		}

		
		public void ApplyStrategySheet()
		{
			foreach (PlayerSession playerSession in this.PlayerSessions)
			{
				if (playerSession.StartingSettings.StartingAreaCode <= 0)
				{
					try
					{
						this.LoadDefaultSheet(playerSession);
					}
					catch (Exception ex)
					{
						this.game.Player.DropSession(playerSession);
						Log.W(string.Format("ApplyStrategySheet Drop User : {0}[{1}]", playerSession.nickname, playerSession.userId));
						Log.W(ex.ToString());
						continue;
					}
				}
				BlisVector blisVector = new BlisVector(this.game.Level.GetPlayerSpawnPoint(playerSession.StartingSettings.StartingAreaCode).transform.position);
				foreach (StartItem startItem in GameDB.recommend.GetStartItem(playerSession.StartingSettings.StartItemGroupCode))
				{
					playerSession.StartingSettings.AddStartingInventoryItem(startItem.itemCode, startItem.count);
				}
				try
				{
					playerSession.Character.SetPosition(blisVector.SamplePosition());
					playerSession.Character.SetStartingSettings(playerSession.StartingSettings);
					playerSession.Character.FlushAllUpdatesBeforeStart();
				}
				catch (Exception ex2)
				{
					Log.W(ex2.ToString());
					Log.W(ex2.StackTrace);
				}
			}
		}

		
		private void LoadDefaultSheet(PlayerSession playerSession)
		{
			CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(playerSession.Character.CharacterCode);
			RecommendArea recommendArea = GameDB.recommend.FindRecommendAreaData(playerSession.Character.CharacterCode, characterMasteryData.weapon1);
			RecommendStarting recommendStarting = GameDB.recommend.FindStartingData(playerSession.Character.CharacterCode, characterMasteryData.weapon1);
			playerSession.StartingSettings.SetStartItemGroupCode(recommendStarting.startItemGroupCode);
			playerSession.StartingSettings.SetStartingEquipment(GameDB.item.FindItemByCode(recommendStarting.startWeapon));
			playerSession.StartingSettings.SetStartingArea(recommendArea.area1Code);
		}

		
		public void ApplyTutorialStrategySheet()
		{
			foreach (PlayerSession playerSession in this.PlayerSessions)
			{
				this.LoadTutorialSheet(playerSession);
				BlisVector blisVector = new BlisVector(this.game.Level.GetPlayerSpawnPoint(playerSession.StartingSettings.StartingAreaCode, playerSession.StartingSettings.StartingAreaIndex).transform.position);
				playerSession.Character.SetPosition(blisVector.SamplePosition());
				playerSession.Character.SetStartingSettings(playerSession.StartingSettings);
				playerSession.Character.FlushAllUpdatesBeforeStart();
			}
		}

		
		private void LoadTutorialSheet(PlayerSession playerSession)
		{
			CharacterSettingData characterSettingData = GameDB.tutorial.GetCharacterSettingData(playerSession.Character.CharacterSettingCode);
			playerSession.StartingSettings.SetCharacterSettings(characterSettingData);
		}

		
		public int GetMinimumMMR()
		{
			if (this.initializedMinimumMMR)
			{
				return this.minimumMMR;
			}
			this.initializedMinimumMMR = true;
			if (0 < this.playerMap.Count)
			{
				this.minimumMMR = this.playerMap.Values.Min((PlayerSession session) => session.teamMmr);
			}
			return this.minimumMMR;
		}

		
		public void BuildMatchingUsers(Dictionary<int, MatchingTeamToken> matchingTeams)
		{
			if (this.matchingUserTeamNoMap.Count > 0)
			{
				return;
			}
			foreach (MatchingTeamToken matchingTeamToken in matchingTeams.Values)
			{
				if (!matchingTeamToken.isBotTeam)
				{
					foreach (MatchingTeamMemberToken matchingTeamMemberToken in matchingTeamToken.teamMembers.Values)
					{
						this.playerCount++;
						matchingTeamMemberToken.privateMmr = matchingTeamMemberToken.mmr;
						matchingTeamMemberToken.mmr = matchingTeamToken.teamMMR;
						this.matchingUserTeamNoMap.Add(matchingTeamMemberToken.userNum, matchingTeamToken.teamNo);
					}
				}
			}
		}

		
		public void BuildMatchingObservers(List<MatchingObserverToken> observers)
		{
			if (this.matchingObserverMap.Count > 0)
			{
				return;
			}
			for (int i = 0; i < observers.Count; i++)
			{
				if (!this.matchingObserverMap.Contains(observers[i].userNum))
				{
					this.observerCount++;
					this.matchingObserverMap.Add(observers[i].userNum);
				}
			}
		}

		
		public bool IsValidUser(long userId)
		{
			return this.matchingUserTeamNoMap.ContainsKey(userId) || this.matchingObserverMap.Contains(userId);
		}

		
		public int GetUserTeamNo(long userId)
		{
			return this.matchingUserTeamNoMap[userId];
		}

		
		public void ToAnnihilateTeam(WorldPlayerCharacter finishingAttacker, WorldPlayerCharacter target)
		{
			if (finishingAttacker == null || finishingAttacker.IsAI)
			{
				return;
			}
			if (!this.game.BattleTeamMap.ContainsKey(finishingAttacker.TeamNumber))
			{
				return;
			}
			foreach (long num in this.game.BattleTeamMap[finishingAttacker.TeamNumber].teamMembers.Keys)
			{
				int num2 = (int)num;
				if (this.playerMap.ContainsKey((long)num2))
				{
					this.playerMap[(long)num2].Character.MMRContext.OnKill(target);
				}
			}
		}

		
		public bool IsAnnihilationTeam(int teamNumber)
		{
			bool result = true;
			foreach (long num in this.game.BattleTeamMap[teamNumber].teamMembers.Keys)
			{
				int num2 = (int)num;
				if (this.playerMap.ContainsKey((long)num2))
				{
					PlayerSession playerSession = this.playerMap[(long)num2];
					if (!playerSession.Character.IsDyingCondition && playerSession.Character.IsAlive)
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		
		private bool IsGettableReward()
		{
			return this.game.MatchingMode == MatchingMode.Rank || this.game.MatchingMode == MatchingMode.Normal;
		}

		
		public bool IsAllReady()
		{
			StringBuilder stringBuilder = new StringBuilder("[Player Ready] ");
			foreach (PlayerSession playerSession in this.playerMap.Values)
			{
				stringBuilder.Append(string.Format("nickname: {0} | isReady: {1}", playerSession.nickname, playerSession.IsReady));
				stringBuilder.Append(", ");
			}
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
			Log.V(stringBuilder.ToString());
			stringBuilder.Clear();
			stringBuilder.Append("[Observer Ready] ");
			foreach (ObserverSession observerSession in this.observerMap.Values)
			{
				stringBuilder.Append(string.Format("nickname: {0} | isReady: {1}", observerSession.nickname, observerSession.IsReady));
				stringBuilder.Append(", ");
			}
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
			Log.V(stringBuilder.ToString());
			if (this.playerMap.All((KeyValuePair<long, PlayerSession> x) => x.Value.IsReady))
			{
				return this.observerMap.All((KeyValuePair<long, ObserverSession> x) => x.Value.IsReady);
			}
			return false;
		}

		
		public int AlivePlayerCharacterCount()
		{
			int num = 0;
			foreach (KeyValuePair<long, PlayerSession> keyValuePair in this.playerMap)
			{
				if (!(keyValuePair.Value.Character == null) && keyValuePair.Value.Character.IsAlive)
				{
					num++;
				}
			}
			return num;
		}

		
		public int AlivePlayersSumMasteryLevel(out int alivePlayerCount)
		{
			int num = 0;
			alivePlayerCount = 0;
			foreach (KeyValuePair<long, PlayerSession> keyValuePair in this.playerMap)
			{
				if (!(keyValuePair.Value.Character == null) && keyValuePair.Value.Character.IsAlive)
				{
					num += keyValuePair.Value.Character.GetSumMasteryLevel();
					alivePlayerCount++;
				}
			}
			return num;
		}

		
		public PlayerSession GetPlayerSession(long userId)
		{
			if (userId < 0L)
			{
				return this.game.Bot.GetBotSession(userId);
			}
			if (this.playerMap.ContainsKey(userId))
			{
				return this.playerMap[userId];
			}
			return null;
		}

		
		public PlayerSession GetObjectIdToPlayerSession(int objectId)
		{
			foreach (PlayerSession playerSession in this.playerMap.Values)
			{
				if (playerSession.ObjectId == objectId)
				{
					return playerSession;
				}
			}
			return null;
		}

		
		public void ExitGame(PlayerSession playerSession)
		{
			if (this.IsBattleResultProcessed(playerSession.Character))
			{
				this.server.Send(playerSession, new RpcExitGame(), NetChannel.ReliableOrdered);
				return;
			}
			if (playerSession.Character.IsAlive)
			{
				playerSession.SetSurrender();
				playerSession.Character.Surrender();
				return;
			}
			playerSession.ExitTeamGame();
			playerSession.Character.MMRContext.OnDeath(null);
			if (this.game.IsTeamMode)
			{
				playerSession.Character.SetRank(-1);
			}
			int rank = this.game.IsTeamMode ? this.game.CalcTeamRank() : this.game.GetCurrentRank();
			MonoBehaviourInstance<GameService>.inst.Player.ProcessBattleResult(playerSession.Character, rank, null);
			this.server.Send(playerSession, new RpcExitGame(), NetChannel.ReliableOrdered);
		}

		
		private readonly Dictionary<long, PlayerSession> playerMap = new Dictionary<long, PlayerSession>();

		
		private readonly Dictionary<long, ObserverSession> observerMap = new Dictionary<long, ObserverSession>();

		
		private readonly Dictionary<int, SnapshotWrapper> playerCharacterSnapshots = new Dictionary<int, SnapshotWrapper>();

		
		private readonly Dictionary<int, SnapshotWrapper> observerCharacterSnapshots = new Dictionary<int, SnapshotWrapper>();

		
		private readonly Dictionary<long, int> matchingUserTeamNoMap = new Dictionary<long, int>();

		
		private readonly List<long> matchingObserverMap = new List<long>();

		
		private readonly List<PlayerService.DeadInfo> curFrameDeadPlayers = new List<PlayerService.DeadInfo>();

		
		private readonly List<WorldPlayerCharacter> noKillerDeathPlayerList = new List<WorldPlayerCharacter>();

		
		private int playerCount;

		
		private int observerCount;

		
		private HashSet<int> BattleResultDoneObjectIds = new HashSet<int>();

		
		private int minimumMMR;

		
		private bool initializedMinimumMMR;

		
		public class DeadInfo
		{
			
			public DeadInfo(WorldPlayerCharacter deadPlayer, long attackerUserId, ObjectType attackerObjectType, int attackerObjectId, int attackerDataCode, string attackerNickName, DamageType damageType, DamageSubType damageSubType, int damageDataCode, int? forceToSetRank)
			{
				this.deadPlayer = deadPlayer;
				this.attackerUserId = attackerUserId;
				this.attackerObjectType = attackerObjectType;
				this.attackerObjectId = attackerObjectId;
				this.attackerDataCode = attackerDataCode;
				this.attackerNickName = attackerNickName;
				this.damageType = damageType;
				this.forceToSetRank = forceToSetRank;
				if (attackerObjectType == ObjectType.PlayerCharacter)
				{
					deadPlayer.PlayerSession.killer = "player";
					deadPlayer.PlayerSession.killerDetail = attackerNickName;
					deadPlayer.PlayerSession.causeOfDeath = this.GetDamageName(damageType, damageSubType, damageDataCode);
					return;
				}
				if (attackerObjectType == ObjectType.Monster)
				{
					deadPlayer.PlayerSession.killer = "wildAnimal";
					deadPlayer.PlayerSession.killerDetail = attackerNickName;
					deadPlayer.PlayerSession.causeOfDeath = this.GetDamageName(damageType, damageSubType, damageDataCode);
					return;
				}
				if (damageType == DamageType.RedZone)
				{
					deadPlayer.PlayerSession.killer = "restrictedArea";
					deadPlayer.PlayerSession.killerDetail = deadPlayer.GetCurrentAreaData(MonoBehaviourInstance<GameService>.inst.CurrentLevel).name;
					deadPlayer.PlayerSession.causeOfDeath = "restrictedAreaExplosion";
					return;
				}
				deadPlayer.PlayerSession.causeOfDeath = "unknown";
			}

			
			private string GetDamageName(DamageType damageType, DamageSubType damageSubType, int damageDataCode)
			{
				if (0 < damageDataCode)
				{
					if (damageType != DamageType.Normal)
					{
						if (damageType == DamageType.Skill)
						{
							SkillData skillData = GameDB.skill.GetSkillData(damageDataCode);
							if (skillData != null)
							{
								return skillData.Name;
							}
						}
					}
					else
					{
						switch (damageSubType)
						{
						case DamageSubType.Normal:
							return "basicAttack";
						case DamageSubType.Trap:
						{
							SummonData summonData = GameDB.character.GetSummonData(damageDataCode);
							if (summonData != null)
							{
								return summonData.name;
							}
							break;
						}
						}
					}
				}
				return string.Empty;
			}

			
			public readonly WorldPlayerCharacter deadPlayer;

			
			public long attackerUserId;

			
			public ObjectType attackerObjectType;

			
			public int attackerObjectId;

			
			public int attackerDataCode;

			
			public string attackerNickName;

			
			public readonly DamageType damageType;

			
			public readonly int? forceToSetRank;
		}
	}
}
