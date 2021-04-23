using UnityEngine;

namespace Blis.Client
{
	public static class UIUtility
	{
		public static Color32 ObserverTeamColor(int index)
		{
			switch (index)
			{
				case 0:
					return new Color32(211, 23, 54, byte.MaxValue);
				case 1:
					return new Color32(209, 106, 24, byte.MaxValue);
				case 2:
					return new Color32(229, 176, 19, byte.MaxValue);
				case 3:
					return new Color32(74, 163, 11, byte.MaxValue);
				case 4:
					return new Color32(68, 169, 154, byte.MaxValue);
				case 5:
					return new Color32(15, 108, 222, byte.MaxValue);
				case 6:
					return new Color32(146, 55, 212, byte.MaxValue);
				case 7:
					return new Color32(214, 48, 165, byte.MaxValue);
				case 8:
					return new Color32(203, 196, 243, byte.MaxValue);
				default:
					return new Color32(211, 23, 54, byte.MaxValue);
			}
		}
	}
}