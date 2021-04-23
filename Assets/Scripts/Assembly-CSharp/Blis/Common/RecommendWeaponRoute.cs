using System;

namespace Blis.Common
{
	
	public class RecommendWeaponRoute
	{
		
		public RecommendWeaponRoute(long id, string title, long userNum, string userNickname, int characterCode, WeaponType weaponType, string weaponCodes, string paths, int count, long updateDtm, string version, RouteFilterType teamMode, int order)
		{
			this.id = id;
			this.title = title;
			this.userNum = userNum;
			this.userNickname = userNickname;
			this.characterCode = characterCode;
			this.weaponType = weaponType;
			this.weaponCodes = weaponCodes;
			this.paths = paths;
			this.count = count;
			this.updateDtm = GameUtil.ConvertFromUnixTimestamp(updateDtm / 1000L);
			this.version = version;
			this.routeFilterType = teamMode;
		}

		
		public readonly long id;

		
		public readonly string title;

		
		public readonly long userNum;

		
		public readonly string userNickname;

		
		public readonly int characterCode;

		
		public readonly WeaponType weaponType;

		
		public readonly string weaponCodes;

		
		public readonly string paths;

		
		public readonly int count;

		
		public readonly DateTime updateDtm;

		
		public readonly string version;

		
		public readonly RouteFilterType routeFilterType;
	}
}
