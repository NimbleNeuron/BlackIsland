using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class BoxIndicator : Splat
	{
		[SerializeField] private Transform square = default;


		[SerializeField] private Transform anchor = default;

		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (fixedDirection != null)
			{
				Vector3 value = fixedDirection.Value;
				anchor.rotation = GameUtil.LookRotation(value);
			}
		}


		protected override void UpdateWidth(float width)
		{
			base.UpdateWidth(width);
			square.localScale = new Vector3(width, square.localScale.y, 1f);
		}


		protected override void UpdateRange(float skillRange)
		{
			base.UpdateScale(skillRange);
			square.localScale = new Vector3(square.localScale.x, skillRange, 1f);
		}
	}
}