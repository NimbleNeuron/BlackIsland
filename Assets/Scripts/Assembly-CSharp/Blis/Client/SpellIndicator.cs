using UnityEngine;

namespace Blis.Client
{
	public abstract class SpellIndicator : Splat
	{
		[SerializeField] private RangeIndicator RangeIndicator = default;

		protected override void UpdateRange(float skillRange)
		{
			if (RangeIndicator != null)
			{
				RangeIndicator.Range = skillRange;
			}
		}


		protected override void UpdateLength(float length)
		{
			if (RangeIndicator != null)
			{
				RangeIndicator.Length = length;
			}
		}
	}
}