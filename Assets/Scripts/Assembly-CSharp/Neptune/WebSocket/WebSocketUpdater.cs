using System.Collections;
using Blis.Common;

namespace Neptune.WebSocket
{
	
	public class WebSocketUpdater : SingletonMonoBehaviour<WebSocketUpdater>
	{
		
		protected override void OnAwakeSingleton()
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
		}

		
		public void UpdateState(WebSocket ws, IEnumerator enumerator)
		{
			base.StartCoroutine(enumerator);
		}

		
		private void OnApplicationPause(bool pauseStatus)
		{
			this.PauseCount += 1L;
		}

		
		internal long PauseCount;
	}
}
