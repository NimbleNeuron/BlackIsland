using System;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public abstract class ScreenDependentSize<T> : ScreenDependentSize, IScreenConfigConnection
	{
		[SerializeField] private string screenConfigName;


		public T OptimizedSize;


		public T MinSize;


		public T MaxSize;


		protected T value;


		protected ScreenDependentSize(T opt, T min, T max, T initValue)
		{
			OptimizedSize = opt;
			MinSize = min;
			MaxSize = max;
			value = initValue;
		}


		public T LastCalculatedSize => value;


		
		public override string ScreenConfigName {
			get => screenConfigName;
			set => screenConfigName = value;
		}


		public T CalculateSize(Component caller)
		{
			UpdateSize(caller);
			return value;
		}


		protected float GetSize(float factor, float opt, float min, float max)
		{
			return Mathf.Clamp(factor * opt, min, max);
		}


		public void SetSize(Component caller, T size)
		{
			int num = 0;
			foreach (SizeModifierCollection sizeModifierCollection in GetModifiers())
			{
				float factor = 1f / sizeModifierCollection.CalculateFactor(caller, screenConfigName);
				CalculateOptimizedSize(size, factor, sizeModifierCollection, num);
				num++;
			}

			value = size;
		}


		public void OverrideLastCalculatedSize(T newValue)
		{
			value = newValue;
		}


		protected abstract void
			CalculateOptimizedSize(T baseValue, float factor, SizeModifierCollection mod, int index);
	}
}