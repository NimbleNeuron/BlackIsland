namespace Blis.Common
{
	
	public class SkillCostTypeComparer : SingletonComparerEnum<SkillCostTypeComparer, SkillCostType>
	{
		
		public override bool Equals(SkillCostType x, SkillCostType y)
		{
			return x == y;
		}

		
		public override int GetHashCode(SkillCostType obj)
		{
			return (int) obj;
		}
	}
}