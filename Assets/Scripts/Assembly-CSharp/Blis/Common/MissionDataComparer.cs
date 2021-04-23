namespace Blis.Common
{
	
	public class MissionDataComparer : SingletonComparerClass<MissionDataComparer, MissionData>
	{
		
		public override bool Equals(MissionData x, MissionData y)
		{
			return x != null && y != null && x.code == y.code && x.seq == y.seq;
		}

		
		public override int GetHashCode(MissionData obj)
		{
			return obj.GetHashCode();
		}
	}
}