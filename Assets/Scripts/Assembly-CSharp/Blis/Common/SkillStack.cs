using System;
using System.Collections.Generic;

namespace Blis.Common
{
	public class SkillStack
	{
		private readonly Dictionary<SkillSlotSet, int> maxStackCharacterSkill =
			new Dictionary<SkillSlotSet, int>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>.Instance);


		private readonly Dictionary<MasteryType, int> maxStackWeaponSkill =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		private readonly Dictionary<SkillSlotSet, int> stackCharacterSkill =
			new Dictionary<SkillSlotSet, int>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>.Instance);


		private readonly Dictionary<SkillSlotSet, float> stackCharacterSkillIntervalTime =
			new Dictionary<SkillSlotSet, float>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>.Instance);


		private readonly List<SkillSlotSet> stackCharacterSkillTimers = new List<SkillSlotSet>();


		private readonly Dictionary<MasteryType, int> stackWeaponSkill =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		private readonly Dictionary<MasteryType, float> stackWeaponSkillIntervalTime =
			new Dictionary<MasteryType, float>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		private readonly List<MasteryType> stackWeaponSkillTimers = new List<MasteryType>();


		private readonly int characterCode;


		private readonly ObjectType objectType = ObjectType.Dummy;


		public Action<SkillSlotSet, MasteryType, int> OnSkillStackValueChange;


		private readonly SkillLevel skillLevel;

		public SkillStack(SkillLevel skillLevel, int characterCode, ObjectType objectType)
		{
			this.skillLevel = skillLevel;
			this.characterCode = characterCode;
			this.objectType = objectType;
		}


		public SkillStackSnapshot CreateSnapshot()
		{
			Dictionary<SkillSlotSet, BlisFixedPoint> dictionary =
				new Dictionary<SkillSlotSet, BlisFixedPoint>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>
					.Instance);
			foreach (KeyValuePair<SkillSlotSet, float> keyValuePair in stackCharacterSkillIntervalTime)
			{
				dictionary.Add(keyValuePair.Key, new BlisFixedPoint(keyValuePair.Value));
			}

			Dictionary<MasteryType, BlisFixedPoint> dictionary2 =
				new Dictionary<MasteryType, BlisFixedPoint>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>
					.Instance);
			foreach (KeyValuePair<MasteryType, float> keyValuePair2 in stackWeaponSkillIntervalTime)
			{
				dictionary2.Add(keyValuePair2.Key, new BlisFixedPoint(keyValuePair2.Value));
			}

			return new SkillStackSnapshot
			{
				stackCharacterSkill = stackCharacterSkill,
				stackCharacterSkillTimers = stackCharacterSkillTimers,
				stackCharacterSkillIntervalTime = dictionary,
				stackWeaponSkill = stackWeaponSkill,
				stackWeaponSkillTimers = stackWeaponSkillTimers,
				stackWeaponSkillIntervalTime = dictionary2
			};
		}


		public void InitSnapshot(SkillStackSnapshot skillStackSnapshot)
		{
			if (skillStackSnapshot != null)
			{
				foreach (KeyValuePair<SkillSlotSet, int> keyValuePair in skillStackSnapshot.stackCharacterSkill)
				{
					stackCharacterSkill[keyValuePair.Key] = keyValuePair.Value;
				}

				foreach (SkillSlotSet item in skillStackSnapshot.stackCharacterSkillTimers)
				{
					if (!stackCharacterSkillTimers.Contains(item))
					{
						stackCharacterSkillTimers.Add(item);
					}
				}

				foreach (KeyValuePair<SkillSlotSet, BlisFixedPoint> keyValuePair2 in skillStackSnapshot
					.stackCharacterSkillIntervalTime)
				{
					stackCharacterSkillIntervalTime[keyValuePair2.Key] = keyValuePair2.Value.Value;
				}

				foreach (KeyValuePair<MasteryType, int> keyValuePair3 in skillStackSnapshot.stackWeaponSkill)
				{
					stackWeaponSkill[keyValuePair3.Key] = keyValuePair3.Value;
				}

				foreach (MasteryType item2 in skillStackSnapshot.stackWeaponSkillTimers)
				{
					if (!stackWeaponSkillTimers.Contains(item2))
					{
						stackWeaponSkillTimers.Add(item2);
					}
				}

				foreach (KeyValuePair<MasteryType, BlisFixedPoint> keyValuePair4 in skillStackSnapshot
					.stackWeaponSkillIntervalTime)
				{
					stackWeaponSkillIntervalTime[keyValuePair4.Key] = keyValuePair4.Value.Value;
				}
			}
		}


		public void InitSkillMaxStack()
		{
			Dictionary<SkillSlotSet, SkillSet>.KeyCollection allSkillSetKey =
				GameDB.skill.GetAllSkillSetKey(characterCode, objectType);
			if (allSkillSetKey == null)
			{
				return;
			}

			maxStackCharacterSkill.Clear();
			foreach (SkillSlotSet skillSlotSet in allSkillSetKey)
			{
				SkillSet skillSetData = GameDB.skill.GetSkillSetData(characterCode, objectType, skillSlotSet);
				if (skillSetData != null)
				{
					int num = skillLevel.GetSkillLevel(skillSlotSet);
					if (num == 0)
					{
						int skill = skillSetData.GetSkill(1, 0);
						SkillData skillData = GameDB.skill.GetSkillData(skill);
						if (GameDB.skill.GetSkillGroupData(skillData.group).stackAble)
						{
							maxStackCharacterSkill.Add(skillSlotSet, 0);
						}
					}
					else
					{
						int skill2 = skillSetData.GetSkill(num, 0);
						SkillData skillData2 = GameDB.skill.GetSkillData(skill2);
						if (GameDB.skill.GetSkillGroupData(skillData2.group).stackAble)
						{
							maxStackCharacterSkill.Add(skillSlotSet, skillData2.maxStack);
						}
					}
				}
			}
		}


		public void InitSkillMaxStack(MasteryType masteryType)
		{
			if (maxStackWeaponSkill.ContainsKey(masteryType))
			{
				return;
			}

			SkillSet skillSetData = GameDB.skill.GetSkillSetData(masteryType);
			if (skillSetData != null)
			{
				int num = skillLevel.GetSkillLevel(masteryType);
				if (num == 0)
				{
					int skill = skillSetData.GetSkill(1, 0);
					SkillData skillData = GameDB.skill.GetSkillData(skill);
					if (GameDB.skill.GetSkillGroupData(skillData.group).stackAble)
					{
						maxStackWeaponSkill.Add(masteryType, 0);
					}
				}
				else
				{
					int skill2 = skillSetData.GetSkill(num, 0);
					SkillData skillData2 = GameDB.skill.GetSkillData(skill2);
					if (GameDB.skill.GetSkillGroupData(skillData2.group).stackAble)
					{
						maxStackWeaponSkill.Add(masteryType, skillData2.maxStack);
					}
				}
			}
		}


		public void ResetCooldown()
		{
			foreach (KeyValuePair<SkillSlotSet, int> keyValuePair in maxStackCharacterSkill)
			{
				if (keyValuePair.Value != 0)
				{
					stackCharacterSkillIntervalTime[keyValuePair.Key] = 0f;
					stackCharacterSkill[keyValuePair.Key] = keyValuePair.Value;
					Action<SkillSlotSet, MasteryType, int> onSkillStackValueChange = OnSkillStackValueChange;
					if (onSkillStackValueChange != null)
					{
						onSkillStackValueChange(keyValuePair.Key, MasteryType.None,
							stackCharacterSkill[keyValuePair.Key]);
					}
				}
			}

			foreach (KeyValuePair<MasteryType, int> keyValuePair2 in maxStackWeaponSkill)
			{
				if (keyValuePair2.Value != 0)
				{
					stackWeaponSkillIntervalTime[keyValuePair2.Key] = 0f;
					stackWeaponSkill[keyValuePair2.Key] = keyValuePair2.Value;
					Action<SkillSlotSet, MasteryType, int> onSkillStackValueChange2 = OnSkillStackValueChange;
					if (onSkillStackValueChange2 != null)
					{
						onSkillStackValueChange2(SkillSlotSet.WeaponSkill, keyValuePair2.Key,
							stackWeaponSkill[keyValuePair2.Key]);
					}
				}
			}

			stackCharacterSkillTimers.Clear();
			stackWeaponSkillTimers.Clear();
		}


		public bool UseSkillStack(SkillSlotSet skillSlotSet, MasteryType weaponType, SkillData skillData,
			float currentTime)
		{
			if (skillData == null)
			{
				Log.E("skillData is null in SkillStack.cs");
				return false;
			}

			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				if (!stackWeaponSkill.ContainsKey(weaponType))
				{
					return false;
				}

				if (stackWeaponSkill[weaponType] <= 0)
				{
					Log.E("stackWeaponSkill under zero : " + stackWeaponSkill[weaponType]);
					return false;
				}
			}
			else
			{
				if (!stackCharacterSkill.ContainsKey(skillSlotSet))
				{
					return false;
				}

				if (stackCharacterSkill[skillSlotSet] <= 0)
				{
					Log.E("stackCharacterSkill under zero : " + stackCharacterSkill[skillSlotSet]);
					return false;
				}
			}

			SkillStackChange(skillSlotSet, weaponType, -1);
			SettingIntervalTime(skillSlotSet, weaponType, skillData.stackUseIntervalTime, currentTime);
			return true;
		}


		public void SkillStackChange(SkillSlotSet skillSlotSet, MasteryType weaponType, int changeAmount)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				Dictionary<MasteryType, int> dictionary = stackWeaponSkill;
				dictionary[weaponType] += changeAmount;
				Action<SkillSlotSet, MasteryType, int> onSkillStackValueChange = OnSkillStackValueChange;
				if (onSkillStackValueChange == null)
				{
					return;
				}

				onSkillStackValueChange(skillSlotSet, weaponType, stackWeaponSkill[weaponType]);
			}
			else
			{
				Dictionary<SkillSlotSet, int> dictionary2 = stackCharacterSkill;
				dictionary2[skillSlotSet] += changeAmount;
				Action<SkillSlotSet, MasteryType, int> onSkillStackValueChange2 = OnSkillStackValueChange;
				if (onSkillStackValueChange2 == null)
				{
					return;
				}

				onSkillStackValueChange2(skillSlotSet, weaponType, stackCharacterSkill[skillSlotSet]);
			}
		}


		public bool CanUseStackSkill(SkillSlotSet skillSlotSet, MasteryType masteryType, float currentTime)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				return stackWeaponSkillIntervalTime[masteryType] <= currentTime && stackWeaponSkill[masteryType] > 0;
			}

			return stackCharacterSkillIntervalTime.ContainsKey(skillSlotSet) &&
			       stackCharacterSkillIntervalTime[skillSlotSet] <= currentTime &&
			       stackCharacterSkill[skillSlotSet] > 0;
		}


		public bool IsStackableSkill(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				return maxStackWeaponSkill.ContainsKey(masteryType);
			}

			return maxStackCharacterSkill.ContainsKey(skillSlotSet);
		}


		public bool IsMaxStack(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				return stackWeaponSkill.ContainsKey(masteryType) && maxStackWeaponSkill.ContainsKey(masteryType) &&
				       stackWeaponSkill[masteryType] >= maxStackWeaponSkill[masteryType];
			}

			return stackCharacterSkill.ContainsKey(skillSlotSet) && maxStackCharacterSkill.ContainsKey(skillSlotSet) &&
			       stackCharacterSkill[skillSlotSet] >= maxStackCharacterSkill[skillSlotSet];
		}


		public void MaxStackSetting(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				if (!maxStackWeaponSkill.ContainsKey(masteryType))
				{
					return;
				}

				SkillSet skillSetData = GameDB.skill.GetSkillSetData(masteryType);
				if (skillSetData != null)
				{
					int num = skillLevel.GetSkillLevel(masteryType);
					if (num == 0)
					{
						return;
					}

					int skill = skillSetData.GetSkill(num, 0);
					SkillData skillData = GameDB.skill.GetSkillData(skill);
					maxStackWeaponSkill[masteryType] = skillData.maxStack;
					if (maxStackWeaponSkill[masteryType] > 0 && !stackWeaponSkill.ContainsKey(masteryType))
					{
						stackWeaponSkill.Add(masteryType, 1);
						stackWeaponSkillIntervalTime.Add(masteryType, 0f);
					}
				}
			}
			else
			{
				if (!maxStackCharacterSkill.ContainsKey(skillSlotSet))
				{
					return;
				}

				SkillSet skillSetData2 = GameDB.skill.GetSkillSetData(characterCode, objectType, skillSlotSet);
				if (skillSetData2 != null)
				{
					int num2 = skillLevel.GetSkillLevel(skillSlotSet);
					if (num2 == 0)
					{
						return;
					}

					int skill2 = skillSetData2.GetSkill(num2, 0);
					SkillData skillData2 = GameDB.skill.GetSkillData(skill2);
					maxStackCharacterSkill[skillSlotSet] = skillData2.maxStack;
					if (maxStackCharacterSkill[skillSlotSet] > 0 && !stackCharacterSkill.ContainsKey(skillSlotSet))
					{
						stackCharacterSkill.Add(skillSlotSet, 1);
						stackCharacterSkillIntervalTime.Add(skillSlotSet, 0f);
					}
				}
			}
		}


		public bool IsChargingSkillStack(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				using (List<MasteryType>.Enumerator enumerator = stackWeaponSkillTimers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == masteryType)
						{
							return true;
						}
					}
				}

				return false;
			}

			using (List<SkillSlotSet>.Enumerator enumerator2 = stackCharacterSkillTimers.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current == skillSlotSet)
					{
						return true;
					}
				}
			}

			return false;
		}


		public void AddTimer(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				stackWeaponSkillTimers.Add(masteryType);
				return;
			}

			stackCharacterSkillTimers.Add(skillSlotSet);
		}


		public void RemoveTimer(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				stackWeaponSkillTimers.Remove(masteryType);
				return;
			}

			stackCharacterSkillTimers.Remove(skillSlotSet);
		}


		public int GetTimerCount(bool isWeaponSkill)
		{
			if (isWeaponSkill)
			{
				return stackWeaponSkillTimers.Count;
			}

			return stackCharacterSkillTimers.Count;
		}


		public MasteryType GetWeaponSkillTimer(int index)
		{
			return stackWeaponSkillTimers[index];
		}


		public SkillSlotSet GetCharacterSkillTimer(int index)
		{
			return stackCharacterSkillTimers[index];
		}


		public int GetSkillStack(SkillSlotSet skillSlotSet, MasteryType weaponType)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				if (!stackWeaponSkill.ContainsKey(weaponType))
				{
					return 0;
				}

				return stackWeaponSkill[weaponType];
			}

			if (!stackCharacterSkill.ContainsKey(skillSlotSet))
			{
				return 0;
			}

			return stackCharacterSkill[skillSlotSet];
		}


		private void SettingIntervalTime(SkillSlotSet skillSlotSet, MasteryType masteryType, float intervalTime,
			float currentTime)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				stackWeaponSkillIntervalTime[masteryType] = intervalTime + currentTime;
				return;
			}

			stackCharacterSkillIntervalTime[skillSlotSet] = intervalTime + currentTime;
		}


		public float GetIntervalTime(SkillSlotSet skillSlotSet, MasteryType masteryType, float currentTime)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				if (!stackWeaponSkillIntervalTime.ContainsKey(masteryType))
				{
					return 0f;
				}

				return stackWeaponSkillIntervalTime[masteryType] - currentTime;
			}

			if (!stackCharacterSkillIntervalTime.ContainsKey(skillSlotSet))
			{
				return 0f;
			}

			return stackCharacterSkillIntervalTime[skillSlotSet] - currentTime;
		}
	}
}