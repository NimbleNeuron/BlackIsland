using UnityEngine;

namespace Blis.Client
{
	public class RotationTweener : Tweener
	{
		public Vector3 from;


		public Vector3 to;

		protected override void OnInit() { }


		protected override void OnRelease() { }


		protected override void OnUpdateAnimationValue(float value)
		{
			Vector3 euler = Vector3.Lerp(from, to, value);
			transform.localRotation = Quaternion.Euler(euler);
		}
	}
}