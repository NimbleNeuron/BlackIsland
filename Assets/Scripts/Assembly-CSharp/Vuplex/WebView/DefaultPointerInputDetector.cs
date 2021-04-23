using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Vuplex.WebView
{
	
	[HelpURL("https://developer.vuplex.com/webview/IPointerInputDetector")]
	public class DefaultPointerInputDetector : MonoBehaviour, IPointerInputDetector, IBeginDragHandler, IEventSystemHandler, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IScrollHandler
	{
		
		
		
		public event EventHandler<EventArgs<Vector2>> BeganDrag;

		
		
		
		public event EventHandler<EventArgs<Vector2>> Dragged;

		
		
		
		public event EventHandler<PointerEventArgs> PointerDown;

		
		
		
		public event EventHandler PointerExited;

		
		
		
		public event EventHandler<EventArgs<Vector2>> PointerMoved;

		
		
		
		public event EventHandler<PointerEventArgs> PointerUp;

		
		
		
		public event EventHandler<ScrolledEventArgs> Scrolled;

		
		
		
		public bool PointerMovedEnabled { get; set; }

		
		public void OnBeginDrag(PointerEventData eventData)
		{
			this._raiseBeganDragEvent(this._convertToEventArgs(eventData));
		}

		
		public void OnDrag(PointerEventData eventData)
		{
			if (!this._positionIsZero(eventData))
			{
				this._raiseDraggedEvent(this._convertToEventArgs(eventData));
			}
		}

		
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			this._raisePointerDownEvent(this._convertToPointerEventArgs(eventData));
		}

		
		public void OnPointerEnter(PointerEventData eventData)
		{
			this._isHovering = true;
		}

		
		public void OnPointerExit(PointerEventData eventData)
		{
			this._isHovering = false;
			this._raisePointerExitedEvent(EventArgs.Empty);
		}

		
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			this._raisePointerUpEvent(this._convertToPointerEventArgs(eventData));
		}

		
		public void OnScroll(PointerEventData eventData)
		{
			Vector2 scrollDelta = new Vector2(-eventData.scrollDelta.x, -eventData.scrollDelta.y);
			this._raiseScrolledEvent(new ScrolledEventArgs(scrollDelta, this._convertToNormalizedPoint(eventData)));
		}

		
		private EventArgs<Vector2> _convertToEventArgs(Vector3 worldPosition)
		{
			return new EventArgs<Vector2>(this._convertToNormalizedPoint(worldPosition));
		}

		
		private EventArgs<Vector2> _convertToEventArgs(PointerEventData pointerEventData)
		{
			return new EventArgs<Vector2>(this._convertToNormalizedPoint(pointerEventData));
		}

		
		protected virtual Vector2 _convertToNormalizedPoint(PointerEventData pointerEventData)
		{
			return this._convertToNormalizedPoint(pointerEventData.pointerCurrentRaycast.worldPosition);
		}

		
		protected virtual Vector2 _convertToNormalizedPoint(Vector3 worldPosition)
		{
			Vector3 vector = base.transform.parent.InverseTransformPoint(worldPosition);
			return new Vector2(1f - vector.x, -1f * vector.y);
		}

		
		private PointerEventArgs _convertToPointerEventArgs(PointerEventData eventData)
		{
			return new PointerEventArgs
			{
				Point = this._convertToNormalizedPoint(eventData),
				Button = (MouseButton)eventData.button,
				ClickCount = eventData.clickCount
			};
		}

		
		private PointerEventData _getLastPointerEventData()
		{
			PointerInputModule pointerInputModule = EventSystem.current.currentInputModule as PointerInputModule;
			if (pointerInputModule == null)
			{
				return null;
			}
			object[] array = new object[]
			{
				-1,
				null,
				false
			};
			pointerInputModule.GetType().InvokeMember("GetPointerData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, pointerInputModule, array);
			return array[1] as PointerEventData;
		}

		
		protected virtual bool _positionIsZero(PointerEventData eventData)
		{
			return eventData.pointerCurrentRaycast.worldPosition == Vector3.zero;
		}

		
		protected void _raiseBeganDragEvent(EventArgs<Vector2> eventArgs)
		{
			EventHandler<EventArgs<Vector2>> beganDrag = this.BeganDrag;
			if (beganDrag != null)
			{
				beganDrag(this, eventArgs);
			}
		}

		
		protected void _raiseDraggedEvent(EventArgs<Vector2> eventArgs)
		{
			EventHandler<EventArgs<Vector2>> dragged = this.Dragged;
			if (dragged != null)
			{
				dragged(this, eventArgs);
			}
		}

		
		protected void _raisePointerDownEvent(PointerEventArgs eventArgs)
		{
			EventHandler<PointerEventArgs> pointerDown = this.PointerDown;
			if (pointerDown != null)
			{
				pointerDown(this, eventArgs);
			}
		}

		
		protected void _raisePointerExitedEvent(EventArgs eventArgs)
		{
			EventHandler pointerExited = this.PointerExited;
			if (pointerExited != null)
			{
				pointerExited(this, eventArgs);
			}
		}

		
		private void _raisePointerMovedIfNeeded()
		{
			if (!this.PointerMovedEnabled || !this._isHovering)
			{
				return;
			}
			PointerEventData pointerEventData = this._getLastPointerEventData();
			if (pointerEventData != null)
			{
				this._raisePointerMovedEvent(this._convertToEventArgs(pointerEventData));
			}
		}

		
		protected void _raisePointerMovedEvent(EventArgs<Vector2> eventArgs)
		{
			EventHandler<EventArgs<Vector2>> pointerMoved = this.PointerMoved;
			if (pointerMoved != null)
			{
				pointerMoved(this, eventArgs);
			}
		}

		
		protected void _raisePointerUpEvent(PointerEventArgs eventArgs)
		{
			EventHandler<PointerEventArgs> pointerUp = this.PointerUp;
			if (pointerUp != null)
			{
				pointerUp(this, eventArgs);
			}
		}

		
		protected void _raiseScrolledEvent(ScrolledEventArgs eventArgs)
		{
			EventHandler<ScrolledEventArgs> scrolled = this.Scrolled;
			if (scrolled != null)
			{
				scrolled(this, eventArgs);
			}
		}

		
		private void Update()
		{
			this._raisePointerMovedIfNeeded();
		}

		
		private bool _isHovering;
	}
}
