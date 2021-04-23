using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LoadingView : BaseUI
	{
		[SerializeField] private Image cursor = default;


		[SerializeField] private UIProgress uiProgress = default;


		[SerializeField] private Text text = default;


		private Coroutine fakeAnimation;


		private RectTransform progressRect;


		
		public static LoadingView inst { get; set; }


		
		public LoadingContext LoadingContext { get; set; }


		protected override void Awake()
		{
			base.Awake();
			progressRect = uiProgress.GetComponent<RectTransform>();
			SetActive(false);
		}


		public void SetActive(bool isActive)
		{
			gameObject.SetActive(isActive);
		}


		private void SetProgress(float value)
		{
			uiProgress.SetValue(value);
			if (cursor != null)
			{
				cursor.rectTransform.anchoredPosition = new Vector2(value * progressRect.rect.width, 0f);
			}
		}


		public void UpdateLoading(string str, float value)
		{
			text.text = str;
			SetProgress(value);
		}
	}
}