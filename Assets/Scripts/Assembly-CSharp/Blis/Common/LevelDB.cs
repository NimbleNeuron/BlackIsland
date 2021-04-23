using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blis.Common
{
	public class LevelDB
	{
		private List<AreaData> areas;


		private List<AreaSoundData> areaSoundDataList;


		private List<CollectibleData> collectibleDataList;


		private LevelData defaultLevelData;


		private List<FootstepData> footstepDataList;


		private List<ItemSpawnData> itemSpawns;


		private List<NearByAreaData> nearByAreaDatas;


		private List<RestrictedAreaData> restrictedAreas;


		private List<SoundGroupData> soundGroupDataList;


		public LevelDB()
		{
			if (GameDB.useDummyData)
			{
				InitDummy();
			}
		}


		public List<AreaData> Areas => areas;


		public LevelData DefaultLevel => defaultLevelData;


		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(AreaPrimitiveData))
			{
				areas = new List<AreaData>();
				foreach (T t in data)
				{
					if (t is AreaPrimitiveData areaData)
					{
						// Debug.Log($"<color=orange>{areaData.code}</color>");
						
						areas.Add(new AreaData(areaData.code, areaData.name,
							areaData.maskCode));
					}
				}
			}

			if (typeFromHandle == typeof(RestrictedAreaData))
			{
				restrictedAreas = data.Cast<RestrictedAreaData>().ToList<RestrictedAreaData>();
				return;
			}

			if (typeFromHandle == typeof(ItemSpawnData))
			{
				itemSpawns = data.Cast<ItemSpawnData>().ToList<ItemSpawnData>();
				return;
			}

			if (typeFromHandle == typeof(CollectibleData))
			{
				collectibleDataList = data.Cast<CollectibleData>().ToList<CollectibleData>();
				return;
			}

			if (typeFromHandle == typeof(NearByAreaData))
			{
				nearByAreaDatas = data.Cast<NearByAreaData>().ToList<NearByAreaData>();
				return;
			}

			if (typeFromHandle == typeof(AreaSoundData))
			{
				areaSoundDataList = data.Cast<AreaSoundData>().ToList<AreaSoundData>();
				return;
			}

			if (typeFromHandle == typeof(SoundGroupData))
			{
				soundGroupDataList = data.Cast<SoundGroupData>().ToList<SoundGroupData>();
				return;
			}

			if (typeFromHandle == typeof(FootstepData))
			{
				footstepDataList = data.Cast<FootstepData>().ToList<FootstepData>();
			}
		}


		public void PostInitialize()
		{
			defaultLevelData = GetLevelData("Default");
		}


		public LevelData GetLevelData(string path)
		{
			List<HyperloopSpawnPoint> spawnPoints =
				SingletonMonoBehaviour<ResourceManager>.inst.GetSpawnPoints<HyperloopSpawnPoint>(
					path + "/HyperloopSpawnPoints");
			List<ItemSpawnPoint> spawnPoints2 =
				SingletonMonoBehaviour<ResourceManager>.inst.GetSpawnPoints<ItemSpawnPoint>(path + "/ItemSpawnPoints");
			List<MonsterSpawnPoint> spawnPoints3 =
				SingletonMonoBehaviour<ResourceManager>.inst.GetSpawnPoints<MonsterSpawnPoint>(
					path + "/MonsterSpawnPoints");
			List<SecurityConsoleSpawnPoint> spawnPoints4 =
				SingletonMonoBehaviour<ResourceManager>.inst.GetSpawnPoints<SecurityConsoleSpawnPoint>(
					path + "/SecurityConsoleSpawnPoints");
			List<SecurityCameraSpawnPoint> spawnPoints5 =
				SingletonMonoBehaviour<ResourceManager>.inst.GetSpawnPoints<SecurityCameraSpawnPoint>(
					path + "/SecurityCameraSpawnPoints");
			List<CharacterSpawnPoint> spawnPoints6 =
				SingletonMonoBehaviour<ResourceManager>.inst.GetSpawnPoints<CharacterSpawnPoint>(
					path + "/CharacterSpawnPoints");
			return new LevelData(areas, itemSpawns, collectibleDataList, nearByAreaDatas, spawnPoints6, spawnPoints2,
				spawnPoints3, spawnPoints, spawnPoints4, spawnPoints5);
		}


		public AreaData GetAreaData(int code)
		{
			return areas.Find(data => data.code == code);
		}


		public List<AreaData> GetBattleAreaData()
		{
			return (from a in areas
				where a.code != 16 && a.code != 17
				select a).ToList<AreaData>();
		}


		public int GetRestrictedAreaCount()
		{
			return restrictedAreas.Count;
		}


		public bool IsFinalAreaRestriction(int count)
		{
			return restrictedAreas.Count <= count;
		}


		public RestrictedAreaInfo GetRestrictedAreaData(int count)
		{
			if (restrictedAreas.Count <= count)
			{
				throw new Exception();
			}

			RestrictedAreaData restrictedAreaData = restrictedAreas[count];
			RestrictedAreaData restrictedAreaData2 =
				count + 1 < restrictedAreas.Count ? restrictedAreas[count + 1] : null;
			return new RestrictedAreaInfo
			{
				durationSeconds = restrictedAreaData.durationSeconds,
				reservedCount = restrictedAreaData2 != null ? restrictedAreaData2.restrictedCount : 0,
				clearCount = restrictedAreaData.clearCount,
				damageOnTime = restrictedAreaData.damageOnTime,
				minimumSurvivors = restrictedAreaData.minimumSurvivors
			};
		}


		public int GetTotalPlayTime(int remainTime, DayNight dayNight, int day = 0)
		{
			int num = dayNight == DayNight.Night ? 1 : 0;
			int num2 = day + (day - 1);
			int num3 = num + num2;
			int num4 = 0;
			for (int i = 0; i < num3; i++)
			{
				num4 += restrictedAreas[i].durationSeconds;
			}

			return num4 - remainTime + 1;
		}


		public CollectibleData GetCollectibleData(int code)
		{
			return collectibleDataList.Find(data => data.code == code);
		}


		public AreaSoundData GetAreaSound(int code)
		{
			return areaSoundDataList.Find(s => s.code == code);
		}


		public List<SoundGroupData> GetSoundGroupData(string name)
		{
			return soundGroupDataList.FindAll(s => s.groupName.Equals(name));
		}


		public FootstepData GetFootStepData(string material)
		{
			return footstepDataList.Find(f => f.material.Equals(material));
		}


		private void InitDummy()
		{
			restrictedAreas = new List<RestrictedAreaData>();
			itemSpawns = new List<ItemSpawnData>();
			areas = new List<AreaData>();
			defaultLevelData = new LevelData();
			InitRestrictedAreaDummy();
		}


		private void AddRestrictedAreaDummy(int code, int durationSeconds, int restrictedCount, int clearCount,
			int damageOnTime, int miniumSurvivors)
		{
			restrictedAreas.Add(new RestrictedAreaData(code, durationSeconds, restrictedCount, clearCount, damageOnTime,
				miniumSurvivors));
		}


		private void InitRestrictedAreaDummy()
		{
			AddRestrictedAreaDummy(1, 300, 0, 0, 8, 10);
			AddRestrictedAreaDummy(2, 300, 3, 0, 8, 9);
			AddRestrictedAreaDummy(3, 300, 3, 3, 10, 8);
			AddRestrictedAreaDummy(4, 300, 5, 3, 10, 7);
			AddRestrictedAreaDummy(5, 300, 5, 0, 12, 6);
			AddRestrictedAreaDummy(6, 300, 2, 0, 12, 5);
			AddRestrictedAreaDummy(7, 300, 2, 0, 14, 4);
			AddRestrictedAreaDummy(8, 0, 1, 0, 15, 3);
		}
	}
}