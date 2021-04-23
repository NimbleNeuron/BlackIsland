namespace Blis.Common
{
	
	public class BlockedSightTypeComparer : SingletonComparerEnum<BlockedSightTypeComparer, BlockedSightType>
	{
		
		public override bool Equals(BlockedSightType x, BlockedSightType y)
		{
			return x == y;
		}

		
		public override int GetHashCode(BlockedSightType obj)
		{
			return (int) obj;
		}
	}
}