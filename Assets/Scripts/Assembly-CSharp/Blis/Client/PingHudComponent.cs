using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class PingHudComponent : BaseUI, IPointerEnterHandler, IEventSystemHandler
	{
		[SerializeField] private PingType pingType = default;


		[SerializeField] private Button btn = default;


		[SerializeField] private Image image = default;


		public IEventDelegate EventDelegate;


		private Sprite sourceSprite;


		public PingType PingType => pingType;


		protected override void Awake()
		{
			sourceSprite = image.sprite;
		}


		public void OnPointerEnter(PointerEventData eventData)
		{
			EventDelegate.OnPointerEnter(pingType);
		}


		public void SetHighlight(bool active)
		{
			if (active)
			{
				image.sprite = btn.spriteState.highlightedSprite;
				return;
			}

			image.sprite = sourceSprite;
		}


		public interface IEventDelegate
		{
			void OnPointerEnter(PingType type);
		}
	}
}