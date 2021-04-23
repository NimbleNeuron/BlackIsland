using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class UIItemRecommend : BaseUI, ISlotEventListener
	{
		public delegate void ClickListItemEvent(ItemData itemData);


		public delegate void RequestCombineEvent(ItemData itemData);


		public GameObject slotPrefab;


		public List<RectTransform> categoryList;


		public RectTransform content;


		private readonly List<ItemDataSlot> recommendSlots = new List<ItemDataSlot>();


		private CanvasGroup canvasGroup;


		public void OnSlotLeftClick(Slot slot)
		{
			ItemDataSlot itemDataSlot = slot as ItemDataSlot;
			if (itemDataSlot != null && itemDataSlot.GetItemData() != null)
			{
				ClickListItemEvent onClickListItem = OnClickListItem;
				if (onClickListItem == null)
				{
					return;
				}

				onClickListItem(itemDataSlot.GetItemData());
			}
		}


		public void OnSlotRightClick(Slot slot)
		{
			ItemDataSlot itemDataSlot = slot as ItemDataSlot;
			if (itemDataSlot != null && itemDataSlot.GetItemData() != null)
			{
				RequestCombineEvent onRequestCombine = OnRequestCombine;
				if (onRequestCombine == null)
				{
					return;
				}

				onRequestCombine(itemDataSlot.GetItemData());
			}
		}


		public void OnDropItem(Slot slot, BaseUI draggedUI) { }


		public void OnThrowItem(Slot slot) { }


		public void OnThrowItemPiece(Slot slot) { }


		public void OnPointerEnter(Slot slot) { }


		public void OnPointerExit(Slot slot) { }


		public void OnSlotDoubleClick(Slot slot) { }

		
		
		public event ClickListItemEvent OnClickListItem;


		
		
		public event RequestCombineEvent OnRequestCombine;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.BindOrAdd<CanvasGroup>(gameObject, ref canvasGroup);
		}


		public void Show()
		{
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}


		public void Hide()
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}


		public void SetRecommendData(Dictionary<RecommendItemType, List<ItemData>> recommendDatas)
		{
			List<RecommendItemType> list = recommendDatas.Keys.ToList<RecommendItemType>();
			int num = 0;
			while (num < list.Count && num < categoryList.Count)
			{
				RenderCategory(categoryList[num], recommendDatas[list[num]], list[num] == RecommendItemType.Key);
				num++;
			}

			RebuildRecommendUI();
		}


		private void RenderCategory(RectTransform parent, List<ItemData> itemDataList, bool recommendMark)
		{
			for (int i = 0; i < itemDataList.Count; i++)
			{
				GameObject gameObject = Instantiate<GameObject>(slotPrefab);
				ItemDataSlot itemDataSlot = null;
				GameUtil.BindOrAdd<ItemDataSlot>(gameObject, ref itemDataSlot);
				gameObject.transform.SetParent(parent);
				gameObject.transform.localScale = Vector3.one;
				ItemData itemData = itemDataList[i];
				itemDataSlot.SetItemData(itemData);
				itemDataSlot.SetSlotType(SlotType.None);
				itemDataSlot.SetSprite(itemData.GetSprite());
				itemDataSlot.SetBackground(itemData.GetGradeSprite());
				itemDataSlot.EnableBestMark(recommendMark);
				itemDataSlot.SetEventListener(this);
				recommendSlots.Add(itemDataSlot);
			}
		}


		public void SetFocusItem(ItemData itemData)
		{
			foreach (ItemDataSlot itemDataSlot in recommendSlots)
			{
				itemDataSlot.EnableSelection(false);
				if (itemDataSlot.GetItemData() != null && itemDataSlot.GetItemData().code == itemData.code)
				{
					itemDataSlot.EnableSelection(true);
				}
			}
		}


		private void RebuildRecommendUI()
		{
			for (int i = 0; i < categoryList.Count; i++)
			{
				MonoBehaviourInstance<BlisLayoutBuilder>.inst.Rebuild(categoryList[i]);
			}

			MonoBehaviourInstance<BlisLayoutBuilder>.inst.Rebuild(content);
		}
	}
}