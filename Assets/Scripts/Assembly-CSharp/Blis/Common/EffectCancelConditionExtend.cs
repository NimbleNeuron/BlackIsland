using System;

namespace Blis.Common
{
	
	public static class EffectCancelConditionExtend
	{
		
		public static bool IsCancel(this EffectCancelCondition condition, CastingCancelType cancelType)
		{
			if (cancelType == CastingCancelType.Forced)
			{
				return true;
			}

			switch (condition)
			{
				case EffectCancelCondition.DamageAndNotControl:
					return true;
				case EffectCancelCondition.NotControl:
					return cancelType == CastingCancelType.Action || cancelType == CastingCancelType.CC;
				case EffectCancelCondition.NonStop:
					return false;
				default:
					throw new NotImplementedException();
			}
		}
	}
}