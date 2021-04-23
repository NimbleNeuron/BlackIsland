using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class WatchingHud : BaseUI
	{
		public void SetActive(bool isActive)
		{
			gameObject.SetActive(isActive);
		}


		public void ExitGame()
		{
			MonoBehaviourInstance<ClientService>.inst.UpdateRankingWatchingExit();
			MonoBehaviourInstance<GameClient>.inst.Request(new ReqExitGame());
			SetActive(false);
		}
	}
}