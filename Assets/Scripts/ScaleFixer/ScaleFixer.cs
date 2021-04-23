using UnityEngine;

namespace ScaleFixer
{
	public class ScaleFixer : MonoBehaviour
	{
		public Vector3 localPositionOffset = Vector3.zero;
		public Vector3 overrideScale = Vector3.one;
		
		private void Awake()
		{
		}

		private void LateUpdate()
		{
			transform.localPosition += localPositionOffset;
			transform.localScale = overrideScale;
		}
	}
}