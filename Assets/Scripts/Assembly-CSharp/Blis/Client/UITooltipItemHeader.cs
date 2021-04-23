using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UITooltipItemHeader : BaseUI
	{
		[SerializeField] private ItemDataSlot itemDataSlot = default;


		[SerializeField] private Text itemName = default;


		[SerializeField] private Text itemAmount = default;


		[SerializeField] private Text itemClass = default;


		private LayoutElement layoutElement;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<LayoutElement>(gameObject, ref layoutElement);
		}


		public void UpdateUI(ItemData itemData, int amount)
		{
			layoutElement.ignoreLayout = false;
			transform.localScale = Vector3.one;
			UpdateItemSlot(itemData);
			itemName.text = LnUtil.GetItemName(itemData.code);
			itemName.color = itemData.itemGrade.GetColor();
			if (amount > 0)
			{
				itemAmount.text = Ln.Format("{0} 개", amount);
			}
			else
			{
				itemAmount.text = null;
			}

			UpdateItemType(itemData);
		}


		private void UpdateItemSlot(ItemData itemData)
		{
			itemDataSlot.SetItemData(itemData);
			itemDataSlot.SetSlotType(SlotType.None);
			itemDataSlot.SetSprite(itemData.GetSprite());
			itemDataSlot.SetBackground(itemData.GetGradeSprite());
		}


		private void UpdateItemType(ItemData itemData)
		{
			switch (itemData.itemType)
			{
				case ItemType.Weapon:
					itemClass.text = Ln.Get(string.Format("WeaponType/{0}",
						itemData.GetSubTypeData<ItemWeaponData>().weaponType));
					return;
				case ItemType.Armor:
					itemClass.text = Ln.Get(string.Format("ArmorType/{0}",
						itemData.GetSubTypeData<ItemArmorData>().armorType));
					return;
				case ItemType.Special:
					itemClass.text = Ln.Get(string.Format("SpecialItemType/{0}",
						itemData.GetSubTypeData<ItemSpecialData>().specialItemType));
					return;
				case ItemType.Misc:
					itemClass.text = Ln.Get(string.Format("MiscItemType/{0}",
						itemData.GetSubTypeData<ItemMiscData>().miscItemType));
					return;
				case ItemType.Consume:
					itemClass.text = Ln.Get(string.Format("ItemConsumableType/{0}",
						itemData.GetSubTypeData<ItemConsumableData>().consumableType));
					return;
				default:
					itemClass.text = itemData.itemType.ToString();
					return;
			}
		}


		public void Clear()
		{
			layoutElement.ignoreLayout = true;
			transform.localScale = Vector3.zero;
		}
	}
}