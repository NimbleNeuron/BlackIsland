using System;
using System.Collections.Generic;

namespace Blis.Common
{
	public class Favorite : ICloneable
	{
		public int characterCode;


		public bool init;


		public int order;


		public List<int> paths;


		public string recommendUserNickname;


		public long recommendWeaponRouteId;


		public RouteFilterType routeFilterType;


		public bool share;


		public long shareWeaponRouteId;


		public int slotId;


		public string title;


		public long userNum;


		public string version;


		public List<int> weaponCodes;


		public WeaponType weaponType;

		public Favorite()
		{
			weaponCodes = new List<int>();
			paths = new List<int>();
		}


		public Favorite(long userNum, int characterCode, int slotId, string title, WeaponType weaponType,
			List<int> weaponCodes, List<int> paths, long recommendWeaponRouteId, string recommendUserNickname,
			bool share, bool init, string version, RouteFilterType teamMode, int order, long shareWeaponRouteId)
		{
			this.userNum = userNum;
			this.characterCode = characterCode;
			this.slotId = slotId;
			this.title = title;
			this.weaponType = weaponType;
			this.weaponCodes = weaponCodes;
			this.paths = paths;
			this.recommendWeaponRouteId = recommendWeaponRouteId;
			this.recommendUserNickname = recommendUserNickname;
			this.share = share;
			this.init = init;
			this.version = version;
			routeFilterType = teamMode;
			this.order = order;
			this.shareWeaponRouteId = shareWeaponRouteId;
		}


		public object Clone()
		{
			return new Favorite
			{
				userNum = userNum,
				characterCode = characterCode,
				slotId = slotId,
				title = title,
				weaponType = weaponType,
				weaponCodes = new List<int>(weaponCodes),
				paths = new List<int>(paths),
				recommendWeaponRouteId = recommendWeaponRouteId,
				recommendUserNickname = recommendUserNickname,
				share = share,
				init = init,
				version = version,
				routeFilterType = routeFilterType,
				order = order,
				shareWeaponRouteId = shareWeaponRouteId
			};
		}
	}
}