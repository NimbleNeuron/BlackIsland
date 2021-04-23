using UnityEngine;

namespace Werewolf.StatusIndicators.Effects
{
	public class ProjectorFixedRotation : MonoBehaviour
	{
		public float Angle;


		private void LateUpdate()
		{
			transform.eulerAngles = new Vector3(90f, Angle, 0f);
		}
	}
}