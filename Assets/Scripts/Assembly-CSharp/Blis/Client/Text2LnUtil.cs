using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	[ExecuteInEditMode]
	public class Text2LnUtil : MonoBehaviour
	{
		private void Awake()
		{
			Text component = GetComponent<Text>();
			if (component == null)
			{
				return;
			}

			TextProperty textProperty = new TextProperty
			{
				font = component.font,
				str = component.text,
				style = component.fontStyle,
				fontSize = component.fontSize,
				richText = component.supportRichText,
				textAnchor = component.alignment,
				alignByGeometry = component.alignByGeometry,
				verticalWrapMode = component.verticalOverflow,
				horizontalWrapMode = component.horizontalOverflow,
				bestFit = component.resizeTextForBestFit,
				minSize = component.resizeTextMinSize,
				maxSize = component.resizeTextMaxSize,
				color = component.color,
				raycast = component.raycastTarget
			};
			DestroyImmediate(component);
			LnText text = gameObject.AddComponent<LnText>();
			textProperty.Apply(text);
			DestroyImmediate(this);
		}


		private struct TextProperty
		{
			public void Apply(Text text)
			{
				text.font = font;
				text.text = str;
				text.fontStyle = style;
				text.fontSize = fontSize;
				text.supportRichText = richText;
				text.alignment = textAnchor;
				text.alignByGeometry = alignByGeometry;
				text.verticalOverflow = verticalWrapMode;
				text.horizontalOverflow = horizontalWrapMode;
				text.resizeTextForBestFit = bestFit;
				text.resizeTextMinSize = minSize;
				text.resizeTextMaxSize = maxSize;
				text.color = color;
				text.raycastTarget = raycast;
			}


			public Font font;


			public string str;


			public FontStyle style;


			public int fontSize;


			public bool richText;


			public TextAnchor textAnchor;


			public bool alignByGeometry;


			public VerticalWrapMode verticalWrapMode;


			public HorizontalWrapMode horizontalWrapMode;


			public bool bestFit;


			public int minSize;


			public int maxSize;


			public Color color;


			public bool raycast;
		}
	}
}