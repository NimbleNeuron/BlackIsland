using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIItemHeader : BaseControl
	{
		[SerializeField] private Image itemImg = default;


		[SerializeField] private Text itemName = default;


		[SerializeField] private Text itemAmount = default;


		[SerializeField] private Text itemClass = default;

		protected override void OnStartUI()
		{
			base.OnStartUI();
			itemImg.color = Color.clear;
		}


		public void EmptyUI()
		{
			itemImg.color = Color.clear;
			itemName.text = "";
			itemAmount.text = "";
			itemClass.text = "";
		}


		public void UpdateUI(Item item)
		{
			UpdateItemSprite(item.ItemData.GetSprite());
			itemName.text = LnUtil.GetItemName(item.ItemData.code);
			itemName.color = item.ItemData.itemGrade.GetColor();
			if (item.Amount > 0)
			{
				itemAmount.text = Ln.Format("{0} 개", item.Amount);
			}
			else
			{
				itemAmount.text = null;
			}

			UpdateItemType(item.ItemData);
		}


		public void UpdateUI(ItemData itemData)
		{
			UpdateItemSprite(itemData.GetSprite());
			itemName.text = LnUtil.GetItemName(itemData.code);
			itemName.color = itemData.itemGrade.GetColor();
			itemAmount.text = null;
			UpdateItemType(itemData);
		}


		private void UpdateItemSprite(Sprite sprite)
		{
			itemImg.sprite = sprite;
			if (sprite != null)
			{
				itemImg.color = Color.white;
				return;
			}

			itemImg.color = Color.clear;
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
	}
}