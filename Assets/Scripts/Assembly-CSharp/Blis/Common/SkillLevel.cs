using System.Collections.Generic;

namespace Blis.Common
{
	public class SkillLevel
	{
		private Dictionary<SkillSlotIndex, int> skillEvolutionLevelMap =
			new Dictionary<SkillSlotIndex, int>(SingletonComparerEnum<SkillSlotIndexComparer, SkillSlotIndex>.Instance);


		private Dictionary<SkillSlotIndex, int> skillLevelMap =
			new Dictionary<SkillSlotIndex, int>(SingletonComparerEnum<SkillSlotIndexComparer, SkillSlotIndex>.Instance);


		private Dictionary<MasteryType, int> weaponSkillEvolutionLevelMap =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		private Dictionary<MasteryType, int> weaponSkillLevelMap =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);

		public SkillLevel()
		{
			skillLevelMap.Clear();
			skillLevelMap.Add(SkillSlotIndex.Attack, 1);
			skillLevelMap.Add(SkillSlotIndex.Passive, 1);
			skillLevelMap.Add(SkillSlotIndex.Active1, 0);
			skillLevelMap.Add(SkillSlotIndex.Active2, 0);
			skillLevelMap.Add(SkillSlotIndex.Active3, 0);
			skillLevelMap.Add(SkillSlotIndex.Active4, 0);
			skillLevelMap.Add(SkillSlotIndex.SpecialSkill, 1);
			skillEvolutionLevelMap.Clear();
			skillEvolutionLevelMap.Add(SkillSlotIndex.Attack, 0);
			skillEvolutionLevelMap.Add(SkillSlotIndex.Passive, 0);
			skillEvolutionLevelMap.Add(SkillSlotIndex.Active1, 0);
			skillEvolutionLevelMap.Add(SkillSlotIndex.Active2, 0);
			skillEvolutionLevelMap.Add(SkillSlotIndex.Active3, 0);
			skillEvolutionLevelMap.Add(SkillSlotIndex.Active4, 0);
			skillEvolutionLevelMap.Add(SkillSlotIndex.SpecialSkill, 0);
		}


		public SkillLevelSnapshot CreateSnapshot()
		{
			return new SkillLevelSnapshot
			{
				skillLevelMap = skillLevelMap,
				skillEvolutionLevelMap = skillEvolutionLevelMap,
				weaponSkillLevelMap = weaponSkillLevelMap,
				weaponSkillEvolutionLevelMap = weaponSkillEvolutionLevelMap
			};
		}


		public void Init(SkillLevelSnapshot snapshot)
		{
			if (snapshot == null)
			{
				return;
			}

			skillLevelMap = snapshot.skillLevelMap;
			skillEvolutionLevelMap = snapshot.skillEvolutionLevelMap;
			weaponSkillLevelMap = snapshot.weaponSkillLevelMap;
			weaponSkillEvolutionLevelMap = snapshot.weaponSkillEvolutionLevelMap;
		}


		public void UpgradeSkill(SkillSlotIndex skillSlotIndex)
		{
			Dictionary<SkillSlotIndex, int> dictionary = skillLevelMap;
			int num = dictionary[skillSlotIndex];
			dictionary[skillSlotIndex] = num + 1;
		}


		public void SetSkillLevel(SkillSlotIndex skillSlotIndex, int level)
		{
			skillLevelMap[skillSlotIndex] = level;
		}


		public int GetSkillLevel(SkillSlotIndex skillSlotIndex)
		{
			if (!skillLevelMap.ContainsKey(skillSlotIndex))
			{
				return 0;
			}

			return skillLevelMap[skillSlotIndex];
		}


		public int GetSkillLevel(SkillSlotSet skillSlotSet)
		{
			SkillSlotIndex skillSlotIndex = skillSlotSet.SlotSet2Index();
			return GetSkillLevel(skillSlotIndex);
		}


		public void UpgradeSkill(MasteryType masteryType)
		{
			if (!weaponSkillLevelMap.ContainsKey(masteryType))
			{
				weaponSkillLevelMap.Add(masteryType, 0);
			}

			Dictionary<MasteryType, int> dictionary = weaponSkillLevelMap;
			int num = dictionary[masteryType];
			dictionary[masteryType] = num + 1;
		}


		public int GetSkillLevel(MasteryType masteryType)
		{
			if (!weaponSkillLevelMap.ContainsKey(masteryType))
			{
				return 0;
			}

			return weaponSkillLevelMap[masteryType];
		}


		public void EvolutionSkill(SkillSlotIndex skillSlotIndex)
		{
			Dictionary<SkillSlotIndex, int> dictionary = skillEvolutionLevelMap;
			int num = dictionary[skillSlotIndex];
			dictionary[skillSlotIndex] = num + 1;
		}


		public int GetEvolutionSkill(SkillSlotIndex skillSlotIndex)
		{
			return skillEvolutionLevelMap[skillSlotIndex];
		}


		public void EvolutionSkill(MasteryType masteryType)
		{
			if (!weaponSkillEvolutionLevelMap.ContainsKey(masteryType))
			{
				weaponSkillEvolutionLevelMap.Add(masteryType, 0);
			}

			Dictionary<MasteryType, int> dictionary = weaponSkillEvolutionLevelMap;
			int num = dictionary[masteryType];
			dictionary[masteryType] = num + 1;
		}


		public int GetEvolutionSkill(MasteryType masteryType)
		{
			if (!weaponSkillEvolutionLevelMap.ContainsKey(masteryType))
			{
				return 0;
			}

			return weaponSkillEvolutionLevelMap[masteryType];
		}
	}
}