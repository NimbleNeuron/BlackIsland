using UnityEngine;

namespace Blis.Common
{
	
	public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		
		private static T _instance;

		
		
		public static T inst => Instance;

		
		
		public static T Instance {
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<T>();
					if (_instance == null)
					{
						_instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
					}
				}

				return _instance;
			}
		}

		
		private void Awake()
		{
			if (_instance == null)
			{
				_instance = this as T;
			}
			else if (_instance != this)
			{
				Destroy(this);
				return;
			}

			OnAwakeSingleton();
		}

		
		private void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = default;
				OnDestroySingleton();
			}
		}

		
		protected virtual void OnAwakeSingleton() { }

		
		protected virtual void OnDestroySingleton() { }
	}
}