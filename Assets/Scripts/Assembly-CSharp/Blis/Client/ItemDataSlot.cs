using Blis.Common;
using Blis.Common.Utils;
using UiEffect;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ItemDataSlot : Slot
	{
		[SerializeField] private SlotType slotType;


		protected Image bestMark;


		protected BlendTweener blink;


		protected BlendColor blinkColor;


		protected Image blockLine;


		protected Image iconHP;


		protected Image iconSP;


		protected Image imgBlink;


		protected ItemData itemData;


		private LayoutElement layoutElement;


		protected Image needMark;


		protected Image ownMark;


		protected Image randomMark;


		protected Image selection;

		public ItemData GetItemData()
		{
			return itemData;
		}


		public SlotType GetSlotType()
		{
			return slotType;
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			if (image != null)
			{
				image.raycastTarget = false;
			}

			layoutElement = gameObject.GetComponent<LayoutElement>();
			iconHP = GameUtil.Bind<Image>(gameObject, "IconHP");
			iconSP = GameUtil.Bind<Image>(gameObject, "IconSP");
			selection = GameUtil.Bind<Image>(gameObject, "Selection");
			ownMark = GameUtil.Bind<Image>(gameObject, "OwnMark");
			randomMark = GameUtil.Bind<Image>(gameObject, "RandomMark");
			bestMark = GameUtil.Bind<Image>(gameObject, "BestMark");
			needMark = GameUtil.Bind<Image>(gameObject, "NeedMark");
			blockLine = GameUtil.Bind<Image>(gameObject, "BlockLine");
			imgBlink = GameUtil.Bind<Image>(gameObject, "Blink");
			blinkColor = GameUtil.Bind<BlendColor>(gameObject, "Blink");
			blink = GameUtil.Bind<BlendTweener>(gameObject, "Blink");
		}


		public override void ResetSlot()
		{
			base.ResetSlot();
			itemData = null;
			if (background != null)
			{
				background.raycastTarget = false;
			}

			if (layoutElement != null)
			{
				layoutElement.ignoreLayout = true;
			}

			if (iconHP != null)
			{
				iconHP.enabled = false;
			}

			if (iconSP != null)
			{
				iconSP.enabled = false;
			}

			if (selection != null)
			{
				selection.enabled = false;
			}

			if (ownMark != null)
			{
				ownMark.enabled = false;
			}

			if (randomMark != null)
			{
				randomMark.enabled = false;
			}

			if (bestMark != null)
			{
				bestMark.enabled = false;
			}

			if (needMark != null)
			{
				needMark.enabled = false;
			}

			if (blockLine != null)
			{
				blockLine.enabled = false;
			}

			StopBlink();
		}


		public void InitSlot()
		{
			OnAwakeUI();
		}


		public virtual void SetItemData(ItemData itemData)
		{
			this.itemData = itemData;
			if (background != null)
			{
				background.raycastTarget = true;
			}

			if (layoutElement != null)
			{
				layoutElement.ignoreLayout = false;
			}
		}


		public void SetSlotType(SlotType slotType)
		{
			this.slotType = slotType;
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


		public void EnableSelection(bool enable)
		{
			if (selection != null)
			{
				selection.enabled = enable;
			}
		}


		public void EnableOwnMark(bool enable)
		{
			if (ownMark != null)
			{
				ownMark.enabled = enable;
			}
		}


		public void EnableRandomMark(bool enable)
		{
			if (randomMark != null)
			{
				randomMark.enabled = enable;
			}
		}


		public void EnableBestMark(bool enable)
		{
			if (bestMark != null)
			{
				bestMark.enabled = enable;
			}
		}


		public void EnableNeedMark(bool enable)
		{
			if (needMark != null)
			{
				needMark.enabled = enable;
			}
		}


		public void EnableBlockLine(bool enable)
		{
			if (blockLine != null)
			{
				blockLine.enabled = enable;
			}
		}


		public void PlayBlink()
		{
			if (imgBlink != null)
			{
				imgBlink.enabled = true;
			}

			if (blinkColor != null)
			{
				blinkColor.enabled = true;
			}

			if (blink != null)
			{
				blink.PlayAnimation();
			}
		}


		public void StopBlink()
		{
			if (blink != null)
			{
				blink.StopAnimation();
			}

			if (blinkColor != null)
			{
				blinkColor.enabled = false;
			}

			if (imgBlink != null)
			{
				imgBlink.enabled = false;
			}
		}


		public bool GetNeedMarkEnable()
		{
			return needMark.enabled;
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (itemData != null && !eventData.dragging && useTooltip &&
			    (GetParentWindow() == null || GetParentWindow().IsOpen))
			{
				bool showCompare = slotType == SlotType.Inventory || slotType == SlotType.ShortCut;
				Item item = null;
				if (SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene)
				{
					item = GetSpeicalMakeItem(itemData);
				}

				if (item != null)
				{
					MonoBehaviourInstance<Tooltip>.inst.SetItem(item, 0, showCompare);
				}
				else
				{
					MonoBehaviourInstance<Tooltip>.inst.SetItem(itemData, 0, showCompare);
				}

				if (slotType == SlotType.None)
				{
					MonoBehaviourInstance<Tooltip>.inst.ShowTracking(GetParentWindow());
					return;
				}

				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(GetParentWindow(), slotType);
			}
		}


		private Item GetSpeicalMakeItem(ItemData data)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return null;
			}

			if (SingletonMonoBehaviour<PlayerController>.inst.CharcterCode != 13)
			{
				return null;
			}

			if (data.itemType != ItemType.Consume)
			{
				return null;
			}

			if (slotType != SlotType.ShortCut)
			{
				return null;
			}

			Item item = new Item(0, itemData.code, 1, 0, itemData);
			item.AddRecoveryItem(30);
			return item;
		}
	}
}