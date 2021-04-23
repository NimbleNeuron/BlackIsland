namespace Blis.Common
{
	
	public class
		MasteryConditionTypeComparer : SingletonComparerEnum<MasteryConditionTypeComparer, MasteryConditionType>
	{
		
		public override bool Equals(MasteryConditionType x, MasteryConditionType y)
		{
			return x == y;
		}

		
		public override int GetHashCode(MasteryConditionType obj)
		{
			return (int) obj;
		}
	}
}