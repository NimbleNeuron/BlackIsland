using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UICombineUpperGrade : BaseControl, ISlotEventListener
	{
		public delegate void ClickUpperGradeItemEvent(ItemData itemData);


		public delegate void RequestCombineEvent(ItemData itemData);


		private const int PageMaxItemCount = 6;


		private readonly List<Item> belongItems = new List<Item>();


		private readonly List<Item> equipments = new List<Item>();


		private readonly List<Item> inventory = new List<Item>();


		private readonly List<ItemData> preferItems = new List<ItemData>();


		private readonly List<WeaponType> weaponTypes = new List<WeaponType>();


		private CanvasGroup canvasGroup;


		private List<ItemData> datasource = new List<ItemData>();


		private CanvasAlphaTweener highLightTweener;


		private bool isRequireUpdate;


		private Button left;


		private int maxPage;


		private Text notice;


		private int page;


		private Button right;


		private ItemDataSlotTable slotTable;


		private void LateUpdate()
		{
			if (isRequireUpdate)
			{
				UpdateUI();
			}
		}


		public void OnSlotLeftClick(Slot slot)
		{
			ItemDataSlot itemDataSlot = slot as ItemDataSlot;
			if (itemDataSlot != null)
			{
				ClickUpperGradeItemEvent onClickUpperGradeItem = OnClickUpperGradeItem;
				if (onClickUpperGradeItem != null)
				{
					onClickUpperGradeItem(itemDataSlot.GetItemData());
				}

				MonoBehaviourInstance<Tooltip>.inst.Hide(GetParentWindow());
			}
		}


		public void OnSlotRightClick(Slot slot)
		{
			ItemDataSlot itemDataSlot = slot as ItemDataSlot;
			if (itemDataSlot != null)
			{
				RequestCombineEvent onRightClickItem = OnRightClickItem;
				if (onRightClickItem == null)
				{
					return;
				}

				onRightClickItem(itemDataSlot.GetItemData());
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

		
		
		public event ClickUpperGradeItemEvent OnClickUpperGradeItem;


		
		
		public event RequestCombineEvent OnRightClickItem;


		public void OnUpdateInventory(List<Item> items)
		{
			inventory.Clear();
			inventory.AddRange(items);
			UpdateBelongItem();
		}


		public void OnUpdateEquipment(List<Item> items)
		{
			equipments.Clear();
			equipments.AddRange(items);
			UpdateBelongItem();
		}


		private void UpdateBelongItem()
		{
			belongItems.Clear();
			belongItems.AddRange(inventory);
			belongItems.AddRange(equipments);
			isRequireUpdate = true;
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			notice = GameUtil.Bind<Text>(gameObject, "Notice");
			slotTable = GameUtil.Bind<ItemDataSlotTable>(gameObject, "Contents/Items");
			left = GameUtil.Bind<Button>(gameObject, "Contents/Left");
			right = GameUtil.Bind<Button>(gameObject, "Contents/Right");
			highLightTweener = GameUtil.Bind<CanvasAlphaTweener>(gameObject, "Blink");
			canvasGroup = GameUtil.Bind<CanvasGroup>(gameObject, "Blink");
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			slotTable.SetSlotEventListener(this);
			UpdateUI();
		}


		public void NextPage()
		{
			int a = page + 1;
			page = a;
			page = Mathf.Min(a, maxPage);
			UpdateUI();
		}


		public void PrePage()
		{
			int a = page - 1;
			page = a;
			page = Mathf.Max(a, 0);
			UpdateUI();
		}


		private void UpdateUI()
		{
			isRequireUpdate = false;
			ResetSlots();
			HighLightOff();
			if (page <= 0)
			{
				left.interactable = false;
				right.interactable = false;
				notice.enabled = true;
				return;
			}

			SortDatasource();
			int num = 6 * (page - 1);
			int num2 = 0;
			while (num2 < 6 && num + num2 < datasource.Count)
			{
				ItemData itemData = datasource[num2 + num];
				ItemDataSlot itemDataSlot = slotTable.CreateSlot(itemData);
				if (itemDataSlot != null)
				{
					itemDataSlot.ResetSlot();
					itemDataSlot.SetItemData(itemData);
					itemDataSlot.SetSlotType(SlotType.None);
					itemDataSlot.SetSprite(itemData.GetSprite());
					if (IsNeedHighLight(itemData))
					{
						itemDataSlot.PlayBlink();
					}

					itemDataSlot.SetBackground(itemData.GetGradeSprite());
					itemDataSlot.EnableBestMark(preferItems.Contains(itemData));
					itemDataSlot.SetLock(!IsLegalItem(itemData));
				}

				num2++;
			}

			left.interactable = page > 1;
			right.interactable = page < maxPage;
		}


		public void EmptyUI()
		{
			datasource.Clear();
			UpdateUI();
		}


		public void Refresh()
		{
			UpdateUI();
		}


		private void ResetSlots()
		{
			slotTable.Clear();
			notice.enabled = false;
		}


		public void SetSourceItemData(ItemData itemData)
		{
			datasource = GameDB.item.GetUpperGradeItems(itemData);
			maxPage = (datasource.Count + 5) / 6;
			page = maxPage > 0 ? 1 : 0;
			isRequireUpdate = true;
		}


		private void SortDatasource()
		{
			datasource.Sort(delegate(ItemData x, ItemData y)
			{
				int num = IsNeedHighLight(y).CompareTo(IsNeedHighLight(x));
				if (num == 0)
				{
					num = IsLegalItem(y).CompareTo(IsLegalItem(x));
				}

				if (num == 0)
				{
					num = x.code.CompareTo(y.code);
				}

				return num;
			});
		}


		private bool IsNeedHighLight(ItemData itemData)
		{
			return !itemData.IsLeafNodeItem() && belongItems.Exists(x => x.itemCode == itemData.makeMaterial1) &&
			       belongItems.Exists(x => x.itemCode == itemData.makeMaterial2);
		}


		private bool IsLegalItem(ItemData itemData)
		{
			return itemData.itemType != ItemType.Weapon ||
			       weaponTypes.Exists(x => x == itemData.GetSubTypeData<ItemWeaponData>().weaponType);
		}


		public void SetWeaponTypes(WeaponType[] weaponTypes)
		{
			this.weaponTypes.Clear();
			this.weaponTypes.AddRange(weaponTypes);
		}


		public void SetPreferItems(List<ItemData> preferItems)
		{
			this.preferItems.Clear();
			this.preferItems.AddRange(preferItems);
		}


		public void HighLightOn()
		{
			highLightTweener.PlayAnimation();
		}


		public void HighLightOff()
		{
			highLightTweener.StopAnimation();
			canvasGroup.alpha = 0f;
		}
	}
}