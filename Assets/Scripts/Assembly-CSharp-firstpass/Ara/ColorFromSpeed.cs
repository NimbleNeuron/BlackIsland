using UnityEngine;

namespace Ara
{
	[RequireComponent(typeof(AraTrail))]
	public class ColorFromSpeed : MonoBehaviour
	{
		[Tooltip(
			"Maps trail speed to color. Control how much speed is transferred to the trail by setting inertia > 0. The trail will be colorized even if physics are disabled. ")]
		public Gradient colorFromSpeed = new Gradient();


		[Tooltip("Min speed used to map speed to color.")]
		public float minSpeed;


		[Tooltip("Max speed used to map speed to color.")]
		public float maxSpeed = 5f;


		private AraTrail trail;


		private void OnEnable()
		{
			trail = GetComponent<AraTrail>();
			trail.onUpdatePoints += SetColorFromSpeed;
		}


		private void OnDisable()
		{
			trail.onUpdatePoints -= SetColorFromSpeed;
		}


		private void SetColorFromSpeed()
		{
			float num = Mathf.Max(1E-05f, maxSpeed - minSpeed);
			for (int i = 0; i < trail.points.Count; i++)
			{
				AraTrail.Point value = trail.points[i];
				value.color = colorFromSpeed.Evaluate((value.velocity.magnitude - minSpeed) / num);
				trail.points[i] = value;
			}
		}
	}
}