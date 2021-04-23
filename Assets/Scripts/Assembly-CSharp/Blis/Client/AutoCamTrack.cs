using UnityEngine;

namespace Blis.Client
{
	public class AutoCamTrack : CameraTrack
	{
		private AutoCam _autoCam;

		private void Start()
		{
			_autoCam = GetComponent<AutoCam>();
		}


		public override void SetTarget(Transform target)
		{
			if (_autoCam)
			{
				_autoCam.SetTarget(target);
			}
		}
	}
}