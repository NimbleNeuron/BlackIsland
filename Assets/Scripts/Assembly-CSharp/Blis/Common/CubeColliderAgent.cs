using UnityEngine;
using UnityEngine.AI;

namespace Blis.Common
{
	public class CubeColliderAgent : ColliderAgent
	{
		protected BoxCollider coll;


		private NavMeshObstacle navMeshObstacle;

		protected override Collider GetCollider()
		{
			return coll;
		}


		public Collider Init(float radius)
		{
			GameUtil.BindOrAdd<BoxCollider>(gameObject, ref coll);
			coll.size = new Vector3(radius, GameConstants.DEFAULT_WORLDOBJECT_COLLIDER_HEIGHT, radius);
			GameUtil.BindOrAdd<NavMeshObstacle>(gameObject, ref navMeshObstacle);
			navMeshObstacle.shape = NavMeshObstacleShape.Box;
			navMeshObstacle.carving = true;
			navMeshObstacle.carveOnlyStationary = true;
			navMeshObstacle.center = coll.center;
			navMeshObstacle.size = Vector3.Scale(coll.size, coll.transform.localScale);
			return coll;
		}
	}
}