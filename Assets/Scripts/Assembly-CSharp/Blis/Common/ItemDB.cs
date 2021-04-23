using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Blis.Common
{
	public class ItemDB
	{
		private readonly List<int> GetCollectAndHuntItemsReturnValue = new List<int>();
		private List<BulletCapacity> bulletCapacityList;
		private List<CollectAndHuntData> collectAndHuntItems;
		private List<ItemArmorData> itemArmorList;
		private List<ItemConsumableData> itemConsumableList;
		private List<ItemFindInfo> itemFindInfos;
		private List<ItemMiscData> itemMiscList;
		private List<ItemOptionCategoryData> itemOptionCategories;
		private Dictionary<int, ItemData> items;
		private List<ItemSpecialData> itemSpecialList;
		private List<ItemWeaponData> itemWeaponList;
		private Dictionary<int, List<ItemData>> upperItems;

		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(ItemWeaponData))
			{
				itemWeaponList = data.Cast<ItemWeaponData>().ToList<ItemWeaponData>();
				return;
			}

			if (typeFromHandle == typeof(ItemArmorData))
			{
				itemArmorList = data.Cast<ItemArmorData>().ToList<ItemArmorData>();
				return;
			}

			if (typeFromHandle == typeof(ItemConsumableData))
			{
				itemConsumableList = data.Cast<ItemConsumableData>().ToList<ItemConsumableData>();
				return;
			}

			if (typeFromHandle == typeof(ItemMiscData))
			{
				itemMiscList = data.Cast<ItemMiscData>().ToList<ItemMiscData>();
				return;
			}

			if (typeFromHandle == typeof(ItemSpecialData))
			{
				itemSpecialList = data.Cast<ItemSpecialData>().ToList<ItemSpecialData>();
				return;
			}

			if (typeFromHandle == typeof(BulletCapacity))
			{
				bulletCapacityList = data.Cast<BulletCapacity>().ToList<BulletCapacity>();
				return;
			}

			if (typeFromHandle == typeof(ItemOptionCategoryData))
			{
				itemOptionCategories = data.Cast<ItemOptionCategoryData>().ToList<ItemOptionCategoryData>();
				return;
			}

			if (typeFromHandle == typeof(ItemFindInfo))
			{
				itemFindInfos = data.Cast<ItemFindInfo>().ToList<ItemFindInfo>();
				return;
			}

			if (typeFromHandle == typeof(CollectAndHuntData))
			{
				collectAndHuntItems = data.Cast<CollectAndHuntData>().ToList<CollectAndHuntData>();
			}
		}


		public void PostInitialize()
		{
			items = new Dictionary<int, ItemData>();
			AddItem<ItemWeaponData>(itemWeaponList);
			AddItem<ItemArmorData>(itemArmorList);
			AddItem<ItemConsumableData>(itemConsumableList);
			AddItem<ItemMiscData>(itemMiscList);
			AddItem<ItemSpecialData>(itemSpecialList);
			upperItems = new Dictionary<int, List<ItemData>>();
			foreach (KeyValuePair<int, ItemData> keyValuePair in items)
			{
				upperItems.Add(keyValuePair.Key, GameDB.item.GetUpperGradeItems(keyValuePair.Value));
			}
		}


		private void AddItem<T>(List<T> itemList) where T : ItemData
		{
			if (itemList == null)
			{
				return;
			}

			foreach (T t in itemList)
			{
				items.Add(t.code, t);
			}
		}


		public ItemData FindItemByCode(int code)
		{
			ItemData result = null;
			if (items.TryGetValue(code, out result))
			{
				return result;
			}

			Log.E("Failed to find item by itemCode[{0}]", code);
			return new ItemData(0, "None", ItemType.None, ItemGrade.None, 0, 0, 0, 0, "");
		}


		public List<ItemData> FindUpperItemsByCode(int code)
		{
			List<ItemData> result = null;
			if (upperItems.TryGetValue(code, out result))
			{
				return result;
			}

			Log.E("Failed to find upperItems by itemCode[{0}]", code);
			return new List<ItemData>();
		}


		public List<int> FindItemsByCategory(ItemOptionCategory itemOptionCategory)
		{
			if (itemOptionCategories != null)
			{
				List<int> list = new List<int>();
				foreach (ItemOptionCategoryData itemOptionCategoryData in itemOptionCategories)
				{
					if (itemOptionCategoryData.itemOptionCategory == itemOptionCategory)
					{
						list.Add(itemOptionCategoryData.itemCode);
					}
				}

				return list;
			}

			Log.E("itemOptionCategories is null");
			return new List<int>();
		}


		public Dictionary<int, ItemData>.ValueCollection GetAllItems()
		{
			return items.Values;
		}


		public ItemData GetRandomWeaponItem(WeaponType weaponType, ItemGrade itemGrade)
		{
			IEnumerable<ItemData> source = from item in items.Values
				where item.itemType == ItemType.Weapon && item.itemGrade == itemGrade &&
				      item.GetSubTypeData<ItemWeaponData>().weaponType == weaponType
				select item;
			return source.ElementAtOrDefault(Random.Range(0, source.Count<ItemData>()));
		}


		public ItemData GetRandomArmorItem(EquipSlotType type, ItemGrade itemGrade)
		{
			IEnumerable<ItemData> source = from item in items.Values
				where item.itemType == ItemType.Armor && item.itemGrade == itemGrade &&
				      item.GetSubTypeData<ItemArmorData>().armorType == type.GetArmorType()
				select item;
			return source.ElementAtOrDefault(Random.Range(0, source.Count<ItemData>()));
		}


		public ItemData GetRandomItemByEquipSlot(EquipSlotType type, WeaponType weaponType, ItemGrade itemGrade)
		{
			ItemType itemType = type == EquipSlotType.Weapon ? ItemType.Weapon : ItemType.Armor;
			IEnumerable<ItemData> source = null;
			if (itemType == ItemType.Armor)
			{
				source = from item in source
					where item.itemType == itemType && item.itemGrade == itemGrade &&
					      item.GetSubTypeData<ItemArmorData>().armorType == type.GetArmorType()
					select item;
			}
			else
			{
				source = from item in source
					where item.itemType == itemType && item.itemGrade == itemGrade &&
					      item.GetSubTypeData<ItemWeaponData>().weaponType == weaponType
					select item;
			}

			return source.ElementAtOrDefault(Random.Range(0, source.Count<ItemData>()));
		}


		public ItemData GetRandomItem(ItemType itemType, ItemGrade itemGrade)
		{
			IEnumerable<ItemData> source = from item in items.Values
				where item.itemType == itemType && item.itemGrade == itemGrade
				select item;
			return source.ElementAtOrDefault(Random.Range(0, source.Count<ItemData>()));
		}


		public ItemData GetRandomConsumableItem(ItemConsumableType type, ItemGrade itemGrade)
		{
			IEnumerable<ItemData> source = from item in items.Values
				where item.itemType == ItemType.Consume && item.itemGrade == itemGrade &&
				      item.GetSubTypeData<ItemConsumableData>().consumableType == type
				select item;
			return source.ElementAtOrDefault(Random.Range(0, source.Count<ItemData>()));
		}


		public ItemData GetRandomSummonItem(ItemGrade itemGrade)
		{
			IEnumerable<ItemData> source = items.Values.Where(delegate(ItemData item)
			{
				if (item.itemType != ItemType.Special || item.itemGrade != itemGrade)
				{
					return false;
				}

				ItemSpecialData subTypeData = item.GetSubTypeData<ItemSpecialData>();
				SummonData summonData = GameDB.character.GetSummonData(subTypeData.summonCode);
				return item.GetSubTypeData<ItemSpecialData>().specialItemType == SpecialItemType.Summon &&
				       summonData.summonAttackType > SummonAttackType.None;
			});
			return source.ElementAtOrDefault(Random.Range(0, source.Count<ItemData>()));
		}


		public int GetBulletCapacity(int itemCode)
		{
			BulletCapacity bulletCapacity = bulletCapacityList.Find(m => m.itemCode == itemCode);
			if (bulletCapacity == null)
			{
				return 0;
			}

			return bulletCapacity.capacity;
		}


		public BulletCapacity GetBulletCapacityData(int itemCode)
		{
			return bulletCapacityList.Find(m => m.itemCode == itemCode);
		}


		public float GetGunReloadTime(int itemCode)
		{
			BulletCapacity bulletCapacity = bulletCapacityList.Find(m => m.itemCode == itemCode);
			if (bulletCapacity == null)
			{
				return 0f;
			}

			return bulletCapacity.time;
		}


		public ItemFindInfo GetItemFindInfo(int itemCode)
		{
			return itemFindInfos.Find(x => x.itemCode == itemCode);
		}


		public List<int> GetCollectAndHuntItems()
		{
			GetCollectAndHuntItemsReturnValue.Clear();
			foreach (CollectAndHuntData collectAndHuntData in collectAndHuntItems)
			{
				GetCollectAndHuntItemsReturnValue.Add(collectAndHuntData.itemCode);
			}

			return GetCollectAndHuntItemsReturnValue;
		}


		public bool IsCollectAndHuntItem(int itemCode)
		{
			using (List<CollectAndHuntData>.Enumerator enumerator = collectAndHuntItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.itemCode == itemCode)
					{
						return true;
					}
				}
			}

			return false;
		}


		public List<ItemData> AnalyzeItem(ItemData itemData)
		{
			List<ItemData> list = new List<ItemData>();
			if (itemData == null)
			{
				return list;
			}

			int i = 0;
			list.Add(itemData);
			while (i < list.Count)
			{
				ItemData itemData2 = list[i];
				if (itemData2.makeMaterial1 > 0)
				{
					list.Add(FindItemByCode(itemData2.makeMaterial1));
				}

				if (itemData2.makeMaterial2 > 0)
				{
					list.Add(FindItemByCode(itemData2.makeMaterial2));
				}

				i++;
			}

			return list;
		}


		public List<ItemData> GetCombinableItems(List<Item> items)
		{
			List<ItemData> list = new List<ItemData>();
			for (int i = 0; i < items.Count; i++)
			{
				if (!list.Contains(items[i].ItemData))
				{
					list.Add(items[i].ItemData);
				}
			}

			List<ItemData> list2 = new List<ItemData>();
			for (int j = 0; j < list.Count; j++)
			{
				List<ItemData> list3 = FindUpperItemsByCode(list[j].code);
				for (int k = j + 1; k < list.Count; k++)
				{
					ItemData compareItem = list[k];
					list2.AddRange(list3.FindAll(x =>
						x.makeMaterial1 == compareItem.code || x.makeMaterial2 == compareItem.code));
				}
			}

			return list2;
		}


		public List<ItemData> AnalyzePreferItemData(List<ItemData> coreItems)
		{
			List<ItemData> list = new List<ItemData>();
			for (int i = 0; i < coreItems.Count; i++)
			{
				ItemData itemData = coreItems[i];
				List<ItemData> list2 = AnalyzeItem(itemData);
				for (int j = 0; j < list2.Count; j++)
				{
					if (!list2[j].IsLeafNodeItem() && !list.Contains(list2[j]))
					{
						list.Add(list2[j]);
					}
				}
			}

			return list;
		}


		public List<ItemData> GetUpperGradeItems(ItemData itemData)
		{
			List<ItemData> list = new List<ItemData>();
			foreach (ItemData itemData2 in GetAllItems())
			{
				if (itemData2.makeMaterial1 == itemData.code || itemData2.makeMaterial2 == itemData.code)
				{
					list.Add(itemData2);
				}
			}

			return list;
		}


		public bool IsCollectibleItem(ItemData itemData)
		{
			return itemFindInfos.Exists(x => x.itemCode == itemData.code && x.collectibleCode > 0);
		}


		public bool IsSameCategory(ItemData itemData1, ItemData itemData2)
		{
			if (itemData1.itemType != itemData2.itemType)
			{
				return false;
			}

			if (itemData1.itemType == ItemType.Weapon)
			{
				return itemData1.GetSubTypeData<ItemWeaponData>().weaponType ==
				       itemData2.GetSubTypeData<ItemWeaponData>().weaponType;
			}

			if (itemData1.itemType == ItemType.Armor)
			{
				return itemData1.GetSubTypeData<ItemArmorData>().armorType ==
				       itemData2.GetSubTypeData<ItemArmorData>().armorType;
			}

			if (itemData1.itemType == ItemType.Consume)
			{
				return itemData1.GetSubTypeData<ItemConsumableData>().consumableType ==
				       itemData2.GetSubTypeData<ItemConsumableData>().consumableType;
			}

			return itemData1.itemType != ItemType.Special ||
			       itemData1.GetSubTypeData<ItemSpecialData>().specialItemType ==
			       itemData2.GetSubTypeData<ItemSpecialData>().specialItemType;
		}


		public Item GetItemFromCharacter(int materialItemCode, ItemType resultItemType, Inventory inventory,
			Equipment equipment)
		{
			if (resultItemType == ItemType.Weapon)
			{
				return equipment.FindEquip(materialItemCode) ?? inventory.FindItem(materialItemCode);
			}

			return inventory.FindItem(materialItemCode) ?? equipment.FindEquip(materialItemCode);
		}


		public bool IsCombinable(ItemData itemData, Inventory inventory, Equipment equipment)
		{
			Item dummyItem = new Item(0, itemData.code, itemData.initialCount, 0, itemData);
			Item item = null;
			Item item2 = null;
			return IsCombinable(dummyItem, inventory, equipment, ref item, ref item2);
		}


		public bool IsCombinable(ItemData itemData, Inventory inventory, Equipment equipment, ref Item material_1,
			ref Item material_2)
		{
			Item dummyItem = new Item(0, itemData.code, itemData.initialCount, 0, itemData);
			return IsCombinable(dummyItem, inventory, equipment, ref material_1, ref material_2);
		}


		public bool IsCombinable(Item dummyItem, Inventory inventory, Equipment equipment, ref Item material_1,
			ref Item material_2)
		{
			int makeMaterial = dummyItem.ItemData.makeMaterial1;
			int makeMaterial2 = dummyItem.ItemData.makeMaterial2;
			Item item = inventory.FindItem(makeMaterial);
			Item item2 = equipment.FindEquip(makeMaterial);
			Item item3 = inventory.FindItem(makeMaterial2);
			Item item4 = equipment.FindEquip(makeMaterial2);
			if (item == null && item2 == null || item3 == null && item4 == null)
			{
				return false;
			}

			if (inventory.CanAddItem(dummyItem))
			{
				return true;
			}

			if (equipment.CanEquip(dummyItem))
			{
				return true;
			}

			bool? flag = null;
			if (material_1 != null)
			{
				if (item != null && material_1.id == item.id)
				{
					flag = true;
				}

				if (item2 != null && material_1.id == item2.id)
				{
					flag = false;
				}
			}

			bool? flag2 = null;
			if (material_2 != null)
			{
				if (item3 != null && material_2.id == item3.id)
				{
					flag2 = true;
				}

				if (item4 != null && material_2.id == item4.id)
				{
					flag2 = false;
				}
			}

			bool result = false;
			bool? flag3 = flag;
			bool flag4 = false;
			if ((flag3.GetValueOrDefault() == flag4) & (flag3 != null))
			{
				if (IsCombinable(item2, dummyItem.ItemData, inventory, equipment))
				{
					material_1 = item2;
					result = true;
				}
				else if (IsCombinable(item, dummyItem.ItemData, inventory, equipment))
				{
					material_1 = item;
					result = true;
				}
			}
			else if (IsCombinable(item, dummyItem.ItemData, inventory, equipment))
			{
				material_1 = item;
				result = true;
			}
			else if (IsCombinable(item2, dummyItem.ItemData, inventory, equipment))
			{
				material_1 = item2;
				result = true;
			}

			flag3 = flag2;
			flag4 = false;
			if ((flag3.GetValueOrDefault() == flag4) & (flag3 != null))
			{
				if (IsCombinable(item4, dummyItem.ItemData, inventory, equipment))
				{
					material_2 = item4;
					result = true;
				}
				else if (IsCombinable(item3, dummyItem.ItemData, inventory, equipment))
				{
					material_2 = item3;
					result = true;
				}
			}
			else if (IsCombinable(item3, dummyItem.ItemData, inventory, equipment))
			{
				material_2 = item3;
				result = true;
			}
			else if (IsCombinable(item4, dummyItem.ItemData, inventory, equipment))
			{
				material_2 = item4;
				result = true;
			}

			return result;
		}


		private bool IsCombinable(Item source, ItemData itemData, Inventory inventory, Equipment equipment)
		{
			if (source == null)
			{
				return false;
			}

			if (source.Amount <= 1)
			{
				Item item = equipment.FindEquipById(source.id);
				if (item != null && itemData.IsEquipItem() &&
				    item.ItemData.GetEquipSlotType().Equals(itemData.GetEquipSlotType()))
				{
					return true;
				}

				if (GameDB.item.IsSameCategory(source.ItemData, itemData))
				{
					return true;
				}

				if (inventory.FindItemById(source.id, source.madeType) != null)
				{
					return true;
				}
			}

			return false;
		}


		public void SetFavoriteSourceDictionary(List<int> favoriteItemCodes, Dictionary<ItemData, int> result,
			bool removeUpper)
		{
			result.Clear();
			List<ItemData> list = new List<ItemData>();
			List<ItemData> list2 = new List<ItemData>();
			List<ItemData> list3 = new List<ItemData>();
			foreach (int code in favoriteItemCodes)
			{
				ItemData item = GameDB.item.FindItemByCode(code);
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}

			for (int i = 0; i < list.Count; i++)
			{
				List<ItemData> list4 = GameDB.item.AnalyzeItem(list[i]);
				for (int j = 0; j < list4.Count; j++)
				{
					ItemData itemData = list4[j];
					if (!itemData.IsLeafNodeItem() && !list3.Contains(itemData))
					{
						list3.Add(itemData);
					}

					if (result.ContainsKey(itemData))
					{
						ItemData key = itemData;
						int num = result[key];
						result[key] = num + 1;
					}
					else
					{
						result.Add(itemData, 1);
					}
				}
			}

			new List<ItemData>();
			List<ItemData> list5 = list3.FindAll(x => x.initialCount > 1);
			list5.Sort((x, y) => y.itemGrade.CompareTo(x.itemGrade));
			foreach (ItemData itemData2 in list5)
			{
				foreach (KeyValuePair<ItemData, int> keyValuePair in result)
				{
					if (itemData2 == keyValuePair.Key)
					{
						if (keyValuePair.Value > 1 && !list2.Contains(itemData2))
						{
							list2.Add(itemData2);
							Recursive(result, list2, itemData2);
						}

						break;
					}
				}
			}

			if (removeUpper)
			{
				foreach (ItemData key2 in list3)
				{
					result.Remove(key2);
				}
			}
		}


		private void Recursive(Dictionary<ItemData, int> result, List<ItemData> recursiveList, ItemData curItemData)
		{
			if (curItemData.makeMaterial1 != 0 && curItemData.makeMaterial2 != 0)
			{
				ItemData material1 = result.Keys.First(x => x.code == curItemData.makeMaterial1);
				ItemData material2 = result.Keys.First(x => x.code == curItemData.makeMaterial2);
				ItemData key;
				if (material1.makeMaterial1 != 0 && material1.makeMaterial2 != 0)
				{
					int minus = GetMinus(result, curItemData);
					key = material1;
					result[key] -= minus;
					ItemData itemData = result.Keys.First(x => x.code == material1.makeMaterial1);
					ItemData itemData2 = result.Keys.First(x => x.code == material1.makeMaterial2);
					key = itemData;
					result[key] -= minus;
					key = itemData2;
					result[key] -= minus;
					recursiveList.Add(material1);
					Recursive(result, recursiveList, material1);
				}
				else
				{
					int minusChild = GetMinusChild(result, curItemData);
					key = material1;
					result[key] -= minusChild;
				}

				if (material2.makeMaterial1 != 0 && material2.makeMaterial2 != 0)
				{
					int minus2 = GetMinus(result, curItemData);
					key = material2;
					result[key] -= minus2;
					ItemData itemData3 = result.Keys.First(x => x.code == material2.makeMaterial1);
					ItemData itemData4 = result.Keys.First(x => x.code == material2.makeMaterial2);
					key = itemData3;
					result[key] -= minus2;
					key = itemData4;
					result[key] -= minus2;
					recursiveList.Add(material2);
					Recursive(result, recursiveList, material2);
					return;
				}

				int minusChild2 = GetMinusChild(result, curItemData);
				key = material2;
				result[key] -= minusChild2;
			}
		}


		private int GetMinus(Dictionary<ItemData, int> result, ItemData itemData)
		{
			int num = 0;
			if (itemData.initialCount == 2)
			{
				num = result[itemData] / itemData.initialCount;
			}
			else if (itemData.initialCount >= 3)
			{
				if (result[itemData] <= itemData.initialCount)
				{
					num = result[itemData] - 1;
				}
				else
				{
					num = result[itemData] / itemData.initialCount;
					if (result[itemData] % itemData.initialCount != 0)
					{
						num++;
					}
				}
			}

			return num;
		}


		public int GetMinusChild(int originNeedCount, int findCount, ItemData itemData)
		{
			int result = 0;
			if (itemData.initialCount == 1)
			{
				result = findCount;
			}
			else if (itemData.initialCount >= 2)
			{
				if (findCount >= originNeedCount)
				{
					int num = originNeedCount / itemData.initialCount;
					result = originNeedCount % itemData.initialCount == 0 ? num : num + 1;
				}
				else
				{
					int num2 = originNeedCount / itemData.initialCount;
					int num3 = originNeedCount % itemData.initialCount == 0 ? num2 : num2 + 1;
					int num4 = (originNeedCount - findCount) / itemData.initialCount;
					int num5 = (originNeedCount - findCount) % itemData.initialCount == 0 ? num4 : num4 + 1;
					result = num3 - num5;
				}
			}

			return result;
		}


		private int GetMinusChild(Dictionary<ItemData, int> result, ItemData itemData)
		{
			int num = 0;
			if (itemData.initialCount == 2)
			{
				num = result[itemData] / itemData.initialCount;
			}
			else if (itemData.initialCount >= 3)
			{
				int num2 = result[itemData] / itemData.initialCount;
				num = result[itemData] % itemData.initialCount == 0 ? num2 : num2 + 1;
				num = result[itemData] - num;
			}

			return num;
		}


		public class NavItemComparer : IComparer<int>
		{
			private readonly List<ItemData> preferItemData;


			private readonly ItemDB itemDB;

			public NavItemComparer(List<ItemData> preferItemData, ItemDB itemDB)
			{
				this.preferItemData = preferItemData;
				this.itemDB = itemDB;
			}


			public int Compare(int x, int y)
			{
				ItemData xItemData = itemDB.FindItemByCode(x);
				ItemData yItemData = itemDB.FindItemByCode(y);
				if (xItemData.itemGrade != yItemData.itemGrade)
				{
					return yItemData.itemGrade.CompareTo(xItemData.itemGrade);
				}

				bool value = preferItemData.Exists(i => i.code == x);
				int num = preferItemData.Exists(i => i.code == y).CompareTo(value);
				if (num == 0)
				{
					List<ItemData> upperGradeItems = itemDB.GetUpperGradeItems(xItemData);
					List<ItemData> upperGradeItems2 = itemDB.GetUpperGradeItems(yItemData);
					value = upperGradeItems.Exists(i => itemDB.IsSameCategory(i, xItemData));
					num = upperGradeItems2.Exists(i => itemDB.IsSameCategory(i, yItemData)).CompareTo(value);
				}

				if (num == 0)
				{
					return x.CompareTo(y);
				}

				return num;
			}
		}


		public class BookItemComparer : IComparer<ItemData>
		{
			public int Compare(ItemData x, ItemData y)
			{
				int num = x.itemType.CompareTo(y.itemType);
				num = num != 0 ? num : x.GetSubType().CompareTo(y.GetSubType());
				num = num != 0 ? num : x.itemGrade.CompareTo(y.itemGrade);
				return num != 0 ? num : x.code.CompareTo(y.code);
			}
		}
	}
}