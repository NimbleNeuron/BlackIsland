using UnityEngine;
using UnityEngine.EventSystems;


public class VideoProgressBar : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerDownHandler
{
	
	public void OnDrag(PointerEventData eventData)
	{
		this.player.TrySkip(eventData);
	}

	
	public void OnPointerDown(PointerEventData eventData)
	{
		this.player.TrySkip(eventData);
	}

	
	public YoutubePlayer player;
}
