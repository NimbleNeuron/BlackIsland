namespace Blis.Common
{
	public class StatTypeComparer : SingletonComparerEnum<StatTypeComparer, StatType>
	{
		public override bool Equals(StatType x, StatType y)
		{
			return x == y;
		}


		public override int GetHashCode(StatType obj)
		{
			return (int) obj;
		}
	}
}