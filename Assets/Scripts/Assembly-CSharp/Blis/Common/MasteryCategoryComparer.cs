namespace Blis.Common
{
	
	public class MasteryCategoryComparer : SingletonComparerEnum<MasteryCategoryComparer, MasteryCategory>
	{
		
		public override bool Equals(MasteryCategory x, MasteryCategory y)
		{
			return x == y;
		}

		
		public override int GetHashCode(MasteryCategory obj)
		{
			return (int) obj;
		}
	}
}