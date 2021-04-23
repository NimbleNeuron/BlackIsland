using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class URLLinker : UIBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler,
		IPointerExitHandler
	{
		public Image imgBG;


		public bool localization;


		public string url;

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!string.IsNullOrEmpty(url))
			{
				if (localization)
				{
					Application.OpenURL(Ln.Get(url));
					return;
				}

				Application.OpenURL(url);
			}
		}


		public void OnPointerEnter(PointerEventData eventData)
		{
			if (imgBG != null)
			{
				imgBG.color = new Color(0.75f, 0.75f, 0.75f, 1f);
			}
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			if (imgBG != null)
			{
				imgBG.color = new Color(1f, 1f, 1f, 1f);
			}
		}
	}
}