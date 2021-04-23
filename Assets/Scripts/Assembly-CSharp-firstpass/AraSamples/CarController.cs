using System.Collections.Generic;
using UnityEngine;

namespace AraSamples
{
	public class CarController : MonoBehaviour
	{
		public List<AxleInfo> axleInfos;


		public float maxMotorTorque;


		public float maxSteeringAngle;


		public void FixedUpdate()
		{
			float motorTorque = maxMotorTorque * Input.GetAxis("Vertical");
			float steerAngle = maxSteeringAngle * Input.GetAxis("Horizontal");
			foreach (AxleInfo axleInfo in axleInfos)
			{
				if (axleInfo.steering)
				{
					axleInfo.leftWheel.steerAngle = steerAngle;
					axleInfo.rightWheel.steerAngle = steerAngle;
				}

				if (axleInfo.motor)
				{
					axleInfo.leftWheel.motorTorque = motorTorque;
					axleInfo.rightWheel.motorTorque = motorTorque;
				}

				ApplyLocalPositionToVisuals(axleInfo.leftWheel);
				ApplyLocalPositionToVisuals(axleInfo.rightWheel);
			}
		}


		public void ApplyLocalPositionToVisuals(WheelCollider collider)
		{
			if (collider.transform.childCount == 0)
			{
				return;
			}

			Transform child = collider.transform.GetChild(0);
			Vector3 position;
			Quaternion rotation;
			collider.GetWorldPose(out position, out rotation);
			child.transform.position = position;
			child.transform.rotation = rotation;
		}
	}
}