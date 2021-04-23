using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class CharacterSelectWindow : BaseWindow
	{
		private ChattingUI chattingUI;


		private bool isLockInput;


		private CharacterSelectView myView;


		private CharacterSelectObserverView observerView;


		private CharacterSelectPlayerView playerView;


		public ChattingUI ChattingUI => chattingUI;

		private bool IsLockInput()
		{
			return isLockInput;
		}


		public override bool IgnoreEscapeInputWindow()
		{
			return true;
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			playerView = GameUtil.Bind<CharacterSelectPlayerView>(gameObject, "PlayerView");
			observerView = GameUtil.Bind<CharacterSelectObserverView>(gameObject, "ObserverView");
			chattingUI = GameUtil.Bind<ChattingUI>(gameObject, "Chatting");
			chattingUI.SetIsLockInput(IsLockInput);
			chattingUI.SetSendEvent(SendChatMessage);
			chattingUI.SetWaitInactive(false);
			MonoBehaviourInstance<MatchingService>.inst.onCharacterSelectEvent -= OnCharacterSelect;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterSelectObserverEvent -= OnCharacterSelectObserver;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterMyPickEvent -= OnCharacterMyPick;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterPickEvent -= OnCharacterPick;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterPickObserverEvent -= OnCharacterPickObserver;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterCancelMyPickEvent -= OnCharacterCancelMyPick;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterCancelPickEvent -= OnCharacterCancelPick;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterCancelPickObserverEvent -=
				OnCharacterCancelPickObserver;
			MonoBehaviourInstance<MatchingService>.inst.onSkinSelectEvent -= OnSkinSelect;
			MonoBehaviourInstance<MatchingService>.inst.onSkinSelectObserverEvent -= OnSkinSelectObserver;
			MonoBehaviourInstance<MatchingService>.inst.onChatMessageEvent -= OnChatMessage;
			MonoBehaviourInstance<MatchingService>.inst.onStandByEvent -= OnStandBy;
			MonoBehaviourInstance<MatchingService>.inst.onStartGameEvent -= OnStartGame;
			MonoBehaviourInstance<MatchingService>.inst.onCloseCharacterSelectWindowEvent -= OnCloseWindow;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterSelectEvent += OnCharacterSelect;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterSelectObserverEvent += OnCharacterSelectObserver;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterMyPickEvent += OnCharacterMyPick;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterPickEvent += OnCharacterPick;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterPickObserverEvent += OnCharacterPickObserver;
			MonoBehaviourInstance<MatchingService>.inst.onSkinSelectEvent += OnSkinSelect;
			MonoBehaviourInstance<MatchingService>.inst.onSkinSelectObserverEvent += OnSkinSelectObserver;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterCancelMyPickEvent += OnCharacterCancelMyPick;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterCancelPickEvent += OnCharacterCancelPick;
			MonoBehaviourInstance<MatchingService>.inst.onCharacterCancelPickObserverEvent +=
				OnCharacterCancelPickObserver;
			MonoBehaviourInstance<MatchingService>.inst.onChatMessageEvent += OnChatMessage;
			MonoBehaviourInstance<MatchingService>.inst.onStandByEvent += OnStandBy;
			MonoBehaviourInstance<MatchingService>.inst.onStartGameEvent += OnStartGame;
			MonoBehaviourInstance<MatchingService>.inst.onCloseCharacterSelectWindowEvent += OnCloseWindow;
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			if (MonoBehaviourInstance<LobbyUI>.inst.CurrentTab == LobbyTab.InventoryTab)
			{
				MonoBehaviourInstance<LobbyUI>.inst.PauseLobbySound();
			}

			Singleton<SoundControl>.inst.PlayBGM("BSER_BGM_StrategyMap", true);
			chattingUI.AddSystemChatting(Ln.Get("게임문화개선캠페인"));
			isLockInput = false;
			playerView.Close();
			observerView.Close();
			if (MonoBehaviourInstance<MatchingService>.inst.IsObserver)
			{
				myView = observerView;
				chattingUI.gameObject.SetActive(false);
				isLockInput = true;
			}
			else
			{
				myView = playerView;
			}

			myView.Open();
			myView.SetOnClickExit(OnClickExit);
			myView.SetFinishTimerAction(OnFinishTimer);
		}


		private void OnFinishTimer() { }


		protected override void OnClose()
		{
			base.OnClose();
			if (MonoBehaviourInstance<LobbyUI>.inst.CurrentTab == LobbyTab.InventoryTab)
			{
				MonoBehaviourInstance<LobbyUI>.inst.PlayLobbySound();
			}

			Singleton<SoundControl>.inst.PlayBGM("BGM_Lobby", true);
			chattingUI.EraseChatting();
			myView.Close();
		}


		public void AddChatting(string nickName, string content)
		{
			chattingUI.AddChatting(nickName, string.Empty, content, false, false, false, true);
		}


		public void SetIsInputLock(bool isLockInput)
		{
			this.isLockInput = isLockInput;
		}


		private void SendChatMessage(string chatContent)
		{
			if (IsLockInput())
			{
				return;
			}

			MonoBehaviourInstance<MatchingService>.inst.SendChatting(chatContent);
		}


		private void OnCharacterSelect(int beforeCharacterCode, int beforeStartingDataCode,
			MatchingService.MatchingUser userInfo, bool isSinglePlay)
		{
			if (beforeCharacterCode != userInfo.CharacterCode)
			{
				myView.CharacterSelect(userInfo.CharacterCode, userInfo.StartingDataCode, userInfo.IsMe);
			}
			else if (userInfo.IsMe && beforeStartingDataCode != userInfo.StartingDataCode)
			{
				myView.WeaponSelect(userInfo.StartingDataCode);
			}

			if (!isSinglePlay)
			{
				myView.UpdateMyTeam(userInfo);
			}
		}


		private void OnCharacterSelectObserver(int teamNumber, int beforeCharacterCode, int beforeStartingDataCode,
			MatchingService.MatchingUser userInfo)
		{
			if (beforeCharacterCode != userInfo.CharacterCode)
			{
				myView.CharacterSelect(teamNumber, userInfo.UserNum, userInfo.CharacterCode, userInfo.StartingDataCode);
			}

			if (beforeStartingDataCode != userInfo.StartingDataCode)
			{
				myView.WeaponSelect(teamNumber, userInfo.UserNum, userInfo.StartingDataCode);
			}
		}


		private void OnCharacterMyPick(int characterCode, int skinCode, bool isSinglePlay)
		{
			myView.PickMyCharacter(characterCode, skinCode, isSinglePlay);
		}


		private void OnCharacterPick(MatchingService.MatchingUser userInfo)
		{
			myView.UpdateMyTeam(userInfo);
			myView.PickCharacter(userInfo);
		}


		private void OnCharacterPickObserver(int teamNumber, MatchingService.MatchingUser userInfo)
		{
			myView.UpdateMyTeam(teamNumber, userInfo);
			myView.PickCharacter(teamNumber, userInfo);
		}


		private void OnSkinSelect(int beforeSkinCode, MatchingService.MatchingUser userInfo, bool isSinglePlay)
		{
			if (beforeSkinCode != userInfo.SkinCode)
			{
				myView.SkinSelect(userInfo.CharacterCode, userInfo.SkinCode, userInfo.IsMe);
			}

			if (!isSinglePlay)
			{
				myView.UpdateMyTeam(userInfo);
			}
		}


		private void OnSkinSelectObserver(int teamNumber, int beforeSkinCode, MatchingService.MatchingUser userInfo)
		{
			if (beforeSkinCode != userInfo.SkinCode)
			{
				myView.SkinSelect(teamNumber, userInfo);
			}
		}


		private void OnCharacterCancelMyPick(MatchingService.MatchingUser userInfo)
		{
			myView.CharacterCancelMyPick(userInfo.CharacterCode);
		}


		private void OnCharacterCancelPick(MatchingService.MatchingUser userInfo)
		{
			myView.CharacterCancelPick(userInfo.CharacterCode);
			myView.UpdateMyTeam(userInfo);
		}


		private void OnCharacterCancelPickObserver(int teamNumber, MatchingService.MatchingUser userInfo)
		{
			myView.CharacterCancelPick(teamNumber, userInfo);
		}


		private void OnChatMessage(MatchingChatMessage msg)
		{
			if (IsOpen)
			{
				AddChatting(msg.sender, SingletonMonoBehaviour<SwearWordManager>.inst.CheckAndReplaceChat(msg.message));
			}
		}


		private void OnStandBy()
		{
			myView.StandBy();
		}


		private void OnStartGame()
		{
			SetIsInputLock(true);
		}


		private bool OnCloseWindow()
		{
			if (!IsOpen)
			{
				return false;
			}

			WindowController.FlashStopWindowEx();
			Close();
			return true;
		}


		private void OnClickExit()
		{
			Popup inst = MonoBehaviourInstance<Popup>.inst;
			string msg = Ln.Get("커스텀 모드 방을 나가시겠습니까?");
			Popup.Button[] array = new Popup.Button[2];
			int num = 0;
			Popup.Button button = new Popup.Button();
			button.type = Popup.ButtonType.Confirm;
			button.text = Ln.Get("나가기");
			button.callback = delegate { MonoBehaviourInstance<MatchingService>.inst.LeaveCharacterSelectPhase(); };
			array[num] = button;
			array[1] = new Popup.Button
			{
				type = Popup.ButtonType.Cancel,
				text = Ln.Get("취소")
			};
			inst.Message(msg, array);
		}
	}
}