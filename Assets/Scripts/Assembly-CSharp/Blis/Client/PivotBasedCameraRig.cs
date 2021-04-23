using UnityEngine;

namespace Blis.Client
{
	public abstract class PivotBasedCameraRig : AbstractTargetFollower
	{
		protected Transform m_Cam;


		protected Vector3 m_LastTargetPosition;


		protected Transform m_Pivot;

		protected virtual void Awake()
		{
			m_Cam = GetComponentInChildren<Camera>().transform;
			m_Pivot = m_Cam.parent;
		}
	}
}