using Blis.Common;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class SimpleSlot : BaseControl
	{
		private Image background;


		private SimpleSlotBehaviour behaviour;


		private Image image;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			image = GameUtil.Bind<Image>(gameObject, "Img");
			background = GameUtil.Bind<Image>(gameObject, "Background");
		}


		public void SetSimpleSlotBehaviour(SimpleSlotBehaviour behaviour)
		{
			this.behaviour = behaviour;
		}


		public void SetIcon(string baseImage)
		{
			if (behaviour != null)
			{
				image.sprite = behaviour.GetIcon();
				image.enabled = true;
				return;
			}

			if (!string.IsNullOrEmpty(baseImage))
			{
				image.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(baseImage);
				image.enabled = true;
				return;
			}

			image.enabled = false;
		}


		public void SetBackground()
		{
			if (behaviour != null)
			{
				background.sprite = behaviour.GetBackground();
			}
		}


		public void SetBackground(string spriteName)
		{
			background.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(spriteName);
		}


		public virtual void Enable()
		{
			EnableIcon(true);
			EnableBackground(true);
		}


		public virtual void Disable()
		{
			SetSimpleSlotBehaviour(null);
			EnableIcon(false);
			EnableBackground(false);
		}


		private void EnableIcon(bool enable)
		{
			image.enabled = enable;
		}


		private void EnableBackground(bool enable)
		{
			background.enabled = enable;
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (!eventData.dragging)
			{
				SimpleSlotBehaviour simpleSlotBehaviour = behaviour;
				if (simpleSlotBehaviour == null)
				{
					return;
				}

				simpleSlotBehaviour.ShowTooltip();
			}
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			SimpleSlotBehaviour simpleSlotBehaviour = behaviour;
			if (simpleSlotBehaviour == null)
			{
				return;
			}

			simpleSlotBehaviour.HideTooltip();
		}
	}
}