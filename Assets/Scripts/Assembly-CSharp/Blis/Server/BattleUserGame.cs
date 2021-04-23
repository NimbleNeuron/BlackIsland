using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Blis.Server
{
	
	public class BattleUserGame
	{
		
		public float amplifierToMonster;

		
		public int attackPower;

		
		public float attackRange;

		
		public float attackSpeed;

		
		public int avgLatency;

		
		public int basicAttackDamageToPlayer;

		
		public int beforeNormalTeamMmr;

		
		public int bestWeapon;

		
		public int bestWeaponLevel;

		
		public int botAdded;

		
		public int botRemain;

		
		public string causeOfDeath;

		
		public int characterLevel;

		
		public int characterNum;

		
		public float coolDownReduction;

		
		public string country;

		
		public string countryCode;

		
		public int craftEpic;

		
		public int craftLegend;

		
		public int craftRare;

		
		public int craftUncommon;

		
		public float criticalStrikeChance;

		
		public float criticalStrikeDamage;

		
		public int damageToMonster;

		
		public int damageToPlayer;

		
		public int defense;

		
		public float duelRating;

		
		public int duration;

		
		public Dictionary<int, int> equipment;

		
		public int gainExp;

		
		public long gameId;

		
		public string gameMode;

		
		public string gameModeSub;

		
		public int gameRank;

		
		public float hpRegen;

		
		public string identifier;

		
		public string ipAddress;

		
		public string isp;

		
		public string killDetail;

		
		public string killer;

		
		public long killerUserNum;

		
		public string language;

		
		public float lifeSteal;

		
		public string location;

		
		public Dictionary<int, int> masteryLevel;

		
		public int matchingMode;

		
		public int matchingTeamMode;

		
		public float matchRating;

		
		public int maxHp;

		
		public int maxLatency;

		
		public int maxSp;

		
		public int minLatency;

		
		public Dictionary<int, int> missionResult;

		
		public int monsterKill;

		
		public float moveSpeed;

		
		public string nickname;

		
		public float outOfCombatMoveSpeed;

		
		public int playerAssistant;

		
		public int playerKill;

		
		public int playTime;

		
		public int preMade;

		
		public int restrictedAreaAccelerated;

		
		public int rewardCoin;

		
		public int safeAreas;

		
		public int seasonId;

		
		public string serverName;

		
		public float sightRange;

		
		public int skillDamageToPlayer;

		
		public Dictionary<int, int> skillEvolutionLevel;

		
		public Dictionary<int, int> skillEvolutionOrder;

		
		public Dictionary<int, int> skillLevelInfo;

		
		public Dictionary<int, int> skillOrderInfo;

		
		public int skinCode;

		
		public float spRegen;

		
		[JsonConverter(typeof(MicrosecondEpochConverter))]
		public DateTime startDtm;

		
		public int teamFinalRank;

		
		public int teamNumber;

		
		public int totalTime;

		
		public float trapDamage;

		
		public int trapDamageToPlayer;

		
		public long userNum;

		
		public int versionMajor;

		
		public int versionMinor;

		
		public int versionSeason;

		
		public int watchTime;

		
		public string zipCode;

		
		public BattleUserGame(BattleGame battleGame, WorldPlayerCharacter character, int rewardACoin,
			long attackerUserId, float matchRating)
		{
			PlayerSession playerSession = character.PlayerSession;
			int num = 0;
			int num2;
			int num3;
			if (playerSession.ObservingStartTime == DateTime.MinValue)
			{
				num2 = (int) (DateTime.Now - MonoBehaviourInstance<GameService>.inst.GameStartTime).TotalSeconds;
				num3 = num2;
			}
			else
			{
				num2 = (int) (playerSession.ObservingStartTime - MonoBehaviourInstance<GameService>.inst.GameStartTime)
					.TotalSeconds;
				num = (int) (DateTime.Now - playerSession.ObservingStartTime).TotalSeconds;
				num3 = num + num2;
			}

			Dictionary<MasteryType, int> masteryLevels = character.Mastery.GetMasteryLevels();
			KeyValuePair<MasteryType, int> keyValuePair = masteryLevels.Where(
				delegate(KeyValuePair<MasteryType, int> item)
				{
					KeyValuePair<MasteryType, int> keyValuePair2 = item;
					return keyValuePair2.Key.IsWeaponMastery();
				}).OrderByDescending(delegate(KeyValuePair<MasteryType, int> item)
			{
				KeyValuePair<MasteryType, int> keyValuePair2 = item;
				return keyValuePair2.Value;
			}).FirstOrDefault<KeyValuePair<MasteryType, int>>();
			Dictionary<int, int> dictionary =
				masteryLevels.ToDictionary(level => (int) level.Key, level => level.Value);
			gameId = battleGame.id;
			seasonId = battleGame.seasonId;
			userNum = character.PlayerSession.userId;
			nickname = playerSession.nickname;
			matchingMode = (int) battleGame.matchingMode;
			matchingTeamMode = (int) battleGame.matchingTeamMode;
			characterNum = character.PlayerSession.characterId;
			skinCode = character.SkinCode;
			characterLevel = character.Status.Level;
			gameRank = character.rank;
			teamFinalRank = MonoBehaviourInstance<GameService>.inst.IsTeamMode ? character.teamRank : -1;
			playerKill = character.Status.PlayerKill;
			playerAssistant = character.Status.PlayerKillAssist;
			monsterKill = character.Status.MonsterKill;
			masteryLevel = dictionary;
			bestWeapon = (int) keyValuePair.Key;
			bestWeaponLevel = keyValuePair.Value;
			equipment = character.Equipment.GetEquips().ToDictionary(x => (int) x.GetEquipSlotType(), x => x.itemCode);
			rewardCoin = rewardACoin;
			damageToPlayer = character.Status.DamageToPlayer;
			damageToMonster = character.Status.DamageToMonster;
			killerUserNum = attackerUserId;
			this.matchRating = matchRating;
			duelRating = character.MMRContext.DuelRating;
			missionResult = character.PlayerSession.missionResultMap;
			duration = Mathf.RoundToInt(MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
			startDtm = battleGame.startDtm;
			identifier = playerSession.Identifier;
			country = playerSession.Country;
			language = playerSession.Language;
			countryCode = playerSession.CountryCode;
			ipAddress = playerSession.IpAddress;
			string[] array = BSERVersion.VERSION.Split('.');
			CharacterStat stat = character.Stat;
			versionSeason = int.Parse(array[0]);
			versionMajor = int.Parse(array[1]);
			versionMinor = int.Parse(array[2]);
			skillLevelInfo = (from x in character.GetSkillUpOrder()
				group x by x.Value).ToDictionary(x => x.Key, x => x.Count<KeyValuePair<int, int>>());
			skillOrderInfo = character.GetSkillUpOrder();
			serverName = MonoBehaviourInstance<GameServer>.inst.DeployProperty.deploySetting.ToString();
			gameMode = battleGame.matchingMode.GetGameMode().ToString();
			gameModeSub = battleGame.matchingMode.GetGameModeSub().ToString();
			maxHp = stat.MaxHp;
			maxSp = stat.MaxSp;
			attackPower = stat.AttackPower;
			defense = stat.Defense;
			hpRegen = stat.HpRegen;
			spRegen = stat.SpRegen;
			attackSpeed = stat.AttackSpeed;
			moveSpeed = stat.MoveSpeed;
			outOfCombatMoveSpeed = stat.MoveSpeedOutOfCombat;
			sightRange = stat.SightRange;
			attackRange = stat.AttackRange;
			criticalStrikeChance = stat.CriticalStrikeChance;
			criticalStrikeDamage = stat.CriticalStrikeDamage * 100f;
			coolDownReduction = stat.CooldownReduction * 100f;
			lifeSteal = stat.LifeSteal;
			amplifierToMonster = stat.AmplifierToMonsterRatio * 100f;
			trapDamage = stat.TrapDamageRatio * 100f;
			teamNumber = playerSession.TeamNumber;
			preMade = playerSession.preMade;
			killer = character.IsAlive ? "" : playerSession.killer;
			killDetail = character.IsAlive ? "" : playerSession.killerDetail;
			causeOfDeath = character.IsAlive ? "" : playerSession.causeOfDeath;
			beforeNormalTeamMmr = playerSession.teamMmr;
			minLatency = playerSession.MinLatency;
			maxLatency = playerSession.MaxLatency;
			avgLatency = playerSession.AvgLatency;
			playTime = num2;
			watchTime = num;
			totalTime = num3;
			botAdded = MonoBehaviourInstance<GameService>.inst.Bot.TotalBotCount;
			botRemain = MonoBehaviourInstance<GameService>.inst.Bot.GetAliveBotCharacterCount();
			restrictedAreaAccelerated = MonoBehaviourInstance<GameService>.inst.Area.AccelerationCount;
			safeAreas = MonoBehaviourInstance<GameService>.inst.Area.GetSafeStateAreaCount();
			isp = playerSession.ISP;
			zipCode = playerSession.ZipCode;
			craftUncommon = playerSession.Character.Status.CraftUncommon;
			craftRare = playerSession.Character.Status.CraftRare;
			craftEpic = playerSession.Character.Status.CraftEpic;
			craftLegend = playerSession.Character.Status.CraftLegend;
			trapDamageToPlayer = playerSession.Character.Status.TrapDamageToPlayer;
			basicAttackDamageToPlayer = playerSession.Character.Status.BasicAttackDamageToPlayer;
			skillDamageToPlayer = playerSession.Character.Status.SkillDamageToPlayer;
		}
	}
}