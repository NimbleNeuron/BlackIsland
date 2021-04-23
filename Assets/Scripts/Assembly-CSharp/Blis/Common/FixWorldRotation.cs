using UnityEngine;

namespace Blis.Common
{
	public class FixWorldRotation : MonoBehaviour
	{
		[SerializeField] private Vector3 euler = default;


		private Quaternion rotation = default;

		public void Awake()
		{
			rotation = Quaternion.Euler(euler);
		}


		public void LateUpdate()
		{
			transform.rotation = rotation;
		}
	}
}