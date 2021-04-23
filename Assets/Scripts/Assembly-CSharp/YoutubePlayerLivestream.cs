using System;
using System.Collections;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using YoutubeLight;


public class YoutubePlayerLivestream : MonoBehaviour
{
	
	private void Start()
	{
		this.GetLivestreamUrl(this._livestreamUrl);
	}

	
	public void GetLivestreamUrl(string url)
	{
		this.StartProcess(new Action<string>(this.OnLiveUrlLoaded), url);
	}

	
	public void StartProcess(Action<string> callback, string url)
	{
		base.StartCoroutine(this.DownloadYoutubeUrl(url, callback));
	}

	
	private void OnLiveUrlLoaded(string url)
	{
		Debug.Log("You can check how to use double clicking in that log");
		Debug.Log("This is the live url, pass to the player: " + url);
	}

	
	private IEnumerator DownloadYoutubeUrl(string url, Action<string> callback)
	{
		YoutubePlayerLivestream playerLivestream = this;
		playerLivestream.downloadYoutubeUrlResponse = new YoutubePlayerLivestream.DownloadUrlResponse();
		string videoId = url.Replace("https://youtube.com/watch?v=", "");
		string str = "https://www.youtube.com/watch?v=" + videoId + "&gl=US&hl=en&has_verified=1&bpctr=9999999999";
		UnityWebRequest request = UnityWebRequest.Get(url);
		request.SetRequestHeader("User-Agent",
			"Mozilla/5.0 (X11; Linux x86_64; rv:10.0) Gecko/20100101 Firefox/10.0 (Chrome)");
		yield return (object) request.SendWebRequest();
		playerLivestream.downloadYoutubeUrlResponse.httpCode = request.responseCode;
		if (request.isNetworkError)
			Debug.Log((object) "Youtube UnityWebRequest isNetworkError!");
		else if (request.isHttpError)
			Debug.Log((object) "Youtube UnityWebRequest isHttpError!");
		else if (request.responseCode == 200L)
		{
			if (request.downloadHandler != null && request.downloadHandler.text != null)
			{
				if (request.downloadHandler.isDone)
				{
					playerLivestream.downloadYoutubeUrlResponse.isValid = true;
					playerLivestream.downloadYoutubeUrlResponse.data = request.downloadHandler.text;
				}
			}
			else
				Debug.Log((object) "Youtube UnityWebRequest Null response");
		}
		else
			Debug.Log((object) ("Youtube UnityWebRequest responseCode:" + (object) request.responseCode));

		playerLivestream.StartCoroutine(
			playerLivestream.GetUrlFromJson(callback, videoId, request.downloadHandler.text));

		// co:dotPeek
		// this.downloadYoutubeUrlResponse = new YoutubePlayerLivestream.DownloadUrlResponse();
		// string videoId = url.Replace("https://youtube.com/watch?v=", "");
		// "https://www.youtube.com/watch?v=" + videoId + "&gl=US&hl=en&has_verified=1&bpctr=9999999999";
		// UnityWebRequest request = UnityWebRequest.Get(url);
		// request.SetRequestHeader("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:10.0) Gecko/20100101 Firefox/10.0 (Chrome)");
		// yield return request.SendWebRequest();
		// this.downloadYoutubeUrlResponse.httpCode = request.responseCode;
		// if (request.isNetworkError)
		// {
		// 	Debug.Log("Youtube UnityWebRequest isNetworkError!");
		// }
		// else if (request.isHttpError)
		// {
		// 	Debug.Log("Youtube UnityWebRequest isHttpError!");
		// }
		// else if (request.responseCode == 200L)
		// {
		// 	if (request.downloadHandler != null && request.downloadHandler.text != null)
		// 	{
		// 		if (request.downloadHandler.isDone)
		// 		{
		// 			this.downloadYoutubeUrlResponse.isValid = true;
		// 			this.downloadYoutubeUrlResponse.data = request.downloadHandler.text;
		// 		}
		// 	}
		// 	else
		// 	{
		// 		Debug.Log("Youtube UnityWebRequest Null response");
		// 	}
		// }
		// else
		// {
		// 	Debug.Log("Youtube UnityWebRequest responseCode:" + request.responseCode);
		// }
		// base.StartCoroutine(this.GetUrlFromJson(callback, videoId, request.downloadHandler.text));
		// yield break;
	}

	
	private IEnumerator GetUrlFromJson(Action<string> callback, string _videoID, string pageSource)
	{
		string json = string.Empty;
		if (Regex.IsMatch(pageSource, "[\"\\']status[\"\\']\\s*:\\s*[\"\\']LOGIN_REQUIRED"))
		{
			Debug.Log("MM");
			string uri = "https://www.youtube.com/get_video_info?video_id=" + _videoID + "&eurl=https://youtube.googleapis.com/v/" + _videoID;
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
			Match match = new Regex("ytplayer\\.config\\s*=\\s*(\\{.+?\\});", RegexOptions.Multiline).Match(pageSource);
			if (match.Success)
			{
				string text = match.Result("$1");
				if (!text.Contains("raw_player_response:ytInitialPlayerResponse"))
				{
					json = JObject.Parse(text)["args"]["player_response"].ToString();
				}
			}
			match = new Regex("ytInitialPlayerResponse\\s*=\\s*({.+?})\\s*;\\s*(?:var\\s+meta|</script|\\n)", RegexOptions.Multiline).Match(pageSource);
			if (match.Success)
			{
				json = match.Result("$1");
			}
			match = new Regex("ytInitialPlayerResponse\\s*=\\s*({.+?})\\s*;", RegexOptions.Multiline).Match(pageSource);
			if (match.Success)
			{
				json = match.Result("$1");
			}
		}
		JObject jobject = JObject.Parse(json);
		if (jobject["videoDetails"]["isLive"].Value<bool>())
		{
			string obj = jobject["streamingData"]["hlsManifestUrl"].ToString();
			callback(obj);
		}
		else
		{
			Debug.Log("This is not a livestream url");
		}
	}

	
	public string _livestreamUrl;

	
	private YoutubePlayerLivestream.DownloadUrlResponse downloadYoutubeUrlResponse;

	
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
}
