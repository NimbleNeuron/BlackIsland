using UnityEngine;
using UnityEngine.AI;

namespace Blis.Common
{
	
	public class CustomBoxColliderAgent : ColliderAgent
	{
		
		protected override Collider GetCollider()
		{
			return this.coll;
		}

		
		public void Init(BoxCollider source)
		{
			BoxCollider boxCollider = null;
			GameUtil.BindOrAdd<BoxCollider>(base.gameObject, ref boxCollider);
			if (source != null)
			{
				boxCollider.center = source.center;
				boxCollider.size = source.size;
			}
			this.coll = boxCollider;
			GameUtil.BindOrAdd<NavMeshObstacle>(base.gameObject, ref this.navMeshObstacle);
			this.navMeshObstacle.shape = NavMeshObstacleShape.Box;
			this.navMeshObstacle.carving = true;
			this.navMeshObstacle.carveOnlyStationary = true;
			this.navMeshObstacle.center = this.coll.center;
			this.navMeshObstacle.size = Vector3.Scale(this.coll.size, this.coll.transform.localScale);
		}

		
		protected BoxCollider coll;

		
		private NavMeshObstacle navMeshObstacle;
	}
}
