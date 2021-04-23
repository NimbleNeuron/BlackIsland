using System.Collections.Generic;

namespace Blis.Common
{
	
	public static class SkillSlotIndexExtensions
	{
		
		private static readonly Dictionary<SkillSlotIndex, SkillSlotSet> skillSlotSetFlags =
			new Dictionary<SkillSlotIndex, SkillSlotSet>(SingletonComparerEnum<SkillSlotIndexComparer, SkillSlotIndex>
				.Instance);

		
		private static readonly Dictionary<SkillSlotIndex, List<SkillSlotSet>> skillSlotSetList =
			new Dictionary<SkillSlotIndex, List<SkillSlotSet>>(
				SingletonComparerEnum<SkillSlotIndexComparer, SkillSlotIndex>.Instance);

		
		public static SkillEvolutionPointType ToEvolutionPointType(this SkillSlotIndex skillSlotIndex)
		{
			switch (skillSlotIndex)
			{
				case SkillSlotIndex.Passive:
					return SkillEvolutionPointType.Passive;
				case SkillSlotIndex.Active1:
					return SkillEvolutionPointType.Active1;
				case SkillSlotIndex.Active2:
					return SkillEvolutionPointType.Active2;
				case SkillSlotIndex.Active3:
					return SkillEvolutionPointType.Active3;
				case SkillSlotIndex.Active4:
					return SkillEvolutionPointType.Active4;
				case SkillSlotIndex.WeaponSkill:
					return SkillEvolutionPointType.WeaponSkill;
				default:
					return SkillEvolutionPointType.CharacterSkill;
			}
		}

		
		public static SkillSlotSet Index2SlotSets(this SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotSetFlags.Count == 0)
			{
				foreach (SkillSlotSet skillSlotSet in GameDB.skill.allSkillSlotSet)
				{
					if (skillSlotSet != SkillSlotSet.None)
					{
						SkillSlotIndex skillSlotIndex2 = skillSlotSet.SlotSet2Index();
						if (skillSlotSetFlags.ContainsKey(skillSlotIndex2))
						{
							Dictionary<SkillSlotIndex, SkillSlotSet> dictionary = skillSlotSetFlags;
							SkillSlotIndex key = skillSlotIndex2;
							dictionary[key] |= skillSlotSet;
						}
						else
						{
							skillSlotSetFlags.Add(skillSlotIndex2, skillSlotSet);
						}
					}
				}
			}

			return skillSlotSetFlags[skillSlotIndex];
		}

		
		public static List<SkillSlotSet> Index2SlotList(this SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotSetList.Count == 0)
			{
				foreach (SkillSlotSet skillSlotSet in GameDB.skill.allSkillSlotSet)
				{
					if (skillSlotSet != SkillSlotSet.None)
					{
						SkillSlotIndex key = skillSlotSet.SlotSet2Index();
						if (skillSlotSetList.ContainsKey(key))
						{
							skillSlotSetList[key].Add(skillSlotSet);
						}
						else
						{
							skillSlotSetList.Add(key, new List<SkillSlotSet>
							{
								skillSlotSet
							});
						}
					}
				}
			}

			return skillSlotSetList[skillSlotIndex];
		}
	}
}