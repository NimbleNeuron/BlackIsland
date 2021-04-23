using UnityEngine;

namespace Blis.Common
{
	
	public class SphereColliderAgent : ColliderAgent
	{
		
		protected override Collider GetCollider()
		{
			return this.coll;
		}

		
		public void Init(float radius)
		{
			GameUtil.BindOrAdd<SphereCollider>(base.gameObject, ref this.coll);
			this.coll.radius = radius;
		}

		
		public void UpdateRadius(float radius)
		{
			this.coll.radius = radius;
		}

		
		public override Vector3 ClosestPointOnBounds(Vector3 position)
		{
			if (!(this.coll == null))
			{
				return this.coll.ClosestPointOnBounds(position);
			}
			return position;
		}

		
		private SphereCollider coll;
	}
}
