using System;
using System.Collections.Generic;
using System.Reflection;
using Blis.Common;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using UnityEngine;

namespace Blis.Server
{
	
	public class GameWorld : WorldBase<WorldObject>
	{
		
		protected override void OnInit()
		{
			SingletonMonoBehaviour<ResourceManager>.inst.LoadInServer();
			GameUtil.BindOrAdd<WorldMonsterPool>(base.gameObject, ref this.monsterPool);
			GameUtil.BindOrAdd<WorldSightObjectPool>(base.gameObject, ref this.sightObjectPool);
			this.monsterPool.InitPool();
			this.sightObjectPool.InitPool();
		}

		
		public void SightObjectPoolLog()
		{
			this.sightObjectPool.PoolCountLog();
		}

		
		public int IncrementAndGetObjectId()
		{
			int num = this.ObjectIdIdx + 1;
			this.ObjectIdIdx = num;
			return num;
		}

		
		public int GetSightId()
		{
			int num = this.sightIdIdx;
			this.sightIdIdx = num + 1;
			return num;
		}

		
		public T Spawn<T>(Vector3 position) where T : WorldObject, new()
		{
			return this.Spawn<T>(position, Quaternion.identity);
		}

		
		public T Spawn<T>(Vector3 position, bool botPlayer) where T : WorldObject, new()
		{
			return this.Spawn<T>(position, Quaternion.identity, botPlayer);
		}

		
		public T Spawn<T>(Vector3 position, Quaternion quaternion) where T : WorldObject, new()
		{
			return this.Spawn<T>(position, quaternion, false);
		}

		
		public T Spawn<T>(Vector3 position, Quaternion quaternion, bool botPlayer) where T : WorldObject, new()
		{
			return this.Spawn<T>(this.IncrementAndGetObjectId(), position, quaternion, botPlayer);
		}

		
		public T Spawn<T>(int objectId, Vector3 position, Quaternion quaternion) where T : WorldObject, new()
		{
			return this.Spawn<T>(objectId, position, quaternion, false);
		}

		
		public T Spawn<T>(int objectId, Vector3 position, Quaternion quaternion, bool botPlayer) where T : WorldObject, new()
		{
			ObjectType objectType = (typeof(T).GetCustomAttribute(typeof(ObjectAttr)) as ObjectAttr).objectType;
			GameObject gameObject;
			if (botPlayer)
			{
				gameObject = SingletonMonoBehaviour<ResourceManager>.inst.GetServerBotPlayerPrefab();
			}
			else
			{
				gameObject = SingletonMonoBehaviour<ResourceManager>.inst.GetServerPrefab(objectType);
			}
			if (gameObject == null)
			{
				throw new GameException(string.Format("Failed to find a prefab of {0}", objectType));
			}
			GameObject gameObject2;
			if (objectType == ObjectType.Projectile)
			{
				gameObject2 = this.SetObjectComponent(this.projectilePool, gameObject, position, quaternion);
				gameObject2.layer = gameObject.layer;
			}
			else if (objectType == ObjectType.Monster)
			{
				gameObject2 = this.SetObjectComponent(this.monsterPool, gameObject, position, quaternion);
			}
			else if (objectType == ObjectType.SightObject)
			{
				gameObject2 = this.SetObjectComponent(this.sightObjectPool, gameObject, position, quaternion);
			}
			else
			{
				gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject, position, quaternion, base.transform);
			}
			T component = gameObject2.GetComponent<T>();
			if (component == null)
			{
				throw new GameException(string.Format("Failed to find GetComponent {0}", objectType));
			}
			component.InitObject(objectId, this);
			base.AddObject(objectId, component);
			gameObject2.name = string.Format("{0}({1})", objectType.ToString(), objectId);
			return component;
		}

		
		private GameObject SetObjectComponent(ObjectPool pool, GameObject prefab, Vector3 position, Quaternion quaternion)
		{
			GameObject pop = pool.Pop(base.transform);
			foreach (Component component in prefab.GetComponents<Component>())
			{
				try
				{
					if (component == null)
					{
						Debug.LogError($"Component is null : {prefab.name}", prefab);
						continue;
					}
					
					if (pop.GetComponent(component.GetType()) == null)
					{
						pop.AddComponent(component.GetType());
					}
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
			pop.transform.SetPositionAndRotation(position, quaternion);
			return pop;
		}

		
		public ProjectileProperty PopProjectileProperty(SkillAgent owner, int code, SkillUseInfo skillUseInfo)
		{
			return this.projectilePropertyPool.Pop(owner, code, skillUseInfo);
		}

		
		public void PushProjectileProperty(ProjectileProperty property)
		{
			this.projectilePropertyPool.Push(property);
		}

		
		public List<SnapshotWrapper> createWorldSnapshot()
		{
			List<SnapshotWrapper> list = new List<SnapshotWrapper>();
			foreach (WorldObject worldObject in base.AllObjects)
			{
				ObjectType objectType = worldObject.ObjectType;
				if (objectType != ObjectType.PlayerCharacter && objectType != ObjectType.StaticItemBox && objectType - ObjectType.BotPlayerCharacter > 3)
				{
					list.Add(worldObject.CreateSnapshotWrapper());
				}
			}
			return list;
		}

		
		public List<SnapshotWrapper> GetWorldSnapshot(int currentSeq)
		{
			if (this.lastSnapshotSeq != currentSeq)
			{
				this.lastSnapshot = this.createWorldSnapshot();
				this.lastSnapshot.Sort((SnapshotWrapper x, SnapshotWrapper y) => x.objectId.CompareTo(y.objectId));
				this.lastSnapshotSeq = currentSeq;
			}
			return this.lastSnapshot;
		}

		
		public void DestroyObject(WorldObject obj)
		{
			if (obj.ObjectType == ObjectType.Monster)
			{
				base.RemoveObject(obj.ObjectId);
				Transform transform = obj.transform;
				for (int i = 0; i < transform.childCount; i++)
				{
					UnityEngine.Object.DestroyImmediate(transform.GetChild(i).gameObject);
				}
				foreach (Component component in obj.GetComponents<Component>())
				{
					if (!(component is BehaviourTreeOwner) && !(component is Blackboard) && !(component is Transform) && !(component is WorldMonster))
					{
						UnityEngine.Object.DestroyImmediate(component);
					}
				}
				if (obj != null)
				{
					obj.RemoveAllAttachedSight();
				}
				this.monsterPool.Push(obj.gameObject);
				return;
			}
			if (obj.ObjectType == ObjectType.SightObject)
			{
				base.RemoveObject(obj.ObjectId);
				this.sightObjectPool.Push(obj.gameObject);
				return;
			}
			base.DestroyObject(obj.ObjectId);
		}

		
		private int ObjectIdIdx;

		
		private int sightIdIdx;

		
		private WorldMonsterPool monsterPool;

		
		private WorldSightObjectPool sightObjectPool;

		
		private ProjectilePropertyPool projectilePropertyPool = new ProjectilePropertyPool();

		
		private int lastSnapshotSeq = -1;

		
		private List<SnapshotWrapper> lastSnapshot;
	}
}
