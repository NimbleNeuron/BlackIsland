using UnityEngine;

namespace Blis.Common.Utils
{
	[DefaultExecutionOrder(-9999)]
	public class MonoBehaviourInstance<T> : MonoBehaviour where T : MonoBehaviourInstance<T>
	{
		private static T instance;


		public static T inst => instance;


		private void Awake()
		{
			if (instance == null)
			{
				instance = this as T;
			}
			else if (instance != this)
			{
				instance = this as T;
			}

			_Awake();
		}


		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = default;
			}

			_OnDestroy();
		}


		protected virtual void _Awake() { }


		protected virtual void _OnDestroy() { }
	}
}