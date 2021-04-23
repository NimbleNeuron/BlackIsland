using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class MarginSizeModifier : ScreenDependentSize<Margin>
	{
		public SizeModifierCollection ModLeft;


		public SizeModifierCollection ModRight;


		public SizeModifierCollection ModTop;


		public SizeModifierCollection ModBottom;


		public MarginSizeModifier(Margin optimizedSize, Margin minSize, Margin maxSize) : base(optimizedSize, minSize,
			maxSize, optimizedSize.Clone())
		{
			ModLeft = new SizeModifierCollection(new SizeModifierCollection.SizeModifier(ImpactMode.PixelWidth, 1f));
			ModRight = new SizeModifierCollection(new SizeModifierCollection.SizeModifier(ImpactMode.PixelWidth, 1f));
			ModTop = new SizeModifierCollection(new SizeModifierCollection.SizeModifier(ImpactMode.PixelWidth, 1f));
			ModBottom = new SizeModifierCollection(new SizeModifierCollection.SizeModifier(ImpactMode.PixelWidth, 1f));
		}


		public override void DynamicInitialization()
		{
			if (value == null)
			{
				value = new Margin();
			}
		}


		public override IEnumerable<SizeModifierCollection> GetModifiers()
		{
			yield return ModLeft;
			yield return ModRight;
			yield return ModTop;
			yield return ModBottom;
		}


		protected override void AdjustSize(float factor, SizeModifierCollection mod, int index)
		{
			if (value == null)
			{
				value = new Margin();
			}

			value[index] = Mathf.RoundToInt(GetSize(factor, OptimizedSize[index], MinSize[index], MaxSize[index]));
		}


		protected override void CalculateOptimizedSize(Margin baseValue, float factor, SizeModifierCollection mod,
			int index)
		{
			OptimizedSize[index] = Mathf.RoundToInt(factor * baseValue[index]);
		}
	}
}