namespace Blis.Common
{
	public enum CastingActionType
	{
		None = 0,
		ToRest = 1,
		ToSearch = 2,
		ToBattle = 3,
		AirSupplyOpen = 4,
		CollectibleOpen = 5,
		BoxOpen = 6,
		
		CollectibleOpenWater = 7,
		CollectibleOpenWood = 8,
		CollectibleOpenStone = 9,
		CollectibleOpenSeaFish = 10,
		CollectibleOpenFreshWaterFish = 11,
		CollectibleOpenPotato = 12,
		CollectibleOpenTreeOfLife = 13,
		CollectibleOpenMeteorite = 14,
		CraftCommon = 15,
		CraftUnCommon = 16,
		CraftRare = 17,
		CraftEpic = 18,
		CraftLegend = 19,

		AreaSecurityCameraSight,
		RemoteSecurityCameraSight,

		HackingBackdoor,
		HackingExclusiveControl,
		HackingShutdown,

		Hyperloop,
		InstallTrap,

		Resurrect,
		Resurrected
	}
}