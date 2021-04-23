using UnityEngine;

namespace VolumetricFogAndMist
{
	[AddComponentMenu("")]
	public class FogAreaCullingManager : MonoBehaviour
	{
		public VolumetricFog fog;

		private void OnEnable()
		{
			if (fog == null)
			{
				fog = GetComponent<VolumetricFog>();
				if (fog == null)
				{
					fog = gameObject.AddComponent<VolumetricFog>();
				}
			}
		}

		private void OnBecameInvisible()
		{
			if (fog != null)
			{
				fog.enabled = false;
			}
		}

		private void OnBecameVisible()
		{
			if (fog != null)
			{
				fog.enabled = true;
			}
		}
	}
}