using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blis.Common
{
	public abstract class WorldBase<Ty> : MonoBehaviour where Ty : ObjectBase
	{
		private readonly List<Ty> cachedAllObjects = new List<Ty>();
		private readonly Dictionary<Type, bool> cacheDirty = new Dictionary<Type, bool>();
		private readonly Dictionary<Type, List<Ty>> cachedObjects = new Dictionary<Type, List<Ty>>();
		private readonly Dictionary<int, Ty> objectMap = new Dictionary<int, Ty>();
		private readonly Dictionary<Type, List<Ty>> objectTypeMap = new Dictionary<Type, List<Ty>>();
		private readonly Dictionary<Type, List<Type>> parentTypesWithSelf = new Dictionary<Type, List<Type>>();

		private bool cacheAllDirty;
		protected WorldObjectPool projectilePool;
		protected Dictionary<int, Ty>.ValueCollection AllObjects => objectMap.Values;

		private const string FIND_GAME_EX = "ObjectType of {0} does not match. {1} is expected but found. {2}({3})";

		private void Awake()
		{
			GameUtil.BindOrAdd<WorldObjectPool>(gameObject, ref projectilePool);
			projectilePool.InitPool();
			OnInit();
		}

		protected abstract void OnInit();

		public List<Ty> AllObjectAlloc()
		{
			if (cacheAllDirty)
			{
				cachedAllObjects.Clear();
				cachedAllObjects.AddRange(objectMap.Values);
				cacheAllDirty = false;
			}

			return cachedAllObjects;
		}

		public List<Ty> GetCachedObjects(Type type)
		{
			bool flag = false;
			if (cacheDirty.ContainsKey(type))
			{
				flag = cacheDirty[type];
			}

			if (!cachedObjects.ContainsKey(type))
			{
				cachedObjects.Add(type, new List<Ty>());
			}

			if (flag)
			{
				cachedObjects[type].Clear();
				if (objectTypeMap.ContainsKey(type))
				{
					cachedObjects[type].AddRange(objectTypeMap[type]);
				}

				cacheDirty[type] = false;
			}

			return cachedObjects[type];
		}

		protected void AddObject(int objectId, Ty obj)
		{
			objectMap.Add(objectId, obj);
			Type type = obj.GetType();
			List<Type> list = GetParentTypesWithSelf(type);
			foreach (Type key in list)
			{
				if (!objectTypeMap.TryGetValue(key, out List<Ty> list2))
				{
					list2 = new List<Ty>();
					objectTypeMap.Add(key, list2);
				}

				list2.Add(obj);
			}

			cacheAllDirty = true;
			foreach (Type key2 in list)
			{
				cacheDirty[key2] = true;
			}
		}

		public bool HasObjectId(int objectId)
		{
			return objectMap.ContainsKey(objectId);
		}

		public T Find<T>(int objectId) where T : Ty
		{
			if (!objectMap.TryGetValue(objectId, out Ty ty))
			{
				throw new GameException(ErrorType.ObjectNotFound,
					$"Failed to find object by ObjectId[{objectId}]");
			}

			if (!(ty is T))
			{
				throw new GameException(string.Format(FIND_GAME_EX, ty.name,
					typeof(T).Name,
					ty.GetType().Name,
					ty.ObjectType)
				);
			}

			return (T) ty;
		}

		public bool TryFind<T>(int objectId, ref T obj) where T : Ty
		{
			bool result;
			try
			{
				obj = Find<T>(objectId);
				result = true;
			}
			catch (GameException)
			{
				obj = default;
				result = false;
			}

			return result;
		}

		public List<T> FindAll<T>() where T : Ty
		{
			List<T> list = new List<T>();
			foreach (KeyValuePair<Type, List<Ty>> keyValuePair in objectTypeMap)
			{
				if (!(keyValuePair.Key != typeof(T)) || keyValuePair.Key.IsSubclassOf(typeof(T)))
				{
					foreach (Ty ty in keyValuePair.Value)
					{
						list.Add(ty as T);
					}
				}
			}

			return list;
		}

		public List<T> FindAll<T>(Func<T, bool> checkCondition) where T : Ty
		{
			List<T> list = new List<T>();
			if (checkCondition == null)
			{
				return list;
			}

			foreach (KeyValuePair<Type, List<Ty>> keyValuePair in objectTypeMap)
			{
				if (!(keyValuePair.Key != typeof(T)) || keyValuePair.Key.IsSubclassOf(typeof(T)))
				{
					foreach (Ty ty in keyValuePair.Value)
					{
						T t = ty as T;
						if (checkCondition(t))
						{
							list.Add(t);
						}
					}
				}
			}

			return list;
		}

		public void FindAllDoAction<T>(Action<T> doAction) where T : Ty
		{
			if (doAction == null)
			{
				return;
			}

			foreach (KeyValuePair<Type, List<Ty>> keyValuePair in objectTypeMap)
			{
				if (keyValuePair.Key == typeof(T) || keyValuePair.Key.IsSubclassOf(typeof(T)))
				{
					foreach (Ty ty in keyValuePair.Value)
					{
						T obj = ty as T;
						doAction(obj);
					}
				}
			}
		}

		public void RemoveOnDestroy(Ty worldObject)
		{
			if (objectMap.TryGetValue(worldObject.ObjectId, out Ty ty))
			{
				objectMap.Remove(worldObject.ObjectId);
				Type type = ty.GetType();
				List<Type> list = GetParentTypesWithSelf(type);
				foreach (Type key in list)
				{
					if (objectTypeMap.TryGetValue(key, out List<Ty> list2))
					{
						list2.Remove(ty);
					}
				}

				cacheAllDirty = true;
				foreach (Type key2 in list)
				{
					cacheDirty[key2] = true;
				}
			}
		}

		protected void DestroyObject(int objectId)
		{
			Ty ty = RemoveObject(objectId);
			if (ty)
			{
				if (!ReturnToPool(ty))
				{
					Destroy(ty.gameObject);
				}

				return;
			}

			throw new GameException(ErrorType.ObjectNotFound,
				$"Failed to find object by ObjectId[${objectId}]");
		}


		protected Ty RemoveObject(int objectId)
		{
			if (!objectMap.TryGetValue(objectId, out Ty ty))
			{
				return null;
			}

			objectMap.Remove(objectId);
			Type type = ty.GetType();
			List<Type> list = GetParentTypesWithSelf(type);
			foreach (Type key in list)
			{
				if (objectTypeMap.TryGetValue(key, out List<Ty> list2))
				{
					list2.Remove(ty);
				}
			}

			cacheAllDirty = true;
			foreach (Type key2 in list)
			{
				cacheDirty[key2] = true;
			}

			return ty;
		}


		public bool ReturnToPool(Ty worldObject)
		{
			if (worldObject.ObjectType == ObjectType.Projectile)
			{
				Transform _trm = worldObject.transform;
				for (int i = _trm.childCount - 1; i >= 0; i--)
				{
					DestroyImmediate(_trm.GetChild(i).gameObject);
				}

				worldObject.RemoveAllAttachedSight();
				projectilePool.Push(_trm.gameObject);
				return true;
			}

			return worldObject.ObjectType == ObjectType.Monster;
		}


		private List<Type> GetParentTypesWithSelf(Type type)
		{
			if (!parentTypesWithSelf.TryGetValue(type, out List<Type> list))
			{
				list = new List<Type>
				{
					type
				};
				parentTypesWithSelf.Add(type, list);
				if (type.IsSubclassOf(typeof(ObjectBase)))
				{
					Type baseType = type.BaseType;
					while (baseType != null && baseType != typeof(ObjectBase))
					{
						list.Add(baseType);
						baseType = baseType.BaseType;
					}
				}
			}

			return list;
		}
	}
}