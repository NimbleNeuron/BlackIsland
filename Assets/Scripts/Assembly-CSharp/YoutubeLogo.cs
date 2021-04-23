using UnityEngine;


public class YoutubeLogo : MonoBehaviour
{
	
	private void OnMouseDown()
	{
		Application.OpenURL(this.youtubeurl);
	}

	
	public string youtubeurl;
}
