namespace Blis.Common
{
	
	public class ForcedMoveSpeedTypeComparer : SingletonComparerEnum<ForcedMoveSpeedTypeComparer, ForcedMoveSpeedType>
	{
		
		public override bool Equals(ForcedMoveSpeedType x, ForcedMoveSpeedType y)
		{
			return x == y;
		}

		
		public override int GetHashCode(ForcedMoveSpeedType obj)
		{
			return (int) obj;
		}
	}
}