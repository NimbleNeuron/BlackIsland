using System;
using System.Collections.Generic;

namespace Blis.Server
{
	
	public class WorldItemBoxCapacityComparer : IComparer<WorldItemBox>
	{
		
		public int Compare(WorldItemBox x, WorldItemBox y)
		{
			if (x == null || y == null)
			{
				throw new ArgumentException("Wrong Type of WorldItemBoxCapacityComparer parameter.");
			}

			int num = x.Capacity.CompareTo(y.Capacity);
			return 0 - num;
		}
	}
}