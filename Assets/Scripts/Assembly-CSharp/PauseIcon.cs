using UnityEngine;
using UnityEngine.UI;


public class PauseIcon : MonoBehaviour
{
	
	private void FixedUpdate()
	{
		if (this.p.pauseCalled)
		{
			this.playImage.gameObject.SetActive(true);
			this.pauseImage.gameObject.SetActive(false);
			return;
		}
		this.pauseImage.gameObject.SetActive(true);
		this.playImage.gameObject.SetActive(false);
	}

	
	public YoutubePlayer p;

	
	public Image playImage;

	
	public Image pauseImage;
}
