using UnityEngine;
using UnityEngine.Video;


public class YoutubeSimplified : MonoBehaviour
{
	
	private void Awake()
	{
		this.videoPlayer = base.GetComponentInChildren<VideoPlayer>();
		this.player = base.GetComponentInChildren<YoutubePlayer>();
		this.player.videoPlayer = this.videoPlayer;
	}

	
	private void Start()
	{
		this.Play();
	}

	
	public void Play()
	{
		if (this.fullscreen)
		{
			this.videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
		}
		this.player.autoPlayOnStart = this.autoPlay;
		this.player.videoQuality = YoutubePlayer.YoutubeVideoQuality.STANDARD;
		if (this.autoPlay)
		{
			this.player.Play(this.url);
		}
	}

	
	private YoutubePlayer player;

	
	public string url;

	
	public bool autoPlay = true;

	
	public bool fullscreen = true;

	
	private VideoPlayer videoPlayer;
}
