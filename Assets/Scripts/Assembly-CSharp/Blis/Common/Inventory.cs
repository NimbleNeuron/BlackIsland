using System;
using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class Inventory
	{
		public readonly int MaxCount = 10;


		private readonly List<InvenItem> ret = new List<InvenItem>();


		private readonly Item[] items;


		private readonly HashSet<int> updatedIndexes = new HashSet<int>();


		public Inventory()
		{
			items = new Item[MaxCount];
		}


		public int Count {
			get { return items.Count(x => x != null); }
		}


		public List<InvenItem> FlushUpdates()
		{
			ret.Clear();
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] != null && items[i].FlushDirty())
				{
					updatedIndexes.Add(i);
				}
			}

			foreach (int num in updatedIndexes)
			{
				ret.Add(new InvenItem(num, items[num]));
			}

			updatedIndexes.Clear();
			return ret;
		}


		public List<InvenItem> CreateSnapshot()
		{
			List<InvenItem> list = new List<InvenItem>();
			for (int i = 0; i < items.Length; i++)
			{
				list.Add(new InvenItem(i, items[i]));
			}

			return list;
		}


		public bool AddItem(Item item, out int remainCount)
		{
			remainCount = 0;
			if (item == null || item.IsEmpty())
			{
				return false;
			}

			if (FindItemById(item.id, item.madeType) != null)
			{
				return false;
			}

			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] != null && items[i].itemCode == item.itemCode && items[i].madeType == item.madeType)
				{
					items[i].Merge(item);
					updatedIndexes.Add(i);
					if (item.IsEmpty())
					{
						return true;
					}
				}
			}

			remainCount = item.Amount;
			for (int j = 0; j < items.Length; j++)
			{
				if (items[j] == null)
				{
					items[j] = item;
					updatedIndexes.Add(j);
					remainCount = 0;
					return true;
				}
			}

			return false;
		}


		public bool ForceAddItem(int index, Item item)
		{
			if (item == null || item.IsEmpty())
			{
				return false;
			}

			items[index] = null;
			items[index] = item;
			return true;
		}


		public bool RemoveItem(Item item)
		{
			if (item == null)
			{
				return false;
			}

			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] != null && items[i].id == item.id)
				{
					items[i] = null;
					updatedIndexes.Add(i);
					return true;
				}
			}

			return false;
		}


		public Item RemoveItemById(int itemId, ItemMadeType madeType)
		{
			Item item = FindItemById(itemId, madeType);
			if (RemoveItem(item))
			{
				return item;
			}

			return null;
		}


		public void SubItemById(int itemId, ItemMadeType madeType)
		{
			Item item = FindItemById(itemId, madeType);
			int num = Array.IndexOf<Item>(items, item);
			if (num >= 0)
			{
				item.SubAmount(1);
				UpdateItem(new InvenItem(num, item));
			}
		}


		public void SwapItem(int indexA, int indexB)
		{
			if (0 <= indexA && indexA < items.Length && 0 <= indexB && indexB < items.Length)
			{
				if (items[indexB] == null)
				{
					return;
				}

				if (items[indexA] != null && !items[indexA].IsFull())
				{
					items[indexB].Merge(items[indexA]);
				}

				Item item = items[indexB];
				items[indexB] = items[indexA];
				items[indexA] = item;
				if (items[indexA] != null && items[indexA].IsEmpty())
				{
					items[indexA] = null;
				}

				if (items[indexB] != null && items[indexB].IsEmpty())
				{
					items[indexB] = null;
				}

				updatedIndexes.Add(indexA);
				updatedIndexes.Add(indexB);
			}
		}


		public bool IsFull()
		{
			return items.Count(item => item == null) == 0;
		}


		public List<Item> GetItems()
		{
			List<Item> list = new List<Item>();
			foreach (Item item in items)
			{
				if (item != null)
				{
					list.Add(item);
				}
			}

			return list;
		}


		public List<int> CreateItemIndexes()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] != null && !items[i].IsEmpty())
				{
					list.Add(i);
				}
			}

			return list;
		}


		public int GetInventoryInsertableCount(ItemData itemData)
		{
			List<Item> list = GetItems();
			int num = 0;
			if (list.Count < MaxCount)
			{
				int num2 = MaxCount - list.Count;
				num += itemData.stackable * num2;
			}

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].itemCode == itemData.code && !list[i].IsFull())
				{
					num += list[i].ItemData.stackable - list[i].Amount;
				}
			}

			return num;
		}


		public int GetItemIndex(Predicate<Item> predicate)
		{
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] != null && predicate(items[i]))
				{
					return i;
				}
			}

			return -1;
		}


		public Item FindByIndex(int index)
		{
			return items[index];
		}


		public Item FindItem(int itemCode)
		{
			Item result = null;
			int num = 100;
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] != null && items[i].ItemData.code.Equals(itemCode) && !items[i].IsEmpty() &&
				    num > items[i].Amount)
				{
					result = items[i];
					num = items[i].Amount;
				}
			}

			return result;
		}


		public List<Item> FindItems(int itemCode)
		{
			return (from x in items
				where x != null && x.ItemData.code == itemCode && !x.IsEmpty()
				select x).Reverse<Item>().ToList<Item>();
		}


		public Item FindItemById(int id, ItemMadeType madeType)
		{
			return items.FirstOrDefault(item => item != null && item.id == id && item.madeType == madeType);
		}


		public bool CanAddItem(Item item)
		{
			List<Item> list = GetItems();
			if (list.Exists(x => x.id == item.id))
			{
				return false;
			}

			if (list.Count < MaxCount)
			{
				return true;
			}

			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].itemCode == item.itemCode && item.madeType.Equals(list[i].madeType) && !list[i].IsFull())
				{
					num += list[i].ItemData.stackable - list[i].Amount;
				}
			}

			return num >= item.Amount;
		}


		public void UpdateItem(InvenItem invenItem)
		{
			items[invenItem.slot] = invenItem.item;
		}


		public void UseItem(Item targetItem)
		{
			int num = Array.IndexOf<Item>(items, targetItem);
			if (num >= 0)
			{
				targetItem.SubAmount(1);
				UpdateItem(new InvenItem(num, targetItem));
			}
		}


		public bool HasItem(int itemCode)
		{
			return items.Count(item => item != null && item.itemCode == itemCode) > 0;
		}
	}
}