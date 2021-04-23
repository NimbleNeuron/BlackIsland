using UnityEngine;

namespace Blis.Client
{
	[RequireComponent(typeof(Transform))]
	public class ScaleTweener : Tweener
	{
		public Vector3 from;


		public Vector3 to;

		protected override void OnInit() { }


		protected override void OnRelease() { }


		protected override void OnUpdateAnimationValue(float value)
		{
			Vector3 localScale = Vector3.Lerp(from, to, value);
			transform.localScale = localScale;
		}
	}
}