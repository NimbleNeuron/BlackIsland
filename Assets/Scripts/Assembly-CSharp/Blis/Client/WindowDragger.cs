using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	[RequireComponent(typeof(Image))]
	public class WindowDragger : BaseControl
	{
		public override void OnDrag(PointerEventData eventData)
		{
			base.OnDrag(eventData);
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				BaseWindow parentWindow = GetParentWindow();
				if (parentWindow != null)
				{
					parentWindow.transform.Translate(eventData.delta);
				}
			}
		}


		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				BaseWindow parentWindow = GetParentWindow();
				RectTransform rectTransform = (RectTransform) parentWindow.transform;
				Vector3[] array = new Vector3[4];
				rectTransform.GetWorldCorners(array);
				Vector2 size = rectTransform.rect.size;
				Vector2 zero = Vector2.zero;
				if (array[1].x + size.x < GameConstants.SCREEN_DRAG_MARGIN.x)
				{
					zero.x = GameConstants.SCREEN_DRAG_MARGIN.x - (array[1].x + size.x);
				}
				else if (Screen.width - array[3].x + size.x < GameConstants.SCREEN_DRAG_MARGIN.x)
				{
					zero.x = Screen.width - array[3].x + size.x - GameConstants.SCREEN_DRAG_MARGIN.x;
				}

				if (array[3].y + size.y < GameConstants.SCREEN_DRAG_MARGIN.y)
				{
					zero.y = GameConstants.SCREEN_DRAG_MARGIN.y - (array[3].y + size.y);
				}
				else if (Screen.height - array[1].y < 0f)
				{
					zero.y = Screen.height - array[1].y;
				}

				parentWindow.transform.Translate(zero);
			}
		}
	}
}