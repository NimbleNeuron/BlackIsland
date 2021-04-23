using UnityEngine;

namespace Werewolf.StatusIndicators.Components
{
	public class Point : SpellIndicator
	{
		[SerializeField] protected bool restrictCursorToRange;


		public override ScalingType Scaling => ScalingType.LengthAndHeight;


		public override void Update()
		{
			transform.position = Manager.Get3DMousePosition();
			if (restrictCursorToRange)
			{
				RestrictCursorToRange();
			}
		}


		private void LateUpdate()
		{
			transform.eulerAngles = new Vector3(90f, 0f, 0f);
		}


		private void RestrictCursorToRange()
		{
			if (Manager != null && Vector3.Distance(Manager.transform.position, transform.position) > range)
			{
				transform.position = Manager.transform.position +
				                     Vector3.ClampMagnitude(transform.position - Manager.transform.position, range);
			}
		}
	}
}