using System.Collections.Generic;
using System.Linq;
using Blis.Common;

namespace Blis.Client
{
	public class ItemService : Singleton<ItemService>
	{
		private readonly List<int> dropAreaList = new List<int>();


		private LevelData levelData;

		public void SetLevelData(LevelData levelData)
		{
			this.levelData = levelData;
		}


		public bool IsDropArea(int areaCode, int itemCode)
		{
			return GetDropArea(itemCode).Any(x => x == areaCode);
		}


		public List<int> GetDropArea(int itemCode)
		{
			dropAreaList.Clear();
			if (levelData == null)
			{
				Log.E("[ItemService] LevelData is null");
				return dropAreaList;
			}

			foreach (ItemSpawnData itemSpawnData in levelData.itemSpawns)
			{
				if (itemSpawnData.itemCode == itemCode && !dropAreaList.Contains(itemSpawnData.areaCode))
				{
					dropAreaList.Add(itemSpawnData.areaCode);
				}
			}

			return dropAreaList;
		}


		public int GetDropCountInBox(int areaCode, int itemCode)
		{
			ItemSpawnData itemSpawnData =
				levelData.itemSpawns.Find(x => x.areaCode == areaCode && x.itemCode == itemCode);
			if (itemSpawnData != null)
			{
				return itemSpawnData.dropCount;
			}

			return 0;
		}


		public IEnumerable<int> GetDropItems(int areaCode, bool containResourceItem)
		{
			IEnumerable<int> enumerable = from x in levelData.itemSpawns
				where x.areaCode == areaCode
				select x.itemCode;
			if (containResourceItem)
			{
				IEnumerable<int> second = from x in (from x in levelData.itemSpawnPoints
						where x.areaCode == areaCode && x.resource
						select x.resourceDataCode).Distinct<int>()
					select levelData.GetCollectibleData(x)
					into x
					where x != null
					select x.itemCode;
				return enumerable.Concat(second);
			}

			return enumerable;
		}
	}
}