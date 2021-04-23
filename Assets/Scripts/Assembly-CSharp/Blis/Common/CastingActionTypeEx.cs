namespace Blis.Common
{
	
	public static class CastingActionTypeEx
	{
		
		public static bool IsCraftAction(this CastingActionType type)
		{
			return type - CastingActionType.CraftCommon <= 4;
		}

		
		public static bool IsMyCastingActionType(this CastingActionType type)
		{
			return type != CastingActionType.Resurrected;
		}

		
		public static bool IgnoreCrowdControl(this CastingActionType type)
		{
			return type == CastingActionType.ToBattle;
		}
	}
}