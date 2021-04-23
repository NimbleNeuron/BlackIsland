namespace Blis.Common
{
	
	public class
		SkillEvolutionPointTypeComparer : SingletonComparerEnum<SkillEvolutionPointTypeComparer, SkillEvolutionPointType
		>
	{
		
		public override bool Equals(SkillEvolutionPointType x, SkillEvolutionPointType y)
		{
			return x == y;
		}

		
		public override int GetHashCode(SkillEvolutionPointType obj)
		{
			return (int) obj;
		}
	}
}