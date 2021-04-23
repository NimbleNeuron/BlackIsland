using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class ClickerModule : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IBeginDragHandler,
		IEndDragHandler, IDragHandler
	{
		private Action<PointerEventData> beginDrag;


		private Action<PointerEventData> click;


		private Action<PointerEventData> dragging;


		private Action<PointerEventData> endDrag;


		public void OnBeginDrag(PointerEventData eventData)
		{
			Action<PointerEventData> action = beginDrag;
			if (action == null)
			{
				return;
			}

			action(eventData);
		}


		public void OnDrag(PointerEventData eventData)
		{
			Action<PointerEventData> action = dragging;
			if (action == null)
			{
				return;
			}

			action(eventData);
		}


		public void OnEndDrag(PointerEventData eventData)
		{
			Action<PointerEventData> action = endDrag;
			if (action == null)
			{
				return;
			}

			action(eventData);
		}


		public void OnPointerClick(PointerEventData eventData)
		{
			Action<PointerEventData> action = click;
			if (action == null)
			{
				return;
			}

			action(eventData);
		}

		public void Init(Action<PointerEventData> click, Action<PointerEventData> beginDrag,
			Action<PointerEventData> endDrag, Action<PointerEventData> dragging)
		{
			this.click = click;
			this.beginDrag = beginDrag;
			this.endDrag = endDrag;
			this.dragging = dragging;
		}
	}
}