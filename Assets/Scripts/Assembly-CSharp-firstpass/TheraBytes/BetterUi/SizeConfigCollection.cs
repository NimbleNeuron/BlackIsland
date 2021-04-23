using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class SizeConfigCollection<T> : ISizeConfigCollection where T : class, IScreenConfigConnection
	{
		[SerializeField] private List<T> items = new List<T>();


		private bool sorted;


		public List<T> Items => items;


		public string GetCurrentConfigName()
		{
			T currentItem = GetCurrentItem(default);
			if (currentItem != null)
			{
				return currentItem.ScreenConfigName;
			}

			return null;
		}


		public void Sort()
		{
			List<string> order = (from o in SingletonScriptableObject<ResolutionMonitor>.Instance.OptimizedScreens
				select o.Name).ToList<string>();
			items.Sort((a, b) => order.IndexOf(a.ScreenConfigName).CompareTo(order.IndexOf(b.ScreenConfigName)));
			sorted = true;
		}


		public T GetCurrentItem(T fallback)
		{
			if (ResolutionMonitor.CurrentScreenConfiguration == null)
			{
				return fallback;
			}

			if (!sorted)
			{
				Sort();
			}

			foreach (T t in items)
			{
				if (string.IsNullOrEmpty(t.ScreenConfigName))
				{
					return fallback;
				}

				ScreenTypeConditions config = ResolutionMonitor.GetConfig(t.ScreenConfigName);
				if (config != null && config.IsActive)
				{
					return t;
				}
			}

			foreach (ScreenTypeConditions screenTypeConditions in ResolutionMonitor.GetCurrentScreenConfigurations())
			{
				using (List<string>.Enumerator enumerator3 = screenTypeConditions.Fallbacks.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						string c = enumerator3.Current;
						T t2 = items.FirstOrDefault(o => o.ScreenConfigName == c);
						if (t2 != null)
						{
							return t2;
						}
					}
				}
			}

			return fallback;
		}
	}
}