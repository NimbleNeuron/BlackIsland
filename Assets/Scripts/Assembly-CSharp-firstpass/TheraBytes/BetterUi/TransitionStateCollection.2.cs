using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public abstract class TransitionStateCollection
	{
		public abstract Object Target { get; }


		public abstract void Apply(string stateName, bool instant);


		internal abstract void SortStates(string[] sortedOrder);


		protected void SortStatesLogic<T>(List<T> states, string[] sortedOrder) where T : TransitionStateBase
		{
			states.Sort(delegate(T a, T b)
			{
				int num = -1;
				int value = -1;
				for (int i = 0; i < sortedOrder.Length; i++)
				{
					if (sortedOrder[i] == a.Name)
					{
						num = i;
					}

					if (sortedOrder[i] == b.Name)
					{
						value = i;
					}
				}

				return num.CompareTo(value);
			});
		}


		[Serializable]
		public abstract class TransitionStateBase
		{
			public string Name;


			public TransitionStateBase(string name)
			{
				Name = name;
			}
		}
	}
}