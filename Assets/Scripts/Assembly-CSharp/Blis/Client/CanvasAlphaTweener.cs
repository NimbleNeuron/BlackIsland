using UnityEngine;

namespace Blis.Client
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasAlphaTweener : Tweener
	{
		public float from;


		public float to;


		private CanvasGroup canvasGroup;

		protected override void OnInit()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = from;
		}


		protected override void OnRelease()
		{
			if (animationType == AnimationType.Once)
			{
				canvasGroup.alpha = to;
			}
		}


		protected override void OnUpdateAnimationValue(float value)
		{
			float alpha = Mathf.Lerp(from, to, value);
			canvasGroup.alpha = alpha;
		}
	}
}