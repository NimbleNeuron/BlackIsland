using Blis.Common;
using UnityEngine;
using Werewolf.StatusIndicators.Services;

namespace Blis.Client
{
	public class SectorIndicator : SpellIndicator
	{
		[SerializeField] private IndicatorPart Base = default;


		[SerializeField] private IndicatorPart LBorder = default;


		[SerializeField] private IndicatorPart RBorder = default;

		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (fixedDirection != null)
			{
				Vector3 direction = FlattenVector(fixedDirection.Value);
				transform.rotation = GameUtil.LookRotation(direction);
				return;
			}

			if (Manager != null)
			{
				Vector3 vector = FlattenVector(Manager.Get3DMousePosition() - Manager.transform.position);
				if (vector != Vector3.zero)
				{
					transform.rotation = GameUtil.LookRotation(vector);
				}
			}
		}


		protected override void UpdateRange(float skillRange)
		{
			base.UpdateRange(skillRange);
			transform.localScale = Vector3.one * skillRange;
		}


		protected override void UpdateAngle(float angle)
		{
			float value = Normalize.GetValue(angle - 1f, 360f);
			Base.SetShaderFloat("_Expand", value);
			LBorder.SetShaderFloat("_Expand", value);
			RBorder.SetShaderFloat("_Expand", value);
			LBorder.transform.localEulerAngles = new Vector3(0f, 0f, angle * 0.5f);
			RBorder.transform.localEulerAngles = new Vector3(0f, 0f, -angle * 0.5f);
		}
	}
}