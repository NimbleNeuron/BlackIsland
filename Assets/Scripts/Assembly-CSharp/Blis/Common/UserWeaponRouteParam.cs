namespace Blis.Common
{
	
	public class UserWeaponRouteParam
	{
		
		public int characterCode;

		
		public int order;

		
		public long recommendWeaponRouteId;

		
		public int slotId;

		
		public UserWeaponRouteParam(int characterCode, int slotId, int order, long recommendWeaponRouteId)
		{
			this.characterCode = characterCode;
			this.slotId = slotId;
			this.order = order;
			this.recommendWeaponRouteId = recommendWeaponRouteId;
		}
	}
}