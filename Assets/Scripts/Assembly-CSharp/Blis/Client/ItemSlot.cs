using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ItemSlot : Slot
	{
		[SerializeField] private SlotType slotType;

		protected Image baseImage;
		protected ColorTweener combineMark;
		protected Image combineMarkImg;
		protected ColorTweener focusFrame;
		protected Image focusFrameImg;
		protected Image iconHP;
		protected Image iconSP;
		protected Item item;
		protected Image needMark;
		protected ParticleSystem showEffect;

		public Item GetItem()
		{
			return item;
		}

		public SlotType GetSlotType()
		{
			return slotType;
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			isDraggable = true;
			focusFrameImg = GameUtil.Bind<Image>(gameObject, "FocusFrame");
			focusFrame = GameUtil.Bind<ColorTweener>(gameObject, "FocusFrame");
			baseImage = GameUtil.Bind<Image>(gameObject, "BaseImage");
			iconHP = GameUtil.Bind<Image>(gameObject, "IconHP");
			iconSP = GameUtil.Bind<Image>(gameObject, "IconSP");
			needMark = GameUtil.Bind<Image>(gameObject, "NeedMark");
			combineMarkImg = GameUtil.Bind<Image>(gameObject, "CombineMark");
			combineMark = GameUtil.Bind<ColorTweener>(gameObject, "CombineMark");
			showEffect = GameUtil.Bind<ParticleSystem>(gameObject, "ShowEffect");
		}

		public override void ResetSlot()
		{
			base.ResetSlot();
			item = null;
			if (focusFrameImg != null)
			{
				focusFrameImg.enabled = false;
			}

			if (focusFrame != null)
			{
				focusFrame.enabled = false;
			}

			if (baseImage != null)
			{
				baseImage.enabled = false;
			}

			if (iconHP != null)
			{
				iconHP.enabled = false;
			}

			if (iconSP != null)
			{
				iconSP.enabled = false;
			}

			if (needMark != null)
			{
				needMark.enabled = false;
			}

			if (combineMarkImg != null)
			{
				combineMarkImg.enabled = false;
			}

			if (combineMark != null)
			{
				combineMark.enabled = false;
			}

			if (showEffect != null)
			{
				showEffect.Stop();
			}
		}
		
		public void ResetCooldownSlot()
		{
			base.ResetSlot();
			item = null;
			if (focusFrameImg != null)
			{
				focusFrameImg.enabled = false;
			}

			if (focusFrame != null)
			{
				focusFrame.enabled = false;
			}

			if (baseImage != null)
			{
				baseImage.enabled = false;
			}

			if (iconHP != null)
			{
				iconHP.enabled = false;
			}

			if (iconSP != null)
			{
				iconSP.enabled = false;
			}

			if (combineMarkImg != null)
			{
				combineMarkImg.enabled = false;
			}

			if (combineMark != null)
			{
				combineMark.enabled = false;
			}

			if (showEffect != null)
			{
				showEffect.Stop();
			}
		}
		
		public void SetItem(Item item)
		{
			this.item = item;
			if (item != null)
			{
				bool enabled = item.ItemData.IsThrowType() || slotType != SlotType.Equipment;
				if (stack != null)
				{
					stack.enabled = enabled;
				}
			}
		}
		
		public void EnableBaseImage(bool enable)
		{
			if (baseImage != null)
			{
				baseImage.enabled = enable;
			}
		}

		public void EnableHPIcon(bool enable)
		{
			if (iconHP != null)
			{
				iconHP.enabled = enable;
			}
		}
		
		public void EnableSPIcon(bool enable)
		{
			if (iconSP != null)
			{
				iconSP.enabled = enable;
			}
		}
		
		public void EnableNeedMark(bool enable)
		{
			if (needMark != null)
			{
				needMark.enabled = enable;
			}
		}

		public void EnableCombineMark(bool enable)
		{
			if (enable)
			{
				if (combineMarkImg != null)
				{
					combineMarkImg.enabled = true;
				}

				if (combineMark != null)
				{
					combineMark.enabled = true;
					combineMark.PlayAnimation();
				}
			}
			else
			{
				if (combineMark != null)
				{
					combineMark.StopAnimation();
					combineMark.enabled = false;
				}

				if (combineMarkImg != null)
				{
					combineMarkImg.enabled = false;
				}
			}
		}

		public void PlayFocusFrame()
		{
			if (focusFrameImg != null)
			{
				focusFrameImg.color = new Color(1f, 1f, 0f, 0f);
				focusFrameImg.enabled = true;
			}

			if (focusFrame != null)
			{
				focusFrame.enabled = true;
				focusFrame.PlayAnimation();
			}
		}

		public void StopFocusFrame()
		{
			if (focusFrame != null)
			{
				focusFrame.StopAnimation();
				focusFrame.enabled = false;
			}

			if (focusFrameImg != null)
			{
				focusFrameImg.enabled = false;
			}
		}
		
		public void PlayEffect()
		{
			if (showEffect != null)
			{
				showEffect.Play();
			}
		}
		
		public bool IsUsable()
		{
			return isLock != null && !isLock.Value && Cooldown.IsUsable();
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (item != null && !eventData.dragging && useTooltip &&
			    (GetParentWindow() == null || GetParentWindow().IsOpen))
			{
				bool showCompare = slotType == SlotType.Inventory || slotType == SlotType.ShortCut;
				MonoBehaviourInstance<Tooltip>.inst.SetItem(item, item.Amount, showCompare);
				if (slotType == SlotType.None || slotType == SlotType.ScoreBoard)
				{
					MonoBehaviourInstance<Tooltip>.inst.ShowTracking(GetParentWindow());
					return;
				}

				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(GetParentWindow(), slotType);
			}
		}
		
		public void SetSlotType(SlotType slotType)
		{
			this.slotType = slotType;
		}
	}
}