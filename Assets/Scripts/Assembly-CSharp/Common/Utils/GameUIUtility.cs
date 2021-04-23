using UnityEngine;

namespace Common.Utils
{
	
	public static class GameUIUtility
	{
		
		public static Vector2 ScreenToRectPos(Camera mainCamera, RectTransform canvasRect, RectTransform rectTransform, Vector2 rectPos)
		{
			Vector2 b = mainCamera.ScreenToViewportPoint(rectPos);
			Vector2 v = canvasRect.rect.size * b - canvasRect.rect.size / 2f;
			Vector2 v2 = canvasRect.TransformPoint(v);
			return rectTransform.InverseTransformPoint(v2);
		}

		
		public static Vector2 WorldPosToMapUVSpace(Vector3 pos) => ((Vector2) GameUIUtility.worldToMapMatrix.MultiplyPoint3x4(pos) + GameUIUtility.worldMapRect.size * 0.5f) / GameUIUtility.worldMapRect.size;
		
		// co: dotPeek
		// public static Vector2 WorldPosToMapUVSpace(Vector3 pos)
		// {
		// 	return (GameUIUtility.worldToMapMatrix.MultiplyPoint3x4(pos) + GameUIUtility.worldMapRect.size * 0.5f) / GameUIUtility.worldMapRect.size;
		// }

		
		public static Vector3 MapSpaceUVToWorldPos(Vector2 uv)
		{
			Vector2 v = uv * GameUIUtility.worldMapRect.size - GameUIUtility.worldMapRect.size * 0.5f;
			return GameUIUtility.mapToWorldMatrix.MultiplyPoint3x4(v);
		}

		
		public static Vector3 MapSapceUVToLocalPos(RectTransform rectTransform, Vector2 uv)
		{
			Vector3 result;
			result.x = rectTransform.rect.width * (uv.x - rectTransform.pivot.x);
			result.y = rectTransform.rect.height * (uv.y - rectTransform.pivot.y);
			result.z = 0f;
			return result;
		}

		
		public static Vector2 UvSpaceToLocalPos(Vector2 uvPos, RectTransform rectTransform)
		{
			Vector2 size = rectTransform.rect.size;
			return uvPos * size - size * 0.5f;
		}

		
		public static Rect worldMapRect = new Rect(new Vector2(4f, -61.5f), new Vector2(328f, 377f));

		
		public static Matrix4x4 worldToMapMatrix = new Matrix4x4(new Vector4(-0.70711f, 0f, -0.70711f, -40.65863f), new Vector4(0.70711f, 0f, -0.70711f, -46.31551f), new Vector4(0f, -1f, 0f, 0.59999f), new Vector4(0f, 0f, 0f, 1f)).transpose;

		
		public static Matrix4x4 mapToWorldMatrix = new Matrix4x4(new Vector4(-0.70711f, 0.70711f, 0f, 4f), new Vector4(0f, 0f, -1f, 0.6f), new Vector4(-0.70711f, -0.70711f, 0f, -61.5f), new Vector4(0f, 0f, 0f, 1f)).transpose;
	}
}
