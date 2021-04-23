namespace Blis.Common
{
	public class RecommendCopyParam
	{
		public int characterCode;


		public bool initRecommend;


		public int order;


		public long recommendWeaponRouteId;


		public int slotId;


		public WeaponType weaponType;

		public RecommendCopyParam(int characterCode, int slotId, long recommendWeaponRouteId, bool initRecommend,
			WeaponType weaponType, int order)
		{
			this.characterCode = characterCode;
			this.slotId = slotId;
			this.recommendWeaponRouteId = recommendWeaponRouteId;
			this.initRecommend = initRecommend;
			this.weaponType = weaponType;
			this.order = order;
		}
	}
}