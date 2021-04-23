using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class UIItemInfo : BaseUI
	{
		public delegate void OnClickItemEvent(ItemData itemData);


		[SerializeField] private UICombineUpperGrade uiCombineUpperGrade = default;


		[SerializeField] private UIItemTree uiItemTree = default;


		[SerializeField] private UIItemCombine uiItemCombine = default;

		
		
		public event OnClickItemEvent OnClickItemHandler = delegate { };


		
		
		public event OnClickItemEvent OnClickTreeItemHandler = delegate { };


		
		
		public event OnClickItemEvent OnRightClickItem = delegate { };


		
		
		public event OnClickItemEvent OnRequestCombineItem = delegate { };


		
		
		public event OnClickItemEvent OnClickNavItem = delegate { };


		
		
		public event OnClickItemEvent OnClickAdminItem = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			uiCombineUpperGrade.OnClickUpperGradeItem += OnClickUpperItem;
			uiCombineUpperGrade.OnRightClickItem += delegate(ItemData data) { OnRightClickItem(data); };
			uiItemTree.OnClickTreeItem += delegate(ItemData data) { OnClickTreeItemHandler(data); };
			uiItemTree.OnClickHistoryItem += delegate(ItemData data) { OnClickItemHandler(data); };
			uiItemTree.OnRightClickItem += delegate(ItemData data) { OnRightClickItem(data); };
			uiItemCombine.OnClickCombineItem += delegate(ItemData data) { OnRequestCombineItem(data); };
			uiItemCombine.OnClickNavigation += delegate(ItemData data) { OnClickNavItem(data); };
			uiItemCombine.OnClickAdmin += delegate(ItemData data) { OnClickAdminItem(data); };
		}


		private void OnClickUpperItem(ItemData itemData)
		{
			if (uiItemTree.GetRootItemData() != itemData)
			{
				uiItemTree.AddItemHistory(uiItemTree.GetRootItemData());
			}

			OnClickItemHandler(itemData);
		}


		public void SetTreeRootItem(ItemData itemData)
		{
			uiItemTree.SetRootItem(itemData);
			SetFocusItem(itemData);
		}


		public void SetFocusItem(ItemData itemData)
		{
			uiItemTree.SetSelection(itemData.code);
			uiCombineUpperGrade.SetSourceItemData(itemData);
			uiItemCombine.SetTargetItem(itemData);
		}


		public void SetWeaponTypes(WeaponType[] weaponType)
		{
			uiCombineUpperGrade.SetWeaponTypes(weaponType);
		}


		public void ResetHistory()
		{
			uiItemTree.ClearHistory();
		}


		public void EmptyUI()
		{
			uiCombineUpperGrade.EmptyUI();
			uiItemTree.ResetSlots();
			uiItemCombine.EmptyUI();
		}


		public void OnUpdateInventory(List<Item> items)
		{
			uiItemCombine.OnUpdateInventory(items);
			uiCombineUpperGrade.OnUpdateInventory(items);
			uiItemTree.Refresh();
		}


		public void OnUpdateEquipment(List<Item> items)
		{
			uiItemCombine.OnUpdateEquipment(items);
			uiCombineUpperGrade.OnUpdateEquipment(items);
			uiItemTree.Refresh();
		}


		public void OnUpdateButtonState()
		{
			uiItemCombine.OnUpdateButtonState();
		}


		public void SetPreferItems(List<ItemData> items)
		{
			uiCombineUpperGrade.SetPreferItems(items);
		}


		public void HighLightUpperGrade(bool enable)
		{
			if (enable)
			{
				uiCombineUpperGrade.HighLightOn();
				return;
			}

			uiCombineUpperGrade.HighLightOff();
		}
	}
}