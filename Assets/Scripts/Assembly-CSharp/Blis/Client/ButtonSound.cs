using UnityEngine;
using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class ButtonSound : UIBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler
	{
		[SerializeField] private string soundName = default;


		[SerializeField] private string soundHoverName = default;

		public void OnPointerClick(PointerEventData eventData)
		{
			if (string.IsNullOrEmpty(soundName))
			{
				return;
			}

			Singleton<SoundControl>.inst.PlayUISound(soundName);
		}


		public void OnPointerEnter(PointerEventData eventData)
		{
			if (string.IsNullOrEmpty(soundHoverName))
			{
				return;
			}

			Singleton<SoundControl>.inst.PlayUISound(soundHoverName);
		}
	}
}