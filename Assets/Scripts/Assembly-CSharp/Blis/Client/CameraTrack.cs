using UnityEngine;

namespace Blis.Client
{
	public abstract class CameraTrack : MonoBehaviour
	{
		public abstract void SetTarget(Transform target);
	}
}