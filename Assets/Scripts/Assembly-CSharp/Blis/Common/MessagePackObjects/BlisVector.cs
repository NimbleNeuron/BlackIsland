using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	public class BlisVector
	{
		[IgnoreMember] private const float FixedPoint = 100f;
		[Key(0)] public readonly int x;
		[Key(1)] public readonly int y;
		[Key(2)] public readonly int z;

		[SerializationConstructor]
		public BlisVector(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public BlisVector(Vector3 v)
		{
			x = Mathf.RoundToInt(v.x * 100f);
			y = Mathf.RoundToInt(v.y * 100f);
			z = Mathf.RoundToInt(v.z * 100f);
		}

		public Vector3 ToVector3()
		{
			return new Vector3(x / 100f, y / 100f, z / 100f);
		}

		public Vector3 SamplePosition()
		{
			Vector3 vector = ToVector3();
			Vector3 result;
			if (MoveAgent.SamplePosition(vector, 2147483640, out result))
			{
				return result;
			}

			return vector;
		}
	}
}