using System.Collections.Generic;
using UnityEngine;

namespace Blis.Client
{
	public class CrossIndicator : SpellIndicator
	{
		[SerializeField] protected List<IndicatorPart> LineHeaders = new List<IndicatorPart>();


		[SerializeField] protected List<IndicatorPart> LineBodys = new List<IndicatorPart>();


		[SerializeField] protected Vector3 direction;


		private Quaternion rotatieAngle;


		public override void Awake()
		{
			base.Awake();
			rotatieAngle = Quaternion.LookRotation(direction);
			transform.rotation = rotatieAngle;
		}


		protected override void LateUpdate()
		{
			base.LateUpdate();
			transform.rotation = rotatieAngle;
		}

		protected override void UpdateRange(float skillRange)
		{
			Vector3 localPosition = new Vector3(0f, skillRange - 1f + 0.5f, 0f);
			for (int i = 0; i < LineHeaders.Count; i++)
			{
				LineHeaders[i].transform.localPosition = localPosition;
			}
		}


		protected override void UpdateWidth(float width)
		{
			base.UpdateWidth(width);
			Vector3 localScale = new Vector3(width, (range - 1f) * 2f, 1f);
			for (int i = 0; i < LineBodys.Count; i++)
			{
				LineBodys[i].transform.localScale = localScale;
			}
		}


		protected override void UpdateScale(float scale)
		{
			base.UpdateRange(scale);
		}


		public void ResetRotation(Vector3 direction)
		{
			this.direction = direction;
			rotatieAngle = Quaternion.LookRotation(this.direction);
		}
	}
}