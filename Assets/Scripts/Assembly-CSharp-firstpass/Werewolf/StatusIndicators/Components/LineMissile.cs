using UnityEngine;

namespace Werewolf.StatusIndicators.Components
{
	public class LineMissile : SpellIndicator
	{
		public GameObject ArrowHead;


		public float MinimumRange;


		private Projector arrowHeadProjector;


		private float arrowHeadScale;


		public override ScalingType Scaling => ScalingType.LengthOnly;


		public override void Update()
		{
			if (Manager != null)
			{
				Vector3 vector = FlattenVector(Manager.Get3DMousePosition()) - Manager.transform.position;
				if (vector != Vector3.zero)
				{
					Manager.transform.rotation = Quaternion.LookRotation(vector);
				}

				Scale = Mathf.Clamp((Manager.Get3DMousePosition() - Manager.transform.position).magnitude, MinimumRange,
					Range - ArrowHeadDistance()) * 2f;
				ArrowHead.transform.localPosition = new Vector3(0f, Scale * 0.5f + ArrowHeadDistance() - 0.12f, 0f);
			}
		}


		public override void Initialize()
		{
			base.Initialize();
			arrowHeadProjector = ArrowHead.GetComponent<Projector>();
			arrowHeadScale = arrowHeadProjector.orthographicSize;
		}


		public override void OnValueChanged()
		{
			base.OnValueChanged();
			arrowHeadProjector.aspectRatio = 1f;
			arrowHeadProjector.orthographicSize = arrowHeadScale;
		}


		private float ArrowHeadDistance()
		{
			return arrowHeadProjector.orthographicSize * 0.96f;
		}
	}
}