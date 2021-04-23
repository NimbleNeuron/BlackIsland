using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class Vector3SizeModifier : ScreenDependentSize<Vector3>
	{
		public SizeModifierCollection ModX;


		public SizeModifierCollection ModY;


		public SizeModifierCollection ModZ;


		public Vector3SizeModifier(Vector3 optimizedSize, Vector3 minSize, Vector3 maxSize) : base(optimizedSize,
			minSize, maxSize, optimizedSize)
		{
			ModX = new SizeModifierCollection(new SizeModifierCollection.SizeModifier(ImpactMode.PixelHeight, 1f));
			ModY = new SizeModifierCollection(new SizeModifierCollection.SizeModifier(ImpactMode.PixelHeight, 1f));
			ModZ = new SizeModifierCollection(Array.Empty<SizeModifierCollection.SizeModifier>());
		}


		public override IEnumerable<SizeModifierCollection> GetModifiers()
		{
			yield return ModX;
			yield return ModY;
			yield return ModZ;
		}


		protected override void AdjustSize(float factor, SizeModifierCollection mod, int index)
		{
			value[index] = GetSize(factor, OptimizedSize[index], MinSize[index], MaxSize[index]);
		}


		protected override void CalculateOptimizedSize(Vector3 baseValue, float factor, SizeModifierCollection mod,
			int index)
		{
			OptimizedSize[index] = factor * baseValue[index];
		}
	}
}