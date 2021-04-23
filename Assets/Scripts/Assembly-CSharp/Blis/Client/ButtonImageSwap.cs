using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Blis.Client
{
	[RequireComponent(typeof(Button))]
	[RequireComponent(typeof(Image))]
	public class ButtonImageSwap : MonoBehaviour
	{
		[SerializeField] private Sprite defaultSprite = default;


		[SerializeField] private Sprite swapSprite = default;


		[SerializeField] private Image image = default;


		public UnityEvent onSwap = new UnityEvent();


		private Button button = default;


		private bool swap = default;

		protected void Awake()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(Swap);
		}


		public void Swap()
		{
			swap = !swap;
			UpdateImage();
			UnityEvent unityEvent = onSwap;
			if (unityEvent == null)
			{
				return;
			}

			unityEvent.Invoke();
		}


		private void UpdateImage()
		{
			if (swap)
			{
				image.sprite = swapSprite;
				return;
			}

			image.sprite = defaultSprite;
		}
	}
}