using System.Collections;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

namespace Blis.Client
{
	public class WebViewPlayerModule : VideoModule
	{
		[SerializeField] private CanvasWebViewPrefab webviewPlayer = default;


		[SerializeField] private RawImage rawImage = default;

		protected override void Awake()
		{
			base.Awake();
		}


		protected override void Start()
		{
			base.Start();
		}


		public override void OnClick()
		{
			base.OnClick();
		}


		public override void Enable()
		{
			base.Enable();
			if (isEnableCanvas)
			{
				PlayForURL(webviewPlayer.InitialUrl, 1f);
			}
		}


		public override void Disable()
		{
			base.Disable();
			Clear();
		}


		public override void Clear()
		{
			base.Clear();
			SetRenderMaterial(false);
		}


		private void SetRenderMaterial(bool isActive)
		{
			rawImage.enabled = isActive;
		}


		public override void SetUrl(string url)
		{
			base.SetUrl(url);
			webviewPlayer.InitialUrl = url;
		}


		public override void SetVolume(float value)
		{
			base.SetVolume(value);
		}


		public override void PlayForURL(string url, float volume)
		{
			base.PlayForURL(url, volume);
			SetUrl(url);
			if (!gameObject.activeInHierarchy || string.IsNullOrEmpty(url))
			{
				return;
			}

			if (Singleton<GameEventLogger>.inst.IsNotChina())
			{
				Singleton<SoundControl>.inst.SetVideoModeMute(true);
			}

			StartCoroutine(Play(url));
		}


		private IEnumerator Play(string url)
		{
			yield return null;
			SetRenderMaterial(true);
			webviewPlayer.WebViewPrefab.LoadUrl(url);
		}
	}
}