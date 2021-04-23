using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/In-Game Resolution Monitor", 30)]
	public class IngameResolutionMonitor : MonoBehaviour
	{
		private void Update()
		{
			ResolutionMonitor.Update();
		}


		private void OnEnable()
		{
			SceneManager.sceneLoaded += SceneLoaded;
		}


		private void OnDisable()
		{
			SceneManager.sceneLoaded -= SceneLoaded;
		}


		public static GameObject Create()
		{
			GameObject gameObject = new GameObject("IngameResolutionMonitor");
			gameObject.AddComponent<IngameResolutionMonitor>();
			DontDestroyOnLoad(gameObject);
			return gameObject;
		}


		private void SceneLoaded(Scene scene, LoadSceneMode mode)
		{
			ResolutionMonitor.SetResolutionDirty();
			ResolutionMonitor.Update();
		}
	}
}