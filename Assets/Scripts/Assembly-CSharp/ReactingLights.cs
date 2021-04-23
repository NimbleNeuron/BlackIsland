using UnityEngine;
using UnityEngine.Video;


public class ReactingLights : MonoBehaviour
{
	
	private void Start()
	{
		this.videoSource.frameReady += this.NewFrame;
		this.videoSource.sendFrameReadyEvents = true;
	}

	
	private void NewFrame(VideoPlayer vplayer, long frame)
	{
		if (!this.createTexture)
		{
			this.createTexture = true;
			switch (this.videoSide)
			{
			case ReactingLights.VideoSide.up:
				this.tex = new Texture2D(this.videoSource.texture.width / 2, 20);
				break;
			case ReactingLights.VideoSide.left:
				this.tex = new Texture2D(20, this.videoSource.texture.height / 2);
				break;
			case ReactingLights.VideoSide.right:
				this.tex = new Texture2D(20, this.videoSource.texture.height / 2);
				break;
			case ReactingLights.VideoSide.down:
				this.tex = new Texture2D(this.videoSource.texture.width / 2, 20);
				break;
			case ReactingLights.VideoSide.center:
				this.tex = new Texture2D(this.videoSource.texture.height / 2, this.videoSource.texture.height / 2);
				break;
			}
		}
		RenderTexture.active = (RenderTexture)this.videoSource.texture;
		switch (this.videoSide)
		{
		case ReactingLights.VideoSide.up:
			this.tex.ReadPixels(new Rect((float)(this.videoSource.texture.width / 2), 0f, (float)(this.videoSource.texture.width / 2), 20f), 0, 0);
			break;
		case ReactingLights.VideoSide.left:
			this.tex.ReadPixels(new Rect(0f, 0f, 20f, (float)(this.videoSource.texture.height / 2)), 0, 0);
			break;
		case ReactingLights.VideoSide.right:
			this.tex.ReadPixels(new Rect((float)(this.videoSource.texture.width - 20), 0f, 20f, (float)(this.videoSource.texture.height / 2)), 0, 0);
			break;
		case ReactingLights.VideoSide.down:
			this.tex.ReadPixels(new Rect((float)(this.videoSource.texture.width / 2), (float)(this.videoSource.texture.height - 20), (float)(this.videoSource.texture.width / 2), 20f), 0, 0);
			break;
		case ReactingLights.VideoSide.center:
			this.tex.ReadPixels(new Rect((float)(this.videoSource.texture.width / 2 - this.videoSource.texture.width / 2), (float)(this.videoSource.texture.height / 2 - this.videoSource.texture.height / 2), (float)(this.videoSource.texture.width / 2), (float)(this.videoSource.texture.height / 2)), 0, 0);
			break;
		}
		this.tex.Apply();
		this.averageColor = this.AverageColorFromTexture(this.tex);
		this.averageColor.a = 1f;
		Light[] array = this.lights;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = this.averageColor;
		}
	}

	
	private Color32 AverageColorFromTexture(Texture2D tex)
	{
		Color32[] pixels = tex.GetPixels32();
		int num = pixels.Length;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < num; i++)
		{
			num2 += (float)pixels[i].r;
			num3 += (float)pixels[i].g;
			num4 += (float)pixels[i].b;
		}
		return new Color32((byte)(num2 / (float)num), (byte)(num3 / (float)num), (byte)(num4 / (float)num), 0);
	}

	
	public VideoPlayer videoSource;

	
	public Light[] lights;

	
	public Color averageColor;

	
	private Texture2D tex;

	
	public ReactingLights.VideoSide videoSide;

	
	private bool createTexture;

	
	public enum VideoSide
	{
		
		up,
		
		left,
		
		right,
		
		down,
		
		center
	}
}
