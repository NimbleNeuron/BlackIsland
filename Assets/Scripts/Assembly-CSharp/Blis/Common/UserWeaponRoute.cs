namespace Blis.Common
{
	public class UserWeaponRoute
	{
		public readonly int characterCode;


		public readonly bool initRecommend;


		public readonly int order;


		public readonly string paths;


		public readonly string recommendUserNickname;


		public readonly long recommendWeaponRouteId;


		public readonly bool share;


		public readonly long shareWeaponRouteId;


		public readonly int slotId;


		public readonly RouteFilterType teamMode;


		public readonly string title;


		public readonly string version;


		public readonly string weaponCodes;


		public readonly WeaponType weaponType;

		public UserWeaponRoute(int characterCode, int slotId, string title, WeaponType weaponType, string weaponCodes,
			string paths, long recommendWeaponRouteId, string recommendUserNickname, bool share, bool initRecommend,
			string version, RouteFilterType teamMode, int order, long shareWeaponRouteId)
		{
			this.characterCode = characterCode;
			this.slotId = slotId;
			this.title = title;
			this.weaponType = weaponType;
			this.weaponCodes = weaponCodes;
			this.paths = paths;
			this.recommendWeaponRouteId = recommendWeaponRouteId;
			this.recommendUserNickname = recommendUserNickname;
			this.share = share;
			this.initRecommend = initRecommend;
			this.version = version;
			this.teamMode = teamMode;
			this.order = order;
			this.shareWeaponRouteId = shareWeaponRouteId;
		}
	}
}