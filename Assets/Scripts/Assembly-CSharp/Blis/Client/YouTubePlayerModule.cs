using System;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Blis.Client
{
	public class YouTubePlayerModule : VideoModule
	{
		[SerializeField] private VideoPlayer audioPlayer = default;


		[SerializeField] private YoutubePlayer youtubePlayer = default;


		[SerializeField] private VideoPlayer videoPlayer = default;


		[SerializeField] private RawImage rawImage = default;


		private RenderTexture renderTexture;

		protected override void Awake()
		{
			base.Awake();
			SetRenderTexture();
			videoPlayer.loopPointReached += delegate { Singleton<SoundControl>.inst.SetVideoModeMute(false); };
		}


		protected override void Start()
		{
			base.Start();
		}


		public override void OnClick()
		{
			base.OnClick();
			if (Math.Round(videoPlayer.time) >= Math.Round(videoPlayer.length))
			{
				PlayForURL(youtubePlayer.youtubeUrl, lastVolume);
				return;
			}

			videoPlayer.playbackSpeed = audioPlayer.playbackSpeed = videoPlayer.playbackSpeed >= 1f ? 0f : 1f;
		}


		public override void Enable()
		{
			base.Enable();
			if (isEnableCanvas)
			{
				PlayForURL(youtubePlayer.youtubeUrl, lastVolume);
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
			ClearRenderTexture();
			SetVideoPlayerSpeed(0f);
		}


		// [return: TupleElementNames(new string[]
		// {
		// 	"currentTime",
		// 	"lastTime",
		// 	"isPlaying"
		// })]
		public override (double, double, bool) GetCurrentTime()
		{
			double item = Math.Round(videoPlayer.time);
			double num = Math.Round(videoPlayer.length);
			if (num == 0.0)
			{
				num = 1.0;
			}

			return (item, num, IsPlaying());
		}


		public override bool IsPlaying()
		{
			return videoPlayer.playbackSpeed > 0f;
		}


		private void SetRenderTexture()
		{
			renderTexture = new RenderTexture((int) rawImage.rectTransform.sizeDelta.x,
				(int) rawImage.rectTransform.sizeDelta.y, 24);
			rawImage.texture = videoPlayer.targetTexture = renderTexture;
		}


		private void ClearRenderTexture()
		{
			renderTexture.Release();
		}


		private void SetVideoPlayerSpeed(float value)
		{
			VideoPlayer videoPlayer = this.videoPlayer;
			audioPlayer.playbackSpeed = value;
			videoPlayer.playbackSpeed = value;
		}


		public override void SetUrl(string url)
		{
			base.SetUrl(url);
			youtubePlayer.youtubeUrl = url;
		}


		public override bool IsHasUrl()
		{
			return !string.IsNullOrEmpty(youtubePlayer.youtubeUrl);
		}


		public override void SetVolume(float value)
		{
			base.SetVolume(value);
			lastVolume = dataVolume * (Singleton<SoundControl>.inst.MuteMasterVolume
				? 0f
				: Singleton<SoundControl>.inst.MasterVolume);
			audioPlayer.SetDirectAudioVolume(0, lastVolume);
		}


		public override void PlayForURL(string url, float volume)
		{
			base.PlayForURL(url, volume);
			ClearRenderTexture();
			SetVideoPlayerSpeed(1f);
			SetUrl(url);
			SetVolume(volume);
			if (!gameObject.activeInHierarchy || string.IsNullOrEmpty(url))
			{
				return;
			}

			if (Singleton<GameEventLogger>.inst.IsNotChina())
			{
				Singleton<SoundControl>.inst.SetVideoModeMute(true);
			}

			youtubePlayer.Play(url);
		}
	}
}