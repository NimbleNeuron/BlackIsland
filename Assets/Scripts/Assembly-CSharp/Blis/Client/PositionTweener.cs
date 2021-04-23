using UnityEngine;

namespace Blis.Client
{
	public class PositionTweener : Tweener
	{
		public Vector3 from;


		public Vector3 to;

		protected override void OnInit() { }


		protected override void OnRelease() { }


		protected override void OnUpdateAnimationValue(float value)
		{
			Vector3 vector = Vector3.Lerp(from, to, value);
			if (transform is RectTransform)
			{
				((RectTransform) transform).anchoredPosition = vector;
				return;
			}

			transform.localPosition = vector;
		}
	}
}