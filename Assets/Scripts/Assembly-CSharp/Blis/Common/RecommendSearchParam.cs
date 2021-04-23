namespace Blis.Common
{
	
	public class RecommendSearchParam
	{
		
		public RecommendSearchParam(RecommendRouteSortType sortType, int characterCode, WeaponType weaponType, int pageIndex, RouteFilterType teamMode, bool recentlyVersion)
		{
			this.sortType = sortType;
			this.characterCode = characterCode;
			this.weaponType = weaponType;
			this.pageIndex = pageIndex;
			this.teamMode = teamMode;
			this.recentlyVersion = recentlyVersion;
		}

		
		public RecommendRouteSortType sortType;

		
		public int characterCode;

		
		public WeaponType weaponType;

		
		public int pageIndex;

		
		public RouteFilterType teamMode;

		
		public bool recentlyVersion;
	}
}
