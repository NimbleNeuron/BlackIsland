using UnityEngine;

namespace TheraBytes.BetterUi
{
	public static class RectTransformExtensions
	{
		public static Rect ToScreenRect(this RectTransform self, bool startAtBottom = false, Canvas canvas = null,
			bool localTransform = false)
		{
			Vector3[] array = new Vector3[4];
			Vector3[] array2 = new Vector3[2];
			if (localTransform)
			{
				self.GetLocalCorners(array);
			}
			else
			{
				self.GetWorldCorners(array);
			}

			int num = startAtBottom ? 0 : 1;
			int num2 = startAtBottom ? 2 : 3;
			if (canvas != null && (canvas.renderMode == RenderMode.ScreenSpaceCamera ||
			                       canvas.renderMode == RenderMode.WorldSpace))
			{
				array2[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, array[num]);
				array2[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, array[num2]);
			}
			else
			{
				array2[0] = RectTransformUtility.WorldToScreenPoint(null, array[num]);
				array2[1] = RectTransformUtility.WorldToScreenPoint(null, array[num2]);
			}

			if (!startAtBottom)
			{
				array2[0].y = Screen.height - array2[0].y;
				array2[1].y = Screen.height - array2[1].y;
			}

			return new Rect(array2[0], array2[1] - array2[0]);
		}
	}
}