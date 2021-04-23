using UnityEngine;

namespace Blis.Common
{
	
	public class CharacterColliderAgent : ColliderAgent
	{
		
		protected override Collider GetCollider()
		{
			return this.coll;
		}

		
		public void Init(float radius)
		{
			GameUtil.BindOrAdd<CapsuleCollider>(base.gameObject, ref this.coll);
			this.coll.radius = radius;
			this.coll.height = GameConstants.DEFAULT_WORLDOBJECT_COLLIDER_HEIGHT;
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

		
		private CapsuleCollider coll;
	}
}
