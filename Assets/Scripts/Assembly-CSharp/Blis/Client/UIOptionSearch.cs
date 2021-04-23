using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIOptionSearch : BaseUI, ISlotEventListener
	{
		public delegate void ListSelectionChangeEvent(ItemData itemData);


		public delegate void RequestCombineEvent(ItemData itemData);


		private readonly List<ItemDataSlot> itemDataSlots = new List<ItemDataSlot>();


		private readonly List<ItemData> preferItems = new List<ItemData>();


		private UIItemCategory allLabel;


		private Dictionary<UIItemCategory, ItemOptionCategory> categoryGroup;


		private List<UIItemCategory> categoryItems;


		private ScrollContentWrapper contentWrapper;


		private UIItemCategory defenceLabel;


		private UIItemCategory etcLabel;


		private UIItemCategory offenceLabel;


		private ScrollRect scrollRect;


		private bool updateRegisterFlag;


		private List<WeaponType> weaponMasteryList;


		private void LateUpdate()
		{
			if (updateRegisterFlag)
			{
				updateRegisterFlag = false;
				if (allLabel.IsPin)
				{
					categoryItems.ForEach(delegate(UIItemCategory x) { x.SetPin(true); });
				}

				foreach (KeyValuePair<UIItemCategory, ItemOptionCategory> keyValuePair in categoryGroup)
				{
					ItemOptionCategory value = keyValuePair.Value;
					if (value - ItemOptionCategory.AttackPower > 3)
					{
						if (value - ItemOptionCategory.Defense > 2)
						{
							if (etcLabel.IsPin)
							{
								keyValuePair.Key.SetPin(true);
							}
						}
						else if (defenceLabel.IsPin)
						{
							keyValuePair.Key.SetPin(true);
						}
					}
					else if (offenceLabel.IsPin)
					{
						keyValuePair.Key.SetPin(true);
					}
				}

				List<ItemOptionCategory> list = (from x in categoryGroup
					where x.Key.IsPin
					select x.Value).ToList<ItemOptionCategory>();
				HashSet<ItemData> hashSet = new HashSet<ItemData>();
				for (int i = 0; i < list.Count; i++)
				{
					foreach (int code in (IEnumerable<int>) GameDB.item.FindItemsByCategory(list[i]))
					{
						ItemData itemData = GameDB.item.FindItemByCode(code);
						if (itemData != null)
						{
							if (itemData.itemType == ItemType.Weapon)
							{
								if (weaponMasteryList.Contains(itemData.GetSubTypeData<ItemWeaponData>().weaponType))
								{
									hashSet.Add(itemData);
								}
							}
							else
							{
								hashSet.Add(itemData);
							}
						}
					}
				}

				List<ItemData> list2 = hashSet.ToList<ItemData>();
				list2.Sort(delegate(ItemData x, ItemData y)
				{
					int num = y.itemGrade.CompareTo(x.itemGrade);
					if (num == 0)
					{
						x.code.CompareTo(y.code);
					}

					return num;
				});
				contentWrapper.SetDataList(list2);
			}
		}


		public void OnSlotLeftClick(Slot slot)
		{
			ItemDataSlot itemDataSlot = slot as ItemDataSlot;
			if (itemDataSlot != null && itemDataSlot.GetItemData() != null)
			{
				ListSelectionChangeEvent onClickListItem = OnClickListItem;
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

		
		
		public event ListSelectionChangeEvent OnClickListItem;


		
		
		public event RequestCombineEvent OnRequestCombine;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			scrollRect = GameUtil.Bind<ScrollRect>(gameObject, "ItemScrollView");
			contentWrapper = GameUtil.Bind<ScrollContentWrapper>(gameObject, "ItemScrollView");
			contentWrapper.OnAppear += OnAppear;
			contentWrapper.OnDisappear += OnDisappear;
			contentWrapper.GetComponentsInChildren<ItemDataSlot>(true, itemDataSlots);
			itemDataSlots.ForEach(delegate(ItemDataSlot x)
			{
				x.SetEventListener(this);
				x.OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
				{
					scrollRect.OnBeginDrag(eventData);
				};
				x.OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
				{
					scrollRect.OnEndDrag(eventData);
				};
				x.OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
				{
					scrollRect.OnDrag(eventData);
				};
				x.OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
				{
					scrollRect.OnScroll(eventData);
				};
			});
			InitCategory();
		}


		private void InitCategory()
		{
			allLabel = GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/All");
			offenceLabel = GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/OffenceLabel");
			defenceLabel = GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/DefenceLabel");
			etcLabel = GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/ETC");
			categoryGroup = new Dictionary<UIItemCategory, ItemOptionCategory>
			{
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_0"),
					ItemOptionCategory.AttackPower
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_1"),
					ItemOptionCategory.AttackSpeed
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_2"),
					ItemOptionCategory.CriticalStrike
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_3"),
					ItemOptionCategory.LifeSteal
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_4"),
					ItemOptionCategory.Defense
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_5"),
					ItemOptionCategory.MaxHp
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_6"),
					ItemOptionCategory.HpRegen
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_7"),
					ItemOptionCategory.MoveSpeed
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_8"),
					ItemOptionCategory.MaxSp
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_9"),
					ItemOptionCategory.SpRegen
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_10"),
					ItemOptionCategory.CooldownReduction
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_11"),
					ItemOptionCategory.SightRange
				}
			};
			categoryItems = new List<UIItemCategory>();
			categoryItems.Add(allLabel);
			categoryItems.Add(offenceLabel);
			categoryItems.Add(defenceLabel);
			categoryItems.Add(etcLabel);
			categoryItems.AddRange(categoryGroup.Keys);
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			for (int i = 0; i < categoryItems.Count; i++)
			{
				categoryItems[i].OnPointerClickEvent += delegate(BaseControl control, PointerEventData ed)
				{
					ResetCategory();
					((UIItemCategory) control).SetPin(true);
					updateRegisterFlag = true;
				};
			}

			allLabel.SetPin(true);
			updateRegisterFlag = true;
			foreach (KeyValuePair<UIItemCategory, ItemOptionCategory> keyValuePair in categoryGroup)
			{
				keyValuePair.Key.Text.text = Ln.Get(string.Format("ItemOptionCategory/{0}", keyValuePair.Value));
			}
		}


		public void SetWeaponMastery(List<WeaponType> weaponMasteryList)
		{
			this.weaponMasteryList = weaponMasteryList;
		}


		private void ResetCategory()
		{
			categoryItems.ForEach(delegate(UIItemCategory x) { x.SetPin(false); });
		}


		private void OnAppear(GameObject gameObject, object data)
		{
			ItemData itemData = (ItemData) data;
			ItemDataSlot component = gameObject.GetComponent<ItemDataSlot>();
			component.SetItemData(itemData);
			component.SetSlotType(SlotType.None);
			component.SetSprite(itemData.GetSprite());
			component.SetBackground(itemData.GetGradeSprite());
			component.EnableBestMark(preferItems.Contains(itemData));
			bool enable = itemData.IsLeafNodeItem() && !GameDB.item.IsCollectibleItem(itemData) &&
			              Singleton<ItemService>.inst.GetDropArea(itemData.code).Count == 0;
			component.EnableRandomMark(enable);
		}


		private void OnDisappear(GameObject go)
		{
			go.GetComponent<ItemDataSlot>().ResetSlot();
		}


		public void Refresh()
		{
			itemDataSlots.ForEach(delegate(ItemDataSlot x)
			{
				if (x.gameObject.activeSelf)
				{
					x.SetItemData(x.GetItemData());
					x.SetSlotType(SlotType.None);
				}
			});
		}


		public void SetPreferItems(List<ItemData> preferItems)
		{
			this.preferItems.Clear();
			this.preferItems.AddRange(preferItems);
		}
	}
}