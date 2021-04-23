namespace Blis.Common
{
	public static class ActionTypeEx
	{
		public static bool IsCanNotAction(this ActionCategoryType canNotActionCategory,
			CastingActionType castingActionType)
		{
			if (canNotActionCategory == ActionCategoryType.None)
			{
				return false;
			}

			if (canNotActionCategory.HasFlag(ActionCategoryType.NoCastAction) &&
			    castingActionType == CastingActionType.BoxOpen)
			{
				return true;
			}

			if (canNotActionCategory.HasFlag(ActionCategoryType.CastAction))
			{
				switch (castingActionType)
				{
					case CastingActionType.ToRest:
					case CastingActionType.AirSupplyOpen:
					case CastingActionType.CollectibleOpenWater:
					case CastingActionType.CollectibleOpenWood:
					case CastingActionType.CollectibleOpenStone:
					case CastingActionType.CollectibleOpenSeaFish:
					case CastingActionType.CollectibleOpenFreshWaterFish:
					case CastingActionType.CollectibleOpenPotato:
					case CastingActionType.CollectibleOpenTreeOfLife:
					case CastingActionType.CraftCommon:
					case CastingActionType.CraftUnCommon:
					case CastingActionType.CraftRare:
					case CastingActionType.CraftEpic:
					case CastingActionType.CraftLegend:
					case CastingActionType.AreaSecurityCameraSight:
					case CastingActionType.RemoteSecurityCameraSight:
					case CastingActionType.HackingBackdoor:
					case CastingActionType.HackingExclusiveControl:
					case CastingActionType.HackingShutdown:
					case CastingActionType.Hyperloop:
					case CastingActionType.InstallTrap:
						return true;
				}
			}

			return canNotActionCategory.HasFlag(ActionCategoryType.ResurrectAction) &&
			       castingActionType == CastingActionType.Resurrect;
		}


		public static bool IsCanNotAction(this ActionCategoryType canNotActionCategory, ActionType actionType)
		{
			return canNotActionCategory != ActionCategoryType.None &&
			       (canNotActionCategory.HasFlag(ActionCategoryType.NoCastAction) &&
			        (actionType - ActionType.ItemPickup <= 1 || actionType - ActionType.OpenNoCastBox <= 1) ||
			        canNotActionCategory.HasFlag(ActionCategoryType.CastAction) &&
			        (actionType - ActionType.ToRest <= 5 || actionType == ActionType.CastInstallSummon) ||
			        canNotActionCategory.HasFlag(ActionCategoryType.ResurrectAction) &&
			        actionType == ActionType.Resurrect ||
			        canNotActionCategory.HasFlag(ActionCategoryType.ItemUseAction) &&
			        (actionType == ActionType.NoCastInstallSummon || actionType == ActionType.ItemUse ||
			         actionType == ActionType.CastInstallSummon) ||
			        canNotActionCategory.HasFlag(ActionCategoryType.ItemEquipOrUnequipAction) &&
			        actionType == ActionType.ItemEquipOrUnequip);
		}
	}
}