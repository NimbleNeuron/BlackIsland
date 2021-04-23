using UnityEngine;

namespace Blis.Client
{
	public class LineIndicatorCustomTarget : LineIndicator
	{
		[SerializeField] private RangeIndicator inner = default;


		private Transform customLineTarget = default;


		private Transform innerRange = default;

		public override void Awake()
		{
			base.Awake();
			if (inner != null)
			{
				innerRange = inner.transform;
			}
		}


		protected override void LateUpdate()
		{
			if (customLineTarget != null)
			{
				lineAnchor.position = customLineTarget.position;
			}

			base.LateUpdate();
			if (innerRange != null)
			{
				innerRange.position = Manager.Get3DMousePosition();
			}
		}


		protected override Vector3 GetMousePosition()
		{
			return Manager.Get3DMousePosition();
		}


		public void SetCustomLineTarget(Transform customTarget)
		{
			customLineTarget = customTarget;
			lineAnchor.position = customTarget.position;
		}


		protected override void UpdateScale(float scale)
		{
			base.UpdateScale(scale);
			if (inner != null)
			{
				inner.Range = scale;
			}
		}
	}
}