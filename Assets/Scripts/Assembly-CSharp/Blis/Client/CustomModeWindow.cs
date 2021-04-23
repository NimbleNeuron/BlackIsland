using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CustomModeWindow : BaseWindow
	{
		private const int maxJoinUserCount = 18;


		private const int maxJoinObserverCount = 5;


		private readonly Color impossibleColor = new Color(0.447f, 0.447f, 0.447f);


		private readonly Color possibleColor = new Color(1f, 0.624f, 0.212f);


		private Button btnClose;


		private Button btnGameStart;


		private CanvasAlphaTweener chattingCanvasAlphaTweener;


		private ChattingUI chattingUI;


		private CanvasAlphaTweener closeBtnCanvasAlphaTweener;


		private CustomObserverSlot[] customObserverSlots;


		private CustomUserSlot[] customUserSlots;


		private string gameStartImpossibleKey = "";


		private GameObject guard;


		private CanvasAlphaTweener guideCanvasAlphaTweener;


		private PositionTweener guidePositionTweener;


		private CanvasAlphaTweener observerCanvasAlphaTweener;


		private PositionTweener observerPositionTweener;


		private GameObject restrictAcc;


		private PositionTweener startBtnPositionTweener;


		private CanvasGroup teamListDuo;


		private CanvasGroup teamListSolo;


		private CanvasGroup teamListTrio;


		private CanvasAlphaTweener titleCanvasAlphaTweener;


		private PositionTweener titlePositionTweener;


		private Toggle toggleRestrictAcc;


		private Text txtGameStart;


		private Text txtJoinObserverCount;


		private Text txtJoinUserCount;


		private LnText txtModeType;


		private Text txtModeTypeNum;


		private Text txtRoomCode;


		private LnText txtServerRegion;


		public ChattingUI ChattingUI => chattingUI;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameObject gameObject = GameUtil.Bind<Transform>(this.gameObject, "Content").gameObject;
			titleCanvasAlphaTweener = GameUtil.Bind<CanvasAlphaTweener>(gameObject, "Title/Content");
			titlePositionTweener = GameUtil.Bind<PositionTweener>(gameObject, "Title/Content");
			observerCanvasAlphaTweener = GameUtil.Bind<CanvasAlphaTweener>(gameObject, "UserList/ObserverList");
			observerPositionTweener = GameUtil.Bind<PositionTweener>(gameObject, "UserList/ObserverList");
			guideCanvasAlphaTweener = GameUtil.Bind<CanvasAlphaTweener>(gameObject, "PlayGuide");
			guidePositionTweener = GameUtil.Bind<PositionTweener>(gameObject, "PlayGuide");
			startBtnPositionTweener = GameUtil.Bind<PositionTweener>(gameObject, "BTN_GameStart");
			closeBtnCanvasAlphaTweener = GameUtil.Bind<CanvasAlphaTweener>(gameObject, "BTN_Close");
			chattingCanvasAlphaTweener = GameUtil.Bind<CanvasAlphaTweener>(gameObject, "Chatting");
			txtModeTypeNum = GameUtil.Bind<Text>(titlePositionTweener.gameObject, "TXT_ModeTypeNum");
			txtModeType = GameUtil.Bind<LnText>(titlePositionTweener.gameObject, "TXT_ModeType");
			txtRoomCode = GameUtil.Bind<Text>(titlePositionTweener.gameObject, "TXT_RoomCode");
			txtJoinUserCount = GameUtil.Bind<Text>(titlePositionTweener.gameObject, "JoinUser/TXT_JoinUserCount");
			txtServerRegion = GameUtil.Bind<LnText>(titlePositionTweener.gameObject, "ServerRegion/TXT_ServerRegion");
			teamListSolo = GameUtil.Bind<CanvasGroup>(gameObject, "UserList/TeamList_Solo");
			teamListSolo.alpha = 0f;
			teamListDuo = GameUtil.Bind<CanvasGroup>(gameObject, "UserList/TeamList_Duo");
			teamListDuo.alpha = 0f;
			teamListTrio = GameUtil.Bind<CanvasGroup>(gameObject, "UserList/TeamList_Trio");
			teamListTrio.alpha = 0f;
			txtJoinObserverCount =
				GameUtil.Bind<Text>(observerCanvasAlphaTweener.gameObject, "BaseBg/Title/TXT_ObserverCount");
			customObserverSlots = observerCanvasAlphaTweener.gameObject.GetComponentsInChildren<CustomObserverSlot>();
			restrictAcc = GameUtil.Bind<Transform>(gameObject, "PlayGuide/RestrictAcc").gameObject;
			toggleRestrictAcc = GameUtil.Bind<Toggle>(restrictAcc, "Toggle_RestrictAcc");
			toggleRestrictAcc.onValueChanged.AddListener(delegate(bool isOn) { ClickToggleRestrictAcc(isOn); });
			btnClose = GameUtil.Bind<Button>(gameObject, "BTN_Close");
			btnClose.onClick.AddListener(delegate { Exit(); });
			btnGameStart = GameUtil.Bind<Button>(gameObject, "BTN_GameStart");
			txtGameStart = GameUtil.Bind<Text>(btnGameStart.gameObject, "TXT_GameStart");
			btnGameStart.onClick.AddListener(delegate { GameStart(); });
			chattingUI = GameUtil.Bind<ChattingUI>(gameObject, "Chatting");
			chattingUI.SetIsLockInput(IsLockInput);
			chattingUI.SetSendEvent(SendChatMessage);
			chattingUI.SetWaitInactive(false);
			guard = transform.FindRecursively("Guard").gameObject;
			MonoBehaviourInstance<MatchingService>.inst.onCloseCustomModeWindowEvent -= OnCloseWindow;
			MonoBehaviourInstance<MatchingService>.inst.onCloseCustomModeWindowEvent += OnCloseWindow;
		}


		public void SetModeType(MatchingTeamMode matchingTeamMode)
		{
			switch (matchingTeamMode)
			{
				case MatchingTeamMode.Solo:
					txtModeTypeNum.text = "1";
					txtModeType.text = Ln.Get("솔로");
					ShowUserList(teamListSolo, true);
					ShowUserList(teamListDuo, false);
					ShowUserList(teamListTrio, false);
					return;
				case MatchingTeamMode.Duo:
					txtModeTypeNum.text = "2";
					txtModeType.text = Ln.Get("듀오");
					ShowUserList(teamListSolo, false);
					ShowUserList(teamListDuo, true);
					ShowUserList(teamListTrio, false);
					return;
				case MatchingTeamMode.Squad:
					txtModeTypeNum.text = "3";
					txtModeType.text = Ln.Get("스쿼드");
					ShowUserList(teamListSolo, false);
					ShowUserList(teamListDuo, false);
					ShowUserList(teamListTrio, true);
					return;
				default:
					return;
			}
		}


		public void InitCustomGameRoom(bool isOwner)
		{
			GlobalUserData.matchingMode = MatchingMode.Custom;
			GlobalUserData.matchingTeamMode = MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.teamMode;
			SetModeType(MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.teamMode);
			txtRoomCode.text = string.Concat("[ ", Ln.Get("방코드"), " : ",
				MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.customGameKey, " ]");
			txtServerRegion.text = Ln.Get(LnType.ServerRegion, GlobalUserData.matchingRegion.ToString());
			new RuleText(restrictAcc, "금지구역 가속 허용");
			if (isOwner)
			{
				toggleRestrictAcc.isOn = true;
			}

			chattingUI.Active(true);
		}


		public void UpdateCustomGameRoom()
		{
			customUserSlots = GetUserSlots(MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.teamMode);
			int num = 0;
			List<CustomGameSlot> slotList = MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.slotList;
			for (int i = 0; i < customUserSlots.Length; i++)
			{
				if (slotList.Count > i)
				{
					if (slotList[i].IsEmptySlot())
					{
						customUserSlots[i].SetEmpty(slotList[i]);
					}
					else
					{
						num++;
						customUserSlots[i].SetCustomUser(slotList[i]);
					}

					customUserSlots[i].ShowAddAIButton(false);
					customUserSlots[i].OnRequestMoveSlot -= MoveSlot;
					customUserSlots[i].OnRequestMoveSlot += MoveSlot;
					customUserSlots[i].OnRequestAddAI -= AddAI;
					customUserSlots[i].OnRequestAddAI += AddAI;
					customUserSlots[i].OnRequestMoveToObserverUser -= OnRequestMoveToObserverUser;
					customUserSlots[i].OnRequestMoveToObserverUser += OnRequestMoveToObserverUser;
					customUserSlots[i].OnRequestBanUser -= BanUser;
					customUserSlots[i].OnRequestBanUser += BanUser;
				}
			}

			int num2 = 0;
			List<CustomGameSlot> observerSlotList =
				MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.observerSlotList;
			for (int j = 0; j < customObserverSlots.Length; j++)
			{
				if (observerSlotList.Count > j)
				{
					if (observerSlotList[j].IsEmptySlot())
					{
						customObserverSlots[j].SetEmpty(observerSlotList[j]);
					}
					else
					{
						num2++;
						customObserverSlots[j].SetCustomUser(observerSlotList[j]);
					}

					customObserverSlots[j].OnRequestMoveSlot -= MoveToObserverSlot;
					customObserverSlots[j].OnRequestMoveSlot += MoveToObserverSlot;
					customObserverSlots[j].OnRequestBanUser -= BanUser;
					customObserverSlots[j].OnRequestBanUser += BanUser;
				}
			}

			txtJoinUserCount.text = string.Format("<color=#FFAE39>{0}</Color> / {1}", num, 18);
			txtJoinObserverCount.text = string.Format("<color=#FFAE39>{0}</Color> / {1}", num2, 5);
			SetGroupUI(MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.teamMode);
			UpdateUIState(MonoBehaviourInstance<MatchingService>.inst.IsCustomGameRoomOwner(Lobby.inst.User.UserNum));
			UpdateAccelerationRestrictUI();
		}


		private void OnRequestMoveToObserverUser(long userNum)
		{
			MonoBehaviourInstance<MatchingService>.inst.MoveUserToObserverSlotCustomGameRoom(userNum);
		}


		private CustomUserSlot[] GetUserSlots(MatchingTeamMode matchingTeamMode)
		{
			switch (matchingTeamMode)
			{
				case MatchingTeamMode.None:
				case MatchingTeamMode.Solo:
					return customUserSlots = teamListSolo.GetComponentsInChildren<CustomUserSlot>(true);
				case MatchingTeamMode.Duo:
					return customUserSlots = teamListDuo.GetComponentsInChildren<CustomUserSlot>(true);
				case MatchingTeamMode.Squad:
					return customUserSlots = teamListTrio.GetComponentsInChildren<CustomUserSlot>(true);
				default:
					return null;
			}
		}


		private void ShowUserList(CanvasGroup canvasGroup, bool show)
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = show;
			canvasGroup.blocksRaycasts = show;
			PositionTweener component = canvasGroup.GetComponent<PositionTweener>();
			CanvasAlphaTweener component2 = canvasGroup.GetComponent<CanvasAlphaTweener>();
			canvasGroup.transform.localPosition = component.from;
			component.StopAnimation();
			component2.StopAnimation();
			if (show)
			{
				component.PlayAnimation();
				component2.PlayAnimation();
			}
		}


		private int GetTeamNumber(int slotIndex)
		{
			if (slotIndex == 0)
			{
				return 1;
			}

			MatchingTeamMode teamMode = MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.teamMode;
			if (teamMode <= MatchingTeamMode.Solo)
			{
				return slotIndex + 1;
			}

			if (teamMode - MatchingTeamMode.Duo > 1)
			{
				return -1;
			}

			return slotIndex / (int) MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.teamMode + 1;
		}


		private void SetGroupUI(MatchingTeamMode matchingTeamMode)
		{
			CustomGroup[] array = null;
			switch (matchingTeamMode)
			{
				case MatchingTeamMode.None:
				case MatchingTeamMode.Solo:
					array = teamListSolo.GetComponentsInChildren<CustomGroup>(true);
					break;
				case MatchingTeamMode.Duo:
					array = teamListDuo.GetComponentsInChildren<CustomGroup>(true);
					break;
				case MatchingTeamMode.Squad:
					array = teamListTrio.GetComponentsInChildren<CustomGroup>(true);
					break;
			}

			int num = 1;
			CustomGroup[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].SetTeamNumber(num);
				num++;
			}

			if (MonoBehaviourInstance<MatchingService>.inst.IsCustomGameRoomOwner(Lobby.inst.User.UserNum))
			{
				array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					List<CustomUserSlot> list = array2[i].CustomUserSlots;
					bool flag = false;
					using (List<CustomUserSlot>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.IsUser)
							{
								flag = true;
								break;
							}
						}
					}

					if (!flag)
					{
						foreach (CustomUserSlot customUserSlot in list)
						{
							if (customUserSlot.IsEmpty)
							{
								customUserSlot.ShowAddAIButton(true);
							}
						}
					}
				}
			}
		}


		public void OnPointerEnterMenuHider()
		{
			if (customUserSlots != null)
			{
				CustomUserSlot[] array = customUserSlots;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].HideMenu();
				}
			}

			if (customObserverSlots != null)
			{
				CustomObserverSlot[] array2 = customObserverSlots;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].HideMenu();
				}
			}
		}


		private void ClickToggleRestrictAcc(bool isOn)
		{
			if (MonoBehaviourInstance<MatchingService>.inst.IsCustomGameRoomOwner(Lobby.inst.User.UserNum))
			{
				MonoBehaviourInstance<MatchingService>.inst.UpdateAcceleration(isOn);
			}
		}


		private void UpdateUIState(bool isOwner)
		{
			toggleRestrictAcc.enabled = isOwner;
			UpdateUIGameStartButton(isOwner);
		}


		private void UpdateUIGameStartButton(bool isOwner)
		{
			btnGameStart.gameObject.SetActive(isOwner);
			if (isOwner)
			{
				bool flag = false;
				gameStartImpossibleKey = "";
				if (MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.teamMode == MatchingTeamMode.Solo ||
				    MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.teamMode == MatchingTeamMode.None)
				{
					flag = MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.slotList.Count > 1;
					gameStartImpossibleKey = flag ? "" : "ImPossibleSoloCustomGameStart";
				}
				else
				{
					List<int> list = new List<int>();
					List<int> list2 = new List<int>();
					foreach (CustomGameSlot customGameSlot in MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom
						.slotList)
					{
						int teamNumber = customGameSlot.teamNumber;
						if (!list2.Contains(teamNumber))
						{
							list2.Add(teamNumber);
						}

						if (customGameSlot.isBot && !list.Contains(teamNumber))
						{
							list.Add(teamNumber);
						}
					}

					flag = list2.Count > 1;
					gameStartImpossibleKey = flag ? "" : "ImPossibleTeamCustomGameStart";
					bool flag2 = false;
					foreach (CustomGameSlot customGameSlot2 in MonoBehaviourInstance<MatchingService>.inst
						.CustomGameRoom.slotList)
					{
						if (list.Contains(customGameSlot2.teamNumber) && customGameSlot2.IsEmptySlot())
						{
							flag2 = true;
							break;
						}
					}

					if (flag2)
					{
						flag = false;
						gameStartImpossibleKey = "ImPossibleBotTeamCustomGameStart";
					}
				}

				txtGameStart.color = flag ? possibleColor : impossibleColor;
			}
		}


		private void UpdateAccelerationRestrictUI()
		{
			toggleRestrictAcc.isOn = MonoBehaviourInstance<MatchingService>.inst.CustomGameRoom.isOnAcceleration;
		}


		private bool IsLockInput()
		{
			return false;
		}


		private void SendChatMessage(string chatContent)
		{
			MonoBehaviourInstance<MatchingService>.inst.SendCustomChatting(chatContent);
		}


		public void AddChatting(string nickName, string content)
		{
			chattingUI.AddChatting(nickName, string.Empty, content, false, false, false, true);
		}


		private void MoveSlot(int destSlotIndex)
		{
			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.MatchCompleted)
			{
				return;
			}

			bool flag = true;
			int teamNumber = GetTeamNumber(destSlotIndex);
			foreach (CustomUserSlot customUserSlot in customUserSlots)
			{
				if (customUserSlot.TeamNumber == teamNumber && !customUserSlot.IsEmpty && !customUserSlot.IsUser)
				{
					flag = false;
					break;
				}
			}

			if (flag)
			{
				MonoBehaviourInstance<MatchingService>.inst.MoveSlotCustomGameRoom(destSlotIndex);
			}
		}


		private void MoveToObserverSlot()
		{
			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.MatchCompleted)
			{
				return;
			}

			MonoBehaviourInstance<MatchingService>.inst.MoveObserverSlotCustomGameRoom();
		}


		private void AddAI(int slotIndex)
		{
			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.MatchCompleted)
			{
				return;
			}

			if (!MonoBehaviourInstance<MatchingService>.inst.IsCustomGameRoomOwner(Lobby.inst.User.UserNum))
			{
				return;
			}

			MonoBehaviourInstance<MatchingService>.inst.AddBotCustomGameRoom(slotIndex);
		}


		private void BanUser(long userNum, int slotNum, string nickName, bool isBot)
		{
			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.MatchCompleted)
			{
				return;
			}

			if (!MonoBehaviourInstance<MatchingService>.inst.IsCustomGameRoomOwner(Lobby.inst.User.UserNum))
			{
				return;
			}

			if (Lobby.inst.User.UserNum == userNum)
			{
				return;
			}

			MonoBehaviourInstance<Popup>.inst.Message(Ln.Format("{0}(을)를 추방하시겠습니까?", nickName), new Popup.Button
			{
				type = Popup.ButtonType.Confirm,
				text = Ln.Get("확인"),
				callback = delegate
				{
					if (isBot)
					{
						MonoBehaviourInstance<MatchingService>.inst.RemoveBotCustomGameRoom(slotNum);
					}
					else
					{
						MonoBehaviourInstance<MatchingService>.inst.KickUserCustomGameRoom(userNum);
					}

					OnPointerEnterMenuHider();
				}
			}, new Popup.Button
			{
				type = Popup.ButtonType.Cancel,
				text = Ln.Get("취소"),
				callback = OnPointerEnterMenuHider
			});
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			if (MonoBehaviourInstance<LobbyUI>.inst.CurrentTab == LobbyTab.InventoryTab)
			{
				MonoBehaviourInstance<LobbyUI>.inst.PauseLobbySound();
			}

			teamListSolo.alpha = 0f;
			teamListDuo.alpha = 0f;
			teamListTrio.alpha = 0f;
			PlayOpenAnimation();
			chattingUI.AddSystemChatting(Ln.Get("게임문화개선캠페인"));
		}


		protected override void OnClose()
		{
			base.OnClose();
			chattingUI.EraseChatting();
			if (MonoBehaviourInstance<LobbyUI>.inst.CurrentTab == LobbyTab.InventoryTab)
			{
				MonoBehaviourInstance<LobbyUI>.inst.PlayLobbySound();
			}
		}


		private void PlayOpenAnimation()
		{
			titleCanvasAlphaTweener.StopAnimation();
			titleCanvasAlphaTweener.PlayAnimation();
			titlePositionTweener.StopAnimation();
			titlePositionTweener.PlayAnimation();
			observerCanvasAlphaTweener.StopAnimation();
			observerCanvasAlphaTweener.PlayAnimation();
			observerPositionTweener.StopAnimation();
			observerPositionTweener.PlayAnimation();
			guideCanvasAlphaTweener.StopAnimation();
			guideCanvasAlphaTweener.PlayAnimation();
			guidePositionTweener.StopAnimation();
			guidePositionTweener.PlayAnimation();
			startBtnPositionTweener.StopAnimation();
			startBtnPositionTweener.PlayAnimation();
			closeBtnCanvasAlphaTweener.StopAnimation();
			closeBtnCanvasAlphaTweener.PlayAnimation();
			chattingCanvasAlphaTweener.StopAnimation();
			chattingCanvasAlphaTweener.PlayAnimation();
		}


		public void Exit()
		{
			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.MatchCompleted)
			{
				return;
			}

			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("커스텀 모드 방을 나가시겠습니까?"), new Popup.Button
			{
				type = Popup.ButtonType.Confirm,
				text = Ln.Get("나가기"),
				callback = delegate
				{
					MonoBehaviourInstance<MatchingService>.inst.LeaveCustomGameRoom();
					Close();
				}
			}, new Popup.Button
			{
				type = Popup.ButtonType.Cancel,
				text = Ln.Get("취소")
			});
		}


		private void GameStart()
		{
			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.MatchCompleted)
			{
				return;
			}

			if (!gameStartImpossibleKey.IsEmpty<char>())
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get(gameStartImpossibleKey), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			guard.SetActive(true);
			MonoBehaviourInstance<MatchingService>.inst.StartCustomGame();
		}


		private bool OnCloseWindow()
		{
			if (!IsOpen)
			{
				return false;
			}

			Close();
			return true;
		}


		private class RuleText
		{
			private readonly EventTrigger eventTrigger;


			private readonly GameObject obj;


			private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


			private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


			private readonly string str;

			public RuleText(GameObject obj, string str)
			{
				this.obj = obj;
				this.str = str;
				GameUtil.BindOrAdd<EventTrigger>(this.obj, ref eventTrigger);
				eventTrigger.triggers.Clear();
				onEnterEvent.AddListener(OnPointerEnter);
				onExitEvent.AddListener(OnPointerExit);
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


			private void OnPointerEnter(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get(str ?? ""));
				Vector2 vector = obj.transform.position;
				vector += GameUtil.ConvertPositionOnScreenResolution(0f, 70f);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
			}


			private void OnPointerExit(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}
		}
	}
}