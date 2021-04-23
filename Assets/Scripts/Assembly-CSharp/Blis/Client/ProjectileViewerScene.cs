using BIFog;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class ProjectileViewerScene : MonoBehaviour
	{
		public void Awake()
		{
			CreateSight();
		}


		private void CreateSight()
		{
			FogSight fogSight = new GameObject("FogSight").AddComponent<FogSight>();
			fogSight.LoadDefaultSetting(20f);
			MonoBehaviourInstance<FogManager>.inst.SetMyFogSight(fogSight);
		}
	}
}