using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	public class Mastery
	{
		
		
		
		public event Mastery.MasteryLevelUp OnMasteryLevelUp = delegate(MasteryType p0, int p1)
		{
		};

		
		public Mastery(WorldPlayerCharacter masteryOwner)
		{
			this.playerCharacter = masteryOwner;
			this.masteries = new Dictionary<MasteryType, MasteryInfo>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);
			this.masteryConditionExps = new int[Enum.GetNames(typeof(MasteryConditionType)).Length];
			CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(this.playerCharacter.CharacterCode);
			this.AddNewMastery(characterMasteryData.weapon1);
			this.AddNewMastery(characterMasteryData.weapon2);
			this.AddNewMastery(characterMasteryData.weapon3);
			this.AddNewMastery(characterMasteryData.weapon4);
			this.AddNewMastery(characterMasteryData.exploration1);
			this.AddNewMastery(characterMasteryData.exploration2);
			this.AddNewMastery(characterMasteryData.exploration3);
			this.AddNewMastery(characterMasteryData.exploration4);
			this.AddNewMastery(characterMasteryData.survival1);
			this.AddNewMastery(characterMasteryData.survival2);
			this.AddNewMastery(characterMasteryData.survival3);
			this.AddNewMastery(characterMasteryData.survival4);
			this.updates = new Dictionary<MasteryType, MasteryValue>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);
		}

		
		private void AddNewMastery(MasteryType masteryType)
		{
			if (masteryType == MasteryType.None)
			{
				return;
			}
			MasteryLevelData masteryLevelData = GameDB.mastery.GetMasteryLevelData(masteryType, 1);
			this.masteries.Add(masteryType, new MasteryInfo(masteryType, 1, 0, (masteryLevelData != null) ? masteryLevelData.nextMasteryExp : 0));
		}

		
		public MasteryInfo GetMasteryInfo(MasteryType type)
		{
			MasteryInfo result;
			if (this.masteries.TryGetValue(type, out result))
			{
				return result;
			}
			return default(MasteryInfo);
		}

		
		public bool HasMastery(MasteryType masteryType)
		{
			return this.masteries.ContainsKey(masteryType);
		}

		
		public List<WeaponType> GetWeaponMastery()
		{
			List<WeaponType> list = new List<WeaponType>();
			foreach (MasteryType masteryType in this.masteries.Keys)
			{
				if (masteryType.IsWeaponMastery())
				{
					list.Add(masteryType.GetWeaponType());
				}
			}
			return list;
		}

		
		public int GetMasteryLevel(MasteryType type)
		{
			if (this.masteries.ContainsKey(type))
			{
				return this.masteries[type].masteryLevel;
			}
			return 0;
		}

		
		public void SetMasteryLevel(MasteryType type, int level)
		{
			if (type == MasteryType.None || level <= 0)
			{
				return;
			}
			MasteryLevelData masteryLevelData = GameDB.mastery.GetMasteryLevelData(type, level);
			MasteryInfo value = new MasteryInfo(type, level, 0, (masteryLevelData != null) ? masteryLevelData.nextMasteryExp : 0);
			this.masteries[type] = value;
		}

		
		public void AddMasteryConditionExp(UpdateMasteryInfo updateInfo)
		{
			MasteryExpData masteryExpData;
			if (updateInfo.conditionType == MasteryConditionType.AssistKillPlayer)
			{
				masteryExpData = GameDB.mastery.GetMasteryExpData(MasteryConditionType.KillPlayer, updateInfo.itemGrade);
			}
			else
			{
				masteryExpData = GameDB.mastery.GetMasteryExpData(updateInfo.conditionType, updateInfo.itemGrade);
			}
			if (masteryExpData == null)
			{
				return;
			}
			int num = updateInfo.takeMasteryValue + this.masteryConditionExps[(int)updateInfo.conditionType];
			int num2 = num / masteryExpData.conditionValue;
			this.masteryConditionExps[(int)updateInfo.conditionType] = num % masteryExpData.conditionValue;
			float num3 = 1f;
			int num4 = MonoBehaviourInstance<GameService>.inst.PlayerCharacter.HighestPlayerLevel - this.playerCharacter.Status.Level;
			if (num4 > 0)
			{
				num3 += (float)num4 * 0.18f;
			}
			MasteryType masteryType = updateInfo.masteryType;
			if (updateInfo.conditionType == MasteryConditionType.KillMonster)
			{
				this.AddMasteryExp(masteryType, (int)((float)(num2 * updateInfo.extraValue) * num3));
				this.AddMasteryExp(MasteryType.Defense, (int)((float)(num2 * updateInfo.extraValue) * num3));
				this.AddMasteryExp(MasteryType.Hunt, (int)((float)(num2 * updateInfo.extraValue) * num3));
				return;
			}
			if (updateInfo.conditionType == MasteryConditionType.KillPlayer)
			{
				this.AddMasteryExp(masteryType, (int)((float)(num2 * (updateInfo.extraValue * 60 + masteryExpData.value1)) * num3));
				this.AddMasteryExp(MasteryType.Defense, (int)((float)(num2 * (updateInfo.extraValue * 60 + masteryExpData.value2)) * num3));
				return;
			}
			if (updateInfo.conditionType != MasteryConditionType.AssistKillPlayer)
			{
				this.AddMasteryExp((masteryExpData.masteryType1 == MasteryType.None) ? masteryType : masteryExpData.masteryType1, (int)((float)(num2 * masteryExpData.value1) * num3));
				this.AddMasteryExp((masteryExpData.masteryType2 == MasteryType.None) ? masteryType : masteryExpData.masteryType2, (int)((float)(num2 * masteryExpData.value2) * num3));
				this.AddMasteryExp((masteryExpData.masteryType3 == MasteryType.None) ? masteryType : masteryExpData.masteryType3, (int)((float)(num2 * masteryExpData.value3) * num3));
				return;
			}
			if (updateInfo.assistMemberCount == 0)
			{
				return;
			}
			int num5 = masteryExpData.value1 / (updateInfo.assistMemberCount * 2) + updateInfo.extraValue * 60 / (updateInfo.assistMemberCount * 2);
			int num6 = masteryExpData.value2 / (updateInfo.assistMemberCount * 2) + updateInfo.extraValue * 60 / (updateInfo.assistMemberCount * 2);
			this.AddMasteryExp(masteryType, (int)((float)(num2 * num5) * num3));
			this.AddMasteryExp(MasteryType.Defense, (int)((float)(num2 * num6) * num3));
		}

		
		private void AddMasteryExp(MasteryType masteryType, int exp)
		{
			if (masteryType == MasteryType.None || exp <= 0)
			{
				return;
			}
			bool flag = false;
			int num = 0;
			if (!this.masteries.ContainsKey(masteryType))
			{
				return;
			}
			exp += (int)((float)exp * ((float)this.addExpPercent * 0.01f));
			MasteryInfo masteryInfo = this.masteries[masteryType];
			int num2 = masteryInfo.masteryLevel;
			int i = masteryInfo.masteryExp;
			int num3 = masteryInfo.masteryMaxExp;
			int oldLevel = num2;
			i += exp;
			while (i >= num3)
			{
				if (num3 == 0)
				{
					i = 0;
					break;
				}
				flag = true;
				num2++;
				i -= num3;
				MasteryLevelData masteryLevelData = GameDB.mastery.GetMasteryLevelData(masteryType, num2);
				num += ((masteryLevelData != null) ? masteryLevelData.giveLevelExp : 0);
				num3 = ((masteryLevelData != null) ? masteryLevelData.nextMasteryExp : 0);
			}
			int num4 = masteryInfo.weaponSkillPoint;
			if (masteryType.IsWeaponMastery())
			{
				num4 += this.CalcWeaponSkillPoint(masteryType, oldLevel, num2);
			}
			this.CheckUpdateMasteryValue(masteryType, num2, i, num4, exp);
			masteryInfo.masteryLevel = num2;
			masteryInfo.masteryExp = i;
			masteryInfo.masteryMaxExp = num3;
			masteryInfo.weaponSkillPoint = num4;
			this.masteries[masteryType] = masteryInfo;
			if (flag)
			{
				Mastery.MasteryLevelUp onMasteryLevelUp = this.OnMasteryLevelUp;
				if (onMasteryLevelUp == null)
				{
					return;
				}
				onMasteryLevelUp(masteryType, num);
			}
		}

		
		public Dictionary<MasteryType, int> GetMasteryLogInfo()
		{
			Dictionary<MasteryType, int> dictionary = new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);
			foreach (KeyValuePair<MasteryType, MasteryInfo> keyValuePair in this.masteries)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value.masteryLevel);
			}
			return dictionary;
		}

		
		public Dictionary<StatType, float> GetMasteryStats(MasteryType weaponMasteryType)
		{
			float[] array = new float[GameConstants.Stat.STAT_LEN];
			foreach (MasteryInfo masteryInfo in this.masteries.Values)
			{
				if (!masteryInfo.masteryType.IsWeaponMastery() || masteryInfo.masteryType == weaponMasteryType)
				{
					MasteryLevelData masteryLevelData = GameDB.mastery.GetMasteryLevelData(masteryInfo.masteryType, masteryInfo.masteryLevel);
					array[(int)masteryLevelData.option1] += masteryLevelData.optionValue1;
					array[(int)masteryLevelData.option2] += masteryLevelData.optionValue2;
					array[(int)masteryLevelData.option3] += masteryLevelData.optionValue3;
				}
			}
			Dictionary<StatType, float> dictionary = new Dictionary<StatType, float>(SingletonComparerEnum<StatTypeComparer, StatType>.Instance);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] > 0f)
				{
					dictionary.Add((StatType)i, array[i]);
				}
			}
			return dictionary;
		}

		
		public List<MasteryValue> GetMasteryValues()
		{
			List<MasteryValue> list = new List<MasteryValue>();
			foreach (KeyValuePair<MasteryType, MasteryInfo> keyValuePair in this.masteries)
			{
				list.Add(new MasteryValue(keyValuePair.Key, keyValuePair.Value.masteryLevel, keyValuePair.Value.masteryExp, keyValuePair.Value.weaponSkillPoint, 0));
			}
			return list;
		}

		
		public Dictionary<MasteryType, int> GetMasteryLevels()
		{
			Dictionary<MasteryType, int> dictionary = new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);
			foreach (KeyValuePair<MasteryType, MasteryInfo> keyValuePair in this.masteries)
			{
				dictionary.Add(keyValuePair.Value.masteryType, keyValuePair.Value.masteryLevel);
			}
			return dictionary;
		}

		
		public Dictionary<MasteryCategory, int> GetMasteryCategoryLevels()
		{
			Dictionary<MasteryCategory, int> dictionary = new Dictionary<MasteryCategory, int>(SingletonComparerEnum<MasteryCategoryComparer, MasteryCategory>.Instance);
			foreach (KeyValuePair<MasteryType, MasteryInfo> keyValuePair in this.masteries)
			{
				if (!dictionary.ContainsKey(keyValuePair.Key.GetCategory()))
				{
					dictionary.Add(keyValuePair.Key.GetCategory(), 0);
				}
				Dictionary<MasteryCategory, int> dictionary2 = dictionary;
				MasteryCategory category = keyValuePair.Key.GetCategory();
				dictionary2[category] += keyValuePair.Value.masteryLevel;
			}
			return dictionary;
		}

		
		public int GetMasteryCategoryLevel(MasteryCategory masteryCategory)
		{
			int num = 0;
			foreach (KeyValuePair<MasteryType, MasteryInfo> keyValuePair in this.masteries)
			{
				if (keyValuePair.Key.GetCategory() == masteryCategory)
				{
					num += keyValuePair.Value.masteryLevel;
				}
			}
			return num;
		}

		
		public void CheatMastery(int count)
		{
			for (int i = 0; i < count; i++)
			{
				foreach (MasteryType masteryType in new List<MasteryType>(this.masteries.Keys))
				{
					MasteryInfo masteryInfo = this.GetMasteryInfo(masteryType);
					MasteryLevelData masteryLevelData = GameDB.mastery.GetMasteryLevelData(masteryType, masteryInfo.masteryLevel);
					this.AddMasteryExp(masteryType, masteryLevelData.nextMasteryExp - masteryInfo.masteryExp);
				}
			}
		}

		
		public void MasteryLevelUpToTarget(MasteryType masteryType, int targetLevel)
		{
			MasteryInfo masteryInfo = this.GetMasteryInfo(masteryType);
			int num = targetLevel - masteryInfo.masteryLevel;
			if (num <= 0)
			{
				return;
			}
			for (int i = 0; i < num; i++)
			{
				MasteryLevelData masteryLevelData = GameDB.mastery.GetMasteryLevelData(masteryType, masteryInfo.masteryLevel);
				this.AddMasteryExp(masteryType, masteryLevelData.nextMasteryExp - masteryInfo.masteryExp);
			}
		}

		
		public int GetSumMasteryLevel()
		{
			int num = 0;
			foreach (KeyValuePair<MasteryType, MasteryInfo> keyValuePair in this.masteries)
			{
				num += keyValuePair.Value.masteryLevel;
			}
			return num;
		}

		
		public List<MasteryValue> FlushUpdates()
		{
			this.ret.Clear();
			this.ret.AddRange(this.updates.Values);
			this.updates.Clear();
			return this.ret;
		}

		
		public int GetMasterySkillPoint(MasteryType masteryType)
		{
			if (masteryType != MasteryType.None)
			{
				return this.masteries[masteryType].weaponSkillPoint;
			}
			return 0;
		}

		
		public void UseWeaponSkillPoint(MasteryType masteryType)
		{
			MasteryInfo value = this.masteries[masteryType];
			value.weaponSkillPoint--;
			this.masteries[masteryType] = value;
		}

		
		private void CheckUpdateMasteryValue(MasteryType masteryType, int level, int exp, int weaponSkillPoint, int addExp)
		{
			MasteryInfo original;
			if (this.masteries.TryGetValue(masteryType, out original) && this.IsDifferent(original, level, exp))
			{
				if (this.updates.ContainsKey(masteryType))
				{
					this.updates.Remove(masteryType);
				}
				this.updates.Add(masteryType, new MasteryValue(masteryType, level, exp, weaponSkillPoint, addExp));
			}
		}

		
		private bool IsDifferent(MasteryInfo original, int newLevel, int newExp)
		{
			return original.masteryLevel != newLevel || original.masteryExp != newExp;
		}

		
		private int CalcWeaponSkillPoint(MasteryType masteryType, int oldLevel, int newLevel)
		{
			int num = 0;
			int i = oldLevel;
			while (i < newLevel)
			{
				i++;
				MasteryLevelData masteryLevelData = GameDB.mastery.GetMasteryLevelData(masteryType, i);
				num += masteryLevelData.weaponSkillPoint;
			}
			return num;
		}

		
		public void AddExpIncrementPercent(int percent)
		{
			this.addExpPercent += percent;
		}

		
		private readonly Dictionary<MasteryType, MasteryInfo> masteries;

		
		private readonly int[] masteryConditionExps;

		
		private Dictionary<MasteryType, MasteryValue> updates;

		
		private WorldPlayerCharacter playerCharacter;

		
		private int addExpPercent;

		
		private readonly List<MasteryValue> ret = new List<MasteryValue>();

		
		public delegate void MasteryLevelUp(MasteryType masteryType, int characterExp);
	}
}
