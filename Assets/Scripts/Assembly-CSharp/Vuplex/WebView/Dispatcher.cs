using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Vuplex.WebView
{
	
	internal class Dispatcher : MonoBehaviour
	{
		
		public static void RunAsync(Action action)
		{
			ThreadPool.QueueUserWorkItem(delegate(object o)
			{
				action();
			});
		}

		
		public static void RunAsync(Action<object> action, object state)
		{
			ThreadPool.QueueUserWorkItem(delegate(object o)
			{
				action(o);
			}, state);
		}

		
		public static void RunOnMainThread(Action action)
		{
			List<Action> backlog = Dispatcher._backlog;
			lock (backlog)
			{
				Dispatcher._backlog.Add(action);
				Dispatcher._queued = true;
			}
		}

		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (Dispatcher._instance == null)
			{
				Dispatcher._instance = new GameObject("Dispatcher").AddComponent<Dispatcher>();
				UnityEngine.Object.DontDestroyOnLoad(Dispatcher._instance.gameObject);
			}
		}

		
		private void Update()
		{
			if (Dispatcher._queued)
			{
				List<Action> backlog = Dispatcher._backlog;
				lock (backlog)
				{
					List<Action> actions = Dispatcher._actions;
					Dispatcher._actions = Dispatcher._backlog;
					Dispatcher._backlog = actions;
					Dispatcher._queued = false;
				}
				foreach (Action action in Dispatcher._actions)
				{
					try
					{
						action();
					}
					catch (Exception arg)
					{
						Debug.LogError("An exception occurred while dispatching an action on the main thread: " + arg);
					}
				}
				Dispatcher._actions.Clear();
			}
		}

		
		private static Dispatcher _instance;

		
		private static volatile bool _queued = false;

		
		private static List<Action> _backlog = new List<Action>(8);

		
		private static List<Action> _actions = new List<Action>(8);
	}
}
