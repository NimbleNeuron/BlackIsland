using System;
using UnityEngine;

namespace AraSamples
{
	[Serializable]
	public class AxleInfo
	{
		public WheelCollider leftWheel;


		public WheelCollider rightWheel;


		public bool motor;


		public bool steering;
	}
}