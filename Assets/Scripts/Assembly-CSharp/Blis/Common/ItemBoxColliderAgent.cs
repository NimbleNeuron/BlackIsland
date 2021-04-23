using UnityEngine;
using UnityEngine.AI;

namespace Blis.Common
{
	
	public class ItemBoxColliderAgent : ColliderAgent
	{
		
		protected override Collider GetCollider()
		{
			return this.coll;
		}

		
		public void Init(ObjectType objectType, NavMeshObstacleShape shape, Collider sourceColl, bool ignoreObstacle)
		{
			if (!ignoreObstacle)
			{
				GameUtil.BindOrAdd<NavMeshObstacle>(base.gameObject, ref this.navMeshObstacle);
				this.navMeshObstacle.shape = shape;
				this.navMeshObstacle.carving = true;
				this.navMeshObstacle.carveOnlyStationary = true;
			}
			if (shape == NavMeshObstacleShape.Box)
			{
				BoxCollider boxCollider = null;
				GameUtil.BindOrAdd<BoxCollider>(base.gameObject, ref boxCollider);
				BoxCollider boxCollider2 = (BoxCollider)sourceColl;
				boxCollider.center = boxCollider2.center;
				boxCollider.size = boxCollider2.size;
				this.coll = boxCollider;
				if (this.navMeshObstacle != null)
				{
					this.navMeshObstacle.center = boxCollider.center;
					this.navMeshObstacle.size = Vector3.Scale(boxCollider.size, (objectType != ObjectType.AirSupplyItemBox) ? sourceColl.transform.localScale : (sourceColl.transform.localScale * 0.7f));
				}
			}
			else if (shape == NavMeshObstacleShape.Capsule)
			{
				CapsuleCollider capsuleCollider = null;
				GameUtil.BindOrAdd<CapsuleCollider>(base.gameObject, ref capsuleCollider);
				CapsuleCollider capsuleCollider2 = (CapsuleCollider)sourceColl;
				capsuleCollider.center = capsuleCollider2.center;
				capsuleCollider.height = capsuleCollider2.height;
				capsuleCollider.radius = capsuleCollider2.radius;
				this.coll = capsuleCollider;
				if (this.navMeshObstacle != null)
				{
					this.navMeshObstacle.center = capsuleCollider.center;
					this.navMeshObstacle.height = capsuleCollider.height;
					this.navMeshObstacle.radius = capsuleCollider.radius;
				}
			}
		}

		
		public override Vector3 ClosestPointOnBounds(Vector3 position)
		{
			if (!(this.coll == null))
			{
				return this.coll.ClosestPoint(position);
			}
			return position;
		}

		
		protected Collider coll;

		
		private NavMeshObstacle navMeshObstacle;
	}
}
