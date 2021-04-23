using UnityEngine;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Controls/Better Scroll Rect", 30)]
	public class BetterScrollRect : ScrollRect
	{
		[SerializeField] [Range(0f, 1f)] private float horizontalStartPosition;


		[SerializeField] [Range(0f, 1f)] private float verticalStartPosition = 1f;


		
		public Vector2 DragStartPosition {
			get => m_ContentStartPosition;
			set => m_ContentStartPosition = value;
		}


		
		public Bounds ContentBounds {
			get => m_ContentBounds;
			set => m_ContentBounds = value;
		}


		
		public float HorizontalStartPosition {
			get => horizontalStartPosition;
			set => horizontalStartPosition = value;
		}


		
		public float VerticalStartPosition {
			get => verticalStartPosition;
			set => verticalStartPosition = value;
		}


		protected override void Start()
		{
			base.Start();
			if (Application.isPlaying)
			{
				ResetToStartPosition();
			}
		}


		public void ResetToStartPosition()
		{
			if (horizontalScrollbar != null)
			{
				horizontalScrollbar.value = horizontalStartPosition;
			}
			else if (horizontal)
			{
				horizontalNormalizedPosition = horizontalStartPosition;
			}

			if (verticalScrollbar != null)
			{
				verticalScrollbar.value = verticalStartPosition;
				return;
			}

			if (vertical)
			{
				verticalNormalizedPosition = verticalStartPosition;
			}
		}
	}
}