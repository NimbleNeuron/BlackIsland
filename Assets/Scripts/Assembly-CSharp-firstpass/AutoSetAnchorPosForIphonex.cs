using UnityEngine;
using UnityEngine.UI;

public class AutoSetAnchorPosForIphonex : MonoBehaviour
{
	public Canvas mCanvas;


	private void Awake()
	{
		if (Screen.width == 1125 && Screen.height == 2436)
		{
			float y = mCanvas.GetComponent<CanvasScaler>().referenceResolution.y;
			float num = 0.054187194f * y;
			RectTransform component = GetComponent<RectTransform>();
			component.offsetMin = new Vector2(0f, num);
			component.offsetMax = new Vector2(0f, -num);
			return;
		}

		if (Screen.height == 1125 && Screen.width == 2436)
		{
			float y2 = mCanvas.GetComponent<CanvasScaler>().referenceResolution.y;
			float y3 = 0.056f * y2;
			float num2 = y2 / 1125f * 132f;
			RectTransform component2 = GetComponent<RectTransform>();
			component2.offsetMin = new Vector2(num2, y3);
			component2.offsetMax = new Vector2(-num2, 0f);
		}
	}
}