using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class LocalWorld : WorldBase<LocalObject>
	{
		private readonly Dictionary<ObjectType, MethodInfo> GetLocalObjectMethodMap =
			new Dictionary<ObjectType, MethodInfo>(SingletonComparerEnum<ObjectTypeComparer, ObjectType>.Instance);

		private Dictionary<ItemGrade, ObjectPool> airSupplyBoxPool;
		private LocalAirSupplyPool airSupplyPool;
		private BatPool batPool;
		private BearPool bearPool;
		private BoarPool boarPool;
		private ChickenPool chickenPool;
		private LocalAirSupplyEpicBoxPool epicBoxPool;
		private Dictionary<ObjectType, Transform> groupMap;
		private bool hasStopped;
		private int itemBoxObjectIdIdx;
		private LocalAirSupplyLegendBoxPool legendBoxPool;
		private Dictionary<MonsterType, ObjectPool> monsterCharacterPool;
		private LocalMonsterPool monsterPool;
		private LocalAirSupplyRareBoxPool rareBoxPool;
		private LocalSightObjectPool sightObjectPool;
		private LocalAirSupplyUncommonBoxPool uncommonBoxPool;
		private WicklinePool wicklinePool;
		private WildDogPool wildDogPool;
		private WolfPool wolfPool;

		public int IncrementAndGetItemBoxObjectId()
		{
			int result = itemBoxObjectIdIdx + 1;
			itemBoxObjectIdIdx = result;
			return result;
		}

		public void SightObjectPoolCountLog()
		{
			sightObjectPool.PoolCountLog();
		}

		protected override void OnInit()
		{
			SingletonMonoBehaviour<ResourceManager>.inst.LoadInWorld();
			InitGetLocalObjectMethodMap();
			
			groupMap = new Dictionary<ObjectType, Transform>(SingletonComparerEnum<ObjectTypeComparer, ObjectType>
				.Instance);
			if (Debug.isDebugBuild)
			{
				foreach (object obj in Enum.GetValues(typeof(ObjectType)))
				{
					ObjectType objectType = (ObjectType) obj;
					if (objectType != ObjectType.None)
					{
						GameObject go = new GameObject(objectType.ToString());
						go.transform.parent = this.gameObject.transform;
						go.transform.localPosition = Vector3.zero;
						groupMap.Add(objectType, go.transform);
					}
				}
			}

			GameUtil.BindOrAdd<LocalAirSupplyPool>(this.gameObject, ref airSupplyPool);
			airSupplyPool.InitPool();
			GameUtil.BindOrAdd<LocalMonsterPool>(this.gameObject, ref monsterPool);
			monsterPool.InitPool();
			GameUtil.BindOrAdd<LocalSightObjectPool>(this.gameObject, ref sightObjectPool);
			sightObjectPool.InitPool();
			InitAirSupplyBoxPool();
			InitMonsterCharacterPool();
			itemBoxObjectIdIdx = 0;
		}
		
		private void InitGetLocalObjectMethodMap()
		{
			MethodInfo method = GetType().GetMethod("GetLocalObject", BindingFlags.Instance | BindingFlags.NonPublic);
			
			foreach (Type type in (from x in typeof(LocalObject).Assembly.GetTypes()
				where !x.IsAbstract && x.IsSubclassOf(typeof(LocalObject))
				select x).ToList<Type>())
			{
				if (type.GetCustomAttribute(typeof(ObjectAttr)) is ObjectAttr objectAttr)
				{
					if (method != null)
						GetLocalObjectMethodMap.Add(objectAttr.objectType, method.MakeGenericMethod(type));
				}
			}
		}
		
		private LocalObject GetLocalObject<T>(GameObject go) where T : LocalObject
		{
			T t = go.GetComponent<T>();
			if (t == null)
			{
				t = go.AddComponent<T>();
			}

			return t;
		}
		
		private LocalObject CreateObject(ObjectType objectType, int objectId, Vector3 position, Quaternion rotation,
			Transform parent = null)
		{
			if (HasObjectId(objectId))
			{
				Log.E($"{objectType} ObjectId[{objectId}] is duplicated.");
				return null;
			}

			GameObject clientPrefab = SingletonMonoBehaviour<ResourceManager>.inst.GetClientPrefab(objectType);
			if (clientPrefab == null)
			{
				Log.E("No prefab for objectType[{0}]", objectType);
				return null;
			}

			if (parent == null)
			{
				parent = Debug.isDebugBuild ? groupMap[objectType] : transform;
			}

			GameObject go;
			switch (objectType)
			{
				case ObjectType.Projectile:
				{
					go = projectilePool.Pop(parent);
					go.layer = clientPrefab.layer;
					go.transform.SetPositionAndRotation(position, rotation);
					foreach (Component component in clientPrefab.GetComponents<Component>())
					{
						if (go.GetComponent(component.GetType()) == null)
						{
							go.AddCopyComponent(component);
						}
					}

					break;
				}
				case ObjectType.AirSupplyItemBox:
					go = airSupplyPool.Pop<LocalAirSupplyItemBox>(parent);
					go.transform.localPosition = position;
					go.transform.rotation = rotation;
					break;
				case ObjectType.Monster:
					go = monsterPool.Pop(parent);
					go.transform.SetPositionAndRotation(position, rotation);
					break;
				case ObjectType.SightObject:
					go = sightObjectPool.Pop(parent);
					go.transform.SetPositionAndRotation(position, rotation);
					break;
				default:
					go = Instantiate<GameObject>(clientPrefab, position, rotation, parent);
					break;
			}

			if (go == null)
			{
				Log.E($"Failed to create gameObject for objectType[{objectType}]");
				return null;
			}

			LocalObject component2 = go.GetComponent<LocalObject>();
			if (component2 == null)
			{
				throw new NullReferenceException($"LocalObject is null. ObjectType: {objectType}, ObjectId: {objectId}");
			}

			AddObject(objectId, component2);
			return component2;
		}

		public LocalObject SpawnSnapshot(SnapshotWrapper snapshotWrapper)
		{
			int objectId = snapshotWrapper.objectId;
			Vector3 position = snapshotWrapper.position;
			Quaternion rotation = snapshotWrapper.rotation;
			ObjectType objectType = snapshotWrapper.objectType;
			LocalObject localObject = CreateObject(objectType, objectId, position, rotation);
			if (localObject == null)
			{
				return null;
			}

			localObject.name = $"{objectType.ToString()}({objectId})";
			localObject.InitObject(objectId);
			localObject.Init(snapshotWrapper.snapshot);
			return localObject;
		}


		public LocalResourceItemBox SpawnResourceItemBox(Vector3 position, Quaternion rotation, int spawnPointCode,
			int resourceDataCode, int areaCode, float initSpawnTime)
		{
			LocalObject localObject = CreateObject(ObjectType.ResourceItemBox, IncrementAndGetItemBoxObjectId(),
				position, rotation);
			if (localObject == null)
			{
				return null;
			}

			localObject.name = $"ResourceItemBox({itemBoxObjectIdIdx})";
			localObject.InitObject(itemBoxObjectIdIdx);
			LocalResourceItemBox component = localObject.GetComponent<LocalResourceItemBox>();
			component.Init(spawnPointCode, resourceDataCode, areaCode, initSpawnTime);
			return component;
		}
		
		public LocalStaticItemBox SpawnStaticItemBox(Vector3 position, Quaternion rotation, int spawnPointCode)
		{
			LocalObject localObject = CreateObject(ObjectType.StaticItemBox, IncrementAndGetItemBoxObjectId(), position,
				rotation);
			if (localObject == null)
			{
				return null;
			}

			localObject.name = $"StaticItemBox({itemBoxObjectIdIdx})";
			localObject.InitObject(itemBoxObjectIdIdx);
			LocalStaticItemBox component = localObject.GetComponent<LocalStaticItemBox>();
			component.Init(spawnPointCode);
			return component;
		}

		public LocalHyperloop SpawnHyperloop(Vector3 position, Quaternion rotation)
		{
			LocalObject localObject =
				CreateObject(ObjectType.Hyperloop, IncrementAndGetItemBoxObjectId(), position, rotation);
			if (localObject == null)
			{
				return null;
			}

			localObject.name = $"Hyperloop({itemBoxObjectIdIdx})";
			localObject.InitObject(itemBoxObjectIdIdx);
			LocalHyperloop component = localObject.GetComponent<LocalHyperloop>();
			component.Init();
			return component;
		}

		public LocalSecurityConsole SpawnSecurityConsole(Vector3 position, Quaternion rotation)
		{
			LocalObject localObject = CreateObject(ObjectType.SecurityConsole, IncrementAndGetItemBoxObjectId(),
				position, rotation);
			if (localObject == null)
			{
				return null;
			}

			localObject.name = $"SecurityConsole({itemBoxObjectIdIdx})";
			localObject.InitObject(itemBoxObjectIdIdx);
			LocalSecurityConsole component = localObject.GetComponent<LocalSecurityConsole>();
			AreaData currentAreaData =
				AreaUtil.GetCurrentAreaData(MonoBehaviourInstance<ClientService>.inst.CurrentLevel,
					component.GetPosition(), 2147483640);
			component.Init(currentAreaData.code);
			return component;
		}

		public LocalSecurityCamera SpawnSecurityCamera(Vector3 position, Quaternion rotation)
		{
			LocalObject localObject = CreateObject(ObjectType.SecurityCamera, IncrementAndGetItemBoxObjectId(),
				position, rotation);
			if (localObject == null)
			{
				return null;
			}

			localObject.name = $"SecurityCamera({itemBoxObjectIdIdx})";
			localObject.InitObject(itemBoxObjectIdIdx);
			LocalSecurityCamera component = localObject.GetComponent<LocalSecurityCamera>();
			AreaData currentAreaData =
				AreaUtil.GetCurrentAreaData(MonoBehaviourInstance<ClientService>.inst.CurrentLevel,
					component.GetPosition(), 2147483640);
			component.Init(currentAreaData.code);
			return component;
		}
		
		public LocalAirSupplyItemBox SpawnAirSupply(SnapshotWrapper snapshotWrapper)
		{
			Vector3 position = snapshotWrapper.position;
			Quaternion rotation = snapshotWrapper.rotation;
			
			ObjectType objectType = snapshotWrapper.objectType;
			int objectId = snapshotWrapper.objectId;
			
			Transform parent = Debug.isDebugBuild ? groupMap[objectType] : transform;
			GameObject go = airSupplyPool.Pop<LocalDrone>(parent);
			
			go.transform.localPosition = position;
			go.transform.localRotation = rotation;
			go.name = $"Done({objectId})";
			
			LocalDrone component = go.GetComponent<LocalDrone>();
			component.Init(delegate(LocalDrone drone) { airSupplyPool.Push<LocalDrone>(drone); });
			
			LocalObject localObject =
				CreateObject(objectType, objectId, Vector3.zero, Quaternion.identity, component.Anchor);
			
			if (localObject == null)
			{
				return null;
			}

			localObject.name = $"{objectType}({objectId})";
			localObject.InitObject(objectId);
			localObject.Init(snapshotWrapper.snapshot);
			
			component.SetAirSupplyItemBox(localObject.GetComponent<LocalAirSupplyItemBox>());
			
			return localObject as LocalAirSupplyItemBox;
		}


		public T AddOrGet<T>(LocalObject localObject) where T : Component
		{
			T t = localObject.gameObject.GetComponent<T>();
			if (t == null)
			{
				t = localObject.gameObject.AddComponent<T>();
			}

			return t;
		}

		public void SpawnSnapshots(IEnumerable<SnapshotWrapper> snapshots)
		{
			if (snapshots == null)
			{
				return;
			}

			using (IEnumerator<SnapshotWrapper> enumerator = snapshots.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SnapshotWrapper snapshot = enumerator.Current;
					LocalObject localObject = SpawnSnapshot(snapshot);
					
					if (localObject != null && snapshot != null)
					{
						localObject.IfTypeOf<LocalCharacter>(localCharacter =>
						{
							CharacterSnapshot characterSnapshot =
								Serializer.Default.Deserialize<CharacterSnapshot>(snapshot.snapshot);
							
							localCharacter.InitStateEffect(characterSnapshot.initialStateEffect);
						});
					}
				}
			}
		}

		public void Stop()
		{
			if (hasStopped)
			{
				return;
			}

			foreach (LocalMovableCharacter localMovableCharacter in FindAll<LocalMovableCharacter>())
			{
				localMovableCharacter.PauseMove();
			}

			hasStopped = true;
		}
		
		public void Resume()
		{
			if (!hasStopped)
			{
				return;
			}

			hasStopped = false;
			foreach (LocalMovableCharacter localMovableCharacter in FindAll<LocalMovableCharacter>())
			{
				localMovableCharacter.PauseMove();
			}
		}
		
		public void DestroyObject(LocalObject obj)
		{
			if (obj == null)
			{
				return;
			}

			obj.IfTypeOf<LocalCharacter>(obj1 => UISystem.Action(new CloseBox
			{
				targetBoxId = obj.ObjectId
			}));
			
			obj.IfTypeOf<LocalAirSupplyItemBox>(obj1 =>
				MonoBehaviourInstance<GameUI>.inst.Events.OnRemoveAirSupply(obj.ObjectId));
			
			switch (obj.ObjectType)
			{
				case ObjectType.AirSupplyItemBox:
				{
					RemoveObject(obj.ObjectId);
					LocalAirSupplyItemBox localAirSupplyItemBox = obj as LocalAirSupplyItemBox;
					
					GameObject go = localAirSupplyItemBox != null ? localAirSupplyItemBox.ReleaseChildren() : null;
					
					if (go != null)
					{
						ReturnAirSupplyBoxToPool(localAirSupplyItemBox.MaxItemGrade, go);
					}

					if (localAirSupplyItemBox != null)
					{
						localAirSupplyItemBox.RemoveAllAttachedSight();
					}

					airSupplyPool.Push<LocalAirSupplyItemBox>(localAirSupplyItemBox);
					return;
				}
				case ObjectType.Monster:
				{
					RemoveObject(obj.ObjectId);
					LocalMonster localMonster = obj as LocalMonster;
					
					GameObject go = localMonster != null ? localMonster.ReleaseChildren() : null;
					
					if (go != null)
					{
						ReturnMonsterCharacterToPool(localMonster.MonsterType, go);
					}

					if (obj != null)
					{
						obj.RemoveAllAttachedSight();
					}

					monsterPool.Push(obj.gameObject);
					return;
				}
				case ObjectType.SightObject:
					RemoveObject(obj.ObjectId);
					sightObjectPool.Push(obj.gameObject);
					return;
				
				default:
					base.DestroyObject(obj.ObjectId);
					break;
			}
		}

		public bool IsPaused()
		{
			return hasStopped;
		}

		public GameObject GetMonsterCharacterObject(MonsterType monsterType, Transform parent)
		{
			if (monsterCharacterPool.TryGetValue(monsterType, out ObjectPool objectPool))
			{
				GameObject go = objectPool.Pop(parent);
				go.name = monsterType.ToString();
				return go;
			}

			return null;
		}

		public void ReturnMonsterCharacterToPool(MonsterType monsterType, GameObject go)
		{
			if (monsterCharacterPool.TryGetValue(monsterType, out ObjectPool objectPool))
			{
				objectPool.Push(go);
			}
		}

		public GameObject GetAirSupplyBoxObject(ItemGrade itemGrade, Transform parent)
		{
			if (airSupplyBoxPool.TryGetValue(itemGrade, out ObjectPool objectPool))
			{
				GameObject go = objectPool.Pop(parent);
				go.name = $"AirSupplyBoxItem({itemGrade})";
				return go;
			}

			return null;
		}

		public void ReturnAirSupplyBoxToPool(ItemGrade itemGrade, GameObject go)
		{
			if (airSupplyBoxPool.TryGetValue(itemGrade, out ObjectPool objectPool))
			{
				objectPool.Push(go);
			}
		}

		private void InitAirSupplyBoxPool()
		{
			airSupplyBoxPool = new Dictionary<ItemGrade, ObjectPool>();
			GameUtil.BindOrAdd<LocalAirSupplyUncommonBoxPool>(gameObject, ref uncommonBoxPool);
			uncommonBoxPool.InitPool();
			airSupplyBoxPool.Add(ItemGrade.Uncommon, uncommonBoxPool);
			GameUtil.BindOrAdd<LocalAirSupplyRareBoxPool>(gameObject, ref rareBoxPool);
			rareBoxPool.InitPool();
			airSupplyBoxPool.Add(ItemGrade.Rare, rareBoxPool);
			GameUtil.BindOrAdd<LocalAirSupplyEpicBoxPool>(gameObject, ref epicBoxPool);
			epicBoxPool.InitPool();
			airSupplyBoxPool.Add(ItemGrade.Epic, epicBoxPool);
			GameUtil.BindOrAdd<LocalAirSupplyLegendBoxPool>(gameObject, ref legendBoxPool);
			legendBoxPool.InitPool();
			airSupplyBoxPool.Add(ItemGrade.Legend, legendBoxPool);
		}


		private void InitMonsterCharacterPool()
		{
			monsterCharacterPool = new Dictionary<MonsterType, ObjectPool>();
			
			GameUtil.BindOrAdd<ChickenPool>(gameObject, ref chickenPool);
			chickenPool.InitPool();
			monsterCharacterPool.Add(MonsterType.Chicken, chickenPool);
			
			GameUtil.BindOrAdd<BatPool>(gameObject, ref batPool);
			batPool.InitPool();
			monsterCharacterPool.Add(MonsterType.Bat, batPool);
			
			GameUtil.BindOrAdd<BoarPool>(gameObject, ref boarPool);
			boarPool.InitPool();
			monsterCharacterPool.Add(MonsterType.Boar, boarPool);
			
			GameUtil.BindOrAdd<WildDogPool>(gameObject, ref wildDogPool);
			wildDogPool.InitPool();
			monsterCharacterPool.Add(MonsterType.WildDog, wildDogPool);
			
			GameUtil.BindOrAdd<WolfPool>(gameObject, ref wolfPool);
			wolfPool.InitPool();
			monsterCharacterPool.Add(MonsterType.Wolf, wolfPool);
			
			GameUtil.BindOrAdd<BearPool>(gameObject, ref bearPool);
			bearPool.InitPool();
			monsterCharacterPool.Add(MonsterType.Bear, bearPool);
			
			GameUtil.BindOrAdd<WicklinePool>(gameObject, ref wicklinePool);
			wicklinePool.InitPool();
			monsterCharacterPool.Add(MonsterType.Wickline, wicklinePool);
		}
	}
}