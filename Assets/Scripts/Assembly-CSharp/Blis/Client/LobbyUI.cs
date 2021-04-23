using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	[DefaultExecutionOrder(-8999)]
	public class LobbyUI : MonoBehaviourInstance<LobbyUI>
	{
		private readonly List<BaseUI> initializeList = new List<BaseUI>();

		private GameObject bringGuard;
		private CharacterGuideWindow characterGuideWindow;
		private CharacterSelectWindow characterSelectWindow;
		private CreditUI creditUI;
		private LobbyTab currentTab;
		private CustomModeSelectionWindow customModeSelectionWindow;
		private CustomModeWindow customModeWindow;
		private DevLoginWindow devLoginWindow;
		private Text guidance;
		private CanvasGroup hudGroup;
		private GameInput input;

		private bool isInitedLobby;
		private bool isOpenTierChangeWindow;

		private JoinCustomGameRoomWindow joinCustomGameRoomWindow;
		private float lastSecretMatchingTime;

		private Dictionary<LobbyTab, ILobbyTab> lobbyTabs = new Dictionary<LobbyTab, ILobbyTab>();
		private LobbyMainMenu mainMenu;
		private MatchingConfirmPopup matchingConfirmPopup;
		private NicknameSettingWindow nicknameSettingWindow;
		private Transform overlayUI;
		private ReconnectPopup reconnectPopup;
		private ResultScoreBoardWindow resultScoreBoardWindow;
		private RewardWindow rewardWindow;
		private Transform serverIndicator;
		private Coroutine serverIndicatorTimer;
		private ServerSelectionWindow serverSelectionWindow;
		private SettingWindow settingWindow;
		private ShopProductWindow shopProductWindow;
		private TierChangeWindow tierChangeWindow;
		private TierInfoWindow tierInfoWindow;
		private ToastMessageUI toastMessage;
		private Tooltip tooltip;
		private LobbyUIEvent uiEvent;

		public LobbyUIEvent UIEvent => uiEvent;
		public LobbyMainMenu MainMenu => mainMenu;
		public NicknameSettingWindow NicknameSettingWindow => nicknameSettingWindow;
		public SettingWindow SettingWindow => settingWindow;
		public ServerSelectionWindow ServerSelectionWindow => serverSelectionWindow;
		public CharacterGuideWindow CharacterGuideWindow => characterGuideWindow;
		public ShopProductWindow ShopProductWindow => shopProductWindow;
		public DevLoginWindow DevLoginWindow => devLoginWindow;
		public ResultScoreBoardWindow ResultScoreBoardWindow => resultScoreBoardWindow;
		public RewardWindow RewardWindow => rewardWindow;
		public CustomModeWindow CustomModeWindow => customModeWindow;
		public CustomModeSelectionWindow CustomModeSelectionWindow => customModeSelectionWindow;
		public JoinCustomGameRoomWindow JoinCustomGameRoomWindow => joinCustomGameRoomWindow;
		public CharacterSelectWindow CharacterSelectWindow => characterSelectWindow;
		public TierInfoWindow TierInfoWindow => tierInfoWindow;
		public TierChangeWindow TierChangeWindow => tierChangeWindow;
		public Transform OverlayUI => overlayUI;
		public Tooltip Tooltip => tooltip;
		public ToastMessageUI ToastMessage => toastMessage;
		public ReconnectPopup ReconnectPopup => reconnectPopup;
		public MatchingConfirmPopup MatchingConfirmPopup => matchingConfirmPopup;

		public LobbyTab CurrentTab => currentTab;

		
		public LobbyTab SetCurrentTab {
			set => currentTab = value;
		}


		public Transform ServerIndicator => serverIndicator;


		private void Update()
		{
			if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
			{
				WindowController.FlashStopWindowEx();
			}
		}


		private void CreateCredit()
		{
			if (creditUI == null)
			{
				GameObject original = Resources.Load<GameObject>("Prefabs/Credit");
				creditUI = Instantiate<GameObject>(original, overlayUI).GetComponent<CreditUI>();
			}
		}


		public T GetLobbyTab<T>(LobbyTab lobbyTab) where T : class, ILobbyTab
		{
			return (T) lobbyTabs[lobbyTab];
		}


		protected override void _Awake()
		{
			base._Awake();
			
			SingletonMonoBehaviour<ResourceManager>.inst.UnloadAll();
			SingletonMonoBehaviour<ResourceManager>.inst.LoadInLobby();
			
			Bind();
			
			input.OnKeyPressed += OnKeyPressed;
			input.OnKeyRelease += OnKeyRelease;
			input.OnMousePressed += OnMousePressed;
			
			RequestDelegate inst = SingletonMonoBehaviour<RequestDelegate>.inst;
			inst.OnStartWaitRequest = (RequestDelegate.StartWaitRequest) Delegate.Remove(inst.OnStartWaitRequest,
				new RequestDelegate.StartWaitRequest(OnStartWaitRequest));
			
			RequestDelegate inst2 = SingletonMonoBehaviour<RequestDelegate>.inst;
			inst2.OnStartWaitRequest = (RequestDelegate.StartWaitRequest) Delegate.Combine(inst2.OnStartWaitRequest,
				new RequestDelegate.StartWaitRequest(OnStartWaitRequest));
			
			RequestDelegate inst3 = SingletonMonoBehaviour<RequestDelegate>.inst;
			inst3.OnStopWaitRequest = (RequestDelegate.StopWaitRequest) Delegate.Remove(inst3.OnStopWaitRequest,
				new RequestDelegate.StopWaitRequest(OnStopWaitRequest));
			
			RequestDelegate inst4 = SingletonMonoBehaviour<RequestDelegate>.inst;
			inst4.OnStopWaitRequest = (RequestDelegate.StopWaitRequest) Delegate.Combine(inst4.OnStopWaitRequest,
				new RequestDelegate.StopWaitRequest(OnStopWaitRequest));
			
			MonoBehaviourInstance<MatchingService>.inst.onCompleteMatchingEvent -= OnCompleteMatching;
			MonoBehaviourInstance<MatchingService>.inst.onCompleteMatchingEvent += OnCompleteMatching;
			MonoBehaviourInstance<MatchingService>.inst.RegisterServerEventHandler(PopupServerErrorWithCommunity);
			
			hudGroup.blocksRaycasts = false;
			hudGroup.alpha = 0f;
			GetComponentsInChildren<BaseUI>(true, initializeList);
			
			foreach (BaseUI baseUI in initializeList)
			{
				baseUI.AwakeUI();
			}

			UISceneContext.currentSceneState = UISceneContext.SceneState.UIAwaked;
		}


		protected override void _OnDestroy()
		{
			base._OnDestroy();
			
			UISceneContext.currentSceneState = UISceneContext.SceneState.Loading;
			
			Lobby inst = Lobby.inst;
			if (inst != null)
			{
				inst.ResetUser();
			}

			RequestDelegate inst2 = SingletonMonoBehaviour<RequestDelegate>.inst;
			inst2.OnStartWaitRequest = (RequestDelegate.StartWaitRequest) Delegate.Remove(inst2.OnStartWaitRequest,
				new RequestDelegate.StartWaitRequest(OnStartWaitRequest));
			RequestDelegate inst3 = SingletonMonoBehaviour<RequestDelegate>.inst;
			inst3.OnStopWaitRequest = (RequestDelegate.StopWaitRequest) Delegate.Remove(inst3.OnStopWaitRequest,
				new RequestDelegate.StopWaitRequest(OnStopWaitRequest));
			if (MonoBehaviourInstance<MatchingService>.inst != null)
			{
				MonoBehaviourInstance<MatchingService>.inst.onCompleteMatchingEvent -= OnCompleteMatching;
			}

			input.OnMousePressed -= OnMousePressed;
		}


		public void InitLobbyUI()
		{
			foreach (BaseUI baseUI in initializeList)
			{
				baseUI.StartUI();
			}
			
			UISceneContext.currentSceneState = UISceneContext.SceneState.UIStarted;
			
			hudGroup.alpha = 1f;
			hudGroup.blocksRaycasts = true;
			isInitedLobby = true;
			
			bool tutorialClearState = Lobby.inst.User.GetTutorialClearState(TutorialType.BasicGuide);
			
			// 튜토리얼 탭이 아닐 때
			if (Singleton<LocalSetting>.inst.LobbyTab != LobbyTab.TutorialTab)
			{
				LobbyTab lobbyTab = tutorialClearState ? LobbyTab.MainTab : LobbyTab.TutorialTab;
				SetLobbyTab(lobbyTab);
				
				if (lobbyTab == LobbyTab.MainTab)
				{
					if (GlobalUserData.dicPlayerResults.Count > 0)
					{
						resultScoreBoardWindow.Open();
						int level = Lobby.inst.User.Level;
						int myLevel = GlobalUserData.myLevel;
						if (level > myLevel)
						{
							rewardWindow.Open();
							rewardWindow.SetLevelUpReward(myLevel, level, RewardType.LevelUp, Lobby.inst.User.GainXP);
							GlobalUserData.myLevel = level;
						}
						else
						{
							TierChangeCheckProcess();
						}
					}

					NicknameCheckProcess();
				}

				bool flag = lobbyTab == LobbyTab.InventoryTab;
				SingletonMonoBehaviour<LobbyCharacterStation>.inst.SetCameraRendering3D(flag);
			}
			else
			{
				// 튜토리얼 탭일 때
				lobbyTabs[LobbyTab.MainTab].OnClose(LobbyTab.TutorialTab);
				SetLobbyTab(LobbyTab.TutorialTab);
				if (GlobalUserData.tutorialClearFlag)
				{
					if (GlobalUserData.tutorialClearCode == 1)
					{
						MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("메뉴오픈"), new Popup.Button
						{
							text = Ln.Get("확인"),
							callback = delegate { ShowTutorialReward(); }
						});
					}
					else
					{
						ShowTutorialReward();
					}
				}

				MainMenu.CommunityHud.HideFriendViewInHome();
			}

			if (tutorialClearState)
			{
				MainMenu.CommunityHud.ShowGroup();
				return;
			}

			MainMenu.CommunityHud.HideGroup();
			MainMenu.CommunityHud.HideFriendViewInHome();
		}


		private void ShowTutorialReward()
		{
			TutorialRewardData tutorialRewardData =
				GameDB.tutorial.GetTutorialRewardData(GlobalUserData.tutorialClearCode);
			rewardWindow.Open();
			rewardWindow.AddReward(new RewardInfo(RewardType.TutotialClear, new List<RewardItemInfo>
			{
				rewardWindow.GetTutorialRewardItemInfo(tutorialRewardData)
			}));
			rewardWindow.ShowReward();
			UIEvent.SetAccountInfo(Lobby.inst.User.Nickname, Lobby.inst.User.Level, Lobby.inst.User.NeedXP);
			GlobalUserData.tutorialClearFlag = false;
			GlobalUserData.tutorialClearCode = 0;
		}


		private void Bind()
		{
			GameUtil.BindOrAdd<GameInput>(gameObject, ref input);
			hudGroup = GameUtil.Bind<CanvasGroup>(gameObject, "LobbyHUD");
			lobbyTabs = new Dictionary<LobbyTab, ILobbyTab>
			{
				{
					LobbyTab.MainTab,
					GameUtil.Bind<LobbyMainTab>(gameObject, "LobbyHUD/MainTab")
				},
				{
					LobbyTab.InventoryTab,
					GameUtil.Bind<LobbyCharacterTab>(gameObject, "LobbyHUD/InventoryTab")
				},
				{
					LobbyTab.FavoritesTab,
					GameUtil.Bind<LobbyFavoritesTab>(gameObject, "LobbyHUD/FavoritesTab")
				},
				{
					LobbyTab.DictionaryTab,
					GameUtil.Bind<LobbyDictionaryTab>(gameObject, "LobbyHUD/DictionaryTab")
				},
				{
					LobbyTab.TutorialTab,
					GameUtil.Bind<LobbyTutorialTab>(gameObject, "LobbyHUD/TutorialTab")
				},
				{
					LobbyTab.ShopTab,
					GameUtil.Bind<LobbyShopTab>(gameObject, "LobbyHUD/ShopTab")
				},
				{
					LobbyTab.InfoTab,
					GameUtil.Bind<LobbyInfoTab>(gameObject, "LobbyHUD/InfoTab")
				}
			};
			mainMenu = GameUtil.Bind<LobbyMainMenu>(gameObject, "LobbyHUD/MainMenu");
			nicknameSettingWindow = GameUtil.Bind<NicknameSettingWindow>(gameObject, "LobbyWindow/NicknameSetting");
			settingWindow = GameUtil.Bind<SettingWindow>(gameObject, "LobbyWindow/SettingWindow");
			serverSelectionWindow = GameUtil.Bind<ServerSelectionWindow>(gameObject, "LobbyWindow/ServerSelect");
			characterGuideWindow = GameUtil.Bind<CharacterGuideWindow>(gameObject, "LobbyWindow/CharacterGuideWindow");
			shopProductWindow = GameUtil.Bind<ShopProductWindow>(gameObject, "LobbyWindow/ShopProductWindow");
			resultScoreBoardWindow =
				GameUtil.Bind<ResultScoreBoardWindow>(gameObject, "LobbyWindow/ResultScoreboardWindow");
			rewardWindow = GameUtil.Bind<RewardWindow>(gameObject, "LobbyWindow/RewardWindow");
			customModeWindow = GameUtil.Bind<CustomModeWindow>(gameObject, "LobbyWindow/CustomModeWindow");
			customModeSelectionWindow =
				GameUtil.Bind<CustomModeSelectionWindow>(gameObject, "LobbyWindow/CustomModeSelectionWindow");
			joinCustomGameRoomWindow =
				GameUtil.Bind<JoinCustomGameRoomWindow>(gameObject, "LobbyWindow/JoinCustomGameRoomWindow");
			characterSelectWindow =
				GameUtil.Bind<CharacterSelectWindow>(gameObject, "LobbyWindow/CharacterSelectWindow");
			serverIndicator = GameUtil.Bind<Transform>(gameObject, "ServerIndicator");
			tierInfoWindow = GameUtil.Bind<TierInfoWindow>(gameObject, "LobbyWindow/TierInfoWindow");
			tierChangeWindow = GameUtil.Bind<TierChangeWindow>(gameObject, "LobbyWindow/TierChangeWindow");
			overlayUI = GameUtil.Bind<Transform>(gameObject, "Overlay");
			tooltip = GameUtil.Bind<Tooltip>(overlayUI.gameObject, "Tooltip");
			toastMessage = GameUtil.Bind<ToastMessageUI>(overlayUI.gameObject, "ToastMessage");
			uiEvent = new LobbyUIEvent(this);
			devLoginWindow = GameUtil.Bind<DevLoginWindow>(gameObject, "DevLogin");
			bringGuard = transform.FindRecursively("BringGuard").gameObject;
			reconnectPopup = GameUtil.Bind<ReconnectPopup>(gameObject, "Popup/ReconnectPopup");
			matchingConfirmPopup = GameUtil.Bind<MatchingConfirmPopup>(gameObject, "Popup/MatchingConfirmPopup");
		}


		public void SetLobbyTab(LobbyTab lobbyTab)
		{
			if (lobbyTab != currentTab)
			{
				if (currentTab == LobbyTab.None || lobbyTabs[currentTab].OnClose(lobbyTab) == TabCloseResult.Success)
				{
					LobbyTab from = currentTab;
					SetCurrentTab = lobbyTab;
					lobbyTabs[currentTab].OnOpen(from);
				}

				mainMenu.SetTabFocus(currentTab);
				mainMenu.SetMatchingButton(currentTab);
				Singleton<LocalSetting>.inst.SetLobbyTab(currentTab);
			}
		}


		private void OnKeyPressed(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (!isInitedLobby)
			{
				return;
			}

			if (inputEvent != GameInputEvent.Escape)
			{
				switch (inputEvent)
				{
					case GameInputEvent.ChatActive:
					case GameInputEvent.ChatActive2:
						if (characterSelectWindow.IsOpen)
						{
							characterSelectWindow.ChattingUI.EnterChat(false);
							return;
						}

						if (customModeWindow.IsOpen)
						{
							customModeWindow.ChattingUI.EnterChat(false);
							return;
						}

						if (Lobby.inst.User.GetTutorialClearState(TutorialType.BasicGuide))
						{
							MainMenu.CommunityHud.ChattingUi.EnterChat(false);
						}

						break;
					case GameInputEvent.AllChatActive:
						if (Lobby.inst.User.GetTutorialClearState(TutorialType.BasicGuide))
						{
							MainMenu.CommunityHud.ChattingUi.EnterChat(false);
						}

						break;
					case GameInputEvent.MaxChatWindow:
						if (Lobby.inst.User.GetTutorialClearState(TutorialType.BasicGuide))
						{
							MainMenu.CommunityHud.ChattingUi.SetMaxChatSize();
							if (!MainMenu.CommunityHud.ChattingUi.IsActive())
							{
								MainMenu.CommunityHud.ChattingUi.Active(false);
							}
						}

						break;
					default:
						switch (inputEvent)
						{
							case GameInputEvent.NormalMatchingSolo:
								StartSecretMatching(MatchingMode.Normal, MatchingTeamMode.Solo);
								return;
							case GameInputEvent.NormalMatchingDuo:
								StartSecretMatching(MatchingMode.Normal, MatchingTeamMode.Duo);
								return;
							case GameInputEvent.NormalMatchingSquad:
								StartSecretMatching(MatchingMode.Normal, MatchingTeamMode.Squad);
								return;
							case GameInputEvent.RankMatchingSolo:
								StartSecretMatching(MatchingMode.Rank, MatchingTeamMode.Solo);
								return;
							case GameInputEvent.RankMatchingDuo:
								StartSecretMatching(MatchingMode.Rank, MatchingTeamMode.Duo);
								return;
							case GameInputEvent.RankMatchingSquad:
								StartSecretMatching(MatchingMode.Rank, MatchingTeamMode.Squad);
								break;
							default:
								return;
						}

						break;
				}
			}
			else
			{
				if (creditUI != null)
				{
					DestroyCredit();
					return;
				}

				if (characterSelectWindow.IsOpen && characterSelectWindow.ChattingUI.IsFocus)
				{
					characterSelectWindow.ChattingUI.DeactivateInput();
					return;
				}

				if (customModeWindow.IsOpen && customModeWindow.ChattingUI.IsFocus)
				{
					customModeWindow.ChattingUI.DeactivateInput();
					return;
				}

				if (MainMenu.CommunityHud.ChattingUi.IsFocus)
				{
					MainMenu.CommunityHud.ChattingUi.DeactivateInput();
					return;
				}

				if (tierChangeWindow.IsOpen && tierChangeWindow.IsPlayAnimation())
				{
					return;
				}

				if (currentTab == LobbyTab.FavoritesTab)
				{
					LobbyFavoritesTab lobbyTab = GetLobbyTab<LobbyFavoritesTab>(currentTab);
					if (lobbyTab.FavoriteEditorPage.IsOpenGuide())
					{
						lobbyTab.FavoriteEditorPage.OnClickGuideClose();
						return;
					}
				}

				if (BaseWindow.FocusedWindow == null)
				{
					settingWindow.Open();
					return;
				}

				if (!BaseWindow.FocusedWindow.IgnoreEscapeInputWindow())
				{
					if (BaseWindow.FocusedWindow.name.Equals("CustomModeWindow"))
					{
						BaseWindow.FocusedWindow.CustomWindowClose();
						return;
					}

					BaseWindow.FocusedWindow.Close();
				}
			}
		}


		private void OnKeyRelease(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (!isInitedLobby)
			{
				return;
			}

			if (inputEvent == GameInputEvent.MaxChatWindow &&
			    Lobby.inst.User.GetTutorialClearState(TutorialType.BasicGuide))
			{
				MainMenu.CommunityHud.ChattingUi.SetMinChatSize();
			}
		}


		private void OnMousePressed(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			bool flag = isInitedLobby;
		}


		public void NicknameCheckProcess()
		{
			if (!Lobby.inst.User.GetTutorialClearState(TutorialType.BasicGuide))
			{
				return;
			}

			if (string.IsNullOrEmpty(Lobby.inst.User.Nickname))
			{
				inst.NicknameSettingWindow.Open();
			}
		}


		public void TierChangeCheckProcess()
		{
			if (isOpenTierChangeWindow)
			{
				return;
			}

			if (Lobby.inst.User.LastBatchMode || Lobby.inst.User.BeforeTierChangeType != RankingTierChangeType.None &&
				Lobby.inst.User.BeforeTierType != Lobby.inst.User.AfterTierType)
			{
				isOpenTierChangeWindow = true;
				inst.TierChangeWindow.Open();
				return;
			}

			if (Lobby.inst.User.AfterTierChangeType != RankingTierChangeType.None)
			{
				isOpenTierChangeWindow = true;
				string text = Lobby.inst.User.AfterTierChangeType == RankingTierChangeType.Promotion
					? Ln.Get("승급 안내 메시지")
					: Ln.Get("강등 안내 메시지");
				Popup inst = MonoBehaviourInstance<Popup>.inst;
				string msg = text;
				Popup.Button[] array = new Popup.Button[1];
				int num = 0;
				Popup.Button button = new Popup.Button();
				button.type = Popup.ButtonType.Confirm;
				button.text = Ln.Get("확인");
				button.callback = delegate { };
				array[num] = button;
				inst.Message(msg, array);
			}
		}


		public void PlayLobbySound()
		{
			SingletonMonoBehaviour<LobbyCharacterStation>.inst.EnableAnimators(true);
			Singleton<SoundControl>.inst.ResumeSfx("Lobby");
			Singleton<SoundControl>.inst.ResumeVoice("Lobby");
		}


		public void PauseLobbySound()
		{
			SingletonMonoBehaviour<LobbyCharacterStation>.inst.EnableAnimators(false);
			Singleton<SoundControl>.inst.PauseSfx("Lobby");
			Singleton<SoundControl>.inst.PauseVoice("Lobby");
		}


		public void StopLobbySound()
		{
			SingletonMonoBehaviour<LobbyCharacterStation>.inst.EnableAnimators(false);
			Singleton<SoundControl>.inst.StopVoiceAudio("Lobby");
			Singleton<SoundControl>.inst.StopSfxAudio("Lobby");
		}


		public void ShowBringGuard(bool show)
		{
			bringGuard.SetActive(show);
		}


		private void OnStartWaitRequest()
		{
			if (serverIndicatorTimer == null)
			{
				serverIndicatorTimer = this.StartThrowingCoroutine(StartServerIndicatorTimer(),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][StartServerIndicatorTimer] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
			}
		}


		private IEnumerator StartServerIndicatorTimer()
		{
			yield return new WaitForSeconds(1f);
			if (serverIndicator != null && !serverIndicator.gameObject.activeSelf)
			{
				serverIndicator.gameObject.SetActive(true);
			}
		}


		private void OnStopWaitRequest()
		{
			if (serverIndicatorTimer != null)
			{
				StopCoroutine(serverIndicatorTimer);
				serverIndicatorTimer = null;
			}

			if (serverIndicator != null && serverIndicator.gameObject.activeSelf)
			{
				serverIndicator.gameObject.SetActive(false);
			}
		}


		private void PopupServerErrorWithCommunity()
		{
			Popup inst = MonoBehaviourInstance<Popup>.inst;
			string msg = Ln.Get(BlisWebSocketError.OutOfService.ToString());
			Popup.Button[] array = new Popup.Button[2];
			array[0] = new Popup.Button
			{
				type = Popup.ButtonType.Confirm,
				text = Ln.Get("확인")
			};
			int num = 1;
			Popup.Button button = new Popup.Button();
			button.type = Popup.ButtonType.Cancel;
			button.text = Ln.Get("기본 커뮤니티");
			button.callback = delegate { Application.OpenURL(Ln.Get("기본 커뮤니티 공지 링크")); };
			array[num] = button;
			inst.Message(msg, array);
		}


		public void ShowCredit()
		{
			DestroyCredit();
			CreateCredit();
			creditUI.Active(true);
		}


		public void DestroyCredit()
		{
			if (creditUI != null)
			{
				creditUI.Active(false);
				Destroy(creditUI.gameObject);
			}
		}


		private void StartSecretMatching(MatchingMode matchingMode, MatchingTeamMode matchingTeamMode)
		{
			if (CurrentTab == LobbyTab.TutorialTab)
			{
				return;
			}

			if (!Lobby.inst.User.GetTutorialClearState(TutorialType.BasicGuide))
			{
				return;
			}

			if (Time.time < lastSecretMatchingTime)
			{
				return;
			}

			if (MonoBehaviourInstance<LobbyService>.inst.StartSecretMatching(matchingMode, matchingTeamMode,
				ImpossibleExistCallback))
			{
				lastSecretMatchingTime = Time.time + 1f;
			}
		}


		private void ImpossibleExistCallback(string content, List<bool> modeActiveStates)
		{
			Popup inst = MonoBehaviourInstance<Popup>.inst;
			Popup.Button[] array = new Popup.Button[2];
			int num = 0;
			Popup.Button button = new Popup.Button();
			button.text = Ln.Get("자세히 보기");
			button.type = Popup.ButtonType.Link;
			button.callback = delegate { Application.OpenURL(Ln.Get("CBT스케줄링크")); };
			array[num] = button;
			array[1] = new Popup.Button
			{
				text = Ln.Get("확인"),
				type = Popup.ButtonType.Confirm
			};
			inst.Message(content, array);
		}


		private void OnCompleteMatching()
		{
			DestroyCredit();
		}
	}
}