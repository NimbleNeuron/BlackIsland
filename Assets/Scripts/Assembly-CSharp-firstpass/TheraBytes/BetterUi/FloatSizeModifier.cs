using System;
using System.Collections.Generic;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class FloatSizeModifier : ScreenDependentSize<float>
	{
		public SizeModifierCollection Mod;


		public FloatSizeModifier(float optimizedSize, float minSize, float maxSize) : base(optimizedSize, minSize,
			maxSize, optimizedSize)
		{
			Mod = new SizeModifierCollection(new SizeModifierCollection.SizeModifier(ImpactMode.PixelHeight, 1f));
		}


		public override IEnumerable<SizeModifierCollection> GetModifiers()
		{
			yield return Mod;
		}


		protected override void AdjustSize(float factor, SizeModifierCollection mod, int index)
		{
			value = GetSize(factor, OptimizedSize, MinSize, MaxSize);
		}


		protected override void CalculateOptimizedSize(float baseValue, float factor, SizeModifierCollection mod,
			int index)
		{
			OptimizedSize = factor * baseValue;
		}
	}
}