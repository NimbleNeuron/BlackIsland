namespace Blis.Common
{
	
	public class MasteryTypeComparer : SingletonComparerEnum<MasteryTypeComparer, MasteryType>
	{
		
		public override bool Equals(MasteryType x, MasteryType y)
		{
			return x == y;
		}

		
		public override int GetHashCode(MasteryType obj)
		{
			return (int) obj;
		}
	}
}