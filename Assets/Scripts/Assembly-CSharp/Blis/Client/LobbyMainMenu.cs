using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LobbyMainMenu : BaseUI, ILnEventHander
	{
		[SerializeField] private Transform tabButtons = default;
		[SerializeField] private Text currentRegionText = default;
		[SerializeField] private Text txt_UserNP = default;
		[SerializeField] private Text txt_UserACoin = default;
		private Button[] buttons = default;
		private CommunityHUD communityHud = default;
		private Button currentButton = default;
		private MatchingRegion currentRegion;
		private MatchingButton matchingButton = default;
		private Button noticeBtn = default;
		private NoticeHUD noticeHud = default;
		private Image noticeRedDot = default;
		private bool openNoticeHud = default;
		private bool updateCurrency = default;
		public MatchingButton MatchingButton => matchingButton;
		public CommunityHUD CommunityHud => communityHud;
		public NoticeHUD NoticeHud => noticeHud;


		private void Update()
		{
			if (updateCurrency)
			{
				txt_UserNP.text = StringUtil.AssetToUnitString(Lobby.inst.User.np);
				txt_UserACoin.text = StringUtil.AssetToUnitString(Lobby.inst.User.aCoin);
				updateCurrency = false;
			}
		}


		private void LateUpdate()
		{
			if (!noticeHud.IsOpen)
			{
				if (openNoticeHud)
				{
					noticeHud.Open();
					openNoticeHud = false;
				}

				return;
			}

			openNoticeHud = false;
			if (!IsMouseButtonUp())
			{
				return;
			}

			if (IsPointerOverUIElement(noticeHud.name))
			{
				return;
			}

			noticeHud.Close();
		}


		public void OnLnDataChange()
		{
			UpdateCurrentRegionLn();
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			matchingButton = GameUtil.Bind<MatchingButton>(gameObject, "MatchingButton");
			communityHud = GameUtil.Bind<CommunityHUD>(gameObject, "CommunityHUD");
			noticeBtn = GameUtil.Bind<Button>(gameObject, "Notice");
			noticeBtn.onClick.AddListener(OnClickNotice);
			noticeRedDot = GameUtil.Bind<Image>(noticeBtn.gameObject, "RedDot");
			noticeRedDot.enabled = false;
			noticeHud = GameUtil.Bind<NoticeHUD>(gameObject, "NoticeHUD");
			buttons = tabButtons.GetComponentsInChildren<Button>();
			Button[] array = buttons;
			for (int i = 0; i < array.Length; i++)
			{
				Button btn = array[i];
				btn.onClick.AddListener(delegate { TabButtonClicked(btn); });
			}

			MonoBehaviourInstance<GameInput>.inst.OnKeyPressed += OnKeyPressed;
		}


		public void UpdateUserCurrency()
		{
			updateCurrency = true;
		}


		public void OnSelectServerRegion(MatchingRegion region)
		{
			currentRegion = region;
			UpdateCurrentRegionLn();
		}


		public void EnableNoticeRedDot(bool enable)
		{
			noticeRedDot.enabled = enable;
		}


		private void OnClickNotice()
		{
			openNoticeHud = true;
		}


		private void OnKeyPressed(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (inputEvent == GameInputEvent.Escape && noticeHud.IsOpen)
			{
				noticeHud.Close();
			}
		}


		private bool IsMouseButtonUp()
		{
			return Input.GetMouseButtonUp(0);
		}


		public void OnClickServerRegion()
		{
			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.Matching)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭 중에는 이용할 수 없습니다"), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.TutorialStart)
			{
				return;
			}

			MonoBehaviourInstance<LobbyUI>.inst.ServerSelectionWindow.ToggleWindow();
		}


		public void OnClickSettingButton()
		{
			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.TutorialStart)
			{
				return;
			}

			MonoBehaviourInstance<LobbyUI>.inst.SettingWindow.ToggleWindow();
		}


		public void OnClickQuit()
		{
			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.TutorialStart)
			{
				return;
			}

			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("게임을 종료하시겠습니까?"), new Popup.Button
			{
				type = Popup.ButtonType.Confirm,
				text = Ln.Get("게임 종료"),
				callback = Application.Quit
			}, new Popup.Button
			{
				type = Popup.ButtonType.Cancel,
				text = Ln.Get("닫기")
			});
		}


		private void OnClickLobbyTab(Button btn)
		{
			LobbyTab lobbyTab;
			if (Enum.TryParse<LobbyTab>(btn.name, out lobbyTab))
			{
				MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(lobbyTab);
			}
		}


		private void UpdateCurrentRegionLn()
		{
			currentRegionText.text = Ln.Get(LnType.ServerRegion, currentRegion.ToString());
		}


		public void SetMatchingButton(LobbyTab lobbyTab)
		{
			if (lobbyTab != LobbyTab.TutorialTab)
			{
				matchingButton.canvasGroup.alpha = 1f;
				matchingButton.canvasGroup.interactable = true;
				matchingButton.canvasGroup.blocksRaycasts = true;
				return;
			}

			matchingButton.canvasGroup.alpha = 0f;
			matchingButton.canvasGroup.interactable = false;
			matchingButton.canvasGroup.blocksRaycasts = false;
		}


		public void OnLobbyStateUpdate(LobbyState lobbyState)
		{
			if (matchingButton.gameObject.activeSelf)
			{
				matchingButton.OnLobbyStateUpdate(lobbyState);
			}
		}


		private void TabButtonClicked(Button btn)
		{
			if (currentButton == btn)
			{
				return;
			}

			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.TutorialStart)
			{
				return;
			}

			if (btn.name.Equals("TutorialTab"))
			{
				if (Lobby.inst.LobbyContext.lobbyState > LobbyState.Ready)
				{
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭 중에는 이용할 수 없습니다"), new Popup.Button
					{
						text = Ln.Get("확인")
					});
					return;
				}
			}
			else if (!Lobby.inst.User.GetTutorialClearState(TutorialType.BasicGuide))
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("필수 튜토리얼 진행후 이용가능합니다."), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			OnClickLobbyTab(btn);
		}


		public void SetTabFocus(LobbyTab lobbyTab)
		{
			foreach (Button button in buttons)
			{
				Component child = button.transform.GetChild(1);
				bool flag = lobbyTab.ToString() == button.name;
				child.gameObject.SetActive(flag);
				if (flag)
				{
					currentButton = button;
				}
			}
		}
	}
}