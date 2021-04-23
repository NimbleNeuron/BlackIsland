namespace Blis.Client
{
	public class LnTypeComparer : SingletonComparerEnum<LnTypeComparer, LnType>
	{
		public override bool Equals(LnType x, LnType y)
		{
			return x == y;
		}


		public override int GetHashCode(LnType obj)
		{
			return (int) obj;
		}
	}
}