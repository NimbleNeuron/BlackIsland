namespace Blis.Common
{
	public class StateTypeComparer : SingletonComparerEnum<StateTypeComparer, StateType>
	{
		public override bool Equals(StateType x, StateType y)
		{
			return x == y;
		}


		public override int GetHashCode(StateType obj)
		{
			return (int) obj;
		}
	}
}