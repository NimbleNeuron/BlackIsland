using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UINavItem : BaseUI, ILnEventHander
	{
		[SerializeField] private GameObject tutorialSquareNavi = default;


		[SerializeField] private GameObject tutorialBoxNaviKnife = default;


		[SerializeField] private GameObject tutorialBoxNaviElectronic = default;


		private Dictionary<int, AreaRestrictionState> areaStateMap;


		private Button closeBtn;


		private int currentAreaCode;


		private LayoutElement layoutElement;


		private ItemDataSlotTableEventListener materialSlotEventListener;


		private ItemDataSlotTable materialTable;


		private LayoutElement materialTableLayoutElement;


		private Dictionary<ItemData, int> ownItems = new Dictionary<ItemData, int>();


		private LayoutElement separatorLayoutElement;


		private Dictionary<ItemData, int> sourceItems = new Dictionary<ItemData, int>();


		private ItemData targetItem;


		private Image titleImg;


		private Text titleName;


		private Text titlePrefix;


		private ItemDataSlotTableEventListener wayPointSlotEventListener;


		private ItemDataSlotTable wayPointTable;


		private LayoutElement wayPointTableLayoutElement;


		public void OnLnDataChange()
		{
			UpdateTitle();
		}

		
		
		public event NavigationEvent.NavWayPointItemClickEvent OnNavWayPointItemClick;


		
		
		public event NavigationEvent.NavMaterialItemClickEvent OnNavMaterialItemClick;


		
		
		public event NavigationEvent.NavigationCloseEvent OnNavigationClose;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<LayoutElement>(gameObject, ref layoutElement);
			titlePrefix = GameUtil.Bind<Text>(gameObject, "Title/Contents/Title");
			titleImg = GameUtil.Bind<Image>(gameObject, "Title/Contents/Item");
			titleName = GameUtil.Bind<Text>(gameObject, "Title/Contents/Name");
			closeBtn = GameUtil.Bind<Button>(gameObject, "Title/BtnClose");
			closeBtn.onClick.RemoveAllListeners();
			closeBtn.onClick.AddListener(Close);
			separatorLayoutElement = GameUtil.Bind<LayoutElement>(gameObject, "Separator");
			wayPointTableLayoutElement = GameUtil.Bind<LayoutElement>(gameObject, "WayPoint");
			materialTableLayoutElement = GameUtil.Bind<LayoutElement>(gameObject, "Material");
			wayPointTable = GameUtil.Bind<ItemDataSlotTable>(gameObject, "WayPoint");
			materialTable = GameUtil.Bind<ItemDataSlotTable>(gameObject, "Material");
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			materialSlotEventListener = new ItemDataSlotTableEventListener(InvokeMaterialClick);
			wayPointSlotEventListener = new ItemDataSlotTableEventListener(InvokeWayPointClick);
			wayPointTable.SetSlotEventListener(wayPointSlotEventListener);
			materialTable.SetSlotEventListener(materialSlotEventListener);
		}


		private void Close()
		{
			NavigationEvent.NavigationCloseEvent onNavigationClose = OnNavigationClose;
			if (onNavigationClose == null)
			{
				return;
			}

			onNavigationClose();
		}


		private void RenderView()
		{
			if (targetItem == null)
			{
				ResetNavUI();
				return;
			}

			wayPointTable.Clear();
			materialTable.Clear();
			foreach (KeyValuePair<ItemData, int> keyValuePair in sourceItems)
			{
				if (keyValuePair.Value > 0)
				{
					ItemDataSlot itemDataSlot = null;
					SlotStyle style = default;
					if (keyValuePair.Key.IsLeafNodeItem())
					{
						itemDataSlot = materialTable.CreateSlot(keyValuePair.Key);
						bool flag = false;
						bool flag2 = false;
						bool flag3 = false;
						foreach (int num in Singleton<ItemService>.inst.GetDropArea(keyValuePair.Key.code))
						{
							flag = true;
							flag3 = flag3 || areaStateMap[num] != AreaRestrictionState.Restricted;
							flag2 = flag2 || num == currentAreaCode;
						}

						if (flag2)
						{
							style.isNeed = true;
						}
						else if (!flag)
						{
							style.isRandomItem = true;
						}
						else if (!flag3)
						{
							style.isRestricted = true;
						}
					}
					else
					{
						itemDataSlot = wayPointTable.CreateSlot(keyValuePair.Key);
						style.isCombineable =
							ownItems.ContainsKey(GameDB.item.FindItemByCode(keyValuePair.Key.makeMaterial1)) &&
							ownItems.ContainsKey(GameDB.item.FindItemByCode(keyValuePair.Key.makeMaterial2));
					}

					int num2 = 0;
					if (ownItems.ContainsKey(keyValuePair.Key))
					{
						num2 = ownItems[keyValuePair.Key];
					}

					int num3 = keyValuePair.Value - num2;
					if (num3 <= 0)
					{
						style.isOwn = true;
						style.isNeed = false;
					}
					else if (keyValuePair.Value > 1)
					{
						style.stackText = string.Format("{0}/{1}", keyValuePair.Value - num3, keyValuePair.Value);
					}

					RenderSlot(itemDataSlot, keyValuePair.Key, style);
				}
			}

			bool flag4 = materialTable.GetCount() > 0;
			materialTableLayoutElement.ignoreLayout = !flag4;
			materialTableLayoutElement.transform.localScale = flag4 ? Vector3.one : Vector3.zero;
			bool flag5 = wayPointTable.GetCount() > 0;
			separatorLayoutElement.ignoreLayout = !flag5;
			separatorLayoutElement.transform.localScale = flag5 ? Vector3.one : Vector3.zero;
			wayPointTableLayoutElement.ignoreLayout = !flag5;
			wayPointTableLayoutElement.transform.localScale = flag5 ? Vector3.one : Vector3.zero;
			UpdateTitle();
		}


		private void RenderSlot(ItemDataSlot itemDataSlot, ItemData itemData, SlotStyle style)
		{
			itemDataSlot.SetItemData(itemData);
			itemDataSlot.SetSlotType(SlotType.Favorite);
			itemDataSlot.SetSprite(itemData.GetSprite());
			itemDataSlot.SetBackground(itemData.GetGradeSprite());
			itemDataSlot.EnableOwnMark(style.isOwn);
			itemDataSlot.SetStackText(style.stackText);
			itemDataSlot.EnableNeedMark(style.isNeed);
			itemDataSlot.EnableBlockLine(style.isRestricted);
			itemDataSlot.EnableRandomMark(style.isRandomItem);
			if (style.isCombineable)
			{
				itemDataSlot.PlayBlink();
			}
		}


		private void UpdateTitle()
		{
			titlePrefix.text = Ln.Get("목표") + " : ";
			titleImg.enabled = true;
			titleName.enabled = true;
			if (targetItem != null)
			{
				titleImg.sprite = targetItem.GetSprite();
				titleName.text = LnUtil.GetItemName(targetItem.code);
				titleName.color = targetItem.itemGrade.GetColor();
			}
		}


		public void ShowTutorialSquareNavi(bool show)
		{
			tutorialSquareNavi.SetActive(show);
		}


		public void ShowTutorialBoxNavi(bool show, int itemCode)
		{
			if (itemCode != 101201)
			{
				if (itemCode == 401211)
				{
					tutorialBoxNaviElectronic.SetActive(show);
				}
			}
			else
			{
				tutorialBoxNaviKnife.SetActive(show);
			}
		}


		private void ResetNavUI()
		{
			materialTableLayoutElement.ignoreLayout = true;
			materialTableLayoutElement.transform.localScale = Vector3.zero;
			separatorLayoutElement.ignoreLayout = true;
			separatorLayoutElement.transform.localScale = Vector3.zero;
			wayPointTableLayoutElement.ignoreLayout = true;
			wayPointTableLayoutElement.transform.localScale = Vector3.zero;
		}


		public void OnClickTargetItem()
		{
			MonoBehaviourInstance<GameUI>.inst.CombineWindow.SelectItem(targetItem);
			MonoBehaviourInstance<GameUI>.inst.OpenWindow(MonoBehaviourInstance<GameUI>.inst.CombineWindow);
		}


		public void OnUpdateTargetItem(ItemData itemData, Dictionary<ItemData, int> sourceItems,
			Dictionary<ItemData, int> ownItems)
		{
			targetItem = itemData;
			this.sourceItems = sourceItems;
			this.ownItems = ownItems;
			RenderView();
		}


		public void SetCurrentArea(int areaCode)
		{
			currentAreaCode = areaCode;
			RenderView();
		}


		public void SetAreaStateMap(Dictionary<int, AreaRestrictionState> areaStateMap)
		{
			this.areaStateMap = areaStateMap;
			RenderView();
		}


		private void InvokeMaterialClick(ItemDataSlot itemDataSlot)
		{
			if (itemDataSlot.GetItemData() != null)
			{
				NavigationEvent.NavMaterialItemClickEvent onNavMaterialItemClick = OnNavMaterialItemClick;
				if (onNavMaterialItemClick == null)
				{
					return;
				}

				onNavMaterialItemClick(itemDataSlot.GetItemData());
			}
		}


		private void InvokeWayPointClick(ItemDataSlot itemDataSlot)
		{
			if (itemDataSlot.GetItemData() != null)
			{
				NavigationEvent.NavWayPointItemClickEvent onNavWayPointItemClick = OnNavWayPointItemClick;
				if (onNavWayPointItemClick == null)
				{
					return;
				}

				onNavWayPointItemClick(targetItem, itemDataSlot.GetItemData());
			}
		}


		public void Show()
		{
			layoutElement.ignoreLayout = false;
			transform.localScale = Vector3.one;
		}


		public void Hide()
		{
			layoutElement.ignoreLayout = true;
			transform.localScale = Vector3.zero;
		}


		private struct SlotStyle
		{
			public bool isCombineable;


			public bool isRandomItem;


			public bool isRestricted;


			public bool isNeed;


			public bool isOwn;


			public string stackText;
		}


		private class ItemDataSlotTableEventListener : ISlotEventListener
		{
				public delegate void ItemDataCallBack(ItemDataSlot itemDataSlot);


			private readonly ItemDataCallBack cb;

			public ItemDataSlotTableEventListener(ItemDataCallBack cb)
			{
				this.cb = cb;
			}


			public void OnSlotLeftClick(Slot slot)
			{
				ItemDataSlot itemDataSlot = (ItemDataSlot) slot;
				if (itemDataSlot == null)
				{
					return;
				}

				if (itemDataSlot.GetItemData() == null)
				{
					return;
				}

				ItemDataCallBack itemDataCallBack = cb;
				if (itemDataCallBack == null)
				{
					return;
				}

				itemDataCallBack(itemDataSlot);
			}


			public void OnSlotRightClick(Slot slot) { }


			public void OnDropItem(Slot slot, BaseUI draggedUI) { }


			public void OnThrowItem(Slot slot) { }


			public void OnThrowItemPiece(Slot slot) { }


			public void OnPointerEnter(Slot slot) { }


			public void OnPointerExit(Slot slot) { }


			public void OnSlotDoubleClick(Slot slot) { }
		}
	}
}