using System;

namespace Blis.Common
{
	public static class ItemBoxSizeEx
	{
		public static int GetBoxCapacity(this ItemBoxSize size)
		{
			switch (size)
			{
				case ItemBoxSize.Small:
					return 3;
				case ItemBoxSize.Middle:
					return 4;
				case ItemBoxSize.Large:
					return 5;
				default:
					throw new ArgumentOutOfRangeException("size", size, null);
			}
		}
	}
}