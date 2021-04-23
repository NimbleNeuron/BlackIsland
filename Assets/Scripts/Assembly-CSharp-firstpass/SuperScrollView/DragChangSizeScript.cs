using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SuperScrollView
{
	public class DragChangSizeScript : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler,
		IDragHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public Camera mCamera;


		public float mBorderSize = 10f;


		public Texture2D mCursorTexture;


		public Vector2 mCursorHotSpot = new Vector2(16f, 16f);


		public bool mIsVertical;


		private RectTransform mCachedRectTransform;


		private bool mIsDragging;


		public Action mOnDragBeginAction;


		public Action mOnDragEndAction;


		public Action mOnDraggingAction;


		public RectTransform CachedRectTransform {
			get
			{
				if (mCachedRectTransform == null)
				{
					mCachedRectTransform = gameObject.GetComponent<RectTransform>();
				}

				return mCachedRectTransform;
			}
		}


		private void LateUpdate()
		{
			if (mCursorTexture == null)
			{
				return;
			}

			if (mIsDragging)
			{
				SetCursor(mCursorTexture, mCursorHotSpot, CursorMode.Auto);
				return;
			}

			Vector2 vector;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(CachedRectTransform, Input.mousePosition,
				mCamera, out vector))
			{
				SetCursor(null, mCursorHotSpot, CursorMode.Auto);
				return;
			}

			if (mIsVertical)
			{
				float num = CachedRectTransform.rect.height + vector.y;
				if (num < 0f)
				{
					SetCursor(null, mCursorHotSpot, CursorMode.Auto);
					return;
				}

				if (num <= mBorderSize)
				{
					SetCursor(mCursorTexture, mCursorHotSpot, CursorMode.Auto);
					return;
				}

				SetCursor(null, mCursorHotSpot, CursorMode.Auto);
			}
			else
			{
				float num2 = CachedRectTransform.rect.width - vector.x;
				if (num2 < 0f)
				{
					SetCursor(null, mCursorHotSpot, CursorMode.Auto);
					return;
				}

				if (num2 <= mBorderSize)
				{
					SetCursor(mCursorTexture, mCursorHotSpot, CursorMode.Auto);
					return;
				}

				SetCursor(null, mCursorHotSpot, CursorMode.Auto);
			}
		}


		public void OnBeginDrag(PointerEventData eventData)
		{
			mIsDragging = true;
			if (mOnDragBeginAction != null)
			{
				mOnDragBeginAction();
			}
		}


		public void OnDrag(PointerEventData eventData)
		{
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(CachedRectTransform, eventData.position, mCamera,
				out vector);
			if (mIsVertical)
			{
				if (vector.y < 0f)
				{
					CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -vector.y);
				}
			}
			else if (vector.x > 0f)
			{
				CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, vector.x);
			}

			if (mOnDraggingAction != null)
			{
				mOnDraggingAction();
			}
		}


		public void OnEndDrag(PointerEventData eventData)
		{
			mIsDragging = false;
			if (mOnDragEndAction != null)
			{
				mOnDragEndAction();
			}
		}


		public void OnPointerEnter(PointerEventData eventData)
		{
			SetCursor(mCursorTexture, mCursorHotSpot, CursorMode.Auto);
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			SetCursor(null, mCursorHotSpot, CursorMode.Auto);
		}


		private void SetCursor(Texture2D texture, Vector2 hotspot, CursorMode cursorMode)
		{
			if (Input.mousePresent)
			{
				Cursor.SetCursor(texture, hotspot, cursorMode);
			}
		}
	}
}