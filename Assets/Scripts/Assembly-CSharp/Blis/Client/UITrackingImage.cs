using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UITrackingImage : BaseTrackUI
	{
		[SerializeField] private Image image = default;

		public void SetSize(Vector2 size)
		{
			rectTransform.sizeDelta = size;
		}


		public void SetSprite(Sprite sprite)
		{
			image.sprite = sprite;
		}


		public void SetColor(Color color)
		{
			image.color = color;
		}
	}
}