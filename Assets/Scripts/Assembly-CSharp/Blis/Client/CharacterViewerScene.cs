using System.Collections;
using BIFog;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class CharacterViewerScene : MonoBehaviourInstance<CharacterViewerScene>
	{
		protected override void _Awake()
		{
			StartCoroutine(StartDBLoad());
			Singleton<SoundControl>.inst.Init();
		}


		private IEnumerator StartDBLoad()
		{
			yield return SingletonMonoBehaviour<GameDBLoader>.inst.LoadCache(GameConstants.GetDataCacheFilePath());
			CreateSight();
		}


		private void CreateSight()
		{
			FogSight fogSight = new GameObject("FogSight").AddComponent<FogSight>();
			fogSight.LoadDefaultSetting(10f);
			MonoBehaviourInstance<FogManager>.inst.SetMyFogSight(fogSight);
		}
	}
}