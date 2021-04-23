using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class LobbyMainTab : LobbyTabBaseUI, ILobbyTab
	{
		[SerializeField] private LobbyCommunityLinkUI lobbyCommunityLink = default;


		[SerializeField] private LobbyAccountInfo lobbyAccountInfo = default;


		[SerializeField] private LobbyDailyMission lobbyDailyMission = default;


		[SerializeField] private LobbyBanner lobbyBanner = default;


		[SerializeField] private GameObject imgKakaoPcIcon = default;


		public void OnOpen(LobbyTab from)
		{
			EnableCanvas(true);
			MonoBehaviourInstance<LobbyUI>.inst.MainMenu.CommunityHud.ShowFriendViewInHome();
		}


		public TabCloseResult OnClose(LobbyTab to)
		{
			EnableCanvas(false);
			MonoBehaviourInstance<LobbyUI>.inst.MainMenu.CommunityHud.HideFriendViewInHome();
			return TabCloseResult.Success;
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
		}


		public void OnLobbyStateUpdate(LobbyState lobbyState) { }


		public void OnNicknameChange(string nickname)
		{
			lobbyAccountInfo.SetNickName(nickname);
		}


		public void SetAccountInfo(string nickName, int level, int needExp)
		{
			lobbyAccountInfo.SetNickName(nickName);
			lobbyAccountInfo.SetLevel(level);
			lobbyAccountInfo.SetExp(needExp);
		}


		public void SetBenefitByKakaoPcCafe(bool benefitByKakaoPcCafe)
		{
			imgKakaoPcIcon.gameObject.SetActive(benefitByKakaoPcCafe);
		}


		public void SetDailyMissions()
		{
			lobbyDailyMission.SetDailyMissions();
		}


		public void InitBanners()
		{
			lobbyBanner.InitBanners();
		}

		private void Ref()
		{
			Reference.Use(lobbyCommunityLink);
		}
	}
}