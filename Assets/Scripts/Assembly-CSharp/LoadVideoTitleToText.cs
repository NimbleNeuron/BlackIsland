using UnityEngine;


public class LoadVideoTitleToText : MonoBehaviour
{
	
	public void SetVideoTitle()
	{
		this.textMesh.text = this.player.GetVideoTitle();
	}

	
	public TextMesh textMesh;

	
	public YoutubePlayer player;
}
