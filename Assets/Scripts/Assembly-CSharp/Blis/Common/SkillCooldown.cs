using System.Collections.Generic;

namespace Blis.Common
{
	public class SkillCooldown
	{
		private readonly Dictionary<SkillSlotSet, bool> holdMap =
			new Dictionary<SkillSlotSet, bool>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>.Instance);


		private readonly Dictionary<MasteryType, bool> weaponSkillHoldMap =
			new Dictionary<MasteryType, bool>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		private Dictionary<SkillSlotSet, float> cooldownMap =
			new Dictionary<SkillSlotSet, float>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>.Instance);


		private Dictionary<SkillSlotSet, float> maxCooldownMap =
			new Dictionary<SkillSlotSet, float>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>.Instance);


		private Dictionary<MasteryType, float> weaponSkillCooldownMap =
			new Dictionary<MasteryType, float>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		public SkillCooldown()
		{
			ResetCooldown();
		}

		public SkillCooldownSnapshot CreateSnapshot()
		{
			return new SkillCooldownSnapshot
			{
				cooldownMap = cooldownMap,
				maxCooldownMap = maxCooldownMap,
				weaponSkillCooldownMap = weaponSkillCooldownMap
			};
		}


		public void Init(SkillCooldownSnapshot snapshot)
		{
			if (snapshot == null)
			{
				return;
			}

			cooldownMap = snapshot.cooldownMap;
			maxCooldownMap = snapshot.maxCooldownMap;
			weaponSkillCooldownMap = snapshot.weaponSkillCooldownMap;
		}


		public void ResetCooldown()
		{
			cooldownMap.Clear();
			maxCooldownMap.Clear();
			weaponSkillCooldownMap.Clear();
		}


		public bool CheckCooldown(SkillSlotSet skillSlotSet, float currentTime)
		{
			if (!cooldownMap.ContainsKey(skillSlotSet))
			{
				cooldownMap[skillSlotSet] = currentTime;
			}

			return cooldownMap[skillSlotSet] <= currentTime;
		}


		public void StartCooldown(SkillSlotSet skillSlotSet, float currentTime, float cooldown)
		{
			cooldownMap[skillSlotSet] = currentTime + cooldown;
			maxCooldownMap[skillSlotSet] = cooldown;
			holdMap[skillSlotSet] = false;
		}


		public void ModifyCooldown(SkillSlotSet skillSlotSet, float currentTime, float modifyValue)
		{
			if (!cooldownMap.ContainsKey(skillSlotSet) || IsHold(skillSlotSet))
			{
				cooldownMap[skillSlotSet] = currentTime;
				holdMap[skillSlotSet] = false;
			}

			Dictionary<SkillSlotSet, float> dictionary = cooldownMap;
			dictionary[skillSlotSet] += modifyValue;
		}


		public void SetHoldCooldown(SkillSlotSet skillSlotSet, bool isHold)
		{
			holdMap[skillSlotSet] = isHold;
		}


		public float GetCooldown(SkillSlotSet skillSlotSet, float currentTime)
		{
			if (!cooldownMap.ContainsKey(skillSlotSet))
			{
				return 0f;
			}

			float num = cooldownMap[skillSlotSet] - currentTime;
			if (num >= 0f)
			{
				return num;
			}

			return 0f;
		}


		public float GetMaxCooldown(SkillSlotSet skillSlotSet)
		{
			if (!maxCooldownMap.ContainsKey(skillSlotSet))
			{
				return 0f;
			}

			return maxCooldownMap[skillSlotSet];
		}


		public bool IsHold(SkillSlotSet skillSlotSet)
		{
			return holdMap.ContainsKey(skillSlotSet) && holdMap[skillSlotSet];
		}


		public bool CheckCooldown(MasteryType masteryType, float currentTime)
		{
			if (!weaponSkillCooldownMap.ContainsKey(masteryType))
			{
				weaponSkillCooldownMap[masteryType] = currentTime;
			}

			return weaponSkillCooldownMap[masteryType] <= currentTime;
		}


		public void StartCooldown(MasteryType masteryType, float currentTime, float cooldown)
		{
			weaponSkillCooldownMap[masteryType] = currentTime + cooldown;
			weaponSkillHoldMap[masteryType] = false;
		}


		public void ModifyCooldown(MasteryType masteryType, float currentTime, float modifyValue)
		{
			if (!weaponSkillCooldownMap.ContainsKey(masteryType) || IsHold(masteryType))
			{
				weaponSkillCooldownMap[masteryType] = currentTime;
				weaponSkillHoldMap[masteryType] = false;
			}

			Dictionary<MasteryType, float> dictionary = weaponSkillCooldownMap;
			dictionary[masteryType] += modifyValue;
		}


		public void SetHoldCooldown(MasteryType masteryType, bool isHold)
		{
			weaponSkillHoldMap[masteryType] = isHold;
		}


		public bool IsHold(MasteryType masteryType)
		{
			return weaponSkillHoldMap.ContainsKey(masteryType) && weaponSkillHoldMap[masteryType];
		}


		public float GetCooldown(MasteryType masteryType, float currentTime)
		{
			if (!weaponSkillCooldownMap.ContainsKey(masteryType))
			{
				return 0f;
			}

			float num = weaponSkillCooldownMap[masteryType] - currentTime;
			if (num >= 0f)
			{
				return num;
			}

			return 0f;
		}
	}
}