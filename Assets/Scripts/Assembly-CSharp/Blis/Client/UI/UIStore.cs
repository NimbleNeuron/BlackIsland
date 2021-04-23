using System;
using Blis.Common;

namespace Blis.Client.UI
{
	public abstract class UIStore<T> : IDisposable, IStore where T : UIStore<T>
	{
		private bool isChanged;


		protected UIStore()
		{
			RegisterDispatcher();
		}


		public void Dispose()
		{
			UnregisterDispatcher();
		}


		public void CommitStore()
		{
			if (isChanged)
			{
				PreCommit();
				Action<T> onStoreChange = OnStoreChange;
				if (onStoreChange != null)
				{
					onStoreChange((T) this);
				}

				isChanged = false;
			}
		}


		public virtual void OnSceneLoaded() { }

		
		
		private event Action<T> OnStoreChange;


		private void RegisterDispatcher()
		{
			SingletonMonoBehaviour<UIDispatcher>.inst.Register<T>((T) this);
		}


		private void UnregisterDispatcher()
		{
			SingletonMonoBehaviour<UIDispatcher>.inst.Unregister<T>((T) this);
		}


		public void AddListener(Action<T> action)
		{
			OnStoreChange += action;
		}


		public void RemoveListener(Action<T> action)
		{
			OnStoreChange -= action;
		}


		public void StoreChange(UIAction data)
		{
			ActionHandle(data);
			isChanged = true;
		}


		protected abstract void ActionHandle(UIAction data);


		protected abstract void PreCommit();
	}
}