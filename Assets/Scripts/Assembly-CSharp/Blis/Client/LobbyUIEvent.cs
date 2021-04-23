using Blis.Common.Utils;

namespace Blis.Client
{
	public class LobbyUIEvent
	{
		private readonly LobbyCharacterTab characterTab;


		private readonly LobbyDictionaryTab dictionaryTab;


		private readonly LobbyFavoritesTab favoritesTab;


		private readonly LobbyUI lobbyUI;


		private readonly LobbyMainTab mainTab;


		private readonly LobbyShopTab shopTab;

		public LobbyUIEvent(LobbyUI ui)
		{
			lobbyUI = ui;
			mainTab = lobbyUI.GetLobbyTab<LobbyMainTab>(LobbyTab.MainTab);
			characterTab = lobbyUI.GetLobbyTab<LobbyCharacterTab>(LobbyTab.InventoryTab);
			dictionaryTab = lobbyUI.GetLobbyTab<LobbyDictionaryTab>(LobbyTab.DictionaryTab);
			favoritesTab = lobbyUI.GetLobbyTab<LobbyFavoritesTab>(LobbyTab.FavoritesTab);
			shopTab = MonoBehaviourInstance<LobbyUI>.inst.GetLobbyTab<LobbyShopTab>(LobbyTab.ShopTab);
		}


		public void OnLobbyStateUpdate(LobbyState lobbyState)
		{
			mainTab.OnLobbyStateUpdate(lobbyState);
			lobbyUI.MainMenu.OnLobbyStateUpdate(lobbyState);
			lobbyUI.SettingWindow.OnLobbyStateUpdate(lobbyState);
		}


		public void OnNicknameChange(string nickname)
		{
			mainTab.OnNicknameChange(nickname);
		}


		public void SetAccountInfo(string nickName, int level, int needExp)
		{
			mainTab.SetAccountInfo(nickName, level, needExp);
		}


		public void SetBenefitByKakaoPcCafe(bool benefitByKakaoPcCafe)
		{
			mainTab.SetBenefitByKakaoPcCafe(benefitByKakaoPcCafe);
		}


		public void SetDailyMissions()
		{
			mainTab.SetDailyMissions();
		}


		public void InitBanners()
		{
			mainTab.InitBanners();
		}


		public void OnChangeMatchingRegion(MatchingRegion region)
		{
			MonoBehaviourInstance<LobbyUI>.inst.MainMenu.OnSelectServerRegion(region);
			MonoBehaviourInstance<LobbyUI>.inst.ServerSelectionWindow.OnSelectServerRegion(region);
		}


		public void OnChangeAccelerateChina(bool isOn)
		{
			MonoBehaviourInstance<LobbyUI>.inst.ServerSelectionWindow.OnChangeAccelerateChina(isOn);
		}


		public void OnClickShowCharacterInfo(int characterCode)
		{
			characterTab.OnClickShowCharacterInfo(characterCode);
		}


		public void OnClickShowSkinInfo(int skinCode)
		{
			characterTab.OnClickShowSkinInfo(skinCode);
		}


		public void OnClickShowTwitchDrops()
		{
			MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(LobbyTab.ShopTab);
			shopTab.OnClickShowTwitchDrops();
		}


		public void OnUpdateMySteamInfo()
		{
			MonoBehaviourInstance<LobbyUI>.inst.MainMenu.MatchingButton.OnUpdateMySteamInfo();
		}
	}
}