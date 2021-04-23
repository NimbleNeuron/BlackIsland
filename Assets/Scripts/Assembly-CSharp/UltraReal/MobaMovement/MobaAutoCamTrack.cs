using UltraReal.SharedAssets.UnityStandardAssets;
using UnityEngine;

namespace UltraReal.MobaMovement
{
	
	public class MobaAutoCamTrack : MobaCameraTrack
	{
		
		private void Start()
		{
			this._autoCam = base.GetComponent<AutoCam>();
		}

		
		public override void SetTarget(Transform target)
		{
			if (this._autoCam)
			{
				this._autoCam.SetTarget(target);
			}
		}

		
		private AutoCam _autoCam;
	}
}
