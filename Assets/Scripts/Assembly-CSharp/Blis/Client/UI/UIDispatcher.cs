using System;
using System.Collections.Generic;
using System.Reflection;
using Blis.Common;
using UnityEngine.SceneManagement;

namespace Blis.Client.UI
{
	public class UIDispatcher : SingletonMonoBehaviour<UIDispatcher>
	{
		private static readonly HashSet<IStore> storeSet = new HashSet<IStore>();


		private static readonly Dictionary<Type, Action<UIAction>> events = new Dictionary<Type, Action<UIAction>>();


		private void LateUpdate()
		{
			foreach (IStore store in storeSet)
			{
				store.CommitStore();
			}
		}

		protected override void OnAwakeSingleton()
		{
			base.OnAwakeSingleton();
			DontDestroyOnLoad(this);
			SceneManager.sceneLoaded += OnSceneLoaded;
		}


		protected override void OnDestroySingleton()
		{
			base.OnDestroySingleton();
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}


		public void Register<T>(T store) where T : UIStore<T>
		{
			storeSet.Add(store);
			UIActionMappingAttribute customAttribute = store.GetType().GetCustomAttribute<UIActionMappingAttribute>();
			if (customAttribute == null)
			{
				Log.E(store.GetType().Name + " need UIActionMapping attribute.");
				return;
			}

			for (int i = 0; i < customAttribute.actions.Length; i++)
			{
				if (events.ContainsKey(customAttribute.actions[i]))
				{
					Dictionary<Type, Action<UIAction>> dictionary = events;
					Type key = customAttribute.actions[i];
					dictionary[key] =
						(Action<UIAction>) Delegate.Combine(dictionary[key], new Action<UIAction>(store.StoreChange));
				}
				else
				{
					events.Add(customAttribute.actions[i], store.StoreChange);
				}
			}
		}


		public void Unregister<T>(T store) where T : UIStore<T>
		{
			storeSet.Remove(store);
			UIActionMappingAttribute customAttribute = store.GetType().GetCustomAttribute<UIActionMappingAttribute>();
			for (int i = 0; i < customAttribute.actions.Length; i++)
			{
				Dictionary<Type, Action<UIAction>> dictionary = events;
				Type key = customAttribute.actions[i];
				dictionary[key] =
					(Action<UIAction>) Delegate.Remove(dictionary[key], new Action<UIAction>(store.StoreChange));
			}
		}


		public void Action(UIAction uiAction)
		{
			Type type = uiAction.GetType();
			if (events.ContainsKey(type))
			{
				events[type](uiAction);
			}
		}


		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (loadSceneMode == LoadSceneMode.Single)
			{
				foreach (IStore store in storeSet)
				{
					store.OnSceneLoaded();
				}
			}
		}
	}
}