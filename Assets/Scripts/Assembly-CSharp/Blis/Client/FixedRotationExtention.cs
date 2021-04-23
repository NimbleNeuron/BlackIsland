using System;
using UnityEngine;

namespace Blis.Client
{
	public class FixedRotationExtention : MonoBehaviour
	{
		[Header("0 이상 360 이하 값만 적용됨.")] [SerializeField]
		private Vector3 angle = Vector3.zero;


		private FixedAxle fixedAxle = FixedAxle.All;


		private Quaternion qAngle = Quaternion.identity;

		private void Awake()
		{
			fixedAxle = FixedAxle.All;
			if (angle.x > 360f || angle.x < 0f)
			{
				fixedAxle &= ~FixedAxle.X;
			}

			if (angle.y > 360f || angle.y < 0f)
			{
				fixedAxle &= ~FixedAxle.Y;
			}

			if (angle.z > 360f || angle.z < 0f)
			{
				fixedAxle &= ~FixedAxle.Z;
			}

			if (fixedAxle == FixedAxle.None)
			{
				Destroy(this);
			}

			qAngle = Quaternion.Euler(angle);
		}


		private void LateUpdate()
		{
			if (fixedAxle == FixedAxle.All)
			{
				transform.rotation = qAngle;
				return;
			}

			Vector3 eulerAngles = transform.eulerAngles;
			if (fixedAxle.HasFlag(FixedAxle.X))
			{
				eulerAngles.x = angle.x;
			}

			if (fixedAxle.HasFlag(FixedAxle.Y))
			{
				eulerAngles.y = angle.y;
			}

			if (fixedAxle.HasFlag(FixedAxle.Z))
			{
				eulerAngles.z = angle.z;
			}

			transform.eulerAngles = eulerAngles;
		}


		[Flags]
		private enum FixedAxle
		{
			None = 0,

			X = 1,

			Y = 2,

			Z = 4,

			All = 7
		}
	}
}