using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public abstract class ScreenDependentSize
	{
		
		public abstract string ScreenConfigName { get; set; }


		protected void UpdateSize(Component caller)
		{
			int num = 0;
			foreach (SizeModifierCollection sizeModifierCollection in GetModifiers())
			{
				float factor = sizeModifierCollection.CalculateFactor(caller, ScreenConfigName);
				AdjustSize(factor, sizeModifierCollection, num);
				num++;
			}
		}


		public virtual void DynamicInitialization() { }


		public abstract IEnumerable<SizeModifierCollection> GetModifiers();


		protected abstract void AdjustSize(float factor, SizeModifierCollection mod, int index);
	}
}