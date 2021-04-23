using UnityEngine;
using UnityEngine.EventSystems;

namespace Vuplex.WebView
{
	
	[HelpURL("https://developer.vuplex.com/webview/IPointerInputDetector")]
	public class CanvasPointerInputDetector : DefaultPointerInputDetector
	{
		
		protected override Vector2 _convertToNormalizedPoint(PointerEventData pointerEventData)
		{
			Canvas canvas = this._getCanvas();
			Camera cam = (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this._getRectTransform(), pointerEventData.position, cam, out localPoint);
			return this._convertToNormalizedPoint(localPoint);
		}

		
		protected override Vector2 _convertToNormalizedPoint(Vector3 worldPosition)
		{
			Vector3 v = this._getRectTransform().InverseTransformPoint(worldPosition);
			return this._convertToNormalizedPoint(v);
		}

		
		private Vector2 _convertToNormalizedPoint(Vector2 localPoint)
		{
			Vector2 vector = Rect.PointToNormalized(this._getRectTransform().rect, localPoint);
			vector.y = 1f - vector.y;
			return vector;
		}

		
		private Canvas _getCanvas()
		{
			if (this._cachedCanvas == null)
			{
				this._cachedCanvas = base.GetComponentInParent<Canvas>();
			}
			return this._cachedCanvas;
		}

		
		private RectTransform _getRectTransform()
		{
			if (this._cachedRectTransform == null)
			{
				this._cachedRectTransform = base.GetComponent<RectTransform>();
			}
			return this._cachedRectTransform;
		}

		
		protected override bool _positionIsZero(PointerEventData eventData)
		{
			return eventData.position == Vector2.zero;
		}

		
		private Canvas _cachedCanvas;

		
		private RectTransform _cachedRectTransform;
	}
}
