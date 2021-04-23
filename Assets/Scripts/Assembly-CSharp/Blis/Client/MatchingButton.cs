using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class MatchingButton : BaseUI
	{
		public delegate void MatchingModeSelectionEvent();


		public CanvasGroup canvasGroup = default;

		[SerializeField] private Image matchingBg = default;
		[SerializeField] private Text matchingTimer = default;
		[SerializeField] private Text matchingNotification = default;
		[SerializeField] private Text matchingMode = default;
		[SerializeField] private Image buttonBlock = default;
		[SerializeField] private Button openModeButton = default;
		[SerializeField] private Button matchStartButton = default;
		[SerializeField] private Button matchStopButton = default;
		[SerializeField] private CanvasGroup modalPanel = default;
		[SerializeField] private LobbyModeSelectionUI modeSelection = default;
		[SerializeField] private Text txtMatchingStart = default;
		[SerializeField] private GameObject openModefxGlow = default;
		[SerializeField] private GameObject matchStartfxGlow = default;
		[SerializeField] private LobbyBannerItem playGuideBanner = default;
		private Coroutine buttonBlockCoroutine;
		private CanvasAlphaTweener canvasAlphaTweener;
		private bool isOpen;
		private LobbyState lobbyState;
		private PositionTweener matchingBgTweener;
		private PositionTweener modeSelectionTweener;
		private Coroutine routine;
		private LnText txtOpenMode = default;

		public bool IsOpen => isOpen;

		protected override void OnDestroy()
		{
			base.OnDestroy();
			CommunityService.updateGroupEvent -= OnUpdateGroupEvent;
			CommunityService.enterLobbyEvent -= OnEnterLobbyEvent;
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			matchStartButton.onClick.AddListener(StartMatching);
			matchStopButton.onClick.AddListener(StopMatching);
			GameUtil.Bind<PositionTweener>(matchingBg.gameObject, ref matchingBgTweener);
			GameUtil.Bind<CanvasAlphaTweener>(modalPanel.gameObject, ref canvasAlphaTweener);
			modeSelectionTweener = GameUtil.Bind<PositionTweener>(modalPanel.gameObject, "ModelSelection");
			txtOpenMode = GameUtil.Bind<LnText>(openModeButton.gameObject, "Label");
			modeSelection.OnGameModeSelect += GameModeSelect;
			CommunityService.updateGroupEvent += OnUpdateGroupEvent;
			CommunityService.enterLobbyEvent += OnEnterLobbyEvent;
			playGuideBanner.gameObject.SetActive(false);
		}


		public void OnUpdateMySteamInfo()
		{
			if (!CommunityService.HasLobby())
			{
				return;
			}

			if (CommunityService.IsLobbyOwner())
			{
				BlockOpenModeButton(false);
				BlockExitButton(false);
				return;
			}

			BlockOpenModeButton(true);
			BlockExitButton(true);
		}


		private void OnUpdateGroupEvent()
		{
			if (!CommunityService.HasLobby() || CommunityService.IsLobbyOwner())
			{
				BlockOpenModeButton(false);
				BlockExitButton(false);
			}
		}


		private void OnEnterLobbyEvent()
		{
			if (!CommunityService.IsLobbyOwner())
			{
				BlockOpenModeButton(true);
				BlockExitButton(true);
				if (IsOpen)
				{
					HideModeSelection();
				}
			}
		}


		private void StartMatching()
		{
			bool hasTeam = CommunityService.HasLobby();
			bool isLobbyOwner = CommunityService.IsLobbyOwner();
			if (MonoBehaviourInstance<LobbyService>.inst.LobbyState == LobbyState.Matching)
			{
				if (!hasTeam)
				{
					MonoBehaviourInstance<LobbyService>.inst.CancelSecretMatching();
					MonoBehaviourInstance<MatchingService>.inst.StopMatchingWithPopup();
					return;
				}

				if (isLobbyOwner)
				{
					MonoBehaviourInstance<LobbyService>.inst.CancelSecretMatching();
					MonoBehaviourInstance<MatchingService>.inst.StopMatchingWithPopup();
					return;
				}
			}

			string selectedMode = modeSelection.GetSelectedMode();
			Log.H(selectedMode);
			
			switch (selectedMode)
			{
				case "SingleEasy": OnSingle(BotDifficulty.EASY);
					break;
				case "SingleNormal":OnSingle(BotDifficulty.NORMAL);
					break; 
				case "SingleHard":OnSingle(BotDifficulty.HARD);
					break; 
				case "Solo": OnMatch(MatchingTeamMode.Solo);
					break;
				case "Duo": OnMatch(MatchingTeamMode.Duo);
					break;
				case "Squad": OnMatch(MatchingTeamMode.Squad);
					break;
			}
			
			void OnSingle(BotDifficulty difficulty)
			{
				GlobalUserData.botDifficulty = difficulty;
				
				if (hasTeam)
				{
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("팀구성 중에는 이용할 수 없습니다."), new Popup.Button
					{
						text = Ln.Get("확인")
					});
					return;
				}

				GlobalUserData.matchingMode = MatchingMode.Test;
				GlobalUserData.matchingTeamMode = MatchingTeamMode.Solo;
				
				if (!MonoBehaviourInstance<LobbyClient>.inst.CheckRegionSchedule(GlobalUserData.matchingMode,
					modeSelection.GetSelectedMode(), modeSelection.GetSelectedModeIndex(), ImpossibleExistCallback))
				{
					return;
				}

				HideModeSelection(false);
				MonoBehaviourInstance<LobbyService>.inst.CancelSecretMatching();
				LobbyService.UpdateLobbyState(LobbyState.MatchCompleted);
				MonoBehaviourInstance<MatchingService>.inst.StartPracticeMatching();
			}

			void OnMatch(MatchingTeamMode teamMode)
			{
				if (CheckRankPenalty())
				{
					return;
				}

				if (hasTeam)
				{
					if (!isLobbyOwner)
					{
						return;
					}

					if ((int)teamMode < CommunityService.LobbyMemberCount)
					{
						return;
					}

					if (!CommunityService.IsAllMemberReady())
					{
						return;
					}
				}

				GlobalUserData.matchingMode = modeSelection.SelectedMatchingMode;
				GlobalUserData.matchingTeamMode = teamMode;
				if (!MonoBehaviourInstance<LobbyClient>.inst.CheckRegionSchedule(GlobalUserData.matchingMode,
					modeSelection.GetSelectedMode(), modeSelection.GetSelectedModeIndex(), ImpossibleExistCallback))
				{
					return;
				}

				HideModeSelection(false);
				MonoBehaviourInstance<LobbyService>.inst.CancelSecretMatching();
				MonoBehaviourInstance<MatchingService>.inst.BeforeMatching(CommunityService.GetMemberUserNumList());
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
				type = Popup.ButtonType.Confirm,
				callback = delegate
				{
					modeSelection.SetActiveMode(modeActiveStates);
					modeSelection.DefaultSelectMode(modeActiveStates);
					modeSelection.SetRankTierChange();
				}
			};
			inst.Message(content, array);
		}


		private bool CheckRankPenalty()
		{
			return modeSelection.SelectedMatchingMode == MatchingMode.Rank && Lobby.inst.CheckRankPenalty();
		}


		private void StopMatching()
		{
			if (CommunityService.HasLobby() && !CommunityService.IsLobbyOwner())
			{
				return;
			}

			MonoBehaviourInstance<MatchingService>.inst.StopMatching();
		}


		private void GameModeSelect(string modeName)
		{
			bool flag;
			SetMatchingRegionSchedule(out flag);
			if (modeName == "Custom")
			{
				matchStartButton.enabled = false;
				txtMatchingStart.color = GameConstants.UIColor.disableButtonTextColor;
				matchStartfxGlow.SetActive(false);
				return;
			}

			if (!(modeName == "Single") && !(modeName == "Normal") && !(modeName == "Team") && !(modeName == "Rank"))
			{
				return;
			}

			matchStartButton.enabled = flag;
			txtMatchingStart.color =
				flag ? GameConstants.UIColor.enableButtonTextColor : GameConstants.UIColor.disableButtonTextColor;
			matchStartfxGlow.SetActive(flag);
		}


		public void ClickGameStart()
		{
			if (CommunityService.HasLobby() && !CommunityService.IsLobbyOwner())
			{
				return;
			}

			if (MonoBehaviourInstance<LobbyService>.inst.LobbyState == LobbyState.Matching)
			{
				MonoBehaviourInstance<MatchingService>.inst.StopMatchingWithPopup();
				return;
			}

			if (playGuideBanner != null)
			{
				bool active = Lobby.inst.User.Level < 10 && BannerService.ApplyPlayBannerUI(playGuideBanner);
				playGuideBanner.gameObject.SetActive(active);
			}

			isOpen = true;
			canvasAlphaTweener.StopAnimation();
			modalPanel.blocksRaycasts = true;
			modalPanel.alpha = 1f;
			openModeButton.gameObject.SetActive(false);
			matchStartButton.gameObject.SetActive(true);
			BlockMatchStartButton(modeSelectionTweener.speed);
			modeSelectionTweener.StopAnimation();
			modeSelectionTweener.PlayAnimation();
			GameModeSelect(GlobalUserData.matchingMode.ToString());
			matchingTimer.text = null;
		}


		private void SetMatchingRegionSchedule(out bool isModeOpen)
		{
			List<bool> matchingRegionSchedule =
				MonoBehaviourInstance<LobbyClient>.inst.GetMatchingRegionSchedule(modeSelection.SelectedMatchingMode);
			isModeOpen = false;
			using (List<bool>.Enumerator enumerator = matchingRegionSchedule.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current)
					{
						isModeOpen = true;
						break;
					}
				}
			}

			modeSelection.ActiveMatchMode(matchingRegionSchedule);
		}


		private void BlockOpenModeButton(bool block)
		{
			txtOpenMode.color =
				block ? GameConstants.UIColor.disableButtonTextColor : GameConstants.UIColor.enableButtonTextColor;
			openModefxGlow.SetActive(!block);
		}


		private void BlockExitButton(bool block)
		{
			matchStopButton.gameObject.SetActive(!block);
			matchingTimer.rectTransform.anchoredPosition3D =
				block ? new Vector3(-60f, 30f, 0f) : new Vector3(-145f, 30f, 0f);
		}


		private void BlockMatchStartButton(float duration)
		{
			matchStartButton.interactable = false;
			if (buttonBlockCoroutine != null)
			{
				StopCoroutine(buttonBlockCoroutine);
			}

			buttonBlockCoroutine = this.StartThrowingCoroutine(
				CoroutineUtil.DelayedAction(duration, delegate { matchStartButton.interactable = true; }),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][BlockMatchStartButton] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		public void HideModeSelection()
		{
			HideModeSelection(true);
		}


		public void HideAllButtons()
		{
			HideModeSelection(false);
		}


		private void HideModeSelection(bool isActiveGameStartButton)
		{
			isOpen = false;
			modalPanel.blocksRaycasts = false;
			matchStartButton.gameObject.SetActive(false);
			canvasAlphaTweener.from = modalPanel.alpha;
			canvasAlphaTweener.to = 0f;
			canvasAlphaTweener.StopAnimation();
			canvasAlphaTweener.PlayAnimation();
			openModeButton.gameObject.SetActive(isActiveGameStartButton);
			if (lobbyState == LobbyState.Ready)
			{
				matchingBg.gameObject.SetActive(false);
			}
		}


		public void OnLobbyStateUpdate(LobbyState lobbyState)
		{
			this.lobbyState = lobbyState;
			switch (lobbyState)
			{
				case LobbyState.Ready:
					if (MonoBehaviourInstance<LobbyService>.inst.SecretMatching)
					{
						return;
					}

					OnReady();
					return;
				case LobbyState.Matching:
					if (MonoBehaviourInstance<LobbyService>.inst.SecretMatching)
					{
						return;
					}

					OnMatching();
					return;
				case LobbyState.MatchCompleted:
					if (MonoBehaviourInstance<LobbyService>.inst.SecretMatching)
					{
						Singleton<SoundControl>.inst.PlayUISound("oui_BattleJoin_v1");
						return;
					}

					OnMatched();
					return;
				default:
					return;
			}
		}


		private void OnReady()
		{
			matchingBg.gameObject.SetActive(false);
			buttonBlock.gameObject.SetActive(false);
			matchStartButton.gameObject.SetActive(false);
			openModeButton.gameObject.SetActive(false);
			if (routine != null)
			{
				StopCoroutine(routine);
			}

			routine = this.StartThrowingCoroutine(
				CoroutineUtil.DelayedAction(1f, delegate { openModeButton.gameObject.SetActive(true); }),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][OnReady] Message:" + exception.Message + ", StackTrace:" + exception.StackTrace);
				});
		}


		private void OnMatching()
		{
			matchingBg.transform.position = matchingBgTweener.from;
			matchingBg.gameObject.SetActive(true);
			matchingBgTweener.StopAnimation();
			matchingBgTweener.PlayAnimation();
			matchingNotification.text = Ln.Get("매칭중입니다.");
			matchingMode.text = GlobalUserData.GetStringMatchingModeDetail();
			matchingBg.gameObject.SetActive(true);
			buttonBlock.gameObject.SetActive(false);
			matchStartButton.gameObject.SetActive(false);
			openModeButton.gameObject.SetActive(false);
			if (routine != null)
			{
				StopCoroutine(routine);
			}

			routine = this.StartThrowingCoroutine(PlayMatchTimer(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][OnMatching] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private void OnMatched()
		{
			Singleton<SoundControl>.inst.PlayUISound("oui_BattleJoin_v1");
			matchingNotification.text = Ln.Get("매칭 완료");
			matchingMode.text = GlobalUserData.GetStringMatchingModeDetail();
			matchingBg.gameObject.SetActive(true);
			buttonBlock.gameObject.SetActive(false);
			matchStartButton.gameObject.SetActive(false);
			openModeButton.gameObject.SetActive(false);
			if (routine != null)
			{
				StopCoroutine(routine);
			}
		}


		private IEnumerator PlayMatchTimer()
		{
			int matchingTime = 0;
			while (lobbyState == LobbyState.Matching)
			{
				matchingTimer.text = string.Format("{0:00} : {1:00}", matchingTime / 60, matchingTime % 60);
				int num = matchingTime;
				matchingTime = num + 1;
				yield return new WaitForSeconds(1f);
			}
		}
	}
}