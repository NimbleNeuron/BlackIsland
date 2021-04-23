using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIItemSources : BaseControl
	{
		private GridLayoutGroup grid;


		private RectTransform gridRectTransform;


		private LayoutElement layoutElement;


		private ItemDataSlotTable slotTable;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			slotTable = GameUtil.Bind<ItemDataSlotTable>(gameObject, "Grid");
			grid = GameUtil.Bind<GridLayoutGroup>(gameObject, "Grid");
			gridRectTransform = (RectTransform) grid.transform;
			GameUtil.Bind<LayoutElement>(gameObject, ref layoutElement);
		}


		public void UpdateUI(ItemData itemData)
		{
			layoutElement.ignoreLayout = false;
			transform.localScale = Vector3.one;
			slotTable.Clear();
			ItemData item = GameDB.item.FindItemByCode(itemData.makeMaterial1);
			ItemData item2 = GameDB.item.FindItemByCode(itemData.makeMaterial2);
			List<ItemData> list = new List<ItemData>();
			list.Add(item);
			list.Add(item2);
			for (int i = 0; i < list.Count; i++)
			{
				ItemData itemData2 = list[i];
				ItemDataSlot itemDataSlot = slotTable.CreateSlot(itemData2);
				if (itemDataSlot != null)
				{
					itemDataSlot.SetItemData(itemData2);
					itemDataSlot.SetSlotType(SlotType.None);
					itemDataSlot.SetSprite(itemData2.GetSprite());
					itemDataSlot.SetBackground(itemData2.GetGradeSprite());
					itemDataSlot.SetUseTooltip(false);
				}
			}

			MonoBehaviourInstance<BlisLayoutBuilder>.inst.Rebuild(gridRectTransform);
			MonoBehaviourInstance<BlisLayoutBuilder>.inst.Rebuild(rectTransform);
		}


		public void Clear()
		{
			layoutElement.ignoreLayout = true;
			transform.localScale = Vector3.zero;
		}
	}
}