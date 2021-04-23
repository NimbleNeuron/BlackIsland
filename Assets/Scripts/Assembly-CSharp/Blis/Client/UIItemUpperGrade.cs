using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIItemUpperGrade : BaseControl
	{
		private ItemData curItem;


		private GridLayoutGroup grid;


		private RectTransform gridRectTransform;


		private LayoutElement layoutElement;


		private Text notice;


		private ItemDataSlotTable slotTable;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			notice = GameUtil.Bind<Text>(gameObject, "Notice");
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
			curItem = itemData;
			List<ItemData> upperGradeItems = GameDB.item.GetUpperGradeItems(curItem);
			upperGradeItems.Sort(delegate(ItemData x, ItemData y)
			{
				int num = IsNeedBlink(y).CompareTo(IsNeedBlink(x));
				if (num == 0)
				{
					num = x.code.CompareTo(y.code);
				}

				return num;
			});
			if (upperGradeItems.Count > 0)
			{
				for (int i = 0; i < upperGradeItems.Count; i++)
				{
					ItemData itemData2 = upperGradeItems[i];
					ItemDataSlot itemDataSlot = slotTable.CreateSlot(itemData2);
					if (itemDataSlot != null)
					{
						itemDataSlot.SetItemData(itemData2);
						itemDataSlot.SetSlotType(SlotType.None);
						itemDataSlot.SetSprite(itemData2.GetSprite());
						itemDataSlot.SetBackground(itemData2.GetGradeSprite());
						itemDataSlot.SetUseTooltip(false);
						if (IsNeedBlink(itemData2))
						{
							itemDataSlot.PlayBlink();
						}
					}
				}

				notice.gameObject.SetActive(false);
			}
			else
			{
				notice.gameObject.SetActive(true);
			}

			MonoBehaviourInstance<BlisLayoutBuilder>.inst.Rebuild(gridRectTransform);
			MonoBehaviourInstance<BlisLayoutBuilder>.inst.Rebuild(rectTransform);
		}


		private bool IsNeedBlink(ItemData targetItem)
		{
			return !(MonoBehaviourInstance<ClientService>.inst == null) &&
			       MonoBehaviourInstance<ClientService>.inst.IsPlayer &&
			       (targetItem.itemType != ItemType.Weapon ||
			        MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsEquipableWeapon(targetItem)) &&
			       MonoBehaviourInstance<ClientService>.inst.MyPlayer.IsCombinableWithMaterial(targetItem, curItem);
		}


		public void Clear()
		{
			layoutElement.ignoreLayout = true;
			transform.localScale = Vector3.zero;
		}
	}
}