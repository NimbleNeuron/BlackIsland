using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Blis.Client
{
	public class UIItemScrollView : BaseUI, ISlotEventListener
	{
		public delegate void ItemSelectEvent(ItemData itemData, MouseButton mouseButton);


		[SerializeField] private ScrollRect scrollRect = default;


		private readonly List<Item> belongItems = new List<Item>();


		private readonly List<ItemDataSlot> itemDataSlots = new List<ItemDataSlot>();


		private ScrollContentWrapper contentWrapper;


		private List<Item> equipment = new List<Item>();


		private int focusedItemCode;


		private List<Item> inventory = new List<Item>();


		private bool isDraggable;


		private List<ItemData> preferItems = new List<ItemData>();


		public void OnSlotLeftClick(Slot slot)
		{
			ItemDataSlot itemDataSlot = slot as ItemDataSlot;
			if (itemDataSlot != null && itemDataSlot.GetItemData() != null)
			{
				OnClickItemHandler(itemDataSlot.GetItemData(), MouseButton.LeftMouse);
			}
		}


		public void OnSlotRightClick(Slot slot)
		{
			ItemDataSlot itemDataSlot = slot as ItemDataSlot;
			if (itemDataSlot != null && itemDataSlot.GetItemData() != null)
			{
				OnClickItemHandler(itemDataSlot.GetItemData(), MouseButton.RightMouse);
			}
		}


		public void OnDropItem(Slot slot, BaseUI draggedUI) { }


		public void OnThrowItem(Slot slot) { }


		public void OnThrowItemPiece(Slot slot) { }


		public void OnPointerEnter(Slot slot) { }


		public void OnPointerExit(Slot slot) { }


		public void OnSlotDoubleClick(Slot slot)
		{
			OnSlotRightClick(slot);
		}

		
		
		public event ItemSelectEvent OnClickItemHandler = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			contentWrapper = scrollRect.GetComponent<ScrollContentWrapper>();
			contentWrapper.OnAppear += OnAppear;
			contentWrapper.OnDisappear += OnDisappear;
			contentWrapper.GetComponentsInChildren<ItemDataSlot>(true, itemDataSlots);
			itemDataSlots.ForEach(delegate(ItemDataSlot x)
			{
				x.SetEventListener(this);
				x.OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
				{
					scrollRect.OnBeginDrag(eventData);
				};
				x.OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
				{
					scrollRect.OnEndDrag(eventData);
				};
				x.OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
				{
					scrollRect.OnDrag(eventData);
				};
				x.OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
				{
					scrollRect.OnScroll(eventData);
				};
			});
		}


		public void SetDataSource(List<ItemData> itemDatas)
		{
			contentWrapper.SetDataList(itemDatas);
		}


		public void SetDraggable(bool draggable)
		{
			isDraggable = draggable;
		}


		private void OnAppear(GameObject go, object data)
		{
			ItemData itemData = (ItemData) data;
			ItemDataSlot component = go.GetComponent<ItemDataSlot>();
			component.SetItemData(itemData);
			component.SetDraggable(isDraggable);
			component.SetSlotType(SlotType.None);
			component.SetSprite(itemData.GetSprite());
			component.SetBackground(itemData.GetGradeSprite());
			component.EnableBestMark(preferItems.Contains(itemData));
			if (SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene)
			{
				bool enable = itemData.IsLeafNodeItem() && !GameDB.item.IsCollectibleItem(itemData) &&
				              Singleton<ItemService>.inst.GetDropArea(itemData.code).Count == 0;
				component.EnableRandomMark(enable);
			}

			component.EnableSelection(itemData.code == focusedItemCode);
			if (belongItems.Exists(x => x.itemCode == itemData.makeMaterial1) &&
			    belongItems.Exists(x => x.itemCode == itemData.makeMaterial2))
			{
				component.PlayBlink();
				return;
			}

			component.StopBlink();
		}


		private void OnDisappear(GameObject go)
		{
			go.GetComponent<ItemDataSlot>().ResetSlot();
		}


		public void SetFocusItemCode(int itemCode)
		{
			focusedItemCode = itemCode;
			for (int i = 0; i < itemDataSlots.Count; i++)
			{
				ItemData itemData = itemDataSlots[i].GetItemData();
				itemDataSlots[i].EnableSelection(itemData != null && itemData.code == focusedItemCode);
			}
		}


		public void SetPreferItems(List<ItemData> items)
		{
			preferItems = items;
		}


		public void OnUpdateInventory(List<Item> items)
		{
			inventory = items;
			UpdateBelongItem();
		}


		public void OnUpdateEquipment(List<Item> items)
		{
			equipment = items;
			UpdateBelongItem();
		}


		private void UpdateBelongItem()
		{
			belongItems.Clear();
			belongItems.AddRange(inventory);
			belongItems.AddRange(equipment);
			contentWrapper.Refresh();
		}
	}
}