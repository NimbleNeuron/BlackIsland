using UnityEngine;

namespace Blis.Client
{
	public class LoadingCreator : MonoBehaviour
	{
		[SerializeField] private GameObject loadingPrefab = default;

		private void Awake()
		{
			if (LoadingView.inst == null)
			{
				GameObject gameObject = Instantiate<GameObject>(loadingPrefab);
				LoadingView loadingView = gameObject != null ? gameObject.GetComponent<LoadingView>() : null;
				if (loadingView != null)
				{
					LoadingView.inst = loadingView;
					DontDestroyOnLoad(gameObject);
				}
			}
		}
	}
}