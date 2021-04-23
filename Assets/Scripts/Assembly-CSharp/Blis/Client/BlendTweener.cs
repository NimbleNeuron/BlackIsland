using UiEffect;
using UnityEngine;

namespace Blis.Client
{
	[RequireComponent(typeof(BlendColor))]
	public class BlendTweener : Tweener
	{
		public Color from;


		public Color to;


		private BlendColor blendColor;

		private void Awake()
		{
			blendColor = GetComponent<BlendColor>();
		}


		protected override void OnInit()
		{
			if (blendColor != null)
			{
				blendColor.enabled = true;
			}
		}


		protected override void OnRelease()
		{
			if (blendColor != null)
			{
				blendColor.enabled = false;
			}
		}


		protected override void OnUpdateAnimationValue(float value)
		{
			blendColor.color = Color.Lerp(from, to, value);
		}
	}
}