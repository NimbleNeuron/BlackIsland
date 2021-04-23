using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class MatchingNotice : BaseUI, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler,
		IPointerDownHandler
	{
		private Color defaultColor;


		private LnText text;


		public void OnPointerDown(PointerEventData eventData)
		{
			Application.OpenURL(Ln.Get("CBT스케줄링크"));
		}


		public void OnPointerEnter(PointerEventData eventData)
		{
			text.color = Color.white;
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			text.color = defaultColor;
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			text = GameUtil.Bind<LnText>(gameObject, "Text");
			defaultColor = text.color;
			gameObject.SetActive(false);
		}
	}
}