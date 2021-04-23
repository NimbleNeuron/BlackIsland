using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class LevelService : ServiceBase
	{
		
		
		public LevelData CurrentLevel
		{
			get
			{
				return this.currentLevel;
			}
		}

		
		
		public List<SnapshotWrapper> LevelSnapshot
		{
			get
			{
				return this.levelSnapshot;
			}
		}

		
		public void LoadLevel(LevelData levelData, string spawnPointPath, bool isTutorial)
		{
			Log.H("Load Level");
			this.currentLevel = levelData;
			this.characterInitialSpawnPoints = SingletonMonoBehaviour<ResourceManager>.inst.GetSpawnPoints<CharacterSpawnPoint>(spawnPointPath + "/InitialCharacterSpawnPoints");
			this.characterInitialSpawnPoints.ForEach(delegate(CharacterSpawnPoint x)
			{
				x.SetIsUsed(false);
			});
			Dictionary<int, List<WorldItemBox>> dictionary = new Dictionary<int, List<WorldItemBox>>();
			this.airSupplyPointMap = new Dictionary<int, List<ItemSpawnPoint>>();
			foreach (ItemSpawnPoint itemSpawnPoint in this.currentLevel.itemSpawnPoints)
			{
				if (itemSpawnPoint.airSupply)
				{
					if (this.airSupplyPointMap.ContainsKey(itemSpawnPoint.areaCode))
					{
						this.airSupplyPointMap[itemSpawnPoint.areaCode].Add(itemSpawnPoint);
					}
					else
					{
						List<ItemSpawnPoint> value = new List<ItemSpawnPoint>
						{
							itemSpawnPoint
						};
						this.airSupplyPointMap.Add(itemSpawnPoint.areaCode, value);
					}
				}
				else if (itemSpawnPoint.resource)
				{
					MonoBehaviourInstance<GameService>.inst.Spawn.SpawnResourceItemBox(itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation, itemSpawnPoint.code, itemSpawnPoint.resourceDataCode, itemSpawnPoint.areaCode, itemSpawnPoint.initSpawnTime);
				}
				else
				{
					Transform transform = itemSpawnPoint.transform;
					WorldStaticItemBox item = MonoBehaviourInstance<GameService>.inst.Spawn.SpawnStaticItemBox(transform.position, transform.rotation, itemSpawnPoint.code, itemSpawnPoint.boxSize);
					List<WorldItemBox> list = dictionary.ContainsKey(itemSpawnPoint.areaCode) ? dictionary[itemSpawnPoint.areaCode] : null;
					if (list == null)
					{
						list = new List<WorldItemBox>();
						dictionary[itemSpawnPoint.areaCode] = list;
					}
					list.Add(item);
				}
			}
			List<ItemSpawnData> list2 = new List<ItemSpawnData>(this.currentLevel.itemSpawns);
			while (list2.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list2.Count);
				ItemSpawnData itemSpawnData = list2[index];
				list2.RemoveAt(index);
				List<WorldItemBox> list3 = dictionary[itemSpawnData.areaCode];
				list3.Shuffle<WorldItemBox>();
				int num = list3.Count - 1;
				int num2 = 0;
				while (num2 < itemSpawnData.dropCount && num >= 0)
				{
					ItemData itemData = GameDB.item.FindItemByCode(itemSpawnData.itemCode);
					WorldItemBox worldItemBox = list3.ElementAt(num);
					worldItemBox.AddItem(this.game.Spawn.CreateItem(itemData, itemData.initialCount));
					if (!worldItemBox.isCapacityAvailable())
					{
						list3.RemoveAt(num);
					}
					num--;
					num2++;
				}
			}
			foreach (HyperloopSpawnPoint hyperloopSpawnPoint in this.currentLevel.hyperloopSpawnPoints)
			{
				MonoBehaviourInstance<GameService>.inst.Spawn.SpawnHyperloop(hyperloopSpawnPoint.transform.position, hyperloopSpawnPoint.transform.rotation);
			}
			foreach (SecurityConsoleSpawnPoint securityConsoleSpawnPoint in this.currentLevel.securityConsoleSpawnPoints)
			{
				MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSecurityConsole(securityConsoleSpawnPoint.transform.position, securityConsoleSpawnPoint.transform.rotation);
			}
			foreach (SecurityCameraSpawnPoint securityCameraSpawnPoint in this.currentLevel.securityCameraSpawnPoints)
			{
				MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSecurityCamera(securityCameraSpawnPoint.transform.position, securityCameraSpawnPoint.transform.rotation);
			}
			if (isTutorial)
			{
				MonoBehaviourInstance<GameService>.inst.Spawn.SpawnTutorialMonster(this.currentLevel);
			}
			else
			{
				MonoBehaviourInstance<GameService>.inst.Spawn.SpawnMonsterCreateTimeZero(this.currentLevel);
			}
			this.levelSnapshot = this.world.createWorldSnapshot();
		}

		
		public CharacterSpawnPoint GetPlayerSpawnPoint(int areaCode)
		{
			List<CharacterSpawnPoint> spawnPointsByAreaCode = this.currentLevel.GetSpawnPointsByAreaCode(areaCode);
			List<CharacterSpawnPoint> list = new List<CharacterSpawnPoint>();
			foreach (CharacterSpawnPoint characterSpawnPoint in spawnPointsByAreaCode)
			{
				if (!characterSpawnPoint.IsUsed)
				{
					list.Add(characterSpawnPoint);
				}
			}
			if (list.Count <= 0)
			{
				int num = int.MaxValue;
				foreach (CharacterSpawnPoint characterSpawnPoint2 in spawnPointsByAreaCode)
				{
					if (!characterSpawnPoint2.AllUsedChildPoints)
					{
						int usedChildCount = characterSpawnPoint2.UsedChildCount;
						if (usedChildCount < num)
						{
							num = usedChildCount;
						}
					}
				}
				if (num < 2147483647)
				{
					List<CharacterSpawnPoint> list2 = new List<CharacterSpawnPoint>();
					foreach (CharacterSpawnPoint characterSpawnPoint3 in spawnPointsByAreaCode)
					{
						if (!characterSpawnPoint3.AllUsedChildPoints && num == characterSpawnPoint3.UsedChildCount)
						{
							list2.Add(characterSpawnPoint3);
						}
					}
					if (0 < list2.Count)
					{
						CharacterSpawnPoint characterSpawnPoint4 = list2[UnityEngine.Random.Range(0, list2.Count)];
						list.AddRange(characterSpawnPoint4.NotUsedChildPoints);
					}
				}
			}
			if (list.Count <= 0)
			{
				return this.GetCharacterSpawnPointNotCheckUse(areaCode);
			}
			CharacterSpawnPoint characterSpawnPoint5 = list[UnityEngine.Random.Range(0, list.Count)];
			characterSpawnPoint5.SetIsUsed(true);
			return characterSpawnPoint5;
		}

		
		private CharacterSpawnPoint GetCharacterSpawnPointNotCheckUse(int areaCode)
		{
			List<CharacterSpawnPoint> spawnPointsByAreaCode = this.currentLevel.GetSpawnPointsByAreaCode(areaCode);
			return spawnPointsByAreaCode[UnityEngine.Random.Range(0, spawnPointsByAreaCode.Count<CharacterSpawnPoint>())].GetRandomPointNotCheckUse();
		}

		
		public CharacterSpawnPoint GetPlayerSpawnPoint(int areaCode, int index)
		{
			List<CharacterSpawnPoint> spawnPointsByAreaCode = this.currentLevel.GetSpawnPointsByAreaCode(areaCode);
			int num = 0;
			CharacterSpawnPoint characterSpawnPoint = null;
			foreach (CharacterSpawnPoint characterSpawnPoint2 in spawnPointsByAreaCode)
			{
				if (num == index - 1)
				{
					characterSpawnPoint = characterSpawnPoint2;
					break;
				}
				num++;
			}
			if (characterSpawnPoint == null)
			{
				throw new GameException("invalid playerCharacter spawn point");
			}
			if (characterSpawnPoint.IsUsed)
			{
				throw new GameException("playerCharacter spawn point is not enough ");
			}
			characterSpawnPoint.SetIsUsed(true);
			return characterSpawnPoint;
		}

		
		public CharacterSpawnPoint GetInitialPlayerSpawnPoint()
		{
			IEnumerable<CharacterSpawnPoint> source = from sp in this.characterInitialSpawnPoints
			where sp != null && !sp.IsUsed
			select sp;
			if (!source.Any<CharacterSpawnPoint>())
			{
				throw new GameException("playerCharacter spawn point is not enough ");
			}
			CharacterSpawnPoint characterSpawnPoint = source.ElementAt(UnityEngine.Random.Range(0, source.Count<CharacterSpawnPoint>()));
			characterSpawnPoint.SetIsUsed(true);
			return characterSpawnPoint;
		}

		
		public ItemSpawnPoint GetAirSupplyPoint(int areaCode)
		{
			List<ItemSpawnPoint> list;
			if (!this.airSupplyPointMap.TryGetValue(areaCode, out list))
			{
				return null;
			}
			if (list == null)
			{
				Log.W("[LevelService.GetAirSupplyPoint] areaCode({0}) is not set drop point.", areaCode);
				return null;
			}
			if (list.Count == 0)
			{
				Log.W("[LevelService.GetAirSupplyPoint] remain Air Supply point count is 0.");
				return null;
			}
			int index = UnityEngine.Random.Range(0, list.Count);
			ItemSpawnPoint result = list[index];
			list.RemoveAt(index);
			return result;
		}

		
		public CharacterSpawnPoint AllocHyperLoopExit(int areaCode)
		{
			List<CharacterSpawnPoint> spawnPointsByAreaCode = this.currentLevel.GetSpawnPointsByAreaCode(areaCode);
			List<CharacterSpawnPoint> list = new List<CharacterSpawnPoint>();
			foreach (CharacterSpawnPoint item in spawnPointsByAreaCode)
			{
				if (!this.reservedHyperLoop.Contains(item))
				{
					list.Add(item);
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			CharacterSpawnPoint characterSpawnPoint = list[UnityEngine.Random.Range(0, list.Count)];
			this.reservedHyperLoop.Add(characterSpawnPoint);
			return characterSpawnPoint;
		}

		
		public void FreeHyperLoopExit(CharacterSpawnPoint point)
		{
			this.reservedHyperLoop.Remove(point);
		}

		
		public AreaData NextSafeAreaData(WorldMovableCharacter character)
		{
			LevelData currentLevel = MonoBehaviourInstance<GameService>.inst.CurrentLevel;
			AreaData currentAreaData = character.GetCurrentAreaData(currentLevel);
			int code = currentAreaData.code;
			if (!currentLevel.nearByAreaMap.ContainsKey(code))
			{
				return null;
			}
			IEnumerable<AreaData> areaDataListByState = MonoBehaviourInstance<GameService>.inst.Area.getAreaDataListByState(AreaRestrictionState.Normal);
			AreaData areaData = areaDataListByState.FirstOrDefault((AreaData area) => currentLevel.nearByAreaMap[code].Exists((int nearbyCode) => area.code == nearbyCode));
			if (areaData == null)
			{
				areaData = areaDataListByState.ElementAtOrDefault(UnityEngine.Random.Range(0, areaDataListByState.Count<AreaData>()));
			}
			return areaData;
		}

		
		private List<CharacterSpawnPoint> characterInitialSpawnPoints;

		
		private Dictionary<int, List<ItemSpawnPoint>> airSupplyPointMap;

		
		private LevelData currentLevel;

		
		private List<SnapshotWrapper> levelSnapshot;

		
		private HashSet<CharacterSpawnPoint> reservedHyperLoop = new HashSet<CharacterSpawnPoint>();
	}
}
