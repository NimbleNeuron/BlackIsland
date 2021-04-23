namespace Blis.Common
{
	
	public class InitRecommendWeaponRoute
	{
		
		public InitRecommendWeaponRoute(int characterCode, WeaponType weaponType, string title, string weaponCodes, string paths)
		{
			this.characterCode = characterCode;
			this.weaponType = weaponType;
			this.title = title;
			this.weaponCodes = weaponCodes;
			this.paths = paths;
		}

		
		public readonly int characterCode;

		
		public readonly WeaponType weaponType;

		
		public readonly string title;

		
		public readonly string weaponCodes;

		
		public readonly string paths;
	}
}
