namespace Blis.Common
{
	
	public class HostileTypeComparer : SingletonComparerEnum<HostileTypeComparer, HostileType>
	{
		
		public override bool Equals(HostileType x, HostileType y)
		{
			return x == y;
		}

		
		public override int GetHashCode(HostileType obj)
		{
			return (int) obj;
		}
	}
}