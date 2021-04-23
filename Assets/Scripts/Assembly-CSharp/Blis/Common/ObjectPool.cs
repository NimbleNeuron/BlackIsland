using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blis.Common
{
	public abstract class ObjectPool : MonoBehaviour
	{
		private readonly Dictionary<Type, PooledObject> _objectPool = new Dictionary<Type, PooledObject>();


		private PooledObject _gameObjectPool;

		public abstract void InitPool();


		protected void AllocPool(int poolCount, GameObject model)
		{
			if (_gameObjectPool != null)
			{
				return;
			}

			if (model == null)
			{
				Log.E("Not exist object.");
				return;
			}

			_gameObjectPool = new PooledObject();
			_gameObjectPool.Initialize(typeof(GameObject), Mathf.Max(1, poolCount), model, transform);
		}


		public GameObject Pop(Transform parent = null)
		{
			PooledObject gameObjectPool = _gameObjectPool;
			if (gameObjectPool == null)
			{
				return null;
			}

			return gameObjectPool.Pop(parent);
		}


		public bool Push(GameObject gameObject)
		{
			if (_gameObjectPool == null)
			{
				return false;
			}

			_gameObjectPool.Push(gameObject, transform);
			return true;
		}


		protected void AllocPool<T>(int poolCount, GameObject model)
		{
			if (_objectPool.ContainsKey(typeof(T)))
			{
				return;
			}

			if (model == null)
			{
				Debug.LogError("Not exist object.");
				return;
			}

			PooledObject pooledObject = new PooledObject();
			pooledObject.Initialize(typeof(T), Mathf.Max(1, poolCount), model, transform);
			_objectPool.Add(typeof(T), pooledObject);
		}


		public GameObject Pop<T>(Transform parent = null) where T : MonoBehaviour
		{
			PooledObject poolItem = GetPoolItem<T>();
			if (poolItem == null)
			{
				return null;
			}

			return poolItem.Pop(parent);
		}


		public bool Push<T>(T item) where T : MonoBehaviour
		{
			PooledObject poolItem = GetPoolItem<T>();
			if (poolItem == null)
			{
				return false;
			}

			poolItem.Push(item.gameObject, transform);
			return true;
		}


		private PooledObject GetPoolItem<T>() where T : MonoBehaviour
		{
			if (_objectPool.ContainsKey(typeof(T)))
			{
				return _objectPool[typeof(T)];
			}

			return null;
		}


		private PooledObject GetPoolItem(Type t)
		{
			if (_objectPool.ContainsKey(t))
			{
				return _objectPool[t];
			}

			return null;
		}


		public void PoolCountLog()
		{
			_gameObjectPool.PoolCountLog(GetType().ToString());
		}


		private class PooledObject
		{
			private readonly List<GameObject> _poolList = new List<GameObject>();


			private int _poolCount;


			private string _poolItemName;


			private GameObject _prefab;


			private Transform _prefabParent;

			public void PoolCountLog(string name) { }


			public void Initialize(Type objectType, int poolCount, GameObject prefab, Transform parent)
			{
				_poolItemName = objectType.ToString();
				_poolCount = poolCount;
				_prefab = prefab;
				_prefabParent = parent;
				Allocate(poolCount, parent);
			}


			private void Allocate(int poolCount, Transform parent)
			{
				_poolCount += poolCount;
				for (int i = 0; i < poolCount; i++)
				{
					_poolList.Add(CreateItem(parent));
				}
			}


			public void Push(GameObject item, Transform parent = null)
			{
				item.name = "[POOL]" + _prefab.name;
				item.transform.SetParent(parent);
				item.SetActive(false);
				_poolList.Add(item);
			}


			public GameObject Pop(Transform parent = null)
			{
				if (_poolList.Count == 0)
				{
					int poolCount = _poolCount / 2 + 1;
					Allocate(poolCount, _prefabParent);
				}

				GameObject gameObject = _poolList[0];
				gameObject.transform.SetParent(parent);
				gameObject.SetActive(true);
				_poolList.RemoveAt(0);
				return gameObject;
			}


			private GameObject CreateItem(Transform parent = null)
			{
				GameObject gameObject = Instantiate<GameObject>(_prefab);
				gameObject.name = "[POOL]" + _prefab.name;
				gameObject.transform.SetParent(parent);
				gameObject.SetActive(false);
				return gameObject;
			}
		}
	}
}