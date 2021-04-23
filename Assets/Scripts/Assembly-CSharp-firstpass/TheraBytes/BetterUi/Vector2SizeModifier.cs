using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class Vector2SizeModifier : ScreenDependentSize<Vector2>
	{
		public SizeModifierCollection ModX;


		public SizeModifierCollection ModY;


		public Vector2SizeModifier(Vector2 optimizedSize, Vector2 minSize, Vector2 maxSize) : base(optimizedSize,
			minSize, maxSize, optimizedSize)
		{
			ModX = new SizeModifierCollection(new SizeModifierCollection.SizeModifier(ImpactMode.PixelHeight, 1f));
			ModY = new SizeModifierCollection(new SizeModifierCollection.SizeModifier(ImpactMode.PixelHeight, 1f));
		}


		public override IEnumerable<SizeModifierCollection> GetModifiers()
		{
			yield return ModX;
			yield return ModY;
		}


		protected override void AdjustSize(float factor, SizeModifierCollection mod, int index)
		{
			value[index] = GetSize(factor, OptimizedSize[index], MinSize[index], MaxSize[index]);
		}


		protected override void CalculateOptimizedSize(Vector2 baseValue, float factor, SizeModifierCollection mod,
			int index)
		{
			OptimizedSize[index] = factor * baseValue[index];
		}
	}
}