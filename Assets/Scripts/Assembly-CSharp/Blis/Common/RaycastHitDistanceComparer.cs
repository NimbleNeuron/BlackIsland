using System.Collections.Generic;
using UnityEngine;

namespace Blis.Common
{
	
	public class RaycastHitDistanceComparer : IComparer<RaycastHit>
	{
		
		public int Compare(RaycastHit x, RaycastHit y)
		{
			return x.distance.CompareTo(y.distance);
		}
	}
}