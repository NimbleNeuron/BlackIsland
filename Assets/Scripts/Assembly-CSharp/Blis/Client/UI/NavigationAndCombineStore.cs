using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client.UI
{
	[UIActionMapping(typeof(UpdateEquipment), typeof(UpdateInventory), typeof(UpdateNavTarget), typeof(UpdateNavFocus))]
	public class NavigationAndCombineStore : UIStore<NavigationAndCombineStore>
	{
		private readonly List<Item> belongItems = new List<Item>();


		private readonly HashSet<ItemData> combineable = new HashSet<ItemData>();


		private readonly List<Item> equipments = new List<Item>();


		private readonly List<Item> inventories = new List<Item>();


		private readonly Dictionary<ItemData, int> needFocusSourceItems = new Dictionary<ItemData, int>();


		private readonly Dictionary<ItemData, int> needSourceItems = new Dictionary<ItemData, int>();


		private readonly Dictionary<ItemData, int> ownFocusSourceItems = new Dictionary<ItemData, int>();


		private readonly Dictionary<ItemData, int> ownSourceItems = new Dictionary<ItemData, int>();


		private readonly List<ItemData> targetItems = new List<ItemData>();


		private ItemData focusItem;

		public List<ItemData> GetTargetItems()
		{
			return targetItems;
		}


		public List<Item> GetInventoryItems()
		{
			return inventories;
		}


		public List<Item> GetEquipItems()
		{
			return equipments;
		}


		public List<Item> GetBelongItems()
		{
			return belongItems;
		}


		public HashSet<ItemData> GetCombinableItems()
		{
			return combineable;
		}


		public Dictionary<ItemData, int> GetNeedSourceItems()
		{
			return needSourceItems;
		}


		public Dictionary<ItemData, int> GetOwnSourceItems()
		{
			return ownSourceItems;
		}


		public ItemData GetFocusItem()
		{
			return focusItem;
		}


		public Dictionary<ItemData, int> GetNeedFocusSourceItems()
		{
			return needFocusSourceItems;
		}


		public Dictionary<ItemData, int> GetOwnFocusSourceItems()
		{
			return ownFocusSourceItems;
		}


		protected override void ActionHandle(UIAction action)
		{
			action.IfTypeIs<UpdateInventory>(delegate(UpdateInventory data)
			{
				inventories.Clear();
				inventories.AddRange(data.inventory);
			});
			action.IfTypeIs<UpdateEquipment>(delegate(UpdateEquipment data)
			{
				equipments.Clear();
				equipments.AddRange(data.equipment);
			});
			action.IfTypeIs<UpdateNavTarget>(delegate(UpdateNavTarget data)
			{
				targetItems.Clear();
				targetItems.AddRange(data.targetItems);
			});
			action.IfTypeIs<UpdateNavFocus>(delegate(UpdateNavFocus data) { focusItem = data.focusItem; });
		}


		protected override void PreCommit()
		{
			belongItems.Clear();
			foreach (Item item6 in inventories)
			{
				Item item = new Item(item6.id, item6.itemCode, item6.Amount, item6.Bullet);
				Item item2 = belongItems.Find(x => x.itemCode == item.itemCode);
				if (item2 != null)
				{
					item2.ForceAddAmount(item.Amount);
				}
				else
				{
					belongItems.Add(item);
				}
			}

			foreach (Item item3 in equipments)
			{
				Item item = new Item(item3.id, item3.itemCode, item3.Amount, item3.Bullet);
				Item item4 = belongItems.Find(x => x.itemCode == item.itemCode);
				if (item4 != null)
				{
					item4.ForceAddAmount(item.Amount);
				}
				else
				{
					belongItems.Add(item);
				}
			}

			GameDB.item.SetFavoriteSourceDictionary((from x in targetItems
				select x.code).ToList<int>(), needSourceItems, false);
			CalcOwnItem(needSourceItems, ownSourceItems, belongItems);
			if (focusItem != null)
			{
				GameDB.item.SetFavoriteSourceDictionary(new List<int>
				{
					focusItem.code
				}, needFocusSourceItems, false);
				CalcOwnItem(needFocusSourceItems, ownFocusSourceItems, belongItems);
			}

			combineable.Clear();
			Item item5 = equipments.Find(x => x.ItemData.itemType == ItemType.Weapon);
			bool flag = true;
			if (item5 != null)
			{
				using (List<Item>.Enumerator enumerator = inventories.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.itemCode == item5.itemCode)
						{
							flag = false;
							break;
						}
					}
				}
			}

			foreach (ItemData itemData in GameDB.item.GetCombinableItems(belongItems))
			{
				if ((itemData.itemType != ItemType.Weapon || itemData.IsLeafNodeItem() ||
				     MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsEquipableWeapon(itemData)) &&
				    (!flag || item5 == null || item5.Amount >= 2 || itemData.itemType == ItemType.Weapon ||
				     itemData.makeMaterial1 != item5.itemCode && itemData.makeMaterial2 != item5.itemCode))
				{
					combineable.Add(itemData);
				}
			}
		}


		private void AnalyzeItem(List<ItemData> itemDataList, Dictionary<ItemData, int> result)
		{
			result.Clear();
			for (int i = 0; i < itemDataList.Count; i++)
			{
				List<ItemData> list = GameDB.item.AnalyzeItem(itemDataList[i]);
				for (int j = 0; j < list.Count; j++)
				{
					ItemData itemData = list[j];
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
		}


		private void CalcOwnItem(Dictionary<ItemData, int> needs, Dictionary<ItemData, int> owns, List<Item> belongs)
		{
			owns.Clear();
			foreach (Item item in belongs)
			{
				if (needs.ContainsKey(item.ItemData))
				{
					int amount = item.Amount;
					if (!item.ItemData.IsLeafNodeItem())
					{
						RecursiveChild(needs, item.ItemData, amount);
					}
					else if (owns.ContainsKey(item.ItemData))
					{
						ItemData itemData = item.ItemData;
						int num = owns[itemData];
						owns[itemData] = num + 1;
					}
					else
					{
						owns.Add(item.ItemData, item.Amount);
					}
				}
			}
		}


		private void RecursiveChild(Dictionary<ItemData, int> needs, ItemData itemData, int findCount)
		{
			if (needs.ContainsKey(itemData))
			{
				int originNeedCount = needs[itemData];
				int num = needs[itemData] - findCount;
				if (num <= 0)
				{
					needs.Remove(itemData);
				}
				else
				{
					needs[itemData] = num;
				}

				if (itemData.makeMaterial1 > 0)
				{
					int minusChild = GameDB.item.GetMinusChild(originNeedCount, findCount, itemData);
					ItemData itemData2 = GameDB.item.FindItemByCode(itemData.makeMaterial1);
					RecursiveChild(needs, itemData2, minusChild);
				}

				if (itemData.makeMaterial2 > 0)
				{
					int minusChild2 = GameDB.item.GetMinusChild(originNeedCount, findCount, itemData);
					ItemData itemData3 = GameDB.item.FindItemByCode(itemData.makeMaterial2);
					RecursiveChild(needs, itemData3, minusChild2);
				}
			}
		}


		public override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			inventories.Clear();
			equipments.Clear();
			targetItems.Clear();
			combineable.Clear();
			focusItem = null;
		}
	}
}