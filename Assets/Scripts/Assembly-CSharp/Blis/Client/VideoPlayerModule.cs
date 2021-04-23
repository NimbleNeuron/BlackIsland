using System;
using Blis.Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class VideoPlayerModule : MonoBehaviour
	{
		[SerializeField] private VideoModule youtubePlayerModule = default;


		[SerializeField] private VideoModule webViewPlayerModule = default;


		[SerializeField] private CanvasGroup btnCanvasGroup = default;


		[SerializeField] private Button btnPlay = default;


		[SerializeField] private Button btnPause = default;


		[SerializeField] private Button btnRePlay = default;

		private void Awake()
		{
			btnPlay.onClick.AddListener(delegate
			{
				GetTarget().OnClick();
				if (Singleton<GameEventLogger>.inst.IsNotChina())
				{
					Singleton<SoundControl>.inst.SetVideoModeMute(true);
				}
			});
			btnPause.onClick.AddListener(delegate
			{
				GetTarget().OnClick();
				if (Singleton<GameEventLogger>.inst.IsNotChina())
				{
					Singleton<SoundControl>.inst.SetVideoModeMute(false);
				}
			});
			btnRePlay.onClick.AddListener(delegate
			{
				GetTarget().OnClick();
				if (Singleton<GameEventLogger>.inst.IsNotChina())
				{
					Singleton<SoundControl>.inst.SetVideoModeMute(true);
				}
			});
			btnCanvasGroup.alpha = 0f;
		}


		private void Update()
		{
			if (Singleton<GameEventLogger>.inst.IsNotChina())
			{
				if (btnCanvasGroup.alpha <= 0f)
				{
					return;
				}

				ValueTuple<double, double, bool> currentTime = GetTarget().GetCurrentTime();
				if (!currentTime.Item3)
				{
					btnPlay.gameObject.SetActive(currentTime.Item1 < currentTime.Item2);
					btnPause.gameObject.SetActive(false);
					btnRePlay.gameObject.SetActive(false);
					return;
				}

				btnPlay.gameObject.SetActive(false);
				btnPause.gameObject.SetActive(currentTime.Item1 < currentTime.Item2);
				btnRePlay.gameObject.SetActive(currentTime.Item1 >= currentTime.Item2);
			}
		}


		private void OnEnable()
		{
			GetTarget().Enable();
		}


		private void OnDisable()
		{
			if (Singleton<GameEventLogger>.inst.IsNotChina())
			{
				Singleton<SoundControl>.inst.SetVideoModeMute(false);
			}

			GetTarget().Disable();
		}


		private VideoModule GetTarget()
		{
			bool flag = Singleton<GameEventLogger>.inst.IsNotChina();
			youtubePlayerModule.gameObject.SetActive(flag);
			webViewPlayerModule.gameObject.SetActive(!flag);
			if (!flag)
			{
				return webViewPlayerModule;
			}

			return youtubePlayerModule;
		}


		public void OnFadeButton(bool isFadeIn)
		{
			if (Singleton<GameEventLogger>.inst.IsNotChina())
			{
				btnCanvasGroup.DOFade(isFadeIn ? 1f : 0f, 0.25f).SetEase(Ease.OutCirc);
			}
		}


		public void Clear()
		{
			GetTarget().Clear();
		}


		public void SetData(string youtubeUrl, string otherPlatformUrl, float volume)
		{
			GetTarget().PlayForURL(Singleton<GameEventLogger>.inst.IsNotChina() ? youtubeUrl : otherPlatformUrl,
				volume);
		}
	}
}