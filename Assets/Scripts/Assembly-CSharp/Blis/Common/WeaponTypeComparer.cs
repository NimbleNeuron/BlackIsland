namespace Blis.Common
{
	
	public class WeaponTypeComparer : SingletonComparerEnum<WeaponTypeComparer, WeaponType>
	{
		
		public override bool Equals(WeaponType x, WeaponType y)
		{
			return x == y;
		}

		
		public override int GetHashCode(WeaponType obj)
		{
			return (int) obj;
		}
	}
}