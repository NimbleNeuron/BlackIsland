using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	[RequireComponent(typeof(Image))]
	public class ColorTweener : Tweener
	{
		public Color from;


		public Color to;


		private Image image;

		protected override void OnInit()
		{
			image = GetComponent<Image>();
			image.color = from;
		}


		protected override void OnRelease()
		{
			if (animationType == AnimationType.Once)
			{
				image.color = to;
				return;
			}

			image.color = from;
		}


		protected override void OnUpdateAnimationValue(float value)
		{
			Color color = Color.Lerp(from, to, value);
			image.color = color;
		}
	}
}