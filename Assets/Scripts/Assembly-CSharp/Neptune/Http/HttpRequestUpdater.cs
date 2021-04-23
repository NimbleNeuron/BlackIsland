using System.Collections;
using Blis.Common;

namespace Neptune.Http
{
	public class HttpRequestUpdater : SingletonMonoBehaviour<HttpRequestUpdater>
	{
		protected override void OnAwakeSingleton()
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
		}

		
		public void UpdateState(HttpRequest request, IEnumerator enumerator)
		{
			base.StartCoroutine(enumerator);
		}
	}
}
