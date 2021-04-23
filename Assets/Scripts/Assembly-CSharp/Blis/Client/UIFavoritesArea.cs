using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIFavoritesArea : BaseUI, ILnEventHander
	{
		[SerializeField] private ItemDataSlotTable itemDataSlotTable = default;


		[SerializeField] private GameObject delete = default;


		[SerializeField] private Image empty = default;


		[SerializeField] private Text areaName = default;


		[SerializeField] private Text index = default;


		[SerializeField] private Image hover = default;


		[SerializeField] private List<CompleteItem> completeItems = default;


		private int areaCode;


		private ISlotEventListener eventListener;


		private int slotIndex;


		public void OnLnDataChange()
		{
			UpdateAreaName();
		}

		
		
		public event Action<int> OnRequestRemove = delegate { };


		public void RequestRemove()
		{
			if (areaCode > 0)
			{
				OnRequestRemove(areaCode);
			}
		}


		public void DeleteEnable(bool enable)
		{
			delete.SetActive(enable);
		}


		public void Clear()
		{
			HideCompleteItems();
			empty.gameObject.SetActive(true);
			itemDataSlotTable.Clear();
		}


		public void ShowCompleteItems(List<int> completeList)
		{
			for (int i = 0; i < completeList.Count; i++)
			{
				if (completeItems.Count > i)
				{
					ItemData itemData = GameDB.item.FindItemByCode(completeList[i]);
					completeItems[i].itemGrade.gameObject.SetActive(true);
					completeItems[i].itemGrade.sprite = itemData.GetGradeSprite();
					completeItems[i].itemIcon.sprite = itemData.GetSprite();
					completeItems[i].SetEvent(completeItems[i].itemIcon.transform);
				}
			}
		}


		public void HideCompleteItems()
		{
			foreach (CompleteItem completeItem in completeItems)
			{
				completeItem.itemGrade.gameObject.SetActive(false);
			}
		}


		public void SetAreaCode(int areaCode)
		{
			empty.gameObject.SetActive(false);
			hover.gameObject.SetActive(false);
			this.areaCode = areaCode;
			UpdateAreaName();
		}


		public ItemDataSlotTable GetItemDataSlotTable()
		{
			return itemDataSlotTable;
		}


		public int GetAreaCode()
		{
			if (!gameObject.activeSelf)
			{
				return 0;
			}

			return areaCode;
		}


		public int GetSlotIndex()
		{
			return slotIndex;
		}


		public void SetIndex(int index)
		{
			this.index.text = index.ToString();
			slotIndex = index;
		}


		public void AddItemData(ItemData itemData, int count)
		{
			ItemDataSlot itemDataSlot = itemDataSlotTable.CreateSlot(itemData);
			if (itemDataSlot != null)
			{
				itemDataSlot.SetItemData(itemData);
				itemDataSlot.SetSlotType(SlotType.None);
				itemDataSlot.SetSprite(itemData.GetSprite());
				itemDataSlot.SetBackground(itemData.GetGradeSprite());
				itemDataSlot.SetDraggable(false);
				itemDataSlot.SetEventListener(eventListener);
				itemDataSlot.SetStackText(count > 0 ? count.ToString() : null);
			}
		}


		private void UpdateAreaName()
		{
			areaName.text = Ln.Get(LnType.Area_Name, areaCode.ToString());
		}


		public void HighLight(List<ItemData> list)
		{
			foreach (ItemData itemData in itemDataSlotTable.GetKeys())
			{
				itemDataSlotTable.FindSlot(itemData).EnableSelection(list != null && list.Contains(itemData));
			}
		}


		public void UnEnableSelection()
		{
			foreach (ItemData t in itemDataSlotTable.GetKeys())
			{
				itemDataSlotTable.FindSlot(t).EnableSelection(false);
			}
		}


		public void EnableHoverEffect(bool enable)
		{
			hover.gameObject.SetActive(enable);
		}


		public void SetEventHandler(ISlotEventListener eventListener)
		{
			this.eventListener = eventListener;
		}


		[Serializable]
		public class CompleteItem
		{
			public Image itemGrade;


			public Image itemIcon;


			private EventTrigger eventTrigger;


			private EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


			private EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


			private Transform tooltipParent;

			public void SetEvent(Transform tooltipParent)
			{
				this.tooltipParent = tooltipParent;
				GameUtil.BindOrAdd<EventTrigger>(itemIcon.gameObject, ref eventTrigger);
				eventTrigger.triggers.Clear();
				onEnterEvent.AddListener(OnPointerEnter);
				onExitEvent.AddListener(OnPointerExit);
				eventTrigger.triggers.Add(new EventTrigger.Entry
				{
					eventID = EventTriggerType.PointerEnter,
					callback = onEnterEvent
				});
				eventTrigger.triggers.Add(new EventTrigger.Entry
				{
					eventID = EventTriggerType.PointerExit,
					callback = onExitEvent
				});
			}


			private void OnPointerEnter(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get("목표 아이템이 완성되는 경로 지역입니다"));
				Vector2 vector = tooltipParent.position;
				vector += GameUtil.ConvertPositionOnScreenResolution(-18f, 57f);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
			}


			private void OnPointerExit(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}
		}
	}
}