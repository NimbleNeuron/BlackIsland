namespace Blis.Common
{
	
	public class SpecialSkillSetData
	{
		
		public readonly SpecialSkillId specialSkillId;

		
		public SkillSet skillSet;

		
		public SpecialSkillSetData(SpecialSkillId specialSkillId, SkillSet skillSet)
		{
			this.specialSkillId = specialSkillId;
			this.skillSet = skillSet;
		}

		
		public class Builder
		{
			
			public readonly SpecialSkillId specialSkillId;

			
			public SkillSet skillSet;

			
			public Builder(SpecialSkillId specialSkillId)
			{
				this.specialSkillId = specialSkillId;
				skillSet = new SkillSet();
			}

			
			public Builder SetActive(int code)
			{
				skillSet.AddActiveSequence(1, code);
				return this;
			}

			
			public static Builder Create(SpecialSkillId specialSkillId)
			{
				return new Builder(specialSkillId);
			}

			
			public SpecialSkillSetData Build()
			{
				return new SpecialSkillSetData(specialSkillId, skillSet);
			}
		}
	}
}