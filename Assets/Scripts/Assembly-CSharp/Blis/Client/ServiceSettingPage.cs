using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class ServiceSettingPage : BasePage
	{
		private GameObject creditButton;


		private GameObject twitchConnectButton;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			creditButton = GameUtil.Bind<Transform>(gameObject, "Credit").gameObject;
			creditButton.SetActive(SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene);
			twitchConnectButton = GameUtil.Bind<Transform>(gameObject, "TwitchConnect").gameObject;
			twitchConnectButton.SetActive(SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene);
		}


		public void CreditButton()
		{
			if (SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
			{
				MonoBehaviourInstance<LobbyUI>.inst.ShowCredit();
			}
		}


		public void TwitchConnectButton()
		{
			if (SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
			{
				Application.OpenURL(string.Format("{0}/connect/twitch?userNum={1}", ApiConstants.RootHttpsUrl,
					Lobby.inst.User.UserNum));
			}
		}


		public void TwitchDisconnectButton()
		{
			if (SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
			{
				Application.OpenURL("https://www.twitch.tv/settings/connections");
			}
		}
	}
}