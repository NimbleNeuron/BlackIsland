namespace Blis.Common
{
	
	public class SkillSlotSetComparer : SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>
	{
		
		public override bool Equals(SkillSlotSet x, SkillSlotSet y)
		{
			return x == y;
		}

		
		public override int GetHashCode(SkillSlotSet obj)
		{
			return (int) obj;
		}
	}
}