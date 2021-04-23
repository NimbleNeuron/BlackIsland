using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;

namespace Blis.Client.UI
{
	public static class UISystem
	{
		private static readonly Dictionary<Type, object> storeDictionary = new Dictionary<Type, object>();

		static UISystem()
		{
			foreach (Type type in (from x in typeof(IStore).Assembly.GetTypes()
				where !x.IsAbstract && !x.IsInterface && x.GetInterfaces().Contains(typeof(IStore))
				select x).ToList<Type>())
			{
				storeDictionary.Add(type, Activator.CreateInstance(type));
			}
		}


		public static void Action(UIAction action)
		{
			SingletonMonoBehaviour<UIDispatcher>.inst.Action(action);
		}


		public static void AddListener<T>(Action<T> action) where T : UIStore<T>
		{
			Type typeFromHandle = typeof(T);
			if (storeDictionary.ContainsKey(typeFromHandle))
			{
				((T) storeDictionary[typeFromHandle]).AddListener(action);
			}
		}


		public static void RemoveListener<T>(Action<T> action) where T : UIStore<T>
		{
			Type typeFromHandle = typeof(T);
			if (storeDictionary.ContainsKey(typeFromHandle))
			{
				((T) storeDictionary[typeFromHandle]).RemoveListener(action);
			}
		}


		public static T GetStore<T>() where T : UIStore<T>
		{
			Type typeFromHandle = typeof(T);
			if (storeDictionary.ContainsKey(typeFromHandle))
			{
				return (T) storeDictionary[typeFromHandle];
			}

			return default;
		}
	}
}