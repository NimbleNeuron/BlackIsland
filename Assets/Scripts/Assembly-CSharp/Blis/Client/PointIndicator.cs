using UnityEngine;

namespace Blis.Client
{
	public class PointIndicator : SpellIndicator
	{
		[SerializeField] private Splat inner = default;


		[SerializeField] private bool restrictCursorToRange = default;


		protected Vector3? pointCompensationValue;


		public Splat Inner => inner;


		
		public bool RestrictCursorToRange {
			get => restrictCursorToRange;
			set => restrictCursorToRange = value;
		}


		
		public Vector3? PointCompensationValue {
			set => pointCompensationValue = value;
		}


		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (Manager != null)
			{
				inner.transform.position = Manager.Get3DMousePosition();
				if (restrictCursorToRange &&
				    Vector3.Distance(Manager.transform.position, inner.transform.position) > range)
				{
					inner.transform.position = Manager.transform.position +
					                           Vector3.ClampMagnitude(
						                           inner.transform.position - Manager.transform.position, range);
				}
			}

			if (pointCompensationValue != null)
			{
				inner.transform.position += pointCompensationValue.Value;
			}
		}


		protected override void UpdateProgress(float progress)
		{
			if (inner != null)
			{
				inner.Progress = progress;
			}
		}


		protected override void UpdateScale(float scale)
		{
			if (inner != null)
			{
				inner.Range = scale;
			}
		}


		protected override void UpdateWidth(float width)
		{
			if (inner != null)
			{
				inner.Width = width;
			}
		}


		public override void Show()
		{
			pointCompensationValue = null;
			base.Show();
		}
	}
}