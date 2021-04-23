using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject()]
	public class BlisFixedPoint
	{
		[IgnoreMember] private const float FixedPointBias = 100f;


		public BlisFixedPoint()
		{
			InternalValue = 0;
		}


		public BlisFixedPoint(float v)
		{
			InternalValue = Mathf.FloorToInt(v * 100f);
		}


		
		[Key(0)] [field: IgnoreMember] public int InternalValue { get; set; }


		[IgnoreMember] public float Value => GetValue();


		public float GetValue()
		{
			return InternalValue / 100f;
		}
	}
}