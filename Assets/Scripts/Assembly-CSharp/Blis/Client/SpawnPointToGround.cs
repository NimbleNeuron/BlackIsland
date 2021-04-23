using Blis.Common;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Client
{
	[ExecuteInEditMode]
	public class SpawnPointToGround : MonoBehaviour
	{
		private void Awake()
		{
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(transform.position, out navMeshHit, 10f, 2147483640))
			{
				transform.position = navMeshHit.position;
			}
			else
			{
				Log.E("There is no Navigation.");
			}

			DestroyImmediate(this);
		}
	}
}