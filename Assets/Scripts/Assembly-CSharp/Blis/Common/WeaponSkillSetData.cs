namespace Blis.Common
{
	
	public class WeaponSkillSetData
	{
		
		public readonly MasteryType masteryType;

		
		public SkillSet skillSet;

		
		public WeaponSkillSetData(MasteryType masteryType, SkillSet skillSet)
		{
			this.masteryType = masteryType;
			this.skillSet = skillSet;
		}

		
		public class Builder
		{
			
			public readonly MasteryType masteryType;

			
			public SkillSet skillSet;

			
			public Builder(MasteryType masteryType)
			{
				this.masteryType = masteryType;
				skillSet = new SkillSet();
			}

			
			public Builder SetActive(int level, int code)
			{
				skillSet.AddActiveSequence(level, code);
				return this;
			}

			
			public static Builder Create(MasteryType masteryType)
			{
				return new Builder(masteryType);
			}

			
			public WeaponSkillSetData Build()
			{
				return new WeaponSkillSetData(masteryType, skillSet);
			}
		}
	}
}