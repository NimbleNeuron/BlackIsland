using UnityEngine;

namespace Blis.Client
{
	[RequireComponent(typeof(Light))]
	public class LightTweener : Tweener
	{
		public Color from;


		public Color to;


		private Light lightComponent;

		protected override void OnInit()
		{
			lightComponent = GetComponent<Light>();
		}


		protected override void OnRelease() { }


		protected override void OnUpdateAnimationValue(float value)
		{
			Color color = Color.Lerp(from, to, value);
			lightComponent.color = color;
		}
	}
}