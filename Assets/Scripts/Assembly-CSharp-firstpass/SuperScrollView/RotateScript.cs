using UnityEngine;

namespace SuperScrollView
{
	public class RotateScript : MonoBehaviour
	{
		public float speed = 1f;


		private void Update()
		{
			Vector3 localEulerAngles = gameObject.transform.localEulerAngles;
			localEulerAngles.z += speed * Time.deltaTime;
			gameObject.transform.localEulerAngles = localEulerAngles;
		}
	}
}