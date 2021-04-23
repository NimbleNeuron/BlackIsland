using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	[RequireComponent(typeof(Image))]
	public class UIBackShade : BaseControl
	{
		private CanvasScaler scaler;


		private Image shade;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			shade = GetComponent<Image>();
			scaler = gameObject.GetComponentInParent<CanvasScaler>();
		}


		public void Resolution()
		{
			if (shade != null)
			{
				RectTransform rectTransform = (RectTransform) scaler.transform;
				shade.rectTransform.sizeDelta =
					new Vector2(rectTransform.rect.width * 10f, rectTransform.rect.height * 10f);
				this.rectTransform.anchoredPosition = Vector2.zero;
			}
		}
	}
}