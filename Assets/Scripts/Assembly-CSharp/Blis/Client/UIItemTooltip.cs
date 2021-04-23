using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIItemTooltip : UITooltipComponent
	{
		[SerializeField] private UIItemTooltipComponent uiEquipParent = default;


		[SerializeField] private Transform separator = default;


		[SerializeField] private UIItemTooltipComponent uiItemParent = default;


		private EquipItemSlot equipItemSlot;

		public void SetItem(Item itemData, int amount, bool showCompare)
		{
			SetItemCommonUI(itemData.ItemData, amount, showCompare);
			uiItemParent.SetItem(itemData, amount, false);
			transform.localScale = Vector3.one;
		}


		public void SetItem(ItemData itemData, int amount, bool showCompare)
		{
			SetItemCommonUI(itemData, amount, showCompare);
			uiItemParent.SetItem(itemData, amount, false);
			transform.localScale = Vector3.one;
		}


		private void SetItemCommonUI(ItemData itemData, int amount, bool showCompare)
		{
			Clear();
			if (showCompare)
			{
				Item compareEquipItemData =
					MonoBehaviourInstance<GameUI>.inst.StatusHud.GetCompareEquipItemData(itemData);
				CompareItem(compareEquipItemData);
			}
		}


		private void CompareItem(Item equipItem)
		{
			if (MonoBehaviourInstance<GameUI>.inst == null)
			{
				return;
			}

			if (equipItem == null)
			{
				return;
			}

			uiEquipParent.SetItem(equipItem, 0, true);
			separator.transform.localScale = Vector3.one;
			separator.GetComponent<LayoutElement>().ignoreLayout = false;
		}


		public override void Clear()
		{
			base.Clear();
			transform.localScale = Vector3.zero;
			uiItemParent.Clear();
			uiEquipParent.Clear();
			separator.transform.localScale = Vector3.zero;
			separator.GetComponent<LayoutElement>().ignoreLayout = true;
		}
	}
}