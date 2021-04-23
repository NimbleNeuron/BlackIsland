using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIItemSearch : BaseUI, ISlotEventListener
	{
		public delegate void ListSelectionChangeEvent(ItemData itemData);


		public delegate void RequestCombineEvent(ItemData itemData);


		private readonly List<ItemData> preferItems = new List<ItemData>();


		private UIItemCategory allLabel;


		private Dictionary<UIItemCategory, ArmorType> armorGroup;


		private UIItemCategory armorLabel;


		private List<UIItemCategory> categoryItems;


		private ScrollContentWrapper contentWrapper;


		private Dictionary<UIItemCategory, ItemConsumableType> foodGroup;


		private UIItemCategory foodLabel;


		private List<ItemDataSlot> itemDataSlots;


		private Dictionary<UIItemCategory, MiscItemType> miscGroup;


		private UIItemCategory miscLabel;


		private ScrollRect scrollRect;


		private Dictionary<UIItemCategory, SpecialItemType> specialGroup;


		private UIItemCategory specialLabel;


		private bool updateRegisterFlag;


		private Dictionary<UIItemCategory, WeaponType> weaponGroup;


		private UIItemCategory weaponLabel;


		private void LateUpdate()
		{
			if (updateRegisterFlag)
			{
				updateRegisterFlag = false;
				if (allLabel.IsPin)
				{
					categoryItems.ForEach(delegate(UIItemCategory x) { x.SetPin(true); });
					foreach (KeyValuePair<UIItemCategory, WeaponType> keyValuePair in weaponGroup)
					{
						if (keyValuePair.Key.gameObject.activeSelf && keyValuePair.Value == WeaponType.None)
						{
							keyValuePair.Key.SetPin(false);
						}
					}
				}

				if (weaponLabel.IsPin)
				{
					foreach (KeyValuePair<UIItemCategory, WeaponType> keyValuePair2 in weaponGroup)
					{
						if (keyValuePair2.Key.IsActive() && keyValuePair2.Value != WeaponType.None)
						{
							keyValuePair2.Key.SetPin(true);
						}
					}
				}

				if (armorLabel.IsPin)
				{
					foreach (KeyValuePair<UIItemCategory, ArmorType> keyValuePair3 in armorGroup)
					{
						keyValuePair3.Key.SetPin(true);
					}
				}

				if (foodLabel.IsPin)
				{
					foreach (KeyValuePair<UIItemCategory, ItemConsumableType> keyValuePair4 in foodGroup)
					{
						keyValuePair4.Key.SetPin(true);
					}
				}

				if (specialLabel.IsPin)
				{
					foreach (KeyValuePair<UIItemCategory, SpecialItemType> keyValuePair5 in specialGroup)
					{
						keyValuePair5.Key.SetPin(true);
					}
				}

				if (miscLabel.IsPin)
				{
					foreach (KeyValuePair<UIItemCategory, MiscItemType> keyValuePair6 in miscGroup)
					{
						keyValuePair6.Key.SetPin(true);
					}
				}

				List<WeaponType> weaponTypes = new List<WeaponType>();
				foreach (KeyValuePair<UIItemCategory, WeaponType> keyValuePair7 in weaponGroup)
				{
					if (keyValuePair7.Key.IsActive() && keyValuePair7.Key.IsPin)
					{
						if (keyValuePair7.Value == WeaponType.None)
						{
							List<WeaponType> equipableType = weaponGroup.Values.ToList<WeaponType>();
							weaponTypes.AddRange(from WeaponType x in Enum.GetValues(typeof(WeaponType))
								where !equipableType.Contains(x)
								select x);
						}
						else
						{
							weaponTypes.Add(keyValuePair7.Value);
						}
					}
				}

				List<ArmorType> armorTypes = new List<ArmorType>();
				foreach (KeyValuePair<UIItemCategory, ArmorType> keyValuePair8 in armorGroup)
				{
					if (keyValuePair8.Key.IsActive() && keyValuePair8.Key.IsPin)
					{
						armorTypes.Add(keyValuePair8.Value);
					}
				}

				List<ItemConsumableType> consumableType = new List<ItemConsumableType>();
				foreach (KeyValuePair<UIItemCategory, ItemConsumableType> keyValuePair9 in foodGroup)
				{
					if (keyValuePair9.Key.IsActive() && keyValuePair9.Key.IsPin)
					{
						consumableType.Add(keyValuePair9.Value);
					}
				}

				List<SpecialItemType> specialItemTypes = new List<SpecialItemType>();
				foreach (KeyValuePair<UIItemCategory, SpecialItemType> keyValuePair10 in specialGroup)
				{
					if (keyValuePair10.Key.IsActive() && keyValuePair10.Key.IsPin)
					{
						specialItemTypes.Add(keyValuePair10.Value);
					}
				}

				List<MiscItemType> miscItemTypes = new List<MiscItemType>();
				foreach (KeyValuePair<UIItemCategory, MiscItemType> keyValuePair11 in miscGroup)
				{
					if (keyValuePair11.Key.IsActive() && keyValuePair11.Key.IsPin)
					{
						miscItemTypes.Add(keyValuePair11.Value);
					}
				}

				contentWrapper.SetDataList((from x in GameDB.item.GetAllItems()
					where false |
					      (x.itemType == ItemType.Weapon &&
					       weaponTypes.Contains(x.GetSubTypeData<ItemWeaponData>().weaponType)) |
					      (x.itemType == ItemType.Armor &&
					       armorTypes.Contains(x.GetSubTypeData<ItemArmorData>().armorType)) |
					      (x.itemType == ItemType.Consume &&
					       consumableType.Contains(x.GetSubTypeData<ItemConsumableData>().consumableType)) |
					      (x.itemType == ItemType.Special &&
					       specialItemTypes.Contains(x.GetSubTypeData<ItemSpecialData>().specialItemType)) |
					      (x.itemType == ItemType.Misc &&
					       miscItemTypes.Contains(x.GetSubTypeData<ItemMiscData>().miscItemType))
					select x).ToList<ItemData>());
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
			itemDataSlots = new List<ItemDataSlot>(contentWrapper.GetComponentsInChildren<ItemDataSlot>(true));
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
			weaponLabel = GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/WeaponLabel");
			armorLabel = GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/ArmorLabel");
			foodLabel = GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/FoodLabel");
			specialLabel = GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/Etc");
			miscLabel = GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/MiscLabel");
			weaponGroup = new Dictionary<UIItemCategory, WeaponType>
			{
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Weapon_0"),
					WeaponType.None
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Weapon_1"),
					WeaponType.None
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Weapon_2"),
					WeaponType.None
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Weapon_3"),
					WeaponType.None
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Weapon_4"),
					WeaponType.None
				}
			};
			armorGroup = new Dictionary<UIItemCategory, ArmorType>
			{
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Armor_0"),
					ArmorType.Chest
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Armor_1"),
					ArmorType.Head
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Armor_2"),
					ArmorType.Arm
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Armor_3"),
					ArmorType.Leg
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Armor_4"),
					ArmorType.Trinket
				}
			};
			foodGroup = new Dictionary<UIItemCategory, ItemConsumableType>
			{
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Food_0"),
					ItemConsumableType.Food
				},
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Food_1"),
					ItemConsumableType.Beverage
				}
			};
			specialGroup = new Dictionary<UIItemCategory, SpecialItemType>
			{
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Special_0"),
					SpecialItemType.Summon
				}
			};
			miscGroup = new Dictionary<UIItemCategory, MiscItemType>
			{
				{
					GameUtil.Bind<UIItemCategory>(gameObject, "Category/List/CategoryItem_Misc_0"),
					MiscItemType.Material
				}
			};
			categoryItems = new List<UIItemCategory>();
			categoryItems.Add(allLabel);
			categoryItems.Add(weaponLabel);
			categoryItems.Add(armorLabel);
			categoryItems.Add(foodLabel);
			categoryItems.Add(specialLabel);
			categoryItems.Add(miscLabel);
			categoryItems.AddRange(weaponGroup.Keys);
			categoryItems.AddRange(armorGroup.Keys);
			categoryItems.AddRange(foodGroup.Keys);
			categoryItems.AddRange(specialGroup.Keys);
			categoryItems.AddRange(miscGroup.Keys);
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
			foreach (KeyValuePair<UIItemCategory, ArmorType> keyValuePair in armorGroup)
			{
				keyValuePair.Key.Text.text = Ln.Get(string.Format("ArmorType/{0}", keyValuePair.Value));
			}

			foreach (KeyValuePair<UIItemCategory, ItemConsumableType> keyValuePair2 in foodGroup)
			{
				keyValuePair2.Key.Text.text = Ln.Get(string.Format("ItemConsumableType/{0}", keyValuePair2.Value));
			}

			foreach (KeyValuePair<UIItemCategory, SpecialItemType> keyValuePair3 in specialGroup)
			{
				keyValuePair3.Key.Text.text = Ln.Get(string.Format("SpecialItemType/{0}", keyValuePair3.Value));
			}

			foreach (KeyValuePair<UIItemCategory, MiscItemType> keyValuePair4 in miscGroup)
			{
				keyValuePair4.Key.Text.text = Ln.Get(string.Format("MiscItemType/{0}", keyValuePair4.Value));
			}
		}


		public void SetWeaponMastery(List<WeaponType> weaponMasteryList)
		{
			foreach (KeyValuePair<UIItemCategory, WeaponType> keyValuePair in weaponGroup)
			{
				keyValuePair.Key.gameObject.SetActive(false);
			}

			Dictionary<UIItemCategory, WeaponType> dictionary = new Dictionary<UIItemCategory, WeaponType>();
			using (Dictionary<UIItemCategory, WeaponType>.Enumerator enumerator2 = weaponGroup.GetEnumerator())
			{
				for (int i = 0; i < weaponMasteryList.Count; i++)
				{
					if (enumerator2.MoveNext())
					{
						KeyValuePair<UIItemCategory, WeaponType> keyValuePair2 = enumerator2.Current;
						keyValuePair2.Key.Text.text = Ln.Get(string.Format("WeaponType/{0}", weaponMasteryList[i]));
						Dictionary<UIItemCategory, WeaponType> dictionary2 = dictionary;
						keyValuePair2 = enumerator2.Current;
						dictionary2.Add(keyValuePair2.Key, weaponMasteryList[i]);
						keyValuePair2 = enumerator2.Current;
						keyValuePair2.Key.gameObject.SetActive(true);
					}
				}

				if (enumerator2.MoveNext())
				{
					KeyValuePair<UIItemCategory, WeaponType> keyValuePair2 = enumerator2.Current;
					keyValuePair2.Key.Text.text = Ln.Get("그 외");
					Dictionary<UIItemCategory, WeaponType> dictionary3 = dictionary;
					keyValuePair2 = enumerator2.Current;
					dictionary3.Add(keyValuePair2.Key, WeaponType.None);
					keyValuePair2 = enumerator2.Current;
					keyValuePair2.Key.gameObject.SetActive(true);
				}
			}

			foreach (KeyValuePair<UIItemCategory, WeaponType> keyValuePair3 in dictionary)
			{
				weaponGroup[keyValuePair3.Key] = keyValuePair3.Value;
			}
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