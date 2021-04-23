using Blis.Common;
using Blis.Common.Utils;
using Steamworks;
using UnityEngine;

namespace Blis.Client
{
	public class CommunityHUD : BaseUI
	{
		private CanvasGroup canvasGroup;


		private CanvasAlphaTweener canvasTweener;


		private ChattingUI chattingUI;


		private CommunityGroup communityGroup;


		private CommunityFriendPopup friendPopup;


		private CommunityFriendView friendView;


		private CommunityFriendViewHome friendViewInHome;


		private CSteamID invitedSteamIDLobby;


		private CSteamID invitedSteamIDUser;


		private CommunityInviteGroupPopup inviteGroupPopup;


		private bool isOpen;


		public ChattingUI ChattingUi => chattingUI;


		protected override void OnDestroy()
		{
			base.OnDestroy();
			CommunityService.inviteLobbyEvent -= OnInviteLobbyEvent;
			CommunityService.updateChatMsgEvent -= OnUpdateChatMsgEvent;
			CommunityService.onUpdatePersonaState -= OnUpdateFriendInfo;
			CommunityService.onUpdateRichPresence -= OnUpdateFriendInfo;
			CommunityService.updateGroupEvent -= OnUpdateGroupEvent;
			SingletonMonoBehaviour<KoreaGameGradeUtil>.inst.onPlayTimeNotice -= OnPlayTimeNotice;
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			canvasGroup = GameUtil.Bind<CanvasGroup>(gameObject, "BackShade");
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			canvasTweener = GameUtil.Bind<CanvasAlphaTweener>(gameObject, "BackShade");
			friendViewInHome = GameUtil.Bind<CommunityFriendViewHome>(gameObject, "CommunityFriendView_Home");
			friendView = GameUtil.Bind<CommunityFriendView>(gameObject, "CommunityFriendView");
			communityGroup = GameUtil.Bind<CommunityGroup>(gameObject, "CommunityGroup");
			inviteGroupPopup = GameUtil.Bind<CommunityInviteGroupPopup>(gameObject, "CommunityInviteGroupPopup");
			friendPopup = GameUtil.Bind<CommunityFriendPopup>(gameObject, "CommunityFriendPopup");
			chattingUI = GameUtil.Bind<ChattingUI>(gameObject, "Chatting");
			chattingUI.SetWaitInactiveSeconds(5f, 5f);
			chattingUI.SetIsLockInput(IsLockInput);
			chattingUI.SetSendEvent(SendChatMessage);
			chattingUI.SetLobbyChat(true);
			CommunityService.inviteLobbyEvent += OnInviteLobbyEvent;
			CommunityService.updateChatMsgEvent += OnUpdateChatMsgEvent;
			CommunityService.onUpdatePersonaState += OnUpdateFriendInfo;
			CommunityService.onUpdateRichPresence += OnUpdateFriendInfo;
			CommunityService.updateGroupEvent += OnUpdateGroupEvent;
			SingletonMonoBehaviour<KoreaGameGradeUtil>.inst.onPlayTimeNotice += OnPlayTimeNotice;
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			communityGroup.SetClickEventCallback(OnClickGroupMemberSlot);
			communityGroup.UpdateGroup();
			inviteGroupPopup.SetButtonEvents(InviteAccept, InviteReject);
		}


		private void OnInviteLobbyEvent()
		{
			if (inviteGroupPopup == null)
			{
				return;
			}

			if (inviteGroupPopup.IsOpen)
			{
				return;
			}

			LobbyInvite_t lobbyInvite_t = default;
			if (!CommunityService.GetInviteRequest(ref lobbyInvite_t))
			{
				return;
			}

			invitedSteamIDUser = new CSteamID(lobbyInvite_t.m_ulSteamIDUser);
			invitedSteamIDLobby = new CSteamID(lobbyInvite_t.m_ulSteamIDLobby);
			string friendSteamName = CommunityService.GetFriendSteamName(invitedSteamIDUser);
			string friendNickName = CommunityService.GetFriendNickName(invitedSteamIDUser);
			inviteGroupPopup.Open(invitedSteamIDUser, friendSteamName, friendNickName);
		}


		public void InviteAccept()
		{
			inviteGroupPopup.Close();
			CommunityService.AcceptInvite(invitedSteamIDLobby, invitedSteamIDUser);
		}


