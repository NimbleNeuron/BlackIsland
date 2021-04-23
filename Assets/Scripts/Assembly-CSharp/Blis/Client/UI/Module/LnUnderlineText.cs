using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client.UI.Module
{
	public class LnUnderlineText : LnText
	{
		[Tooltip("Must be a positive integer")]
		public int underlineStart;


		[SerializeField] private bool Underline;


		private new CanvasRenderer canvasRenderer;


		private bool currentUnderlineState;


		private GameObject lineGameObject;


		private Image lineImage;


		private RectTransform lineRectTransform;


		private TextGenerator textGenerator;


		private RectTransform textRectTransform;


		private int underlineEnd;


		private void Update()
		{
			if (currentUnderlineState != Underline)
			{
				if (!Underline)
				{
					getUnderlineObject();
					DestroyImmediate(lineGameObject);
				}

				currentUnderlineState = Underline;
			}

			if (Underline && underlineStart >= 0)
			{
				if (lineGameObject == null || textGenerator == null || lineImage == null || lineRectTransform == null ||
				    textRectTransform == null)
				{
					getUnderlineObject();
					if (lineGameObject == null)
					{
						addUnderline();
					}
				}

				updateUnderline();
			}
		}

		public void SetUnderline(bool underline)
		{
			Underline = underline;
		}


		public void updateUnderline()
		{
			if (Underline && lineGameObject != null)
			{
				if (underlineEnd != text.Length)
				{
					underlineEnd = text.Length;
				}

				lineImage.color = canvasRenderer.GetColor();
				if (textGenerator.characterCount < 0)
				{
					return;
				}

				UICharInfo[] charactersArray = textGenerator.GetCharactersArray();
				if (underlineEnd <= underlineStart || underlineEnd >= charactersArray.Length)
				{
					return;
				}

				UILineInfo[] linesArray = textGenerator.GetLinesArray();
				if (linesArray.Length < 1)
				{
					return;
				}

				float num = linesArray[0].height;
				Canvas componentInParent = gameObject.GetComponentInParent<Canvas>();
				float num2 = 1f / componentInParent.scaleFactor;
				lineRectTransform.anchoredPosition =
					new Vector2(
						num2 * (charactersArray[underlineStart].cursorPos.x +
						        charactersArray[underlineEnd].cursorPos.x) / 2f,
						num2 * (charactersArray[underlineStart].cursorPos.y - num / 1f));
				lineRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
					num2 * Mathf.Abs(charactersArray[underlineStart].cursorPos.x -
					                 charactersArray[underlineEnd].cursorPos.x));
				lineRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num / 10f);
			}
		}


		private void getUnderlineObject()
		{
			foreach (object obj in this.transform)
			{
				Transform transform = (Transform) obj;
				if (transform.name == "Underline")
				{
					textRectTransform = gameObject.GetComponent<RectTransform>();
					textGenerator = cachedTextGenerator;
					lineGameObject = transform.gameObject;
					lineImage = lineGameObject.GetComponent<Image>();
					canvasRenderer = GetComponent<CanvasRenderer>();
					lineImage.color = canvasRenderer.GetColor();
					lineRectTransform = lineGameObject.GetComponent<RectTransform>();
					lineRectTransform.SetParent(this.transform, false);
					lineRectTransform.anchorMin = textRectTransform.pivot;
					lineRectTransform.anchorMax = textRectTransform.pivot;
				}
			}
		}


		private void addUnderline()
		{
			textRectTransform = gameObject.GetComponent<RectTransform>();
			textGenerator = cachedTextGenerator;
			lineGameObject = new GameObject("Underline");
			lineImage = lineGameObject.AddComponent<Image>();
			canvasRenderer = GetComponent<CanvasRenderer>();
			lineImage.color = canvasRenderer.GetColor();
			lineRectTransform = lineGameObject.GetComponent<RectTransform>();
			lineRectTransform.SetParent(transform, false);
			lineRectTransform.anchorMin = textRectTransform.pivot;
			lineRectTransform.anchorMax = textRectTransform.pivot;
		}
	}
}