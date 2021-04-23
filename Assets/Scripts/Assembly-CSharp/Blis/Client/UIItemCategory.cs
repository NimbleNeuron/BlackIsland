using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIItemCategory : BaseControl
	{
		public const float offset = 2f;


		public Color common;


		public Color focus;


		[SerializeField] private Text text = default;


		[SerializeField] private RectTransform select = default;


		[SerializeField] private RectTransform hover = default;


		private Vector2 baseOffset;


		private bool isPin;


		public Text Text => text;


		public bool IsPin => isPin;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			SetPin(false);
			baseOffset = text.rectTransform.offsetMin;
		}


		public void SetPin(bool pin)
		{
			if (pin == isPin)
			{
				return;
			}

			isPin = pin;
			if (pin)
			{
				text.rectTransform.offsetMin = baseOffset;
				hover.gameObject.SetActive(false);
				select.gameObject.SetActive(true);
				return;
			}

			text.rectTransform.offsetMin = baseOffset;
			hover.gameObject.SetActive(false);
			select.gameObject.SetActive(false);
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			text.rectTransform.offsetMin = new Vector2(baseOffset.x + 2f, baseOffset.y);
			hover.gameObject.SetActive(true);
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			text.rectTransform.offsetMin = baseOffset;
			hover.gameObject.SetActive(false);
		}
	}
}