using UnityEngine;
using UnityEngine.AI;

namespace Blis.Common
{
	public class ItemSpawnPoint : MonoBehaviour
	{
		public int code;


		public int areaCode;


		public float initSpawnTime;


		public ItemBoxType boxType;


		[ConditionalField("boxType", ItemBoxType.Static)]
		public ItemBoxSize boxSize;


		[ConditionalField("boxType", ItemBoxType.Resource)]
		public int resourceDataCode;


		public NavMeshObstacleShape shape = NavMeshObstacleShape.Box;


		public bool ignoreNavObstacle;


		public bool airSupply => boxType == ItemBoxType.AirSupply;


		public bool resource => boxType == ItemBoxType.Resource;


		public void Awake()
		{
			gameObject.SetActive(false);
		}


		private void OnDrawGizmos()
		{
			Gizmos.color = new Color(0.54901963f, 1f, 1f, 255f);
			Gizmos.DrawCube(transform.position, new Vector3(0.4f, 0.4f, 0.4f));
		}


		public Collider GetCollider()
		{
			Collider collider = null;
			if (shape == NavMeshObstacleShape.Box)
			{
				collider = GetComponent<BoxCollider>();
				if (collider == null)
				{
					collider = gameObject.AddComponent<BoxCollider>();
				}
			}
			else if (shape == NavMeshObstacleShape.Capsule)
			{
				collider = GetComponent<CapsuleCollider>();
				if (collider == null)
				{
					collider = gameObject.AddComponent<CapsuleCollider>();
				}
			}

			return collider;
		}
	}
}