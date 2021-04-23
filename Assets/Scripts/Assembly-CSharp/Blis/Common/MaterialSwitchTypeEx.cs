namespace Blis.Common
{
	
	public static class MaterialSwitchTypeEx
	{
		
		public static bool HasFlag(this MaterialSwitchType a, MaterialSwitchType hasFlag)
		{
			return hasFlag == (a & hasFlag);
		}

		
		public static MaterialSwitchType AddFlag(this MaterialSwitchType a, MaterialSwitchType addFlag)
		{
			if (addFlag == MaterialSwitchType.Original)
			{
				return a.RemoveAllFlag();
			}

			a |= addFlag;
			return a;
		}

		
		public static MaterialSwitchType RemoveFlag(this MaterialSwitchType a, MaterialSwitchType removeFlag)
		{
			a &= ~removeFlag;
			return a;
		}

		
		public static MaterialSwitchType RemoveAllFlag(this MaterialSwitchType a)
		{
			return MaterialSwitchType.Original;
		}
	}
}