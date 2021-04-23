using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	[RequireComponent(typeof(Image))]
	public class UIMapSlice : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler,
		IPointerClickHandler, IPointerDownHandler
	{
		[HideInInspector] public int areaCode;


		public Image rollover;


		public IMapSlicePointerEventHandler eventHandlerHandler;


		private Image image;


		private Image BaseImage {
			get
			{
				if (image == null)
				{
					image = GetComponent<Image>();
					image.alphaHitTestMinimumThreshold = 0.1f;
				}

				return image;
			}
		}


		private void OnEnable()
		{
			if (rollover != null)
			{
				rollover.enabled = false;
			}
		}


		public void OnPointerClick(PointerEventData eventData)
		{
			IMapSlicePointerEventHandler mapSlicePointerEventHandler = eventHandlerHandler;
			if (mapSlicePointerEventHandler == null)
			{
				return;
			}

			mapSlicePointerEventHandler.OnPointerMapClick(areaCode, eventData.button);
		}


		public void OnPointerDown(PointerEventData eventData) { }


		public void OnPointerEnter(PointerEventData eventData)
		{
			IMapSlicePointerEventHandler mapSlicePointerEventHandler = eventHandlerHandler;
			if (mapSlicePointerEventHandler == null)
			{
				return;
			}

			mapSlicePointerEventHandler.OnPointerMapEnter(areaCode);
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			IMapSlicePointerEventHandler mapSlicePointerEventHandler = eventHandlerHandler;
			if (mapSlicePointerEventHandler == null)
			{
				return;
			}

			mapSlicePointerEventHandler.OnPointerMapExit(areaCode);
		}


		public void SetEventHandler(IMapSlicePointerEventHandler handler)
		{
			eventHandlerHandler = handler;
		}


		public void SetMapState(bool enable, Color color)
		{
			if (enable)
			{
				BaseImage.color = color;
				return;
			}

			BaseImage.color = Color.clear;
		}


		public void SetRollOver(bool enable)
		{
			if (rollover != null)
			{
				rollover.enabled = enable;
			}
		}
	}
}