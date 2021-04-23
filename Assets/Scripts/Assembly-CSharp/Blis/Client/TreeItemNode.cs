using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class TreeItemNode : BaseUI
	{
		[SerializeField] private ItemDataSlot slot = default;


		[SerializeField] private Text itemName = default;


		[SerializeField] private Text itemAmount = default;


		[SerializeField] private Image itemAmountBg = default;


		[SerializeField] private Button downTreeBtn = default;


		[SerializeField] private TreeItemNode leftNode = default;


		[SerializeField] private TreeItemNode rightNode = default;


		[SerializeField] private RectTransform childLiner = default;


		private readonly List<Image> lineImages = new List<Image>();


		private ItemData itemData;


		public TreeItemNode LeftNode => leftNode;


		public TreeItemNode RightNode => rightNode;


		private new void Awake() { }


		public void SetItemData(ItemData itemData)
		{
			this.itemData = itemData;
			slot.ResetSlot();
			slot.SetItemData(itemData);
			slot.SetSlotType(SlotType.None);
			if (itemData != null)
			{
				EnableSlot(true);
				SetName(LnUtil.GetItemName(itemData.code));
				SetAmount(itemData.initialCount);
				slot.SetSprite(itemData.GetSprite());
				slot.SetBackground(itemData.GetGradeSprite());
				bool flag = false;
				if (MonoBehaviourInstance<ClientService>.inst != null &&
				    MonoBehaviourInstance<ClientService>.inst.IsPlayer)
				{
					flag = GameDB.item.IsCombinable(itemData,
						MonoBehaviourInstance<ClientService>.inst.MyPlayer.Inventory,
						MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.Equipment);
				}

				if (childLiner != null)
				{
					childLiner.GetComponentsInChildren<Image>(lineImages);
					foreach (Image image in lineImages)
					{
						if (flag)
						{
							image.color = new Color32(189, 210, 210, byte.MaxValue);
						}
						else
						{
							image.color = new Color32(108, 120, 120, byte.MaxValue);
						}
					}
				}

				if (flag)
				{
					slot.PlayBlink();
				}

				if (SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene)
				{
					bool enable = itemData.IsLeafNodeItem() && !GameDB.item.IsCollectibleItem(itemData) &&
					              Singleton<ItemService>.inst.GetDropArea(itemData.code).Count == 0;
					slot.EnableRandomMark(enable);
				}
			}
		}


		public void ResetUI()
		{
			EnableSlot(false);
			SetName(null);
			SetAmount(0);
			EnableChildLiner(false);
			EnableDownTreeBtn(false, null);
			slot.ResetSlot();
		}


		public ItemData GetItemData()
		{
			return slot.GetItemData();
		}


		public void SetName(string name)
		{
			if (itemName != null)
			{
				if (!itemName.gameObject.activeSelf)
				{
					itemName.gameObject.SetActive(true);
				}

				itemName.text = name;
			}
		}


		public void SetAmount(int amount)
		{
			if (itemAmount != null && itemAmountBg != null)
			{
				if (amount > 0)
				{
					itemAmountBg.enabled = true;
					itemAmount.text = Ln.Format("{0} 개", amount);
					return;
				}

				itemAmountBg.enabled = false;
				itemAmount.text = null;
			}
		}


		public void SetSelection(bool select)
		{
			slot.EnableSelection(select);
		}


		public void SetEventListener(ISlotEventListener listener)
		{
			slot.SetEventListener(listener);
		}


		private void EnableSlot(bool enable)
		{
			slot.transform.localScale = enable ? Vector3.one : Vector3.zero;
		}


		public void EnableChildLiner(bool enable)
		{
			if (childLiner != null)
			{
				childLiner.transform.localScale = enable ? Vector3.one : Vector3.zero;
			}
		}


		public void EnableDownTreeBtn(bool enable, Action action)
		{
			if (downTreeBtn != null)
			{
				downTreeBtn.transform.localScale = enable ? Vector3.one : Vector3.zero;
				downTreeBtn.interactable = enable;
				downTreeBtn.onClick.RemoveAllListeners();
				if (action != null)
				{
					downTreeBtn.onClick.AddListener(delegate { action(); });
				}
			}
		}
	}
}