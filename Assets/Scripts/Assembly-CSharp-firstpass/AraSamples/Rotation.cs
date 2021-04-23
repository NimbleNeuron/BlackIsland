using UnityEngine;

namespace AraSamples
{
	public class Rotation : MonoBehaviour
	{
		public float speed = 10f;


		public Vector3 axis;


		private void Update()
		{
			transform.Rotate(axis, speed * Time.deltaTime);
		}
	}
}