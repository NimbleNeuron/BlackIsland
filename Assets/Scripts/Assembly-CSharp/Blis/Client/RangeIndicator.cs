using UnityEngine;

namespace Blis.Client
{
	public class RangeIndicator : Splat
	{
		protected override void UpdateRange(float skillRange)
		{
			transform.localScale = Vector3.one * skillRange;
		}
	}
}