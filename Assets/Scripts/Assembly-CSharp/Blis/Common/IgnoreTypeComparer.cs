namespace Blis.Common
{
	public class IgnoreTypeComparer : SingletonComparerEnum<IgnoreTypeComparer, IgnoreType>
	{
		public override bool Equals(IgnoreType x, IgnoreType y)
		{
			return x == y;
		}


		public override int GetHashCode(IgnoreType obj)
		{
			return (int) obj;
		}
	}
}