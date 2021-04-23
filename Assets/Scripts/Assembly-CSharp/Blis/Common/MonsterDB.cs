using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utils;
using Random = UnityEngine.Random;

namespace Blis.Common
{
	public class MonsterDB
	{
		private readonly List<ItemDropGroupData> cloneDropItems = new List<ItemDropGroupData>();


		private List<ItemDropGroupData> dropItemList;


		private Dictionary<int, List<ItemDropGroupData>> fixedDropItemMap;


		private List<MonsterLevelUpStatData> monsterLevelUpStatList;


		private List<MonsterData> monsterList;


		private Dictionary<int, MonsterData> monsterMap;


		private List<MonsterSpawnLevelData> monsterSpawnLevelDataList;


		private Dictionary<int, RandomDistribution<ItemDropGroupData>> randomDropItemMap;

		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(MonsterData))
			{
				monsterList = data.Cast<MonsterData>().ToList<MonsterData>();
				return;
			}

			if (typeFromHandle == typeof(ItemDropGroupData))
			{
				dropItemList = data.Cast<ItemDropGroupData>().ToList<ItemDropGroupData>();
				return;
			}

			if (typeFromHandle == typeof(MonsterLevelUpStatData))
			{
				monsterLevelUpStatList = data.Cast<MonsterLevelUpStatData>().ToList<MonsterLevelUpStatData>();
				return;
			}

			if (typeFromHandle == typeof(MonsterSpawnLevelData))
			{
				monsterSpawnLevelDataList = data.Cast<MonsterSpawnLevelData>().ToList<MonsterSpawnLevelData>();
			}
		}


		public void PostInitialize()
		{
			CreateMonsterMap();
			CreateDropItemMap();
		}


		private void CreateMonsterMap()
		{
			monsterMap = new Dictionary<int, MonsterData>();
			foreach (MonsterData monsterData in monsterList)
			{
				if (monsterMap.ContainsKey(monsterData.code))
				{
					Log.W("MonsterCode[{0}] is duplicated", monsterData.code);
				}
				else
				{
					monsterMap.Add(monsterData.code, monsterData);
				}
			}
		}


		private void CreateDropItemMap()
		{
			fixedDropItemMap = new Dictionary<int, List<ItemDropGroupData>>();
			randomDropItemMap = new Dictionary<int, RandomDistribution<ItemDropGroupData>>();
			Dictionary<int, List<ItemDropGroupData>> dictionary = new Dictionary<int, List<ItemDropGroupData>>();
			foreach (ItemDropGroupData itemDropGroupData in dropItemList)
			{
				Dictionary<int, List<ItemDropGroupData>> dictionary2;
				if (itemDropGroupData.dropType == ItemDropType.Exclusive)
				{
					dictionary2 = fixedDropItemMap;
				}
				else
				{
					if (itemDropGroupData.dropType != ItemDropType.Random)
					{
						continue;
					}

					dictionary2 = dictionary;
				}

				List<ItemDropGroupData> list;
				if (!dictionary2.ContainsKey(itemDropGroupData.groupCode))
				{
					list = new List<ItemDropGroupData>();
					dictionary2.Add(itemDropGroupData.groupCode, list);
				}
				else
				{
					list = dictionary2[itemDropGroupData.groupCode];
				}

				list.Add(itemDropGroupData);
			}

			foreach (int key in dictionary.Keys)
			{
				RandomDistribution<ItemDropGroupData> value =
					new RandomDistribution<ItemDropGroupData>(dictionary[key], data => data.probability);
				randomDropItemMap.Add(key, value);
			}
		}


		public List<ItemDropGroupData> GetFixedDropItems(int dropGroup)
		{
			cloneDropItems.Clear();
			if (fixedDropItemMap.ContainsKey(dropGroup))
			{
				cloneDropItems.AddRange(fixedDropItemMap[dropGroup]);
			}

			return cloneDropItems;
		}


		public List<ItemDropGroupData> GetRandomDropItems(int dropGroup)
		{
			cloneDropItems.Clear();
			foreach (ItemDropGroupData itemDropGroupData in dropItemList)
			{
				if (itemDropGroupData.groupCode == dropGroup && itemDropGroupData.dropType == ItemDropType.Random)
				{
					cloneDropItems.Add(itemDropGroupData);
				}
			}

			return cloneDropItems;
		}


		public List<Item> SampleDropItem(Func<int> itemIdGenerator, int dropGroup, int dropCount,
			bool useInitialCount = false)
		{
			List<Item> list = new List<Item>();
			List<ItemDropGroupData> list2;
			if (fixedDropItemMap.TryGetValue(dropGroup, out list2))
			{
				foreach (ItemDropGroupData data in list2)
				{
					AddDropItem(itemIdGenerator, useInitialCount, data, list);
				}
			}

			RandomDistribution<ItemDropGroupData> randomDistribution;
			if (randomDropItemMap.TryGetValue(dropGroup, out randomDistribution))
			{
				for (int i = 0; i < dropCount; i++)
				{
					ItemDropGroupData data2 = randomDistribution.Sample();
					AddDropItem(itemIdGenerator, useInitialCount, data2, list);
				}
			}

			return list;
		}


		private static void AddDropItem(Func<int> itemIdGenerator, bool useInitialCount, ItemDropGroupData data,
			List<Item> retItems)
		{
			int itemCode = data.itemCode;
			int amount;
			if (useInitialCount)
			{
				amount = GameDB.item.FindItemByCode(itemCode).initialCount;
			}
			else
			{
				amount = Random.Range(data.min, data.max + 1);
			}

			int bulletCapacity = GameDB.item.GetBulletCapacity(itemCode);
			retItems.Add(new Item(itemIdGenerator(), itemCode, amount, bulletCapacity));
		}


		public List<MonsterData> GetAllMonsterData()
		{
			return monsterList;
		}


		public MonsterData GetMonsterData(int monsterCode)
		{
			MonsterData result;
			if (monsterMap.TryGetValue(monsterCode, out result))
			{
				return result;
			}

			Log.E("Failed to find monsterData from monsterCode[{0}]", monsterCode);
			return null;
		}


		public MonsterData GetMonsterData(MonsterType monsterType)
		{
			return monsterMap.Values.FirstOrDefault(m => m.monster == monsterType);
		}


		public MonsterLevelUpStatData GetMonsterLevelUpStatData(int monsterCode)
		{
			return monsterLevelUpStatList.Find(data => data.code == monsterCode);
		}


		public MonsterSpawnLevelData GetMonsterSpawnLevelData(int playerLevel)
		{
			return monsterSpawnLevelDataList.Find(data => data.playerLevel == playerLevel);
		}
	}
}