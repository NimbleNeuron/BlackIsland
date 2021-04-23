using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	public static class RealIvyMathUtils
	{
		public static float DistanceBetweenPointAndSegmentSS(Vector2 point, Vector2 a, Vector2 b)
		{
			float num = (point.x - a.x) * (b.x - a.x) + (point.y - a.y) * (b.y - a.y);
			num /= (b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y);
			float sqrMagnitude;
			if (num < 0f)
			{
				sqrMagnitude = (point - a).sqrMagnitude;
			}
			else if (num >= 0f && num <= 1f)
			{
				Vector2 b2 = new Vector2(a.x + num * (b.x - a.x), a.y + num * (b.y - a.y));
				sqrMagnitude = (point - b2).sqrMagnitude;
			}
			else
			{
				sqrMagnitude = (point - b).sqrMagnitude;
			}

			return sqrMagnitude;
		}


		public struct Segment
		{
			public Vector2 a;


			public Vector2 b;
		}
	}
}