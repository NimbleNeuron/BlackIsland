using UnityEngine;

namespace Blis.Client
{
	internal class MouseHit
	{
		public enum HitType
		{
			INVALID,
			OBJECT,
			GROUND,
			FOG,
			UI
		}

		public readonly HitType hitType;
		public readonly Vector3 point;
		public readonly LocalObject target;

		public MouseHit(HitType hitType, Vector3 point, LocalObject target)
		{
			this.hitType = hitType;
			this.point = point;
			this.target = target;
		}
	}
}