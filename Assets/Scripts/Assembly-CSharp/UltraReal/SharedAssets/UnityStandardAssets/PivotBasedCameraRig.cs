using UnityEngine;

namespace UltraReal.SharedAssets.UnityStandardAssets
{
	
	public abstract class PivotBasedCameraRig : AbstractTargetFollower
	{
		
		protected virtual void Awake()
		{
			this.m_Cam = base.GetComponentInChildren<Camera>().transform;
			this.m_Pivot = this.m_Cam.parent;
		}

		
		protected Transform m_Cam;

		
		protected Transform m_Pivot;

		
		protected Vector3 m_LastTargetPosition;
	}
}
