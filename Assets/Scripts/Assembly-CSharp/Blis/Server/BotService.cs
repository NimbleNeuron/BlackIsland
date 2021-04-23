using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	public class BotService : ServiceBase
	{
		
		
		public BotDifficulty Difficulty
		{
			get
			{
				return this.difficulty;
			}
		}

		
		
		public List<WorldPlayerCharacter> Characters
		{
			get
			{
				return this.botPlayerCharacters;
			}
		}

		
		
		public int TotalBotCount
		{
			get
			{
				return this.totalBotCount;
			}
		}

		
		public void CreateBotPlayers(MatchingTeamMode matchingTeamMode, BotDifficulty botDifficulty, int botCount)
		{
			this.difficulty = botDifficulty;
			List<MatchingTeamToken> list = new List<MatchingTeamToken>();
			int num = 10000;
			int num2 = 1;
			List<int> creatableBot = GameDB.bot.GetCreatableBot();
			for (int i = 0; i < botCount; i++)
			{
				int num3 = 0;
				if (MatchingTeamMode.None < matchingTeamMode)
				{
					num3 = i / (int)matchingTeamMode;
				}
				MatchingTeamToken matchingTeamToken = new MatchingTeamToken
				{
					teamNo = num + num3,
					teamMMR = 0,
					isBotTeam = true
				};
				int num4 = creatableBot[UnityEngine.Random.Range(0, creatableBot.Count)];
				CharacterData characterData = GameDB.character.GetCharacterData(num4);
				MatchingTeamMemberToken value = new MatchingTeamMemberToken
				{
					userNum = (long)UnityEngine.Random.Range(int.MinValue, 0),
					nickname = this.GetBotNickName(characterData, num2),
					characterCode = num4
				};
				matchingTeamToken.teamMembers = new Dictionary<long, MatchingTeamMemberToken>
				{
					{
						(long)num2,
						value
					}
				};
				list.Add(matchingTeamToken);
				num2++;
			}
			this.CreateBotPlayers(botDifficulty, list);
		}

		
		public void CreateBotPlayers(BotDifficulty botDifficulty, List<MatchingTeamToken> botTeams)
		{
			this.totalBotCount = botTeams.Count * this.game.MatchingTeamMode.GetMemberCountPerTeam();
			this.difficulty = botDifficulty;
			int num = 0;
			int memberCountPerTeam = MonoBehaviourInstance<GameService>.inst.MatchingTeamMode.GetMemberCountPerTeam();
			List<AreaData> list = new List<AreaData>();
			int botStartAreaCode = this.GetBotStartAreaCode(ref list);
			foreach (MatchingTeamToken matchingTeamToken in botTeams)
			{
				foreach (KeyValuePair<long, MatchingTeamMemberToken> keyValuePair in matchingTeamToken.teamMembers)
				{
					CharacterData characterData = GameDB.character.GetCharacterData(keyValuePair.Value.characterCode);
					WorldBotPlayerCharacter worldBotPlayerCharacter = this.game.Spawn.SpawnBotPlayerCharacter(characterData.code, matchingTeamToken.teamNo, SpecialSkillId.None);
					if (worldBotPlayerCharacter == null)
					{
						Log.E("Can't Spawn Bot Player - Character Code : {0}", keyValuePair.Value.characterCode);
					}
					else
					{
						worldBotPlayerCharacter.SetSession(new PlayerSession(-1, keyValuePair.Value.userNum, keyValuePair.Value.nickname, keyValuePair.Value.characterCode, keyValuePair.Value.weaponCode, keyValuePair.Value.skinCode, matchingTeamToken.teamNo, 0, 0, 0, 0, new List<int>(), new Dictionary<EmotionPlateType, int>(), "", "", "", "", ""));
						worldBotPlayerCharacter.SetStartAreaCode(botStartAreaCode);
						worldBotPlayerCharacter.PlayerSession.SetCharacter(worldBotPlayerCharacter);
						this.botSnapshot.Add(worldBotPlayerCharacter.ObjectId, worldBotPlayerCharacter.CreateSnapshotWrapper());
						this.botPlayerCharacters.Add(worldBotPlayerCharacter);
						MonoBehaviourInstance<GameService>.inst.AddTempNicknameDic(worldBotPlayerCharacter.PlayerSession.userId, worldBotPlayerCharacter.PlayerSession.nickname);
						num++;
						if (num % memberCountPerTeam == 0)
						{
							worldBotPlayerCharacter.SetTeamRole(BotTeamRole.LEADER);
							botStartAreaCode = this.GetBotStartAreaCode(ref list);
						}
						else
						{
							worldBotPlayerCharacter.SetTeamRole(BotTeamRole.FOLLOWER);
						}
						this.AddBotTeamMap(worldBotPlayerCharacter);
					}
				}
			}
		}

		
		public void CreateTutorialBotPlayers(BotDifficulty botDifficulty, int botCount)
		{
			this.difficulty = botDifficulty;
			List<int> botSettingDataCode = GameDB.tutorial.GetTutorialSettingData(MonoBehaviourInstance<GameService>.inst.MatchingMode.ConvertToTutorialType()).botSettingDataCode;
			int num = 10000;
			int memberCountPerTeam = MonoBehaviourInstance<GameService>.inst.MatchingTeamMode.GetMemberCountPerTeam();
			for (int i = 0; i < botCount; i++)
			{
				CharacterSettingData characterSettingData = GameDB.tutorial.GetCharacterSettingData(botSettingDataCode[i]);
				int characterCode = characterSettingData.characterCode;
				CharacterData characterData = GameDB.character.GetCharacterData(characterCode);
				WorldBotPlayerCharacter worldBotPlayerCharacter;
				if (characterSettingData.startingIndex == 0)
				{
					worldBotPlayerCharacter = this.game.Spawn.SpawnBotPlayerCharacter(characterCode, num, SpecialSkillId.None);
				}
				else
				{
					worldBotPlayerCharacter = this.game.Spawn.SpawnBotPlayerCharacter(characterCode, num, characterSettingData.startingArea, characterSettingData.startingIndex, SpecialSkillId.None);
				}
				if (worldBotPlayerCharacter == null)
				{
					Log.E(string.Format("Can't Spawn Bot Player - CharacterCode : {0}", characterCode));
				}
				else
				{
					this.game.Tutorial.OnSpawnBotPlayerCharacter(worldBotPlayerCharacter);
					worldBotPlayerCharacter.SetSession(new PlayerSession(-1, (long)UnityEngine.Random.Range(0, int.MaxValue), characterData.name, characterCode, characterSettingData.equipmentWeapon, 0, num, 0, 0, 0, 0, new List<int>(), new Dictionary<EmotionPlateType, int>(), "", "", "", "", ""));
					worldBotPlayerCharacter.SetCharacterSettingCode(characterSettingData.code);
					worldBotPlayerCharacter.PlayerSession.SetCharacter(worldBotPlayerCharacter);
					this.botSnapshot.Add(worldBotPlayerCharacter.ObjectId, worldBotPlayerCharacter.CreateSnapshotWrapper());
					this.botPlayerCharacters.Add(worldBotPlayerCharacter);
					if ((i + 1) % memberCountPerTeam == 0)
					{
						worldBotPlayerCharacter.SetTeamRole(BotTeamRole.LEADER);
						num++;
					}
					else
					{
						worldBotPlayerCharacter.SetTeamRole(BotTeamRole.FOLLOWER);
					}
					this.AddBotTeamMap(worldBotPlayerCharacter);
				}
			}
		}

		
		private string GetBotNickName(CharacterData data, int number)
		{
			if (this.difficulty == BotDifficulty.PVP)
			{
				if (this.botNickNameList.IsEmpty<string>())
				{
					this.botNickNameList = GameDB.bot.GetNickNameList(MonoBehaviourInstance<GameServer>.inst.ServerRegion);
					if (this.botNickNameList.IsEmpty<string>())
					{
						this.botNickNameList = GameDB.bot.GetNickNameList("Ohio");
					}
				}
				int index = UnityEngine.Random.Range(0, this.botNickNameList.Count);
				string result = this.botNickNameList.ElementAt(index);
				this.botNickNameList.RemoveAt(index);
				return result;
			}
			return StringUtil.CreateBotNickname(data.name, (long)number);
		}

		
		private int GetBotStartAreaCode(ref List<AreaData> areaDataList)
		{
			if (areaDataList.Count == 0)
			{
				areaDataList.AddRange(GameDB.level.GetBattleAreaData());
			}
			int index = UnityEngine.Random.Range(0, areaDataList.Count);
			AreaData areaData = areaDataList.ElementAt(index);
			areaDataList.RemoveAt(index);
			return areaData.code;
		}

		
		private void AddBotTeamMap(WorldBotPlayerCharacter botPlayer)
		{
			if (this.botTeamMap.ContainsKey(botPlayer.TeamNumber))
			{
				Dictionary<int, WorldBotPlayerCharacter> dictionary = this.botTeamMap[botPlayer.TeamNumber];
				dictionary.Add(botPlayer.ObjectId, botPlayer);
				using (Dictionary<int, WorldBotPlayerCharacter>.ValueCollection.Enumerator enumerator = dictionary.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						WorldBotPlayerCharacter worldBotPlayerCharacter = enumerator.Current;
						if (worldBotPlayerCharacter.ObjectId != botPlayer.ObjectId)
						{
							worldBotPlayerCharacter.PlayerSession.AddTeamMember(botPlayer.PlayerSession);
							botPlayer.PlayerSession.AddTeamMember(worldBotPlayerCharacter.PlayerSession);
							if (botPlayer.TeamRole == BotTeamRole.LEADER)
							{
								worldBotPlayerCharacter.SetLeader(botPlayer);
							}
						}
					}
					return;
				}
			}
			Dictionary<int, WorldBotPlayerCharacter> value = new Dictionary<int, WorldBotPlayerCharacter>
			{
				{
					botPlayer.ObjectId,
					botPlayer
				}
			};
			this.botTeamMap.Add(botPlayer.TeamNumber, value);
		}

		
		public void SettingBotTeam()
		{
			Dictionary<int, List<WorldPlayerCharacter>> dictionary = new Dictionary<int, List<WorldPlayerCharacter>>();
			foreach (KeyValuePair<int, Dictionary<int, WorldBotPlayerCharacter>> keyValuePair in this.botTeamMap)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value.Values.Cast<WorldPlayerCharacter>().ToList<WorldPlayerCharacter>());
			}
			MonoBehaviourInstance<GameService>.inst.SettingBotTeam(dictionary);
		}

		
		public void SetStrategySheet()
		{
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.botPlayerCharacters)
			{
				WorldBotPlayerCharacter worldBotPlayerCharacter = (WorldBotPlayerCharacter)worldPlayerCharacter;
				CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(worldBotPlayerCharacter.CharacterCode);
				List<MasteryType> list = (from w in new MasteryType[]
				{
					characterMasteryData.weapon1,
					characterMasteryData.weapon2,
					characterMasteryData.weapon3,
					characterMasteryData.weapon4
				}
				where w != MasteryType.None && w != MasteryType.SniperRifle && w != MasteryType.DualSword
				select w).ToList<MasteryType>();
				MasteryType masteryType = list.ElementAt(UnityEngine.Random.Range(0, list.Count));
				RecommendStarting recommendStarting = GameDB.recommend.FindStartingData(worldBotPlayerCharacter.CharacterCode, masteryType);
				worldBotPlayerCharacter.PlayerSession.StartingSettings.SetStartingEquipment(GameDB.item.FindItemByCode(recommendStarting.startWeapon));
				worldBotPlayerCharacter.PlayerSession.StartingSettings.SetStartingArea(worldBotPlayerCharacter.StartAreaCode);
				MonoBehaviourInstance<GameService>.inst.Server.Broadcast(new RpcUpdateStrategy
				{
					userId = worldBotPlayerCharacter.PlayerSession.userId,
					teamNumber = worldBotPlayerCharacter.PlayerSession.TeamNumber,
					startingAreaCode = worldBotPlayerCharacter.PlayerSession.StartingSettings.StartingAreaCode
				}, NetChannel.ReliableOrdered);
			}
		}

		
		public void SetTutorialStrategySheet()
		{
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.botPlayerCharacters)
			{
				CharacterSettingData characterSettingData = GameDB.tutorial.GetCharacterSettingData(worldPlayerCharacter.CharacterSettingCode);
				worldPlayerCharacter.PlayerSession.StartingSettings.SetCharacterSettings(characterSettingData);
			}
		}

		
		public SnapshotWrapper GetSnapshot(int objectId)
		{
			return this.botSnapshot[objectId];
		}

		
		public int GetAliveBotCharacterCount()
		{
			return this.botPlayerCharacters.Count((WorldPlayerCharacter bot) => bot.IsAlive);
		}

		
		public void ApplyTutorialStrategySheet()
		{
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.botPlayerCharacters)
			{
				worldPlayerCharacter.SetStartingSettings(worldPlayerCharacter.PlayerSession.StartingSettings);
				worldPlayerCharacter.FlushAllUpdatesBeforeStart();
			}
		}

		
		public void ApplyStrategySheet()
		{
			Log.V("[GameServer] bot apply strategy sheet. 1");
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.botPlayerCharacters)
			{
				BlisVector blisVector = new BlisVector(this.game.Level.GetPlayerSpawnPoint(worldPlayerCharacter.PlayerSession.StartingSettings.StartingAreaCode).transform.position);
				worldPlayerCharacter.SetPosition(blisVector.SamplePosition());
				int key = worldPlayerCharacter.PlayerSession.StartingSettings.StartingWeapon.Key;
				int value = worldPlayerCharacter.PlayerSession.StartingSettings.StartingWeapon.Value;
				ItemData itemData = GameDB.item.FindItemByCode(key);
				worldPlayerCharacter.EquipItem(this.game.Spawn.CreateItem(itemData, value));
				foreach (KeyValuePair<int, int> keyValuePair in worldPlayerCharacter.PlayerSession.StartingSettings.StartingArmors)
				{
					if (keyValuePair.Key != 0)
					{
						ItemData itemData2 = GameDB.item.FindItemByCode(keyValuePair.Key);
						worldPlayerCharacter.EquipItem(this.game.Spawn.CreateItem(itemData2, keyValuePair.Value));
					}
				}
				foreach (KeyValuePair<int, int> keyValuePair2 in worldPlayerCharacter.PlayerSession.StartingSettings.StartingInventoryItems)
				{
					ItemData itemData3 = GameDB.item.FindItemByCode(keyValuePair2.Key);
					int num;
					worldPlayerCharacter.AddInventoryItem(this.game.Spawn.CreateItem(itemData3, keyValuePair2.Value), out num);
				}
				worldPlayerCharacter.FlushAllUpdatesBeforeStart();
			}
			Log.V("[GameServer] bot apply strategy sheet. 2");
		}

		
		public void InitMMRContext()
		{
			int minimumMMR = this.game.Player.GetMinimumMMR();
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.botPlayerCharacters)
			{
				((WorldBotPlayerCharacter)worldPlayerCharacter).SetMMRContext(minimumMMR);
			}
		}

		
		public void SetNextLeader(WorldBotPlayerCharacter deadBot)
		{
			if (deadBot.IsAlive && !deadBot.IsDyingCondition)
			{
				return;
			}
			if (deadBot.TeamNumber <= 0 || !this.botTeamMap.ContainsKey(deadBot.TeamNumber))
			{
				return;
			}
			deadBot.SetTeamRole(BotTeamRole.FOLLOWER);
			Dictionary<int, WorldBotPlayerCharacter> dictionary = this.botTeamMap[deadBot.TeamNumber];
			WorldBotPlayerCharacter worldBotPlayerCharacter = null;
			foreach (WorldBotPlayerCharacter worldBotPlayerCharacter2 in dictionary.Values)
			{
				if (worldBotPlayerCharacter2.IsAlive && !worldBotPlayerCharacter2.IsDyingCondition)
				{
					worldBotPlayerCharacter = worldBotPlayerCharacter2;
					worldBotPlayerCharacter.SetTeamRole(BotTeamRole.LEADER);
				}
				else if (worldBotPlayerCharacter != null)
				{
					worldBotPlayerCharacter2.SetLeader(worldBotPlayerCharacter);
				}
			}
		}

		
		public PlayerSession GetBotSession(long userId)
		{
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.botPlayerCharacters)
			{
				if (worldPlayerCharacter.PlayerSession.userId == userId)
				{
					return worldPlayerCharacter.PlayerSession;
				}
			}
			return null;
		}

		
		private BotDifficulty difficulty;

		
		private readonly List<WorldPlayerCharacter> botPlayerCharacters = new List<WorldPlayerCharacter>();

		
		private readonly Dictionary<int, SnapshotWrapper> botSnapshot = new Dictionary<int, SnapshotWrapper>();

		
		private int botEquipIndex;

		
		private readonly Dictionary<int, Dictionary<int, WorldBotPlayerCharacter>> botTeamMap = new Dictionary<int, Dictionary<int, WorldBotPlayerCharacter>>();

		
		private List<string> botNickNameList = new List<string>();

		
		private int totalBotCount;
	}
}
