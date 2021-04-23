using Blis.Common;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LobbyInfoTab : LobbyTabBaseUI, ILobbyTab
	{
		private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


		private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();
		private int currentExp;


		private InfoState currentState;


		private EventTrigger eventTrigger;


		private Image imgEmblem;


		private Image imgExpValue;


		private InfoBattleRecordPage infoBattleRecordPage;


		private InfoLegauePage infoLegauePage;


		private InfoOverviewPage infoOverviewPage;


		private bool isbattleRecordCache;


		private bool isOverviewCache;


		private Toggle menuGameList;


		private Toggle menuLeague;


		private Toggle menuOverview;


		private int sectionExp;


		private Text txtLevelValue;


		private Text txtNickName;


		private Text txtTooltip;


		public void OnOpen(LobbyTab from)
		{
			EnableCanvas(true);
			OpenInfo(currentState);
			SetAccountInfo();
		}


		public TabCloseResult OnClose(LobbyTab to)
		{
			EnableCanvas(false);
			return TabCloseResult.Success;
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			isOverviewCache = false;
			isbattleRecordCache = false;
			infoOverviewPage = GameUtil.Bind<InfoOverviewPage>(gameObject, "InfoOverview");
			infoBattleRecordPage = GameUtil.Bind<InfoBattleRecordPage>(gameObject, "InfoBattleRecord");
			infoLegauePage = GameUtil.Bind<InfoLegauePage>(gameObject, "InfoLeague");
			menuOverview = GameUtil.Bind<Toggle>(gameObject, "Submenu/MenuOverview");
			menuGameList = GameUtil.Bind<Toggle>(gameObject, "Submenu/MenuBattleRecord");
			menuLeague = GameUtil.Bind<Toggle>(gameObject, "Submenu/MenuLeague");
			txtNickName = GameUtil.Bind<Text>(gameObject, "Profile/AccountInfo/TXT_NickName");
			txtLevelValue = GameUtil.Bind<Text>(gameObject, "Profile/AccountInfo/TXT_LevelValue");
			imgExpValue = GameUtil.Bind<Image>(gameObject, "Profile/AccountInfo/IMG_Exp/IMG_ExpValue");
			imgEmblem = GameUtil.Bind<Image>(gameObject, "Profile/AccountInfo/IMG_Emblem");
			txtTooltip = GameUtil.Bind<Text>(gameObject, "Profile/AccountInfo/TXT_ExpToolTip");
			menuOverview.onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(isOn, InfoState.OVERVIEW); });
			menuGameList.onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(isOn, InfoState.GAMELIST); });
			menuLeague.onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(isOn, InfoState.LEAGUE); });
			GameUtil.BindOrAdd<EventTrigger>(imgExpValue.gameObject, ref eventTrigger);
			eventTrigger.triggers.Clear();
			onEnterEvent.AddListener(ExpVarOnPointerEnter);
			onExitEvent.AddListener(ExpVarOnPointerExit);
			eventTrigger.triggers.Add(new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerEnter,
				callback = onEnterEvent
			});
			eventTrigger.triggers.Add(new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerExit,
				callback = onExitEvent
			});
		}


		private void OnToggleChange(bool isOn, InfoState infoState)
		{
			if (isOn && currentState != infoState)
			{
				OpenInfo(infoState);
			}
		}


		private void SetAccountInfo()
		{
			txtNickName.text = Lobby.inst.User.Nickname;
			txtLevelValue.text = string.Format("Lv.{0}", Lobby.inst.User.Level);
			sectionExp = GameDB.user.GetNeedXP(Lobby.inst.User.Level);
			currentExp = sectionExp - Lobby.inst.User.NeedXP;
			imgExpValue.fillAmount = currentExp / (float) sectionExp;
		}


		private void OpenInfo(InfoState currentRankState)
		{
			currentState = currentRankState;
			infoOverviewPage.ClosePage();
			infoBattleRecordPage.ClosePage();
			infoLegauePage.ClosePage();
			switch (currentState)
			{
				case InfoState.OVERVIEW:
					if (isOverviewCache)
					{
						infoOverviewPage.OpenPage();
						return;
					}

					isOverviewCache = true;
					InfoService.GetRankingSeasonTiers(delegate
					{
						InfoService.GetBattleOverview(delegate { infoOverviewPage.OpenPage(); },
							Lobby.inst.User.UserNum,
							InfoService.GetRankingSeasonId(infoOverviewPage.CurrentSeasonIndex));
					});
					return;
				case InfoState.GAMELIST:
					if (isbattleRecordCache)
					{
						infoBattleRecordPage.OpenPage();
						return;
					}

					isbattleRecordCache = true;
					InfoService.GetBattleGames(delegate { infoBattleRecordPage.OpenPage(); }, Lobby.inst.User.UserNum);
					return;
				case InfoState.LEAGUE:
					InfoService.GetUserRankings(delegate { infoLegauePage.OpenPage(); },
						infoLegauePage.CurrentTeamMode);
					return;
				default:
					return;
			}
		}


		private void ExpVarOnPointerEnter(BaseEventData eventData)
		{
			txtTooltip.gameObject.SetActive(true);
			txtTooltip.text = string.Format("{0}/{1}", currentExp, sectionExp);
		}


		private void ExpVarOnPointerExit(BaseEventData eventData)
		{
			txtTooltip.gameObject.SetActive(false);
		}


		private enum InfoState
		{
			OVERVIEW,

			GAMELIST,

			LEAGUE
		}
	}
}