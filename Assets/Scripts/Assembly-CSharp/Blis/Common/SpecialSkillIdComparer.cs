namespace Blis.Common
{
	
	public class SpecialSkillIdComparer : SingletonComparerEnum<SpecialSkillIdComparer, SpecialSkillId>
	{
		
		public override bool Equals(SpecialSkillId x, SpecialSkillId y)
		{
			return x == y;
		}

		
		public override int GetHashCode(SpecialSkillId objSpecialSkillId)
		{
			return (int) objSpecialSkillId;
		}
	}
}