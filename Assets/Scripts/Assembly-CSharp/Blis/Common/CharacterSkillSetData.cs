using System.Collections.Generic;

namespace Blis.Common
{
	
	public class CharacterSkillSetData
	{
		
		public readonly int characterCode;

		
		public readonly Dictionary<SkillSlotIndex, SkillSlotSet> defaultSkillSet;

		
		public readonly Dictionary<SkillSlotSet, SkillSet> skillCodeMap;

		
		public CharacterSkillSetData(int characterCode, Dictionary<SkillSlotSet, SkillSet> skillCodeMap,
			Dictionary<SkillSlotIndex, SkillSlotSet> defaultSkillSet)
		{
			this.characterCode = characterCode;
			this.skillCodeMap = skillCodeMap;
			this.defaultSkillSet = defaultSkillSet;
		}

		
		public class Builder
		{
			
			public readonly int characterCode;

			
			public readonly Dictionary<SkillSlotIndex, SkillSlotSet> defaultSkillSet;

			
			public readonly Dictionary<SkillSlotSet, SkillSet> skillCodeMap;

			
			public Builder(int characterCode)
			{
				this.characterCode = characterCode;
				skillCodeMap =
					new Dictionary<SkillSlotSet, SkillSet>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>
						.Instance);
				defaultSkillSet =
					new Dictionary<SkillSlotIndex, SkillSlotSet>(
						SingletonComparerEnum<SkillSlotIndexComparer, SkillSlotIndex>.Instance);
			}

			
			public Builder SetDefaultSkillSet(SkillSlotIndex skillSlotIndex, SkillSlotSet skillSlotSet)
			{
				if (skillSlotSet.IsValidRange(skillSlotIndex))
				{
					defaultSkillSet[skillSlotIndex] = skillSlotSet;
				}

				return this;
			}

			
			public Builder SetActive(SkillSlotSet skillSlotSet, int level, int code)
			{
				if (!skillCodeMap.ContainsKey(skillSlotSet))
				{
					skillCodeMap.Add(skillSlotSet, new SkillSet());
				}

				skillCodeMap[skillSlotSet].AddActiveSequence(level, code);
				return this;
			}

			
			public static Builder Create(int characterCode)
			{
				return new Builder(characterCode);
			}

			
			public CharacterSkillSetData Build()
			{
				if (!defaultSkillSet.ContainsKey(SkillSlotIndex.Attack))
				{
					defaultSkillSet.Add(SkillSlotIndex.Attack, SkillSlotSet.Attack_1);
				}

				if (!defaultSkillSet.ContainsKey(SkillSlotIndex.Passive))
				{
					defaultSkillSet.Add(SkillSlotIndex.Passive, SkillSlotSet.Passive_1);
				}

				if (!defaultSkillSet.ContainsKey(SkillSlotIndex.Active1))
				{
					defaultSkillSet.Add(SkillSlotIndex.Active1, SkillSlotSet.Active1_1);
				}

				if (!defaultSkillSet.ContainsKey(SkillSlotIndex.Active2))
				{
					defaultSkillSet.Add(SkillSlotIndex.Active2, SkillSlotSet.Active2_1);
				}

				if (!defaultSkillSet.ContainsKey(SkillSlotIndex.Active3))
				{
					defaultSkillSet.Add(SkillSlotIndex.Active3, SkillSlotSet.Active3_1);
				}

				if (!defaultSkillSet.ContainsKey(SkillSlotIndex.Active4))
				{
					defaultSkillSet.Add(SkillSlotIndex.Active4, SkillSlotSet.Active4_1);
				}

				defaultSkillSet[SkillSlotIndex.WeaponSkill] = SkillSlotSet.WeaponSkill;
				defaultSkillSet[SkillSlotIndex.SpecialSkill] = SkillSlotSet.SpecialSkill;
				return new CharacterSkillSetData(characterCode, skillCodeMap, defaultSkillSet);
			}
		}
	}
}