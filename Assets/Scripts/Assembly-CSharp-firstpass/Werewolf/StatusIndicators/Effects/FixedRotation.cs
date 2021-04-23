using UnityEngine;

namespace Werewolf.StatusIndicators.Effects
{
	public class FixedRotation : MonoBehaviour
	{
		public Vector3 Rotation;


		private Quaternion qRotaion;


		private void Awake()
		{
			qRotaion = Quaternion.Euler(Rotation);
		}


		private void LateUpdate()
		{
			transform.rotation = qRotaion;
		}
	}
}