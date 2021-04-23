using UnityEngine;

namespace Werewolf.StatusIndicators.Demo
{
	public class CameraMovement : MonoBehaviour
	{
		public GameObject player;


		public float offsetX = -5f;


		public float offsetZ;


		public float maximumDistance = 2f;


		public float playerVelocity = 10f;


		private float movementX;


		private float movementZ;


		private void Update()
		{
			movementX = (player.transform.position.x + offsetX - transform.position.x) / maximumDistance;
			movementZ = (player.transform.position.z + offsetZ - transform.position.z) / maximumDistance;
			transform.position += new Vector3(movementX * playerVelocity * Time.deltaTime, 0f,
				movementZ * playerVelocity * Time.deltaTime);
		}
	}
}