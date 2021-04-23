namespace Blis.Common
{
	public class SkillIdComparer : SingletonComparerEnum<SkillIdComparer, SkillId>
	{
		public override bool Equals(SkillId x, SkillId y)
		{
			return x == y;
		}


		public override int GetHashCode(SkillId obj)
		{
			return (int) obj;
		}
	}
}