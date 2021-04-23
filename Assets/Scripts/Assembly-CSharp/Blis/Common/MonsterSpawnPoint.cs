using UnityEngine;

namespace Blis.Common
{
	public class MonsterSpawnPoint : MonoBehaviour
	{
		public int monsterCode;


		public int level;

		private void OnDrawGizmos()
		{
			Gizmos.color = new Color(0.54901963f, 0.2f, 0.2f, 255f);
			Gizmos.DrawCube(transform.position, new Vector3(0.4f, 0.4f, 0.4f));
		}
	}
}