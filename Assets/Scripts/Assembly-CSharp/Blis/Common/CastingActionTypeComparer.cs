namespace Blis.Common
{
	
	public class CastingActionTypeComparer : SingletonComparerEnum<CastingActionTypeComparer, CastingActionType>
	{
		
		public override bool Equals(CastingActionType x, CastingActionType y)
		{
			return x == y;
		}

		
		public override int GetHashCode(CastingActionType obj)
		{
			return (int) obj;
		}
	}
}