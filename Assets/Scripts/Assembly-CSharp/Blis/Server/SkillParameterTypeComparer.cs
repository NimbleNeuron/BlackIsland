namespace Blis.Server
{
	
	public class
		SkillParameterTypeComparer : SingletonComparerEnum<SkillParameterTypeComparer, SkillScriptParameterType>
	{
		
		public override bool Equals(SkillScriptParameterType x, SkillScriptParameterType y)
		{
			return x == y;
		}

		
		public override int GetHashCode(SkillScriptParameterType obj)
		{
			return (int) obj;
		}
	}
}