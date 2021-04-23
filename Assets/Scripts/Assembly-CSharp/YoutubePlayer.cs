using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using YoutubeLight;


public class YoutubePlayer : MonoBehaviour
{
	
	private void Awake()
	{
		if (PlayerPrefs.HasKey("utube_config"))
		{
			this.magicResult = new YoutubePlayer.MagicContent();
			this.magicResult.regexForFuncName = PlayerPrefsX.GetStringArray("utube_regex_funcName");
			this.magicResult.regexForHtmlJson = PlayerPrefs.GetString("utube_regex_htmlJson");
			this.magicResult.regexForHtmlPlayerVersion = PlayerPrefs.GetString("utube_regex_htmlPlayerVersion");
		}
		else
		{
			PlayerPrefs.SetInt("utube_config", 1);
			this.magicResult = new YoutubePlayer.MagicContent();
			PlayerPrefsX.SetStringArray("utube_regex_funcName", this.magicResult.defaultFuncName);
			PlayerPrefs.SetString("utube_regex_htmlJson", this.magicResult.defaultHtmlJson);
			PlayerPrefs.SetString("utube_regex_htmlPlayerVersion", this.magicResult.defaultHtmlPlayerVersion);
			this.magicResult.regexForFuncName = PlayerPrefsX.GetStringArray("utube_regex_funcName");
			this.magicResult.regexForHtmlJson = PlayerPrefs.GetString("utube_regex_htmlJson");
			this.magicResult.regexForHtmlPlayerVersion = PlayerPrefs.GetString("utube_regex_htmlPlayerVersion");
		}
		if (!this.playUsingInternalDevicePlayer && !this.loadYoutubeUrlsOnly)
		{
			if (this.videoQuality == YoutubePlayer.YoutubeVideoQuality.STANDARD)
			{
				this.videoPlayer.skipOnDrop = this._skipOnDrop;
				if (this.audioPlayer != null)
				{
					this.audioPlayer.transform.gameObject.SetActive(false);
				}
			}
			if (this.videoPlayer.renderMode == VideoRenderMode.CameraFarPlane || this.videoPlayer.renderMode == VideoRenderMode.CameraNearPlane)
			{
				this.fullscreenModeEnabled = true;
				return;
			}
			this.fullscreenModeEnabled = false;
		}
	}

	
	private void UpdateRegexMethodsFromServer()
	{
	}

	
	private IEnumerator CallServerForUpdate()
	{
		UnityWebRequest request = UnityWebRequest.Get("http://test-youtube-unity.herokuapp.com/api/regexUpdater");
		yield return request.SendWebRequest();
		if (request.isDone)
		{
			this.RegexUpdaterLoaded(JSON.Parse(request.downloadHandler.text));
		}
	}

	
	private void RegexUpdaterLoaded(JSONNode regexJsonData)
	{
		if (regexJsonData["patterns"] == null)
		{
			PlayerPrefs.SetInt("utube_config", 1);
			this.magicResult = new YoutubePlayer.MagicContent();
			PlayerPrefsX.SetStringArray("utube_regex_funcName", this.magicResult.defaultFuncName);
			PlayerPrefs.SetString("utube_regex_htmlJson", this.magicResult.defaultHtmlJson);
			PlayerPrefs.SetString("utube_regex_htmlPlayerVersion", this.magicResult.defaultHtmlPlayerVersion);
			this.magicResult.regexForFuncName = PlayerPrefsX.GetStringArray("utube_regex_funcName");
			this.magicResult.regexForHtmlJson = PlayerPrefs.GetString("utube_regex_htmlJson");
			this.magicResult.regexForHtmlPlayerVersion = PlayerPrefs.GetString("utube_regex_htmlPlayerVersion");
			return;
		}
		JSONArray asArray = regexJsonData["patterns"].AsArray;
		string[] array = new string[asArray.Count];
		for (int i = 0; i < asArray.Count; i++)
		{
			string value = asArray[i].Value;
			array[i] = value;
		}
		this.magicResult = new YoutubePlayer.MagicContent();
		PlayerPrefsX.SetStringArray("utube_regex_funcName", array);
		PlayerPrefs.SetString("utube_regex_htmlJson", regexJsonData["htmlJson"]);
		PlayerPrefs.SetString("utube_regex_htmlPlayerVersion", regexJsonData["htmlPlayerVersion"]);
		this.magicResult.regexForFuncName = PlayerPrefsX.GetStringArray("utube_regex_funcName");
		this.magicResult.regexForHtmlJson = PlayerPrefs.GetString("utube_regex_htmlJson");
		this.magicResult.regexForHtmlPlayerVersion = PlayerPrefs.GetString("utube_regex_htmlPlayerVersion");
		Debug.Log("<color='yellow'>Regex updated from server, if the error continues mail support at kelvinparkour@gmail.com</color>");
	}

	
	public void Start()
	{
		if (this.playUsingInternalDevicePlayer)
		{
			this.loadYoutubeUrlsOnly = true;
		}
		if (!this.loadYoutubeUrlsOnly)
		{
			base.Invoke("VerifyFrames", 2f);
			this.FixCameraEvent();
			this.Skybox3DSettup();
			if (this.videoFormat == YoutubePlayer.VideoFormatType.WEBM)
			{
				this.videoPlayer.skipOnDrop = this._skipOnDrop;
				this.audioPlayer.skipOnDrop = this._skipOnDrop;
			}
			this.audioPlayer.seekCompleted += this.AudioSeeked;
			this.videoPlayer.seekCompleted += this.VideoSeeked;
		}
		this.PrepareVideoPlayerCallbacks();
		if (this.autoPlayOnStart)
		{
			if (this.customPlaylist)
			{
				this.PlayYoutubeVideo(this.youtubeUrls[this.currentUrlIndex]);
			}
			else
			{
				this.PlayYoutubeVideo(this.youtubeUrl);
			}
		}
		if (this.videoQuality == YoutubePlayer.YoutubeVideoQuality.STANDARD)
		{
			this.lowRes = true;
			return;
		}
		this.lowRes = false;
	}

	
	public void CallNextUrl()
	{
		if (!this.customPlaylist)
		{
			return;
		}
		if (this.currentUrlIndex + 1 < this.youtubeUrls.Length)
		{
			this.currentUrlIndex++;
		}
		else
		{
			this.currentUrlIndex = 0;
		}
		this.PlayYoutubeVideo(this.youtubeUrls[this.currentUrlIndex]);
	}

	
	private void TryToLoadThumbnailBeforeOpenVideo(string id)
	{
		string videoId = id.Replace("https://youtube.com/watch?v=", "");
		base.StartCoroutine(this.DownloadThumbnail(videoId));
	}

	
	private IEnumerator DownloadThumbnail(string videoId)
	{
		UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://img.youtube.com/vi/" + videoId + "/0.jpg");
		yield return request.SendWebRequest();
		Texture2D content = DownloadHandlerTexture.GetContent(request);
		this.videoPlayer.targetMaterialRenderer.material.mainTexture = content;
	}

	
	private void Skybox3DSettup()
	{
		if (this.is3DLayoutVideo)
		{
			if (this.layout3d == YoutubePlayer.Layout3D.OverUnder)
			{
				RenderSettings.skybox = (Material)Resources.Load("Materials/PanoramicSkybox3DOverUnder");
				return;
			}
			if (this.layout3d == YoutubePlayer.Layout3D.sideBySide)
			{
				RenderSettings.skybox = (Material)Resources.Load("Materials/PanoramicSkybox3Dside");
			}
		}
	}

	
	public void ToogleFullsScreenMode()
	{
		this.fullscreenModeEnabled = !this.fullscreenModeEnabled;
		if (!this.fullscreenModeEnabled)
		{
			this.videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
			if (this.videoPlayer.targetCamera == null)
			{
				this.videoPlayer.targetCamera = this.mainCamera;
			}
		}
		else
		{
			this.videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
		}
	}

	
	private void FixCameraEvent()
	{
		if (this.mainCamera == null)
		{
			if (Camera.main != null)
			{
				this.mainCamera = Camera.main;
			}
			else
			{
				this.mainCamera = UnityEngine.Object.FindObjectOfType<Camera>();
				Debug.Log("Add the main camera to the mainCamera field");
			}
		}
		if (this.videoControllerCanvas != null)
		{
			this.videoControllerCanvas.GetComponent<Canvas>().worldCamera = this.mainCamera;
		}
		if (this.videoPlayer.renderMode == VideoRenderMode.CameraFarPlane || this.videoPlayer.renderMode == VideoRenderMode.CameraNearPlane)
		{
			this.videoPlayer.targetCamera = this.mainCamera;
		}
	}

	
	private void OnApplicationPause(bool pause)
	{
		if (!this.playUsingInternalDevicePlayer && !this.loadYoutubeUrlsOnly && this.videoPlayer.isPrepared)
		{
			if (this.audioPlayer != null)
			{
				this.audioPlayer.Pause();
			}
			this.videoPlayer.Pause();
		}
	}

	
	private void OnApplicationFocus(bool focus)
	{
	}

	
	private void OnEnable()
	{
		if (this.autoPlayOnEnable && !this.pauseCalled)
		{
			base.StartCoroutine(this.WaitThingsGetDone());
		}
	}

	
	private IEnumerator WaitThingsGetDone()
	{
		yield return new WaitForSeconds(1f);
		if (this.youtubeUrlReady && this.videoPlayer.isPrepared)
		{
			this.Play();
		}
		else if (!this.youtubeUrlReady)
		{
			this.Play(this.youtubeUrl);
		}
	}

	
	private void VerifyFrames()
	{
		if (!this.playUsingInternalDevicePlayer && this.videoPlayer.isPlaying)
		{
			if (this.lastFrame == this.videoPlayer.frame)
			{
				this.audioPlayer.Pause();
				this.videoPlayer.Pause();
				base.StartCoroutine(this.WaitSync());
			}
			this.lastFrame = this.videoPlayer.frame;
			base.Invoke("VerifyFrames", 2f);
		}
	}

	
	private IEnumerator ReleaseNeedUpdate()
	{
		yield return new WaitForSeconds(40f);
		this.canUpdate = true;
	}

	
	private void FixedUpdate()
	{
		if (!this.loadYoutubeUrlsOnly)
		{
			if (this.videoPlayer.isPlaying && Time.frameCount % (int)(this.videoPlayer.frameRate + 1f) == 0)
			{
				if (this.lastTimePlayed == this.videoPlayer.time)
				{
					this.ShowLoading();
					Debug.Log("Buffering");
				}
				else
				{
					this.HideLoading();
				}
				this.lastTimePlayed = this.videoPlayer.time;
			}
			if (!this.playUsingInternalDevicePlayer)
			{
				if (this.videoPlayer.isPlaying)
				{
					this.HideLoading();
				}
				else if (!this.pauseCalled)
				{
					this.ShowLoading();
				}
			}
		}
		if (!this.loadYoutubeUrlsOnly)
		{
			if (this.showPlayerControls && this.videoPlayer.isPlaying)
			{
				this.totalVideoDuration = (float)Mathf.RoundToInt(this.videoPlayer.frameCount / this.videoPlayer.frameRate);
				if (!this.lowRes)
				{
					this.audioDuration = (float)Mathf.RoundToInt(this.audioPlayer.frameCount / this.audioPlayer.frameRate);
					if (this.audioDuration < this.totalVideoDuration && this.audioPlayer.url != "")
					{
						this.currentVideoDuration = (float)Mathf.RoundToInt((float)this.audioPlayer.frame / this.audioPlayer.frameRate);
					}
					else
					{
						this.currentVideoDuration = (float)Mathf.RoundToInt((float)this.videoPlayer.frame / this.videoPlayer.frameRate);
					}
				}
				else
				{
					this.currentVideoDuration = (float)Mathf.RoundToInt((float)this.videoPlayer.frame / this.videoPlayer.frameRate);
				}
			}
			if (this.videoPlayer.frameCount > 0UL && this.progress != null)
			{
				this.progress.fillAmount = (float)this.videoPlayer.frame / this.videoPlayer.frameCount;
			}
		}
		if (YoutubePlayer.needUpdate)
		{
			YoutubePlayer.needUpdate = false;
			base.StartCoroutine(this.ReleaseNeedUpdate());
			this.UpdateRegexMethodsFromServer();
		}
		if (this.gettingYoutubeURL)
		{
			this.currentRequestTime += Time.deltaTime;
			if (this.currentRequestTime >= (float)this.maxRequestTime && !this.ignoreTimeout)
			{
				this.gettingYoutubeURL = false;
				if (this.debug)
				{
					Debug.Log("<color=blue>Max time reached, trying again!</color>");
				}
				this.RetryPlayYoutubeVideo();
			}
		}
		if (this.videoAreReadyToPlay)
		{
			this.videoAreReadyToPlay = false;
		}
		this.ErrorCheck();
		if (!this.loadYoutubeUrlsOnly)
		{
			if (this.showPlayerControls)
			{
				if (this.videoQuality != YoutubePlayer.YoutubeVideoQuality.STANDARD)
				{
					this.lowRes = false;
				}
				else
				{
					this.lowRes = true;
				}
				if (this.currentTimeString != null && this.totalTimeString != null)
				{
					this.currentTimeString.text = this.FormatTime(Mathf.RoundToInt(this.currentVideoDuration));
					if (!this.lowRes)
					{
						if (this.audioDuration < this.totalVideoDuration && this.audioPlayer.url != "")
						{
							this.totalTimeString.text = this.FormatTime(Mathf.RoundToInt(this.audioDuration));
						}
						else
						{
							this.totalTimeString.text = this.FormatTime(Mathf.RoundToInt(this.totalVideoDuration));
						}
					}
					else
					{
						this.totalTimeString.text = this.FormatTime(Mathf.RoundToInt(this.totalVideoDuration));
					}
				}
			}
			if (!this.showPlayerControls)
			{
				if (this.mainControllerUi != null)
				{
					this.mainControllerUi.SetActive(false);
				}
			}
			else
			{
				this.mainControllerUi.SetActive(true);
			}
		}
		if (this.decryptedUrlForAudio)
		{
			this.decryptedUrlForAudio = false;
			this.DecryptAudioDone(this.decryptedAudioUrlResult);
			this.decryptedUrlForVideo = true;
		}
		if (this.decryptedUrlForVideo)
		{
			this.decryptedUrlForVideo = false;
			this.DecryptVideoDone(this.decryptedVideoUrlResult);
		}
		if (!this.loadYoutubeUrlsOnly && this.videoPlayer.isPrepared && !this.videoPlayer.isPlaying)
		{
			if (this.audioPlayer != null)
			{
				if (this.audioPlayer.isPrepared && !this.videoStarted)
				{
					this.videoStarted = true;
					this.VideoStarted(this.videoPlayer);
				}
			}
			else if (!this.videoStarted)
			{
				this.videoStarted = true;
				this.VideoStarted(this.videoPlayer);
			}
		}
		if (!this.loadYoutubeUrlsOnly)
		{
			if (this.videoPlayer.frame != 0L && !this.videoEnded && (int)this.videoPlayer.frame >= (int)this.videoPlayer.frameCount)
			{
				this.videoEnded = true;
				this.PlaybackDone(this.videoPlayer);
			}
			if (this.videoPlayer.isPrepared)
			{
				if (this.videoQuality != YoutubePlayer.YoutubeVideoQuality.STANDARD)
				{
					if (this.audioPlayer.isPrepared && !this.startedPlayingWebgl)
					{
						this.startedPlayingWebgl = true;
						this.StartPlayingWebgl();
					}
				}
				else if (!this.startedPlayingWebgl)
				{
					this.startedPlayingWebgl = true;
					this.StartPlayingWebgl();
				}
			}
		}
	}

	
	private void PrepareVideoPlayerCallbacks()
	{
		this.videoPlayer.errorReceived += this.VideoErrorReceived;
		this.videoPlayer.loopPointReached += this.PlaybackDone;
		if (this.videoQuality != YoutubePlayer.YoutubeVideoQuality.STANDARD)
		{
			this.audioPlayer.errorReceived += this.VideoErrorReceived;
		}
	}

	
	private void ShowLoading()
	{
		if (this.loadingContent != null)
		{
			this.loadingContent.SetActive(true);
		}
	}

	
	private void HideLoading()
	{
		if (this.loadingContent != null)
		{
			this.loadingContent.SetActive(false);
		}
	}

	
	public void Play(string url)
	{
		this.logTest = "Getting URL";
		this.Stop();
		this.startedPlayingWebgl = false;
		this.PlayYoutubeVideo(url);
	}

	
	private string CheckVideoUrlAndExtractThevideoId(string url)
	{
		if (url.Contains("?t="))
		{
			int num = url.LastIndexOf("?t=");
			string text = url.Remove(0, num);
			text = text.Replace("?t=", "");
			this.startFromSecond = true;
			this.startFromSecondTime = int.Parse(text);
			url = url.Remove(num);
		}
		if (!this.TryNormalizeYoutubeUrlLocal(url, out url))
		{
			url = "none";
			this.OnYoutubeError("Not a Youtube Url");
		}
		return url;
	}

	
	public void OnYoutubeError(string errorType)
	{
		Debug.Log("<color=red>" + errorType + "</color>");
	}

	
	private bool TryNormalizeYoutubeUrlLocal(string url, out string normalizedUrl)
	{
		url = url.Trim();
		url = url.Replace("youtu.be/", "youtube.com/watch?v=");
		url = url.Replace("www.youtube", "youtube");
		url = url.Replace("youtube.com/embed/", "youtube.com/watch?v=");
		if (url.Contains("/v/"))
		{
			url = "https://youtube.com" + new Uri(url).AbsolutePath.Replace("/v/", "/watch?v=");
		}
		url = url.Replace("/watch#", "/watch?");
		string str;
		if (!HTTPHelperYoutube.ParseQueryString(url).TryGetValue("v", out str))
		{
			normalizedUrl = null;
			return false;
		}
		normalizedUrl = "https://youtube.com/watch?v=" + str;
		return true;
	}

	
	private void ResetThings()
	{
		this.gettingYoutubeURL = false;
		this.videoAreReadyToPlay = false;
		this.audioDecryptDone = false;
		this.videoDecryptDone = false;
		this.isRetry = false;
		this.youtubeUrlReady = false;
		if (this.audioPlayer != null)
		{
			this.audioPlayer.seekCompleted += this.AudioSeeked;
		}
		this.videoPlayer.seekCompleted += this.VideoSeeked;
		this.videoPlayer.frameDropped += this.VideoPlayer_frameDropped;
		if (this.audioPlayer != null)
		{
			this.audioPlayer.frameDropped += this.AudioPlayer_frameDropped;
		}
		this.waitAudioSeek = false;
	}

	
	public void PlayFromDefaultUrl()
	{
		this.Play(this.youtubeUrl);
	}

	
	public void PlayYoutubeVideo(string videoId)
	{
		if (this.videoQuality == YoutubePlayer.YoutubeVideoQuality.STANDARD)
		{
			this.lowRes = true;
		}
		else
		{
			this.lowRes = false;
		}
		this.ResetThings();
		videoId = this.CheckVideoUrlAndExtractThevideoId(videoId);
		if (videoId == "none")
		{
			return;
		}
		if (this.showThumbnailBeforeVideoLoad)
		{
			this.TryToLoadThumbnailBeforeOpenVideo(videoId);
		}
		this.youtubeUrlReady = false;
		this.ShowLoading();
		this.youtubeUrl = videoId;
		this.isRetry = false;
		this.lastTryVideoId = videoId;
		this.lastPlayTime = Time.time;
		if (!this.ForceGetWebServer)
		{
			this.currentRequestTime = 0f;
			this.gettingYoutubeURL = true;
			this.GetDownloadUrls(new Action(this.UrlsLoaded), this.youtubeUrl, this);
			return;
		}
		base.StartCoroutine(this.WebRequest(this.youtubeUrl));
	}

	
	public void DecryptAudioDone(string url)
	{
		this.audioUrl = url;
		this.audioDecryptDone = true;
		if (this.videoDecryptDone)
		{
			if (string.IsNullOrEmpty(this.decryptedAudioUrlResult))
			{
				this.RetryPlayYoutubeVideo();
				return;
			}
			this.videoAreReadyToPlay = true;
			this.OnYoutubeUrlsLoaded();
		}
	}

	
	public void DecryptVideoDone(string url)
	{
		this.videoUrl = url;
		this.videoDecryptDone = true;
		if (!this.audioDecryptDone)
		{
			if (this.videoQuality == YoutubePlayer.YoutubeVideoQuality.STANDARD)
			{
				if (string.IsNullOrEmpty(this.decryptedVideoUrlResult))
				{
					this.RetryPlayYoutubeVideo();
					return;
				}
				this.videoAreReadyToPlay = true;
				this.OnYoutubeUrlsLoaded();
			}
			return;
		}
		if (string.IsNullOrEmpty(this.decryptedVideoUrlResult))
		{
			this.RetryPlayYoutubeVideo();
			return;
		}
		this.videoAreReadyToPlay = true;
		this.OnYoutubeUrlsLoaded();
	}

	
	public string GetVideoTitle()
	{
		return this.videoTitle;
	}

	
	private void UrlsLoaded()
	{
		this.gettingYoutubeURL = false;
		List<VideoInfo> list = this.youtubeVideoInfos;
		this.videoDecryptDone = false;
		this.audioDecryptDone = false;
		this.decryptedUrlForVideo = false;
		this.decryptedUrlForAudio = false;
		if (this.videoQuality == YoutubePlayer.YoutubeVideoQuality.STANDARD)
		{
			using (List<VideoInfo>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					VideoInfo videoInfo = enumerator.Current;
					if (videoInfo.FormatCode == 18)
					{
						if (videoInfo.RequiresDecryption)
						{
							this.DecryptDownloadUrl(videoInfo.DownloadUrl, "", videoInfo.HtmlPlayerVersion, true);
						}
						else
						{
							this.videoUrl = videoInfo.DownloadUrl;
							this.videoAreReadyToPlay = true;
							this.OnYoutubeUrlsLoaded();
						}
						this.videoTitle = videoInfo.Title;
					}
				}
				return;
			}
		}
		bool flag = false;
		string encrytedUrlAudio = "";
		string encryptedUrlVideo = "";
		string html = "";
		this.videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
		list.Reverse();
		foreach (VideoInfo videoInfo2 in list)
		{
			if (videoInfo2.FormatCode == 18)
			{
				if (videoInfo2.RequiresDecryption)
				{
					flag = true;
					html = videoInfo2.HtmlPlayerVersion;
					encrytedUrlAudio = videoInfo2.DownloadUrl;
				}
				else
				{
					encrytedUrlAudio = videoInfo2.DownloadUrl;
					this.audioUrl = videoInfo2.DownloadUrl;
				}
				this.videoTitle = videoInfo2.Title;
				break;
			}
		}
		int num = 360;
		switch (this.videoQuality)
		{
		case YoutubePlayer.YoutubeVideoQuality.STANDARD:
			num = 360;
			break;
		case YoutubePlayer.YoutubeVideoQuality.HD:
			num = 720;
			break;
		case YoutubePlayer.YoutubeVideoQuality.FULLHD:
			num = 1080;
			break;
		case YoutubePlayer.YoutubeVideoQuality.UHD1440:
			num = 1440;
			break;
		case YoutubePlayer.YoutubeVideoQuality.UHD2160:
			num = 2160;
			break;
		}
		bool flag2 = false;
		list.Reverse();
		foreach (VideoInfo videoInfo3 in list)
		{
			VideoType videoType = (this.videoFormat == YoutubePlayer.VideoFormatType.MP4) ? VideoType.Mp4 : VideoType.WebM;
			if (videoInfo3.VideoType == videoType && videoInfo3.Resolution == num)
			{
				if (videoInfo3.RequiresDecryption)
				{
					if (this.debug)
					{
						Debug.Log("REQUIRE DECRYPTION!");
					}
					this.logTest = "Decry";
					flag = true;
					encryptedUrlVideo = videoInfo3.DownloadUrl;
				}
				else
				{
					Debug.Log(videoInfo3.DownloadUrl);
					encryptedUrlVideo = videoInfo3.DownloadUrl;
					this.videoUrl = videoInfo3.DownloadUrl;
					this.videoAreReadyToPlay = true;
					this.OnYoutubeUrlsLoaded();
				}
				flag2 = true;
				break;
			}
		}
		if (!flag2 && num == 1440)
		{
			foreach (VideoInfo videoInfo4 in list)
			{
				if (videoInfo4.FormatCode == 271)
				{
					Debug.Log(string.Concat(new object[]
					{
						"FIXING!! ",
						videoInfo4.Resolution,
						" | ",
						videoInfo4.VideoType,
						" | ",
						videoInfo4.FormatCode
					}));
					if (videoInfo4.RequiresDecryption)
					{
						flag = true;
						encryptedUrlVideo = videoInfo4.DownloadUrl;
					}
					else
					{
						encryptedUrlVideo = videoInfo4.DownloadUrl;
						this.videoUrl = videoInfo4.DownloadUrl;
						this.videoAreReadyToPlay = true;
						this.OnYoutubeUrlsLoaded();
					}
					flag2 = true;
					break;
				}
			}
		}
		if (!flag2 && num == 2160)
		{
			foreach (VideoInfo videoInfo5 in list)
			{
				if (videoInfo5.FormatCode == 313)
				{
					if (this.debug)
					{
						Debug.Log("Found but with unknow format in results, check to see if the video works normal.");
					}
					if (videoInfo5.RequiresDecryption)
					{
						flag = true;
						encryptedUrlVideo = videoInfo5.DownloadUrl;
					}
					else
					{
						encryptedUrlVideo = videoInfo5.DownloadUrl;
						this.videoUrl = videoInfo5.DownloadUrl;
						this.videoAreReadyToPlay = true;
						this.OnYoutubeUrlsLoaded();
					}
					flag2 = true;
					break;
				}
			}
		}
		if (!flag2)
		{
			if (this.debug)
			{
				Debug.Log("Desired quality not found, playing with low quality, check if the video id: " + this.youtubeUrl + " support that quality!");
			}
			foreach (VideoInfo videoInfo6 in list)
			{
				if (videoInfo6.VideoType == VideoType.Mp4 && videoInfo6.Resolution == 360)
				{
					if (videoInfo6.RequiresDecryption)
					{
						this.videoQuality = YoutubePlayer.YoutubeVideoQuality.STANDARD;
						flag = true;
						encryptedUrlVideo = videoInfo6.DownloadUrl;
						break;
					}
					encryptedUrlVideo = videoInfo6.DownloadUrl;
					this.videoUrl = videoInfo6.DownloadUrl;
					this.videoAreReadyToPlay = true;
					this.OnYoutubeUrlsLoaded();
					break;
				}
			}
		}
		if (flag)
		{
			this.DecryptDownloadUrl(encryptedUrlVideo, encrytedUrlAudio, html, false);
		}
	}

	
	private void StartPlayingWebgl()
	{
		YoutubePlayer.YoutubeVideoQuality youtubeVideoQuality = this.videoQuality;
		if (this.playUsingInternalDevicePlayer && Application.isMobilePlatform)
		{
			base.StartCoroutine(this.HandHeldPlayback());
			return;
		}
		this.StartPlayback();
	}

	
	private IEnumerator HandHeldPlayback()
	{
		Debug.Log("This runs in mobile devices only!");
		yield return new WaitForSeconds(1f);
		this.PlaybackDone(this.videoPlayer);
	}

	
	private void StartPlayback()
	{
		if (this.objectsToRenderTheVideoImage.Length != 0)
		{
			GameObject[] array = this.objectsToRenderTheVideoImage;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<Renderer>().material.mainTexture = this.videoPlayer.texture;
			}
		}
		this.videoEnded = false;
		this.OnVideoStarted.Invoke();
		YoutubePlayer.YoutubeVideoQuality youtubeVideoQuality = this.videoQuality;
		this.HideLoading();
		this.waitAudioSeek = true;
		if (this.videoQuality != YoutubePlayer.YoutubeVideoQuality.STANDARD && !this.noAudioAtacched)
		{
			this.audioPlayer.Pause();
			this.videoPlayer.Pause();
		}
		this.audioPlayer.time = 0.0;
		this.videoPlayer.time = 0.0;
		this.audioPlayer.Play();
		this.videoPlayer.Play();
		if (this.startFromSecond)
		{
			this.startedFromTime = true;
			if (this.videoQuality == YoutubePlayer.YoutubeVideoQuality.STANDARD)
			{
				this.videoPlayer.time = (double)this.startFromSecondTime;
				return;
			}
			this.audioPlayer.time = (double)this.startFromSecondTime;
		}
	}

	
	private void ErrorCheck()
	{
		if (!this.ForceGetWebServer && !this.isRetry && this.lastStartedTime < this.lastErrorTime && this.lastErrorTime > this.lastPlayTime)
		{
			if (this.debug)
			{
				Debug.Log("Error detected!, retry with low quality!");
			}
			this.isRetry = true;
		}
	}

	
	public int GetMaxQualitySupportedByDevice()
	{
		if (Screen.orientation == ScreenOrientation.LandscapeLeft)
		{
			return Screen.currentResolution.height;
		}
		if (Screen.orientation == ScreenOrientation.Portrait)
		{
			return Screen.currentResolution.width;
		}
		return Screen.currentResolution.height;
	}

	
	private IEnumerator WebRequest(string videoID)
	{
		UnityWebRequest request = UnityWebRequest.Get("https://lightshaftstream.herokuapp.com/api/info?url=" + videoID + "&format=best&flatten=true");
		yield return request.SendWebRequest();
		this.newRequestResults = new YoutubePlayer.YoutubeResultIds();
		JSONNode jsonnode = JSON.Parse(request.downloadHandler.text);
		JSONNode jsonnode2 = jsonnode["videos"][0]["formats"];
		Debug.Log(request.downloadHandler.text);
		this.newRequestResults.bestFormatWithAudioIncluded = jsonnode["videos"][0]["url"];
		for (int i = 0; i < jsonnode2.Count; i++)
		{
			if (jsonnode2[i]["format_id"] == "160")
			{
				this.newRequestResults.lowQuality = jsonnode2[i]["url"];
			}
			else if (jsonnode2[i]["format_id"] == "133")
			{
				this.newRequestResults.lowQuality = jsonnode2[i]["url"];
			}
			else if (jsonnode2[i]["format_id"] == "134")
			{
				this.newRequestResults.standardQuality = jsonnode2[i]["url"];
			}
			else if (jsonnode2[i]["format_id"] == "136")
			{
				this.newRequestResults.hdQuality = this.newRequestResults.bestFormatWithAudioIncluded;
			}
			else if (jsonnode2[i]["format_id"] == "137")
			{
				this.newRequestResults.fullHdQuality = jsonnode2[i]["url"];
			}
			else if (jsonnode2[i]["format_id"] == "266")
			{
				this.newRequestResults.ultraHdQuality = jsonnode2[i]["url"];
			}
			else if (jsonnode2[i]["format_id"] == "139")
			{
				this.newRequestResults.audioUrl = jsonnode2[i]["url"];
			}
		}
		this.audioUrl = this.newRequestResults.bestFormatWithAudioIncluded;
		this.videoUrl = this.newRequestResults.bestFormatWithAudioIncluded;
		switch (this.videoQuality)
		{
		case YoutubePlayer.YoutubeVideoQuality.STANDARD:
			this.videoUrl = this.newRequestResults.bestFormatWithAudioIncluded;
			break;
		case YoutubePlayer.YoutubeVideoQuality.HD:
			this.videoUrl = this.newRequestResults.hdQuality;
			break;
		case YoutubePlayer.YoutubeVideoQuality.FULLHD:
			this.videoUrl = this.newRequestResults.fullHdQuality;
			break;
		case YoutubePlayer.YoutubeVideoQuality.UHD1440:
			this.videoUrl = this.newRequestResults.fullHdQuality;
			break;
		case YoutubePlayer.YoutubeVideoQuality.UHD2160:
			this.videoUrl = this.newRequestResults.ultraHdQuality;
			break;
		}
		if (this.videoUrl == "")
		{
			this.videoUrl = this.newRequestResults.bestFormatWithAudioIncluded;
		}
		this.videoAreReadyToPlay = true;
		this.OnYoutubeUrlsLoaded();
	}

	
	private string ConvertToWebglUrl(string url)
	{
		string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(url));
		if (this.debug)
		{
			Debug.Log(url);
		}
		return "https://youtubewebgl.herokuapp.com/download.php?mime=video/mp4&title=generatedvideo&token=" + str;
	}

	
	public void RetryPlayYoutubeVideo()
	{
		this.Stop();
		this.currentRetryTime++;
		this.logTest = "Retry!!";
		if (this.currentRetryTime < this.retryTimeUntilToRequestFromServer)
		{
			if (!this.ForceGetWebServer)
			{
				this.StopIfPlaying();
				if (this.debug)
				{
					Debug.Log("Youtube Retrying...:" + this.lastTryVideoId);
				}
				this.logTest = "retry";
				this.isRetry = true;
				this.ShowLoading();
				this.youtubeUrl = this.lastTryVideoId;
				this.PlayYoutubeVideo(this.youtubeUrl);
			}
		}
		else
		{
			this.currentRetryTime = 0;
			this.StopIfPlaying();
			if (this.debug)
			{
				Debug.Log("Youtube Retrying...:" + this.lastTryVideoId);
			}
			this.logTest = "retry";
			this.isRetry = true;
			this.ShowLoading();
			this.youtubeUrl = this.lastTryVideoId;
			this.PlayYoutubeVideo(this.youtubeUrl);
		}
	}

	
	private void StopIfPlaying()
	{
		if (!this.loadYoutubeUrlsOnly)
		{
			if (this.debug)
			{
				Debug.Log("Stopping video");
			}
			if (this.videoPlayer.isPlaying)
			{
				this.videoPlayer.Stop();
			}
			if (this.audioPlayer.isPlaying)
			{
				this.audioPlayer.Stop();
			}
		}
	}

	
	public void UrlReadyToUse()
	{
		Debug.Log("Here you can call your external video player if you want, passing that two variables:");
		if (this.videoQuality != YoutubePlayer.YoutubeVideoQuality.STANDARD)
		{
			Debug.Log("Your video Url: " + this.videoUrl);
			Debug.Log("Your audio video Url: " + this.audioUrl);
			return;
		}
		Debug.Log("Yout video Url:" + this.videoUrl);
	}

	
	public void OnYoutubeUrlsLoaded()
	{
		this.youtubeUrlReady = true;
		if (!this.loadYoutubeUrlsOnly)
		{
			Uri uri = new Uri(this.videoUrl);
			this.videoUrl = this.videoUrl.Replace(uri.Host, "redirector.googlevideo.com");
			if (this.videoQuality != YoutubePlayer.YoutubeVideoQuality.STANDARD)
			{
				uri = new Uri(this.audioUrl);
				this.audioUrl = this.audioUrl.Replace(uri.Host, "redirector.googlevideo.com");
			}
			if (this.debug)
			{
				Debug.Log("Play!!" + this.videoUrl);
			}
			this.startedPlayingWebgl = false;
			this.videoPlayer.source = VideoSource.Url;
			this.videoPlayer.url = this.videoUrl;
			this.videoPlayer.Prepare();
			if (this.videoQuality != YoutubePlayer.YoutubeVideoQuality.STANDARD)
			{
				this.audioPlayer.source = VideoSource.Url;
				this.audioPlayer.url = this.audioUrl;
				this.audioPlayer.Prepare();
			}
		}
		else if (this.playUsingInternalDevicePlayer)
		{
			base.StartCoroutine(this.HandHeldPlayback());
		}
		else
		{
			this.UrlReadyToUse();
		}
		this.OnYoutubeUrlAreReady.Invoke(this.videoUrl);
	}

	
	public void OnYoutubeVideoAreReadyToPlay()
	{
		this.OnVideoReadyToStart.Invoke();
		this.StartPlayingWebgl();
	}

	
	private IEnumerator PreventFinishToBeCalledTwoTimes()
	{
		yield return new WaitForSeconds(1f);
		this.finishedCalled = false;
	}

	
	public void OnVideoPlayerFinished()
	{
		if (!this.finishedCalled)
		{
			this.finishedCalled = true;
			base.StartCoroutine(this.PreventFinishToBeCalledTwoTimes());
			if (!this.loadYoutubeUrlsOnly)
			{
				if (this.videoPlayer.isPrepared)
				{
					if (this.debug)
					{
						Debug.Log("Finished");
					}
					if (this.videoPlayer.isLooping)
					{
						this.videoPlayer.time = 0.0;
						this.videoPlayer.frame = 0L;
						this.audioPlayer.time = 0.0;
						this.audioPlayer.frame = 0L;
						this.videoPlayer.Play();
						if (!this.noAudioAtacched)
						{
							this.audioPlayer.Play();
						}
					}
					base.CancelInvoke("CheckIfIsSync");
					this.OnVideoFinished.Invoke();
					if (this.customPlaylist && this.autoPlayNextVideo)
					{
						Debug.Log("Calling next video of playlist");
						this.CallNextUrl();
					}
				}
			}
			else if (this.playUsingInternalDevicePlayer)
			{
				base.CancelInvoke("CheckIfIsSync");
				this.OnVideoFinished.Invoke();
			}
		}
	}

	
	private void PlaybackDone(VideoPlayer vPlayer)
	{
		this.videoStarted = false;
		this.OnVideoPlayerFinished();
	}

	
	private void VideoStarted(VideoPlayer source)
	{
		if (!this.videoStarted)
		{
			this.lastStartedTime = Time.time;
			this.lastErrorTime = this.lastStartedTime;
			if (this.debug)
			{
				Debug.Log("Youtube Video Started");
			}
		}
	}

	
	private void VideoErrorReceived(VideoPlayer source, string message)
	{
		this.lastErrorTime = Time.time;
		this.RetryPlayYoutubeVideo();
		Debug.Log("Youtube VideoErrorReceived! Retry: " + message);
	}

	
	public void Play()
	{
		this.pauseCalled = false;
		if (this.videoQuality == YoutubePlayer.YoutubeVideoQuality.STANDARD)
		{
			this.videoPlayer.Play();
			return;
		}
		this.videoPlayer.Play();
		if (!this.noAudioAtacched)
		{
			this.audioPlayer.Play();
		}
	}

	
	public void Pause()
	{
		this.pauseCalled = true;
		if (this.videoQuality == YoutubePlayer.YoutubeVideoQuality.STANDARD)
		{
			this.videoPlayer.Pause();
		}
		else
		{
			this.videoPlayer.Pause();
			this.audioPlayer.Pause();
		}
		this.OnVideoPaused.Invoke();
	}

	
	public void PlayPause()
	{
		if (this.youtubeUrlReady && this.videoPlayer.isPrepared)
		{
			if (!this.pauseCalled)
			{
				this.Pause();
				return;
			}
			this.Play();
		}
	}

	
	private void Update()
	{
		if (!this.loadYoutubeUrlsOnly && this.showPlayerControls && this.autoHideControlsTime > 0)
		{
			if (this.UserInteract())
			{
				this.hideScreenTime = 0f;
				if (this.mainControllerUi != null)
				{
					this.mainControllerUi.SetActive(true);
				}
			}
			else
			{
				this.hideScreenTime += Time.deltaTime;
				if (this.hideScreenTime >= (float)this.autoHideControlsTime)
				{
					this.hideScreenTime = (float)this.autoHideControlsTime;
					this.HideControllers();
				}
			}
		}
	}

	
	public void Seek(float time)
	{
		this.waitAudioSeek = true;
		this.Pause();
		if (this.videoQuality == YoutubePlayer.YoutubeVideoQuality.STANDARD)
		{
			this.videoPlayer.time = (double)time;
			return;
		}
		this.audioPlayer.time = (double)time;
	}

	
	private void HideControllers()
	{
		if (this.mainControllerUi != null)
		{
			this.mainControllerUi.SetActive(false);
			this.showingVolume = false;
			this.showingPlaybackSpeed = false;
			this.volumeSlider.gameObject.SetActive(false);
			this.playbackSpeed.gameObject.SetActive(false);
		}
	}

	
	public void Volume()
	{
		if (this.videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
		{
			this.audioPlayer.SetDirectAudioVolume(0, this.volumeSlider.value);
			this.videoPlayer.SetDirectAudioVolume(0, this.volumeSlider.value);
			return;
		}
		if (this.videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
		{
			this.videoPlayer.GetComponent<AudioSource>().volume = this.volumeSlider.value;
			this.videoPlayer.SetDirectAudioVolume(0, this.volumeSlider.value);
			return;
		}
		this.videoPlayer.GetComponent<AudioSource>().volume = this.volumeSlider.value;
		this.videoPlayer.SetDirectAudioVolume(0, this.volumeSlider.value);
	}

	
	public void Speed()
	{
		if (this.videoPlayer.canSetPlaybackSpeed)
		{
			if (this.playbackSpeed.value == 0f)
			{
				this.videoPlayer.playbackSpeed = 0.5f;
				this.audioPlayer.playbackSpeed = 0.5f;
				return;
			}
			this.videoPlayer.playbackSpeed = this.playbackSpeed.value;
			this.audioPlayer.playbackSpeed = this.playbackSpeed.value;
		}
	}

	
	public void VolumeSlider()
	{
		if (this.showingVolume)
		{
			this.showingVolume = false;
			this.volumeSlider.gameObject.SetActive(false);
			return;
		}
		this.showingVolume = true;
		this.volumeSlider.gameObject.SetActive(true);
	}

	
	public void PlaybackSpeedSlider()
	{
		if (this.showingPlaybackSpeed)
		{
			this.showingPlaybackSpeed = false;
			this.playbackSpeed.gameObject.SetActive(false);
			return;
		}
		this.showingPlaybackSpeed = true;
		this.playbackSpeed.gameObject.SetActive(true);
	}

	
	private void VideoPreparedSeek(VideoPlayer p)
	{
	}

	
	private void AudioPreparedSeek(VideoPlayer p)
	{
	}

	
	public void Stop()
	{
		if (!this.playUsingInternalDevicePlayer)
		{
			if (this.audioPlayer != null)
			{
				this.audioPlayer.seekCompleted -= this.AudioSeeked;
			}
			this.videoPlayer.seekCompleted -= this.VideoSeeked;
			this.videoPlayer.frameDropped -= this.VideoPlayer_frameDropped;
			if (this.audioPlayer != null)
			{
				this.audioPlayer.frameDropped -= this.AudioPlayer_frameDropped;
			}
			this.videoPlayer.Stop();
			if (!this.lowRes && this.audioPlayer != null)
			{
				this.audioPlayer.Stop();
			}
		}
	}

	
	private void SeekVideoDone(VideoPlayer vp)
	{
		this.videoSeekDone = true;
		this.videoPlayer.seekCompleted -= this.SeekVideoDone;
		if (!this.lowRes)
		{
			if (this.videoSeekDone && this.videoAudioSeekDone)
			{
				this.isSyncing = false;
				base.StartCoroutine(this.SeekFinished());
			}
		}
		else
		{
			this.isSyncing = false;
			this.HideLoading();
		}
	}

	
	private void SeekVideoAudioDone(VideoPlayer vp)
	{
		Debug.Log("NAAN");
		this.videoAudioSeekDone = true;
		this.audioPlayer.seekCompleted -= this.SeekVideoAudioDone;
		if (!this.lowRes && this.videoSeekDone && this.videoAudioSeekDone)
		{
			this.isSyncing = false;
			base.StartCoroutine(this.SeekFinished());
		}
	}

	
	private IEnumerator SeekFinished()
	{
		yield return new WaitForEndOfFrame();
		this.HideLoading();
	}

	
	private string FormatTime(int time)
	{
		int num = time / 3600;
		int num2 = time % 3600 / 60;
		int num3 = time % 3600 % 60;
		if (num == 0 && num2 != 0)
		{
			return num2.ToString("00") + ":" + num3.ToString("00");
		}
		if (num == 0 && num2 == 0)
		{
			return "00:" + num3.ToString("00");
		}
		return string.Concat(new string[]
		{
			num.ToString("00"),
			":",
			num2.ToString("00"),
			":",
			num3.ToString("00")
		});
	}

	
	private bool UserInteract()
	{
		if (Application.isMobilePlatform)
		{
			return Input.touches.Length >= 1;
		}
		return Input.GetMouseButtonDown(0) || Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f;
	}

	
	public void DecryptDownloadUrl(string encryptedUrlVideo, string encrytedUrlAudio, string html, bool videoOnly)
	{
		this.EncryptUrlForAudio = encrytedUrlAudio;
		this.EncryptUrlForVideo = encryptedUrlVideo;
		if (videoOnly)
		{
			string[] array = this.EncryptUrlForVideo.Replace("&sig=", "|").Replace("lsig=", "|").Replace("&ratebypass=yes", "").Split(new char[]
			{
				'|'
			});
			this.lsigForVideo = array[array.Length - 2];
			this.encryptedSignatureVideo = array[array.Length - 1];
			base.StartCoroutine(this.Downloader(YoutubePlayer.jsUrl, false));
			return;
		}
		string[] array2 = this.EncryptUrlForVideo.Replace("&sig=", "|").Replace("lsig=", "|").Replace("&ratebypass=yes", "").Split(new char[]
		{
			'|'
		});
		this.lsigForVideo = array2[array2.Length - 2];
		this.encryptedSignatureVideo = array2[array2.Length - 1];
		string[] array3 = this.EncryptUrlForAudio.Replace("&sig=", "|").Replace("lsig=", "|").Replace("&ratebypass=yes", "").Split(new char[]
		{
			'|'
		});
		this.lsigForAudio = array3[array3.Length - 2];
		this.encryptedSignatureAudio = array3[array3.Length - 1];
		base.StartCoroutine(this.Downloader(YoutubePlayer.jsUrl, true));
	}

	
	public void ReadyForExtract(string r, bool audioExtract)
	{
		if (audioExtract)
		{
			this.SetMasterUrlForAudio(r);
			if (SystemInfo.processorCount > 1)
			{
				this.thread1 = new Thread(delegate()
				{
					this.DoRegexFunctionsForAudio(r);
				});
				this.thread1.Start();
				return;
			}
			this.DoRegexFunctionsForAudio(r);
		}
		else
		{
			this.SetMasterUrlForVideo(r);
			if (SystemInfo.processorCount > 1)
			{
				this.thread1 = new Thread(delegate()
				{
					this.DoRegexFunctionsForVideo(r);
				});
				this.thread1.Start();
				return;
			}
			this.DoRegexFunctionsForVideo(r);
		}
	}

	
	private IEnumerator Downloader(string uri, bool audio)
	{
		UnityWebRequest request = UnityWebRequest.Get(uri);
		yield return request.SendWebRequest();
		this.ReadyForExtract(request.downloadHandler.text, audio);
	}

	
	private IEnumerator WebGlRequest(string videoID)
	{
		UnityWebRequest request = UnityWebRequest.Get("https://lightshaftstream.herokuapp.com/api/info?url=" + videoID + "&format=best&flatten=true");
		yield return request.SendWebRequest();
		this.startedPlayingWebgl = false;
		this.webGlResults = new YoutubePlayer.YoutubeResultIds();
		JSONNode jsonnode = JSON.Parse(request.downloadHandler.text);
		JSONNode jsonnode2 = jsonnode["videos"][0]["formats"];
		this.webGlResults.bestFormatWithAudioIncluded = jsonnode["videos"][0]["url"];
		for (int i = 0; i < jsonnode2.Count; i++)
		{
			if (jsonnode2[i]["format_id"] == "160")
			{
				this.webGlResults.lowQuality = jsonnode2[i]["url"];
			}
			else if (jsonnode2[i]["format_id"] == "133")
			{
				this.webGlResults.lowQuality = jsonnode2[i]["url"];
			}
			else if (jsonnode2[i]["format_id"] == "134")
			{
				this.webGlResults.standardQuality = jsonnode2[i]["url"];
			}
			else if (jsonnode2[i]["format_id"] == "136")
			{
				this.webGlResults.hdQuality = jsonnode2[i]["url"];
			}
			else if (jsonnode2[i]["format_id"] == "137")
			{
				this.webGlResults.fullHdQuality = jsonnode2[i]["url"];
			}
			else if (jsonnode2[i]["format_id"] == "266")
			{
				this.webGlResults.ultraHdQuality = jsonnode2[i]["url"];
			}
			else if (jsonnode2[i]["format_id"] == "139")
			{
				this.webGlResults.audioUrl = jsonnode2[i]["url"];
			}
		}
		this.WebGlGetVideo(this.webGlResults.bestFormatWithAudioIncluded);
	}

	
	public void WebGlGetVideo(string url)
	{
		this.logTest = "Getting Url Player";
		string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(url));
		this.videoUrl = "https://youtubewebgl.herokuapp.com/download.php?mime=video/mp4&title=generatedvideo&token=" + str;
		this.videoQuality = YoutubePlayer.YoutubeVideoQuality.STANDARD;
		this.logTest = this.videoUrl + " Done";
		Debug.Log("Play!! " + this.videoUrl);
		this.videoPlayer.source = VideoSource.Url;
		this.videoPlayer.url = this.videoUrl;
		this.videoPlayer.Prepare();
		this.videoPlayer.prepareCompleted += this.WeblPrepared;
	}

	
	private void WeblPrepared(VideoPlayer source)
	{
		this.startedPlayingWebgl = true;
		base.StartCoroutine(this.WebGLPlay());
		this.logTest = "Playing!!";
	}

	
	private IEnumerator WebGLPlay()
	{
		yield return new WaitForSeconds(2f);
		this.StartPlayingWebgl();
	}

	
	private void OnGUI()
	{
		if (this.debug)
		{
			GUI.Label(new Rect(0f, 0f, 400f, 30f), this.logTest);
		}
	}

	
	public void SetMasterUrlForAudio(string url)
	{
		this.masterURLForAudio = url;
	}

	
	public void SetMasterUrlForVideo(string url)
	{
		this.masterURLForVideo = url;
	}

	
	public void DoRegexFunctionsForVideo(string jsF)
	{
		this.masterURLForVideo = jsF;
		string text = "";
		this.patternNames = this.magicResult.regexForFuncName;
		foreach (string pattern in this.patternNames)
		{
			string value = Regex.Match(jsF, pattern).Groups[1].Value;
			if (!string.IsNullOrEmpty(value))
			{
				text = value;
				break;
			}
		}
		if (text.Contains("$"))
		{
			text = "\\" + text;
		}
		Debug.Log(text);
		string pattern2 = "(?!h\\.)" + text + "=function\\(\\w+\\)\\{.*?\\}";
		string value2 = Regex.Match(jsF, pattern2, RegexOptions.Singleline).Value;
		Debug.Log(value2);
		string[] array2 = value2.Split(new char[]
		{
			';'
		});
		string text2 = "";
		string text3 = "";
		string text4 = "";
		string text5 = "";
		foreach (string currentLine in array2.Skip(1).Take(array2.Length - 2))
		{
			if (!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text4))
			{
				break;
			}
			string functionFromLine = YoutubePlayer.GetFunctionFromLine(currentLine);
			string pattern3 = string.Format("{0}:\\bfunction\\b\\(\\w+\\)", functionFromLine);
			string pattern4 = string.Format("{0}:\\bfunction\\b\\([a],b\\).(\\breturn\\b)?.?\\w+\\.", functionFromLine);
			string pattern5 = string.Format("{0}:\\bfunction\\b\\(\\w+\\,\\w\\).\\bvar\\b.\\bc=a\\b", functionFromLine);
			if (functionFromLine == "nt")
			{
				string pattern6 = "encodeURIComponent:\\bfunction\\b\\(\\w+\\)";
				if (!Regex.Match(jsF, pattern6).Success && Regex.Matches(jsF, pattern3).Count > 1)
				{
					text2 = functionFromLine;
				}
			}
			else if (Regex.Match(jsF, pattern3).Success)
			{
				text2 = functionFromLine;
			}
			if (Regex.Match(jsF, pattern4).Success)
			{
				text3 = functionFromLine;
			}
			if (Regex.Match(jsF, pattern5).Success)
			{
				text4 = functionFromLine;
			}
		}
		foreach (string text6 in array2.Skip(1).Take(array2.Length - 2))
		{
			string functionFromLine = YoutubePlayer.GetFunctionFromLine(text6);
			Match match;
			if ((match = Regex.Match(text6, "\\(\\w+,(?<index>\\d+)\\)")).Success && functionFromLine == text4)
			{
				text5 = text5 + "w" + match.Groups["index"].Value + " ";
			}
			if ((match = Regex.Match(text6, "\\(\\w+,(?<index>\\d+)\\)")).Success && functionFromLine == text3)
			{
				text5 = text5 + "s" + match.Groups["index"].Value + " ";
			}
			if (functionFromLine == text2)
			{
				text5 += "r ";
			}
		}
		text5 = text5.Trim();
		if (string.IsNullOrEmpty(text5))
		{
			Debug.Log("Operation is empty for low qual, trying again.");
			if (this.canUpdate)
			{
				YoutubePlayer.needUpdate = true;
				this.canUpdate = false;
			}
			this.decryptedVideoUrlResult = null;
			return;
		}
		string newValue = MagicHands.DecipherWithOperations(this.encryptedSignatureVideo, text5);
		this.decryptedVideoUrlResult = HTTPHelperYoutube.ReplaceQueryStringParameter(this.EncryptUrlForVideo, YoutubePlayer.SignatureQuery, newValue, this.lsigForVideo);
		this.decryptedUrlForVideo = true;
	}

	
	private static int GetOpIndex(string op)
	{
		return int.Parse(new Regex(".(\\d+)").Match(op).Result("$1"));
	}

	
	private static char[] SpliceFunction(char[] a, int b)
	{
		return a.Splice(b);
	}

	
	private static char[] SwapFunction(char[] a, int b)
	{
		char c = a[0];
		a[0] = a[b % a.Length];
		a[b % a.Length] = c;
		return a;
	}

	
	private static char[] ReverseFunction(char[] a)
	{
		Array.Reverse(a);
		return a;
	}

	
	public void DoRegexFunctionsForAudio(string jsF)
	{
		this.masterURLForAudio = jsF;
		string input = this.masterURLForAudio;
		this.patternNames = this.magicResult.regexForFuncName;
		string text = "";
		foreach (string pattern in this.patternNames)
		{
			string value = Regex.Match(input, pattern).Groups[1].Value;
			if (!string.IsNullOrEmpty(value))
			{
				text = value;
				break;
			}
		}
		if (text.Contains("$"))
		{
			text = "\\" + text;
		}
		string pattern2 = "(?!h\\.)" + text + "=function\\(\\w+\\)\\{.*?join.*\\};";
		string[] array2 = Regex.Match(input, pattern2).Value.Split(new char[]
		{
			';'
		});
		string text2 = "";
		string text3 = "";
		string text4 = "";
		string text5 = "";
		foreach (string currentLine in array2.Skip(1).Take(array2.Length - 2))
		{
			if (!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text4))
			{
				break;
			}
			string functionFromLine = YoutubePlayer.GetFunctionFromLine(currentLine);
			string pattern3 = string.Format("{0}:\\bfunction\\b\\(\\w+\\)", functionFromLine);
			string pattern4 = string.Format("{0}:\\bfunction\\b\\([a],b\\).(\\breturn\\b)?.?\\w+\\.", functionFromLine);
			string pattern5 = string.Format("{0}:\\bfunction\\b\\(\\w+\\,\\w\\).\\bvar\\b.\\bc=a\\b", functionFromLine);
			if (functionFromLine == "nt")
			{
				string pattern6 = "encodeURIComponent:\\bfunction\\b\\(\\w+\\)";
				if (!Regex.Match(input, pattern6).Success && Regex.Matches(input, pattern3).Count > 1)
				{
					text2 = functionFromLine;
				}
			}
			else if (Regex.Match(input, pattern3).Success)
			{
				text2 = functionFromLine;
			}
			if (Regex.Match(input, pattern4).Success)
			{
				text3 = functionFromLine;
			}
			if (Regex.Match(input, pattern5).Success)
			{
				text4 = functionFromLine;
			}
		}
		foreach (string text6 in array2.Skip(1).Take(array2.Length - 2))
		{
			string functionFromLine = YoutubePlayer.GetFunctionFromLine(text6);
			Match match;
			if ((match = Regex.Match(text6, "\\(\\w+,(?<index>\\d+)\\)")).Success && functionFromLine == text4)
			{
				text5 = text5 + "w" + match.Groups["index"].Value + " ";
			}
			if ((match = Regex.Match(text6, "\\(\\w+,(?<index>\\d+)\\)")).Success && functionFromLine == text3)
			{
				text5 = text5 + "s" + match.Groups["index"].Value + " ";
			}
			if (functionFromLine == text2)
			{
				text5 += "r ";
			}
		}
		text5 = text5.Trim();
		if (string.IsNullOrEmpty(text5))
		{
			Debug.Log("Operation is empty, trying again.");
			if (this.canUpdate)
			{
				YoutubePlayer.needUpdate = true;
				this.canUpdate = false;
			}
			this.decryptedAudioUrlResult = null;
			this.decryptedVideoUrlResult = null;
		}
		else
		{
			string newValue = MagicHands.DecipherWithOperations(this.encryptedSignatureAudio, text5);
			string newValue2 = MagicHands.DecipherWithOperations(this.encryptedSignatureVideo, text5);
			this.decryptedAudioUrlResult = HTTPHelperYoutube.ReplaceQueryStringParameter(this.EncryptUrlForAudio, YoutubePlayer.SignatureQuery, newValue, this.lsigForAudio);
			this.decryptedVideoUrlResult = HTTPHelperYoutube.ReplaceQueryStringParameter(this.EncryptUrlForVideo, YoutubePlayer.SignatureQuery, newValue2, this.lsigForVideo);
		}
		this.decryptedUrlForAudio = true;
	}

	
	private void DelayForAudio()
	{
		this.decryptedUrlForVideo = true;
	}

	
	private static string GetFunctionFromLine(string currentLine)
	{
		return new Regex("\\w+\\.(?<functionID>\\w+)\\(").Match(currentLine).Groups["functionID"].Value;
	}

	
	public IEnumerator WebGlRequest(Action<string> callback, string id, string host)
	{
		Debug.Log(host + "getvideo.php?videoid=" + id + "&type=Download");
		UnityWebRequest request = UnityWebRequest.Get(host + "getvideo.php?videoid=" + id + "&type=Download");
		yield return request.SendWebRequest();
		callback(request.downloadHandler.text);
	}

	
	public void GetDownloadUrls(Action callback, string videoUrl, YoutubePlayer player)
	{
		if (videoUrl == null)
		{
			throw new ArgumentNullException("videoUrl");
		}
		if (!YoutubePlayer.TryNormalizeYoutubeUrl(videoUrl, out videoUrl))
		{
			throw new ArgumentException("URL is not a valid youtube URL!");
		}
		base.StartCoroutine(this.DownloadYoutubeUrl(videoUrl, callback, player));
	}

	
	private IEnumerator YoutubeURLDownloadFinished(Action callback, YoutubePlayer player)
	{
		string text = this.youtubeUrl.Replace("https://youtube.com/watch?v=", "");
		string json = string.Empty;
		if (Regex.IsMatch(this.jsonForHtmlVersion, "[\"\\']status[\"\\']\\s*:\\s*[\"\\']LOGIN_REQUIRED"))
		{
			Debug.Log("MM");
			string uri = "https://www.youtube.com/get_video_info?video_id=" + text + "&eurl=https://youtube.googleapis.com/v/" + text;
			UnityWebRequest request = UnityWebRequest.Get(uri);
			request.SetRequestHeader("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:10.0) Gecko/20100101 Firefox/10.0 (Chrome)");
			yield return request.SendWebRequest();
			if (request.isNetworkError)
			{
				Debug.Log("Youtube UnityWebRequest isNetworkError!");
			}
			else if (request.isHttpError)
			{
				Debug.Log("Youtube UnityWebRequest isHttpError!");
			}
			else if (request.responseCode != 200L)
			{
				Debug.Log("Youtube UnityWebRequest responseCode:" + request.responseCode);
			}
			json = UnityWebRequest.UnEscapeURL(HTTPHelperYoutube.ParseQueryString(request.downloadHandler.text)["player_response"]);
			request = null;
		}
		else
		{
			try
			{
				Match match = new Regex("ytplayer\\.config\\s*=\\s*(\\{.+?\\});", RegexOptions.Multiline).Match(this.jsonForHtmlVersion);
				if (match.Success)
				{
					string text2 = match.Result("$1");
					if (!text2.Contains("raw_player_response:ytInitialPlayerResponse"))
					{
						json = JObject.Parse(text2)["args"]["player_response"].ToString();
					}
				}
				match = new Regex("ytInitialPlayerResponse\\s*=\\s*({.+?})\\s*;\\s*(?:var\\s+meta|</script|\\n)", RegexOptions.Multiline).Match(this.jsonForHtmlVersion);
				if (match.Success)
				{
					json = match.Result("$1");
				}
				match = new Regex("ytInitialPlayerResponse\\s*=\\s*({.+?})\\s*;", RegexOptions.Multiline).Match(this.jsonForHtmlVersion);
				if (match.Success)
				{
					json = match.Result("$1");
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Source + " " + ex.StackTrace);
				Debug.Log("retry!");
			}
		}
		if (this.downloadYoutubeUrlResponse.isValid)
		{
			if (YoutubePlayer.IsVideoUnavailable(this.downloadYoutubeUrlResponse.data))
			{
				throw new VideoNotAvailableException();
			}
			try
			{
				JObject json2 = JObject.Parse(json);
				string text3 = new Regex("\"jsUrl\"\\s*:\\s*\"([^\"]+)\"").Match(this.jsonForHtmlVersion).Result("$1").Replace("\\/", "/");
				YoutubePlayer.jsUrl = "https://www.youtube.com" + text3;
				if (this.debug)
				{
					Debug.Log(YoutubePlayer.jsUrl);
				}
				string text4 = YoutubePlayer.TryMatchHtmlVersion(text3, this.magicResult.regexForHtmlPlayerVersion);
				if (this.debug)
				{
					Debug.Log(text4);
				}
				this.htmlVersion = text4;
				string message = YoutubePlayer.GetVideoTitle(json2);
				if (this.debug)
				{
					Debug.Log(message);
				}
				List<VideoInfo> list = YoutubePlayer.GetVideoInfos(YoutubePlayer.ExtractDownloadUrls(json2), message).ToList<VideoInfo>();
				if (string.IsNullOrEmpty(this.htmlVersion))
				{
					this.RetryPlayYoutubeVideo();
				}
				this.youtubeVideoInfos = list;
				foreach (VideoInfo videoInfo in this.youtubeVideoInfos)
				{
					videoInfo.HtmlPlayerVersion = text4;
				}
				callback();
			}
			catch (Exception ex2)
			{
				if (!this.loadYoutubeUrlsOnly)
				{
					Debug.Log("Resolver Exception!: " + ex2.Message);
					Debug.Log(ex2.Source + " " + ex2.StackTrace);
					Debug.Log(Application.persistentDataPath);
					if (Application.isEditor && this.debug)
					{
						YoutubePlayer.WriteLog("log_download_exception", "jsonForHtml: " + this.jsonForHtmlVersion);
					}
					Debug.Log("retry!");
					if (player != null)
					{
						player.RetryPlayYoutubeVideo();
					}
					else
					{
						Debug.LogError("Connection to Youtube Server Error! Try Again");
					}
				}
			}
		}
	}

	
	public static bool TryNormalizeYoutubeUrl(string url, out string normalizedUrl)
	{
		url = url.Trim();
		url = url.Replace("youtu.be/", "youtube.com/watch?v=");
		url = url.Replace("www.youtube", "youtube");
		url = url.Replace("youtube.com/embed/", "youtube.com/watch?v=");
		if (url.Contains("/v/"))
		{
			url = "https://youtube.com" + new Uri(url).AbsolutePath.Replace("/v/", "/watch?v=");
		}
		url = url.Replace("/watch#", "/watch?");
		string str;
		if (!HTTPHelperYoutube.ParseQueryString(url).TryGetValue("v", out str))
		{
			normalizedUrl = null;
			return false;
		}
		normalizedUrl = "https://youtube.com/watch?v=" + str;
		return true;
	}

	
	private static IEnumerable<YoutubePlayer.ExtractionInfo> ExtractDownloadUrls(JObject json)
	{
		List<string> urls = new List<string>();
		List<string> stringList = new List<string>();
		JObject jobject = json;
		if (jobject["streamingData"][(object) "formats"][(object) 0][(object) "cipher"] != null)
		{
			foreach (JToken jtoken in (IEnumerable<JToken>) jobject["streamingData"][(object) "formats"])
				stringList.Add(jtoken[(object) "cipher"].ToString());
			foreach (JToken jtoken in (IEnumerable<JToken>) jobject["streamingData"][(object) "adaptiveFormats"])
				stringList.Add(jtoken[(object) "cipher"].ToString());
		}
		else if (jobject["streamingData"][(object) "formats"][(object) 0][(object) "signatureCipher"] != null)
		{
			foreach (JToken jtoken in (IEnumerable<JToken>) jobject["streamingData"][(object) "formats"])
				stringList.Add(jtoken[(object) "signatureCipher"].ToString());
			foreach (JToken jtoken in (IEnumerable<JToken>) jobject["streamingData"][(object) "adaptiveFormats"])
				stringList.Add(jtoken[(object) "signatureCipher"].ToString());
		}
		else
		{
			foreach (JToken jtoken in (IEnumerable<JToken>) jobject["streamingData"][(object) "formats"])
				urls.Add(jtoken[(object) "url"].ToString());
			foreach (JToken jtoken in (IEnumerable<JToken>) jobject["streamingData"][(object) "adaptiveFormats"])
				urls.Add(jtoken[(object) "url"].ToString());
		}

		foreach (string s in stringList)
		{
			IDictionary<string, string> queryString = HTTPHelperYoutube.ParseQueryString(s);
			bool flag = false;
			YoutubePlayer.SignatureQuery = !queryString.ContainsKey("sp") ? "signatures" : "sig";
			string url;
			if (queryString.ContainsKey("s") || queryString.ContainsKey("signature"))
			{
				flag = queryString.ContainsKey("s");
				string str = queryString.ContainsKey("s") ? queryString["s"] : queryString["signature"];
				url = (!(YoutubePlayer.sp != "none")
					? string.Format("{0}&{1}={2}", (object) queryString["url"], (object) YoutubePlayer.SignatureQuery,
						(object) str)
					: string.Format("{0}&{1}={2}", (object) queryString["url"], (object) YoutubePlayer.SignatureQuery,
						(object) str)) + (queryString.ContainsKey("fallback_host")
					? "&fallback_host=" + queryString["fallback_host"]
					: string.Empty);
			}
			else
				url = queryString["url"];

			string str1 = HTTPHelperYoutube.UrlDecode(HTTPHelperYoutube.UrlDecode(url));
			if (!HTTPHelperYoutube.ParseQueryString(str1).ContainsKey("ratebypass"))
				str1 += string.Format("&{0}={1}", (object) "ratebypass", (object) "yes");
			yield return new YoutubePlayer.ExtractionInfo()
			{
				RequiresDecryption = flag,
				Uri = new Uri(str1)
			};
		}

		foreach (string url in urls)
		{
			string str = HTTPHelperYoutube.UrlDecode(HTTPHelperYoutube.UrlDecode(url));
			if (!HTTPHelperYoutube.ParseQueryString(str).ContainsKey("ratebypass"))
				str += string.Format("&{0}={1}", (object) "ratebypass", (object) "yes");
			yield return new YoutubePlayer.ExtractionInfo()
			{
				RequiresDecryption = false,
				Uri = new Uri(str)
			};
		}

		// co: dotPeek
		// List<string> urls = new List<string>();
		// List<string> list = new List<string>();
		// if (json["streamingData"]["formats"][0]["cipher"] != null)
		// {
		// 	foreach (JToken jtoken in ((IEnumerable<JToken>)json["streamingData"]["formats"]))
		// 	{
		// 		list.Add(jtoken["cipher"].ToString());
		// 	}
		// 	using (IEnumerator<JToken> enumerator = ((IEnumerable<JToken>)json["streamingData"]["adaptiveFormats"]).GetEnumerator())
		// 	{
		// 		while (enumerator.MoveNext())
		// 		{
		// 			JToken jtoken2 = enumerator.Current;
		// 			list.Add(jtoken2["cipher"].ToString());
		// 		}
		// 		goto IL_2AB;
		// 	}
		// }
		// if (json["streamingData"]["formats"][0]["signatureCipher"] != null)
		// {
		// 	foreach (JToken jtoken3 in ((IEnumerable<JToken>)json["streamingData"]["formats"]))
		// 	{
		// 		list.Add(jtoken3["signatureCipher"].ToString());
		// 	}
		// 	using (IEnumerator<JToken> enumerator = ((IEnumerable<JToken>)json["streamingData"]["adaptiveFormats"]).GetEnumerator())
		// 	{
		// 		while (enumerator.MoveNext())
		// 		{
		// 			JToken jtoken4 = enumerator.Current;
		// 			list.Add(jtoken4["signatureCipher"].ToString());
		// 		}
		// 		goto IL_2AB;
		// 	}
		// }
		// foreach (JToken jtoken5 in ((IEnumerable<JToken>)json["streamingData"]["formats"]))
		// {
		// 	urls.Add(jtoken5["url"].ToString());
		// }
		// foreach (JToken jtoken6 in ((IEnumerable<JToken>)json["streamingData"]["adaptiveFormats"]))
		// {
		// 	urls.Add(jtoken6["url"].ToString());
		// }
		// IL_2AB:
		// foreach (string s in list)
		// {
		// 	IDictionary<string, string> dictionary = HTTPHelperYoutube.ParseQueryString(s);
		// 	bool requiresDecryption = false;
		// 	if (dictionary.ContainsKey("sp"))
		// 	{
		// 		YoutubePlayer.SignatureQuery = "sig";
		// 	}
		// 	else
		// 	{
		// 		YoutubePlayer.SignatureQuery = "signatures";
		// 	}
		// 	string text;
		// 	if (dictionary.ContainsKey("s") || dictionary.ContainsKey("signature"))
		// 	{
		// 		requiresDecryption = dictionary.ContainsKey("s");
		// 		string arg = dictionary.ContainsKey("s") ? dictionary["s"] : dictionary["signature"];
		// 		if (YoutubePlayer.sp != "none")
		// 		{
		// 			text = string.Format("{0}&{1}={2}", dictionary["url"], YoutubePlayer.SignatureQuery, arg);
		// 		}
		// 		else
		// 		{
		// 			text = string.Format("{0}&{1}={2}", dictionary["url"], YoutubePlayer.SignatureQuery, arg);
		// 		}
		// 		string str = dictionary.ContainsKey("fallback_host") ? ("&fallback_host=" + dictionary["fallback_host"]) : string.Empty;
		// 		text += str;
		// 	}
		// 	else
		// 	{
		// 		text = dictionary["url"];
		// 	}
		// 	text = HTTPHelperYoutube.UrlDecode(text);
		// 	text = HTTPHelperYoutube.UrlDecode(text);
		// 	if (!HTTPHelperYoutube.ParseQueryString(text).ContainsKey("ratebypass"))
		// 	{
		// 		text += string.Format("&{0}={1}", "ratebypass", "yes");
		// 	}
		// 	yield return new YoutubePlayer.ExtractionInfo
		// 	{
		// 		RequiresDecryption = requiresDecryption,
		// 		Uri = new Uri(text)
		// 	};
		// }
		// List<string>.Enumerator enumerator2 = default(List<string>.Enumerator);
		// foreach (string text2 in urls)
		// {
		// 	text2 = HTTPHelperYoutube.UrlDecode(text2);
		// 	text2 = HTTPHelperYoutube.UrlDecode(text2);
		// 	if (!HTTPHelperYoutube.ParseQueryString(text2).ContainsKey("ratebypass"))
		// 	{
		// 		text2 += string.Format("&{0}={1}", "ratebypass", "yes");
		// 	}
		// 	yield return new YoutubePlayer.ExtractionInfo
		// 	{
		// 		RequiresDecryption = false,
		// 		Uri = new Uri(text2)
		// 	};
		// }
		// enumerator2 = default(List<string>.Enumerator);
		// yield break;
		// yield break;
	}

	
	private static string GetAdaptiveStreamMap(JObject json)
	{
		JToken jtoken = json["args"]["adaptive_fmts"];
		if (jtoken == null)
		{
			jtoken = json["args"]["url_encoded_fmt_stream_map"];
			if (jtoken == null)
			{
				jtoken = JObject.Parse(json["args"]["player_response"].ToString())["streamingData"]["adaptiveFormats"];
			}
		}
		return jtoken.ToString();
	}

	
	private static string GetHtml5PlayerVersion(JObject json, string regexForHtmlPVersions)
	{
		Regex regex = new Regex(regexForHtmlPVersions);
		string text = json["assets"]["js"].ToString();
		YoutubePlayer.jsUrl = "https://www.youtube.com" + text;
		Match match = regex.Match(text);
		if (match.Success)
		{
			return match.Result("$1");
		}
		match = new Regex("player_ias(.+?).js").Match(text);
		if (match.Success)
		{
			return match.Result("$1");
		}
		return new Regex("player-(.+?).js").Match(text).Result("$1");
	}

	
	private static string TryMatchHtmlVersion(string input, string regexForHtmlPVersions)
	{
		Match match = new Regex(regexForHtmlPVersions).Match(input);
		if (match.Success)
		{
			return match.Result("$1");
		}
		match = new Regex("player_ias(.+?).js").Match(input);
		if (match.Success)
		{
			return match.Result("$1");
		}
		return new Regex("player-(.+?).js").Match(input).Result("$1");
	}

	
	private static string GetStreamMap(JObject json)
	{
		JToken jtoken = json["args"]["url_encoded_fmt_stream_map"];
		if (jtoken == null)
		{
			jtoken = JObject.Parse(json["args"]["player_response"].ToString())["streamingData"]["formats"];
		}
		string text = (jtoken == null) ? null : jtoken.ToString();
		if (text != null && !text.Contains("been+removed"))
		{
			return text;
		}
		if (text.Contains("been+removed"))
		{
			throw new VideoNotAvailableException("Video is removed or has an age restriction.");
		}
		return null;
	}

	
	private static IEnumerable<VideoInfo> GetVideoInfos(IEnumerable<YoutubePlayer.ExtractionInfo> extractionInfos, string videoTitle)
	{
		List<VideoInfo> list = new List<VideoInfo>();
		foreach (YoutubePlayer.ExtractionInfo extractionInfo in extractionInfos)
		{
			string s = HTTPHelperYoutube.ParseQueryString(extractionInfo.Uri.Query)["itag"];
			int formatCode = int.Parse(s);
			VideoInfo videoInfo2 = VideoInfo.Defaults.SingleOrDefault((VideoInfo videoInfo) => videoInfo.FormatCode == formatCode);
			if (videoInfo2 != null)
			{
				videoInfo2 = new VideoInfo(videoInfo2)
				{
					DownloadUrl = extractionInfo.Uri.ToString(),
					Title = videoTitle,
					RequiresDecryption = extractionInfo.RequiresDecryption
				};
			}
			else
			{
				videoInfo2 = new VideoInfo(formatCode)
				{
					DownloadUrl = extractionInfo.Uri.ToString()
				};
			}
			list.Add(videoInfo2);
		}
		return list;
	}

	
	private static string GetVideoTitle(JObject json)
	{
		JToken jtoken = json["videoDetails"]["title"];
		if (jtoken != null)
		{
			return jtoken.ToString();
		}
		return string.Empty;
	}

	
	private static bool IsVideoUnavailable(string pageSource)
	{
		return pageSource.Contains("<div id=\"watch-player-unavailable\">");
	}

	
	private IEnumerator DownloadUrl(string url, Action<string> callback, VideoInfo videoInfo)
	{
		UnityWebRequest request = UnityWebRequest.Get(url);
		request.SetRequestHeader("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:10.0) Gecko/20100101 Firefox/10.0 (Chrome)");
		yield return request.SendWebRequest();
		if (request.isNetworkError)
		{
			Debug.Log("Youtube UnityWebRequest isNetworkError!");
		}
		else if (request.isHttpError)
		{
			Debug.Log("Youtube UnityWebRequest isHttpError!");
		}
		else if (request.responseCode != 200L)
		{
			Debug.Log("Youtube UnityWebRequest responseCode:" + request.responseCode);
		}
	}

	
	private IEnumerator DownloadYoutubeUrl(string url, Action callback, YoutubePlayer player)
	{
		this.downloadYoutubeUrlResponse = new YoutubePlayer.DownloadUrlResponse();
		string str = url.Replace("https://youtube.com/watch?v=", "");
		string uri = "https://www.youtube.com/watch?v=" + str + "&gl=US&hl=en&has_verified=1&bpctr=9999999999";
		UnityWebRequest request = UnityWebRequest.Get(uri);
		request.SetRequestHeader("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:10.0) Gecko/20100101 Firefox/10.0 (Chrome)");
		yield return request.SendWebRequest();
		this.downloadYoutubeUrlResponse.httpCode = request.responseCode;
		if (request.isNetworkError)
		{
			Debug.Log("Youtube UnityWebRequest isNetworkError!");
		}
		else if (request.isHttpError)
		{
			Debug.Log("Youtube UnityWebRequest isHttpError!");
		}
		else if (request.responseCode == 200L)
		{
			if (request.downloadHandler != null && request.downloadHandler.text != null)
			{
				if (request.downloadHandler.isDone)
				{
					this.downloadYoutubeUrlResponse.isValid = true;
					this.jsonForHtmlVersion = request.downloadHandler.text;
					this.downloadYoutubeUrlResponse.data = request.downloadHandler.text;
				}
			}
			else
			{
				Debug.Log("Youtube UnityWebRequest Null response");
			}
		}
		else
		{
			Debug.Log("Youtube UnityWebRequest responseCode:" + request.responseCode);
		}
		base.StartCoroutine(this.YoutubeURLDownloadFinished(callback, player));
	}

	
	public static void WriteLog(string filename, string c)
	{
		string path = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/",
			filename,
			"_",
			DateTime.Now.ToString("ddMMyyyyhhmmssffff"),
			".txt"
		});
		Debug.Log("Log written in: " + Application.persistentDataPath);
		File.WriteAllText(path, c);
	}

	
	private static void ThrowYoutubeParseException(Exception innerException, string videoUrl)
	{
		throw new YoutubeParseException("Could not parse the Youtube page for URL " + videoUrl + "\nThis may be due to a change of the Youtube page structure.\nPlease report this bug at kelvinparkour@gmail.com with a subject message 'Parse Error' ", innerException);
	}

	
	public void TrySkip(PointerEventData eventData)
	{
		Vector2 vector;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.progress.rectTransform, eventData.position, eventData.pressEventCamera, out vector))
		{
			float pct = Mathf.InverseLerp(this.progress.rectTransform.rect.xMin, this.progress.rectTransform.rect.xMax, vector.x);
			this.SkipToPercent(pct);
		}
	}

	
	private void SkipToPercent(float pct)
	{
		float num = this.videoPlayer.frameCount * pct;
		this.videoPlayer.Pause();
		if (this.videoQuality != YoutubePlayer.YoutubeVideoQuality.STANDARD)
		{
			this.audioPlayer.Pause();
		}
		this.waitAudioSeek = true;
		if (this.videoQuality == YoutubePlayer.YoutubeVideoQuality.STANDARD)
		{
			this.videoPlayer.frame = (long)num;
			return;
		}
		this.audioPlayer.frame = (long)num;
	}

	
	private IEnumerator VideoSeekCall()
	{
		yield return new WaitForSeconds(1f);
		this.videoPlayer.time = this.audioPlayer.time;
	}

	
	private void VideoSeeked(VideoPlayer source)
	{
		if (!this.waitAudioSeek)
		{
			if (this.startedFromTime)
			{
				base.StartCoroutine(this.PlayNowFromTime(2f));
				return;
			}
			base.StartCoroutine(this.PlayNow());
		}
		else
		{
			if (this.startedFromTime)
			{
				base.StartCoroutine(this.PlayNowFromTime(2f));
				return;
			}
			base.StartCoroutine(this.PlayNow());
		}
	}

	
	private void AudioSeeked(VideoPlayer source)
	{
		if (!this.waitAudioSeek)
		{
			base.StartCoroutine(this.VideoSeekCall());
			return;
		}
		base.StartCoroutine(this.VideoSeekCall());
	}

	
	private IEnumerator WaitSync()
	{
		yield return new WaitForSeconds(2f);
		this.Play();
		base.Invoke("VerifyFrames", 2f);
	}

	
	private IEnumerator PlayNow()
	{
		if (this.videoQuality == YoutubePlayer.YoutubeVideoQuality.STANDARD)
		{
			yield return new WaitForSeconds(0f);
		}
		else
		{
			yield return new WaitForSeconds(1f);
		}
		if (!this.pauseCalled)
		{
			this.Play();
			base.StartCoroutine(this.ReleaseDrop());
		}
		else
		{
			base.StopCoroutine("PlayNow");
		}
	}

	
	private void CheckIfIsSync()
	{
	}

	
	private IEnumerator ReleaseDrop()
	{
		yield return new WaitForSeconds(2f);
	}

	
	private IEnumerator PlayNowFromTime(float time)
	{
		yield return new WaitForSeconds(time);
		this.startedFromTime = false;
		if (!this.pauseCalled)
		{
			this.Play();
		}
		else
		{
			base.StopCoroutine("PlayNowFromTime");
		}
	}

	
	private void AudioPlayer_frameDropped(VideoPlayer source)
	{
	}

	
	private void VideoPlayer_frameDropped(VideoPlayer source)
	{
	}

	
	private const string USER_AGENT = "Mozilla/5.0 (X11; Linux x86_64; rv:10.0) Gecko/20100101 Firefox/10.0 (Chrome)";

	
	private const bool INTERNALDEBUG = false;

	
	[Space]
	[Tooltip("You can put urls that start at a specific time example: 'https://youtu.be/1G1nCxxQMnA?t=67'")]
	public string youtubeUrl;

	
	[Space]
	[Space]
	[Tooltip("The desired video quality you want to play. It's in experimental mod, because we need to use 2 video players in qualities 720+, you can expect some desync, but we are working to find a definitive solution to that. Thanks to DASH format.")]
	public YoutubePlayer.YoutubeVideoQuality videoQuality;

	
	[Space]
	public bool customPlaylist;

	
	[DrawIf("customPlaylist", true, DrawIfAttribute.DisablingType.DontDraw)]
	public bool autoPlayNextVideo;

	
	[Header("If is a custom playlist put urls here")]
	public string[] youtubeUrls;

	
	private int currentUrlIndex;

	
	[Space]
	[Header("Playback Options")]
	[Space]
	[Tooltip("Start playing the video from a desired time")]
	public bool startFromSecond;

	
	[DrawIf("startFromSecond", true, DrawIfAttribute.DisablingType.DontDraw)]
	public int startFromSecondTime;

	
	[Space]
	[Tooltip("Play the video when the script initialize")]
	public bool autoPlayOnStart = true;

	
	[Header("For Mobiles Leave MP4 ")]
	public YoutubePlayer.VideoFormatType videoFormat;

	
	[Space]
	[Tooltip("Play or continue when OnEnable is called")]
	public bool autoPlayOnEnable;

	
	[Space]
	[Header("Use Device Video player (Standard quality only)")]
	[Tooltip("Play video in mobiles using the mobile device video player not unity internal player")]
	public bool playUsingInternalDevicePlayer;

	
	[Space]
	[Header("Only load the url to use in a custom player.")]
	[Space]
	[Tooltip("If you want to use your custom player, you can enable this and set the callback OnYoutubeUrlLoaded and get the public variables audioUrl or videoUrl of that script.")]
	public bool loadYoutubeUrlsOnly;

	
	[Space]
	[Header("Render the same video to more objects")]
	[Tooltip("Render the same video player material to a different materials, if you want")]
	public GameObject[] objectsToRenderTheVideoImage;

	
	[Space]
	[Header("Option for 3D video Only.")]
	[Tooltip("If the video is a 3D video sidebyside or Over/Under")]
	public bool is3DLayoutVideo;

	
	[DrawIf("is3DLayoutVideo", true, DrawIfAttribute.DisablingType.DontDraw)]
	public YoutubePlayer.Layout3D layout3d;

	
	[Space]
	[Header("Video Controller Canvas")]
	public GameObject videoControllerCanvas;

	
	[Space]
	public Camera mainCamera;

	
	[Space]
	[Header("Loading Settings")]
	[Tooltip("This enable and disable related to the loading needs.")]
	public GameObject loadingContent;

	
	[Header("Custom user Events To use with video player only")]
	[Tooltip("When the url's are loaded")]
	public UrlLoadEvent OnYoutubeUrlAreReady;

	
	[Tooltip("When the videos are ready to play")]
	public UnityEvent OnVideoReadyToStart;

	
	[Tooltip("When the video start playing")]
	public UnityEvent OnVideoStarted;

	
	[Tooltip("When the video pause")]
	public UnityEvent OnVideoPaused;

	
	[Tooltip("When the video finish")]
	public UnityEvent OnVideoFinished;

	
	[Space]
	[Header("The unity video players")]
	[Tooltip("The unity video player")]
	public VideoPlayer videoPlayer;

	
	[Tooltip("The audio player, (Needed for videos that dont have audio included 720p+)")]
	public VideoPlayer audioPlayer;

	
	[Space]
	[Tooltip("Show the output in the console")]
	public bool debug;

	
	[Space]
	[Tooltip("Ignore timeout is good for very low connections")]
	public bool ignoreTimeout;

	
	[Space]
	[SerializeField]
	[Header("If the video stucks you can try to disable this.")]
	private bool _skipOnDrop = true;

	
	[HideInInspector]
	public string videoUrl;

	
	[HideInInspector]
	public string audioUrl;

	
	[HideInInspector]
	public bool ForceGetWebServer;

	
	[Space]
	[Header("Screen Controls")]
	[Tooltip("Show the video controller in screen [slider with progress, video time, play pause, etc...]")]
	public bool showPlayerControls;

	
	private int maxRequestTime = 5;

	
	private float currentRequestTime;

	
	private int retryTimeUntilToRequestFromServer = 1;

	
	private int currentRetryTime;

	
	private bool gettingYoutubeURL;

	
	private bool videoAreReadyToPlay;

	
	private float lastPlayTime;

	
	private bool audioDecryptDone;

	
	private bool videoDecryptDone;

	
	private bool isRetry;

	
	private string lastTryVideoId;

	
	private float lastStartedTime;

	
	private bool youtubeUrlReady;

	
	private static bool needUpdate = false;

	
	private YoutubePlayer.YoutubeResultIds newRequestResults;

	
	private static string jsUrl;

	
	private const string serverURI = "https://lightshaftstream.herokuapp.com/api/info?url=";

	
	private const string formatURI = "&format=best&flatten=true";

	
	private const string VIDEOURIFORWEBGLPLAYER = "https://youtubewebgl.herokuapp.com/download.php?mime=video/mp4&title=generatedvideo&token=";

	
	private bool fullscreenModeEnabled;

	
	private long lastFrame = -1L;

	
	private double lastTimePlayed = double.PositiveInfinity;

	
	private bool videoEnded;

	
	private bool noAudioAtacched = default;

	
	private string videoTitle = "";

	
	private bool startedFromTime;

	
	private bool finishedCalled;

	
	private bool videoStarted;

	
	private float lastErrorTime;

	
	[HideInInspector]
	public bool pauseCalled;

	
	[DrawIf("showPlayerControls", true, DrawIfAttribute.DisablingType.DontDraw)]
	[Tooltip("Hide the controls if use not interact in desired time, 0 equals to not hide")]
	public int autoHideControlsTime;

	
	[DrawIf("showPlayerControls", true, DrawIfAttribute.DisablingType.DontDraw)]
	[Tooltip("The main controller ui parent")]
	public GameObject mainControllerUi;

	
	[DrawIf("showPlayerControls", true, DrawIfAttribute.DisablingType.DontDraw)]
	[Tooltip("Slider with duration and progress")]
	public Image progress;

	
	[DrawIf("showPlayerControls", true, DrawIfAttribute.DisablingType.DontDraw)]
	[Tooltip("Volume slider")]
	public Slider volumeSlider;

	
	[DrawIf("showPlayerControls", true, DrawIfAttribute.DisablingType.DontDraw)]
	[Tooltip("Playback speed")]
	public Slider playbackSpeed;

	
	[DrawIf("showPlayerControls", true, DrawIfAttribute.DisablingType.DontDraw)]
	[Tooltip("Current Time")]
	public Text currentTimeString;

	
	[DrawIf("showPlayerControls", true, DrawIfAttribute.DisablingType.DontDraw)]
	[Tooltip("Total Time")]
	public Text totalTimeString;

	
	private float totalVideoDuration;

	
	private float currentVideoDuration;

	
	private bool videoSeekDone;

	
	private bool videoAudioSeekDone;

	
	private bool lowRes;

	
	private float hideScreenTime;

	
	private float audioDuration;

	
	private bool showingPlaybackSpeed;

	
	private bool showingVolume;

	
	private string lsigForVideo;

	
	private string lsigForAudio;

	
	private Thread thread1;

	
	private YoutubePlayer.YoutubeResultIds webGlResults;

	
	private bool startedPlayingWebgl;

	
	private string logTest = "/";

	
	[HideInInspector]
	public bool isSyncing;

	
	[Header("Experimental")]
	public bool showThumbnailBeforeVideoLoad;

	
	private string thumbnailVideoID;

	
	private const string RateBypassFlag = "ratebypass";

	
	[HideInInspector]
	public static string SignatureQuery = "sig";

	
	[HideInInspector]
	public string encryptedSignatureVideo;

	
	[HideInInspector]
	public string encryptedSignatureAudio;

	
	[HideInInspector]
	private string masterURLForVideo;

	
	[HideInInspector]
	private string masterURLForAudio;

	
	private string[] patternNames = new string[]
	{
		""
	};

	
	[HideInInspector]
	public bool decryptedUrlForVideo;

	
	[HideInInspector]
	public bool decryptedUrlForAudio;

	
	[HideInInspector]
	public string decryptedVideoUrlResult = "";

	
	[HideInInspector]
	public string decryptedAudioUrlResult = "";

	
	public List<VideoInfo> youtubeVideoInfos;

	
	private string htmlVersion = "";

	
	private static string sp = "";

	
	[HideInInspector]
	public string EncryptUrlForVideo;

	
	[HideInInspector]
	public string EncryptUrlForAudio;

	
	private YoutubePlayer.DownloadUrlResponse downloadYoutubeUrlResponse;

	
	[HideInInspector]
	public string jsonForHtmlVersion = "";

	
	private bool waitAudioSeek;

	
	[HideInInspector]
	public bool checkIfSync;

	
	private YoutubePlayer.MagicContent magicResult;

	
	private bool canUpdate = true;

	
	public enum YoutubeVideoQuality
	{
		
		STANDARD,
		
		HD,
		
		FULLHD,
		
		UHD1440,
		
		UHD2160
	}

	
	public enum VideoFormatType
	{
		
		MP4,
		
		WEBM
	}

	
	public enum PlayerType
	{
		
		simple,
		
		advanced
	}

	
	public enum Layout3D
	{
		
		sideBySide,
		
		OverUnder
	}

	
	public class YoutubeResultIds
	{
		
		public string lowQuality;

		
		public string standardQuality;

		
		public string mediumQuality;

		
		public string hdQuality;

		
		public string fullHdQuality;

		
		public string ultraHdQuality;

		
		public string bestFormatWithAudioIncluded;

		
		public string audioUrl;
	}

	
	public class Html5PlayerResult
	{
		
		public Html5PlayerResult(string _script, string _result, bool _valid)
		{
			this.scriptName = _script;
			this.result = _result;
			this.isValid = _valid;
		}

		
		public string scriptName;

		
		public string result;

		
		public bool isValid;
	}

	
	private class DownloadUrlResponse
	{
		
		public DownloadUrlResponse()
		{
			this.data = null;
			this.isValid = false;
			this.httpCode = 0L;
		}

		
		public string data;

		
		public bool isValid;

		
		public long httpCode;
	}

	
	private class ExtractionInfo
	{
		
		
		
		public bool RequiresDecryption { get; set; }

		
		
		
		public Uri Uri { get; set; }
	}

	
	private class MagicContent
	{
		
		public string[] regexForFuncName;

		
		public string regexForHtmlJson;

		
		public string regexForHtmlPlayerVersion;

		
		public string[] defaultFuncName = new string[]
		{
			"(?:\\b|[^\\w$])([\\w$]{2})\\s*=\\s*function\\(\\s*a\\s*\\)\\s*{\\s*a\\s*=\\s*a\\.split\\(\\s*\"\"\\s*\\)",
			"(\\w+)=function\\(\\w+\\){(\\w+)=\\2\\.split\\(\\x22{2}\\);.*?return\\s+\\2\\.join\\(\\x22{2}\\)}",
			"\\b[cs]\\s*&&\\s*[adf]\\.set\\([^,]+\\s*,\\s*encodeURIComponent\\s*\\(\\s*([\\w$]+)\\("
		};

		
		public string defaultHtmlJson = "ytplayer\\.config\\s*=\\s*(\\{.+?\\});ytplayer";

		
		public string defaultHtmlPlayerVersion = "player_ias-(.+?).js";
	}
}
