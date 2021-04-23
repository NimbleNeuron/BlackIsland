using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class EmotionPlateHudComponent : BaseUI, IPointerEnterHandler, IEventSystemHandler
	{
		[SerializeField] private EmotionPlateType type = default;


		[SerializeField] private Button btn = default;


		[SerializeField] private Image image = default;


		private Image btnImage = default;


		private Sprite btnSourceSprite = default;


		private Image equippedEmotionIcon = default;


		public IEventDelegate EventDelegate;


		private bool isEmpty = true;


		public EmotionPlateType Type => type;


		public void OnPointerEnter(PointerEventData eventData)
		{
			if (EventDelegate == null)
			{
				return;
			}

			EventDelegate.OnPointerEnter(type);
		}


		protected override void OnAwakeUI()
		{
			equippedEmotionIcon = GameUtil.Bind<Image>(btn.gameObject, "Icon");
			GameUtil.Bind<Image>(btn.gameObject, ref btnImage);
			btnSourceSprite = btnImage.sprite;
		}


		public void SetEquippedEmotionIcon(EmotionIconData emotionIconData)
		{
			if (emotionIconData != null)
			{
				equippedEmotionIcon.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetEmotionSprite(emotionIconData.sprite);
				equippedEmotionIcon.enabled = true;
				isEmpty = false;
				return;
			}

			equippedEmotionIcon.enabled = false;
			isEmpty = true;
		}


		public void SetHighlight(bool active)
		{
			if (active)
			{
				image.sprite = btn.spriteState.highlightedSprite;
				return;
			}

			image.sprite = btnSourceSprite;
		}


		public bool IsComponentEmpty()
		{
			return isEmpty;
		}


		public interface IEventDelegate
		{
			void OnPointerEnter(EmotionPlateType type);
		}
	}
}