		public void InviteReject()
		{
			inviteGroupPopup.Close();
			if (CommunityService.IsRemainInviteRequest())
			{
				OnInviteLobbyEvent();
			}
		}


		private void OnUpdateChatMsgEvent()
		{
			if (Lobby.inst == null || Lobby.inst.User == null)
			{
				return;
			}

			if (!Lobby.inst.User.GetTutorialClearState(TutorialType.BasicGuide))
			{
				return;
			}

			ChattingUI chattingUI = this.chattingUI;
			if (chattingUI == null)
			{
				return;
			}

			chattingUI.UpdateChatting(CommunityService.GetLobbyChatMsg());
		}


		private void OnUpdateFriendInfo(CSteamID steamID)
		{
			if (CommunityService.IsMe(steamID))
			{
				MonoBehaviourInstance<LobbyUI>.inst.UIEvent.OnUpdateMySteamInfo();
			}

			CommunityFriendViewHome communityFriendViewHome = friendViewInHome;
			if (communityFriendViewHome != null)
			{
				communityFriendViewHome.UpdateFriendInfo(steamID);
			}

			CommunityFriendView communityFriendView = friendView;
			if (communityFriendView != null)
			{
				communityFriendView.UpdateFriendInfo(steamID);
			}

			CommunityGroup communityGroup = this.communityGroup;
			if (communityGroup != null)
			{
				communityGroup.UpdateFriendInfo(steamID);
			}

			CommunityFriendPopup communityFriendPopup = friendPopup;
			if (communityFriendPopup == null)
			{
				return;
			}

			communityFriendPopup.UpdatePopup(steamID);
		}


		private void OnUpdateGroupEvent()
		{
			CommunityGroup communityGroup = this.communityGroup;
			if (communityGroup == null)
			{
				return;
			}

			communityGroup.UpdateGroup();
		}


		private bool IsLockInput()
		{
			return false;
		}


		private void SendChatMessage(string chatContent)
		{
			if (string.IsNullOrEmpty(chatContent))
			{
				return;
			}

			CommunityService.SendChatContent(chatContent);
		}


		public void OnClickGroupMemberSlot(CSteamID steamID)
		{
			if (!steamID.IsValid())
			{
				CommunityService.RefreshFriendInfo();
				Show();
				return;
			}

			ShowFriendPopup(steamID);
		}


		public void Show()
		{
			if (isOpen)
			{
				friendView.UpdateFriends(false);
				return;
			}

			isOpen = true;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			canvasTweener.from = canvasGroup.alpha;
			canvasTweener.to = 1f;
			canvasTweener.PlayAnimation();
			friendView.Show();
		}


		public void Hide()
		{
			if (!isOpen)
			{
				return;
			}

			isOpen = false;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			canvasTweener.from = canvasGroup.alpha;
			canvasTweener.to = 0f;
			canvasTweener.PlayAnimation();
			friendView.Hide();
		}


		public void ShowGroup()
		{
			communityGroup.Show();
			ChattingUI chattingUI = this.chattingUI;
			if (chattingUI == null)
			{
				return;
			}

			chattingUI.gameObject.SetActive(true);
		}


		public void HideGroup()
		{
			communityGroup.Hide();
			ChattingUI chattingUI = this.chattingUI;
			if (chattingUI == null)
			{
				return;
			}

			chattingUI.gameObject.SetActive(false);
		}


		public void ShowFriendPopup(CSteamID steamID)
		{
			friendPopup.Open(steamID);
		}


		public void HideFriendPopup()
		{
			friendPopup.Close();
		}


		public void ShowFriendViewInHome()
		{
			friendViewInHome.Show();
		}


		public void HideFriendViewInHome()
		{
			friendViewInHome.Hide();
		}


		public void AddSystemChatting(string message)
		{
			if (Lobby.inst == null || Lobby.inst.User == null)
			{
				return;
			}

			if (!Lobby.inst.User.GetTutorialClearState(TutorialType.BasicGuide))
			{
				return;
			}

			ChattingUI chattingUI = this.chattingUI;
			if (chattingUI == null)
			{
				return;
			}

			chattingUI.AddSystemChatting(message);
		}


		private void OnPlayTimeNotice(int hour)
		{
			AddSystemChatting(Ln.Format("접속시간경과안내", hour));
		}
	}
}