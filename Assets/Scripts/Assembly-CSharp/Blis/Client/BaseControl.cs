using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class BaseControl : BaseUI, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler,
		IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler,
		IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler,
		ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler
	{
		public delegate void PointerEvent(BaseControl baseControl, PointerEventData eventData);


		private BaseWindow _cachedParentWindow;


		private bool _isParentCached;


		private bool isHover;


		protected bool IsHover => isHover;


		protected override void OnDisable()
		{
			base.OnDisable();
			if (isHover)
			{
				OnPointerExit(new PointerEventData(EventSystem.current));
			}
		}


		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			_cachedParentWindow = null;
			_isParentCached = false;
		}


		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			OnFocused();
			PointerEvent onBeginDragEvent = OnBeginDragEvent;
			if (onBeginDragEvent == null)
			{
				return;
			}

			onBeginDragEvent(this, eventData);
		}


		public virtual void OnCancel(BaseEventData eventData) { }


		public virtual void OnDeselect(BaseEventData eventData) { }


		public virtual void OnDrag(PointerEventData eventData)
		{
			PointerEvent onDragEvent = OnDragEvent;
			if (onDragEvent == null)
			{
				return;
			}

			onDragEvent(this, eventData);
		}


		public virtual void OnDrop(PointerEventData eventData)
		{
			OnFocused();
		}


		public virtual void OnEndDrag(PointerEventData eventData)
		{
			PointerEvent onEndDragEvent = OnEndDragEvent;
			if (onEndDragEvent == null)
			{
				return;
			}

			onEndDragEvent(this, eventData);
		}


		public virtual void OnInitializePotentialDrag(PointerEventData eventData) { }


		public virtual void OnMove(AxisEventData eventData) { }


		public virtual void OnPointerClick(PointerEventData eventData)
		{
			OnFocused();
			PointerEvent onPointerClickEvent = OnPointerClickEvent;
			if (onPointerClickEvent == null)
			{
				return;
			}

			onPointerClickEvent(this, eventData);
		}


		public virtual void OnPointerDown(PointerEventData eventData)
		{
			PointerEvent onPointerDownEvent = OnPointerDownEvent;
			if (onPointerDownEvent == null)
			{
				return;
			}

			onPointerDownEvent(this, eventData);
		}


		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			isHover = true;
			PointerEvent onPointerEnterEvent = OnPointerEnterEvent;
			if (onPointerEnterEvent == null)
			{
				return;
			}

			onPointerEnterEvent(this, eventData);
		}


		public virtual void OnPointerExit(PointerEventData eventData)
		{
			isHover = false;
			PointerEvent onPointerExitEvent = OnPointerExitEvent;
			if (onPointerExitEvent == null)
			{
				return;
			}

			onPointerExitEvent(this, eventData);
		}


		public virtual void OnPointerUp(PointerEventData eventData)
		{
			PointerEvent onPointerUpEvent = OnPointerUpEvent;
			if (onPointerUpEvent == null)
			{
				return;
			}

			onPointerUpEvent(this, eventData);
		}


		public virtual void OnScroll(PointerEventData eventData)
		{
			PointerEvent onScrollEvent = OnScrollEvent;
			if (onScrollEvent == null)
			{
				return;
			}

			onScrollEvent(this, eventData);
		}


		public virtual void OnSelect(BaseEventData eventData)
		{
			OnFocused();
		}


		public virtual void OnSubmit(BaseEventData eventData)
		{
			OnFocused();
		}


		public virtual void OnUpdateSelected(BaseEventData eventData) { }


		
		
		public event PointerEvent OnPointerEnterEvent;


		
		
		public event PointerEvent OnPointerExitEvent;


		
		
		public event PointerEvent OnPointerDownEvent;


		
		
		public event PointerEvent OnPointerUpEvent;


		
		
		public event PointerEvent OnPointerClickEvent;


		
		
		public event PointerEvent OnBeginDragEvent;


		
		
		public event PointerEvent OnEndDragEvent;


		
		
		public event PointerEvent OnDragEvent;


		
		
		public event PointerEvent OnScrollEvent;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			_isParentCached = false;
			_cachedParentWindow = null;
		}


		public BaseWindow GetParentWindow()
		{
			if (_isParentCached)
			{
				return _cachedParentWindow;
			}

			_cachedParentWindow = GetComponentInParent<BaseWindow>();
			_isParentCached = true;
			return _cachedParentWindow;
		}


		protected virtual void OnFocused()
		{
			BaseWindow parentWindow = GetParentWindow();
			if (parentWindow == null)
			{
				return;
			}

			parentWindow.OnFocused();
		}
	}
}