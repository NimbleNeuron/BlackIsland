using Blis.Client;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Common
{
	
	public class UIItemTooltipComponent : BaseUI
	{
		
		public void SetItem(Item itemData, int amount, bool isEquip)
		{
			this.SetItemCommonUI(itemData.ItemData, amount, isEquip);
			this.uiItemBody.UpdateUI(itemData);
		}

		
		public void SetItem(ItemData itemData, int amount, bool isEquip)
		{
			this.SetItemCommonUI(itemData, amount, isEquip);
			this.uiItemBody.UpdateUI(itemData);
		}

		
		private void SetItemCommonUI(ItemData itemData, int amount, bool isEquip)
		{
			this.Clear();
			this.uiItemHeader.UpdateUI(itemData, amount);
			this.uiItemUpperGrade.UpdateUI(itemData);
			this.layoutElement.ignoreLayout = false;
			base.transform.localScale = Vector3.one;
			if (isEquip)
			{
				this.uiEquipTitleEquip.transform.localScale = Vector3.one;
			}
		}

		
		public void Clear()
		{
			this.layoutElement.ignoreLayout = true;
			base.transform.localScale = Vector3.zero;
			UITooltipItemHeader uitooltipItemHeader = this.uiItemHeader;
			if (uitooltipItemHeader != null)
			{
				uitooltipItemHeader.Clear();
			}
			UIItemBody uiitemBody = this.uiItemBody;
			if (uiitemBody != null)
			{
				uiitemBody.Clear();
			}
			UIItemUpperGrade uiitemUpperGrade = this.uiItemUpperGrade;
			if (uiitemUpperGrade != null)
			{
				uiitemUpperGrade.Clear();
			}
			UIItemSources uiitemSources = this.uiItemSources;
			if (uiitemSources != null)
			{
				uiitemSources.Clear();
			}
			if (this.uiEquipTitleEquip != null)
			{
				this.uiEquipTitleEquip.transform.localScale = Vector3.zero;
			}
		}

		
		[SerializeField]
		private UITooltipItemHeader uiItemHeader = default;

		
		[SerializeField]
		private UIItemBody uiItemBody = default;

		
		[SerializeField]
		private UIItemUpperGrade uiItemUpperGrade = default;

		
		[SerializeField]
		private UIItemSources uiItemSources = default;

		
		[SerializeField]
		private GameObject uiEquipTitleEquip = default;

		
		[SerializeField]
		private LayoutElement layoutElement = default;
	}
}
