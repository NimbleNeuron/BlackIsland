using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blis.Client
{
	public class MatchingService : MonoBehaviourInstance<MatchingService>, IMatchingNotificationHandler
	{
		public delegate void OnCharacterCancelPickEvent(MatchingUser userInfo);
		public delegate void OnCharacterMyPickEvent(int characterCode, int skinCode, bool isSinglePlay);
		public delegate void OnCharacterPickEvent(MatchingUser userInfo);
		public delegate void OnCharacterPickObserverEvent(int teamNumber, MatchingUser userInfo);
		public delegate void OnCharacterSelectEvent(int beforeCharacterCode, int beforeStartingDataCode,
			MatchingUser userInfo, bool isSinglePlay);
		public delegate void OnCharacterSelectObserverEvent(int teamNumber, int beforeCharacterCode,
			int beforeStartingDataCode, MatchingUser userInfo);
		public delegate void OnChatMessageEvent(MatchingChatMessage msg);
		public delegate bool OnCloseWindowEvent();
		public delegate void OnCompleteMatchingEvent();
		public delegate void OnSkinSelectEvent(int beforeSkinCode, MatchingUser userInfo, bool isSinglePlay);
		public delegate void OnSkinSelectObserverEvent(int teamNumber, int beforeSkinCode, MatchingUser userInfo);
		public delegate void OnStandByEvent();
		public delegate void OnStartGameEvent();

		public readonly List<int> banCharacters = new List<int>();
		private readonly List<int> freeCharacters = new List<int>();

		private readonly SortedDictionary<int, Dictionary<long, MatchingUser>> matchingAllTeamInfos =
			new SortedDictionary<int, Dictionary<long, MatchingUser>>();

		private readonly Dictionary<long, MatchingUser> matchingTeamUserInfos = new Dictionary<long, MatchingUser>();

		private Bootstrap bootstrap;
		private CustomGameRoom customGameRoom;
		private MatchingSocket matchingSocket;
		private DateTime matchMakingStartTime;
		private Coroutine startPracticeGame;
		public CustomGameRoom CustomGameRoom => customGameRoom;
		public int TeamMemberCount => matchingTeamUserInfos.Count;
		public MatchingMode MatchingMode => GlobalUserData.matchingMode;
		private MatchingRegion MatchingRegion => GlobalUserData.matchingRegion;
		public MatchingTeamMode MatchingTeamMode => GlobalUserData.matchingTeamMode;
		private long MyUserNum => Lobby.inst.User.UserNum;

		public bool IsObserver {
			get
			{
				return MatchingMode == MatchingMode.Custom && customGameRoom != null &&
				       customGameRoom.observerSlotList.Any(x => x.userNum == MyUserNum);
			}
		}

		public void OnStartCustomGame(MatchingResult matchingResult)
		{
			Singleton<SoundControl>.inst.StopBGM();
			bootstrap.LoadClient(matchingResult, MyUserNum, Singleton<LocalSetting>.inst.setting.accelerateChina);
			matchingSocket.RegisterCloseCallback(null);
			matchingSocket.Close();
			LobbyService.UpdateLobbyState(LobbyState.MatchCompleted);
			Destroy(gameObject);
			Singleton<GameEventLogger>.inst.SetRoomKey(matchingResult.battleTokenKey);
			Singleton<GameEventLogger>.inst.SetGameMode(matchingResult.matchingMode.GetGameMode().ToString());
		}


		public void OnUpdateCustomGameRoom(CustomGameRoom customGameRoom)
		{
			this.customGameRoom = customGameRoom;
			MonoBehaviourInstance<LobbyUI>.inst.CustomModeWindow.UpdateCustomGameRoom();
		}


		public void OnCustomGameRoomChatMessage(MatchingChatMessage message)
		{
			if (MonoBehaviourInstance<LobbyUI>.inst.CustomModeWindow.IsOpen)
			{
				MonoBehaviourInstance<LobbyUI>.inst.CustomModeWindow.AddChatting(message.sender,
					SingletonMonoBehaviour<SwearWordManager>.inst.CheckAndReplaceChat(message.message));
			}
		}


		public void OnKickedOutCustomGameRoom()
		{
			LobbyService.UpdateLobbyState(LobbyState.Ready);
			MonoBehaviourInstance<LobbyUI>.inst.CustomModeWindow.Close();
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("방장에 의해 방에서 추방되었습니다."), new Popup.Button
			{
				text = Ln.Get("확인")
			});
		}


		public void OnStartMatching()
		{
			LobbyService.UpdateLobbyState(LobbyState.Matching);
			matchMakingStartTime = DateTime.Now;
			OnCompleteMatchingEvent onCompleteMatchingEvent = onStartMatchingEvent;
			if (onCompleteMatchingEvent == null)
			{
				return;
			}

			onCompleteMatchingEvent();
		}


		public void OnStopMatching()
		{
			LobbyService.UpdateLobbyState(LobbyState.Ready);
			OnCompleteMatchingEvent onCompleteMatchingEvent = onStopMatchingEvent;
			if (onCompleteMatchingEvent == null)
			{
				return;
			}

			onCompleteMatchingEvent();
		}


		public void OnCompleteMatching()
		{
			WindowController.Find();
			WindowController.FlashWindowExUntilStop();
			LobbyService.UpdateLobbyState(LobbyState.MatchCompleted);
			OnCompleteMatchingEvent onCompleteMatchingEvent = this.onCompleteMatchingEvent;
			if (onCompleteMatchingEvent == null)
			{
				return;
			}

			onCompleteMatchingEvent();
		}


		public void OnExitMatchingOtherUser()
		{
			LobbyService.UpdateLobbyState(LobbyState.Ready);
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("팀원 중 누군가가 매칭 실패"), new Popup.Button
			{
				text = Ln.Get("확인")
			});
		}


		public void OnOtherMemberNotJoinMatching()
		{
			LobbyService.UpdateLobbyState(LobbyState.Ready);
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("팀원 중 누군가가 매칭 실패"), new Popup.Button
			{
				text = Ln.Get("확인")
			});
		}


		public void OnChatMessage(MatchingChatMessage msg)
		{
			OnChatMessageEvent onChatMessageEvent = this.onChatMessageEvent;
			if (onChatMessageEvent == null)
			{
				return;
			}

			onChatMessageEvent(msg);
		}


		public void OnCharacterSelectPhase(List<MatchingTeamMember> teamMembers, List<long> freeCharacterList,
			List<long> banCharacterList)
		{
			CloseMatchingConfirmPopup();
			SetFreeCharacters(freeCharacterList);
			SetBanCharacter(banCharacterList);
			matchingTeamUserInfos.Clear();
			int num = 1;
			foreach (MatchingTeamMember matchingTeamMember in from x in teamMembers
				orderby x.userNum
				select x)
			{
				matchingTeamUserInfos.Add(matchingTeamMember.userNum,
					new MatchingUser(num, MyUserNum == matchingTeamMember.userNum, matchingTeamMember.userNum,
						matchingTeamMember.nickname, matchingTeamMember.characterCode, matchingTeamMember.weaponCode,
						matchingTeamMember.pick));
				num++;
			}

			MonoBehaviourInstance<LobbyUI>.inst.CharacterSelectWindow.Open();
		}


		public void OnObserverCharacterSelectPhase(List<MatchingTeam> matchingTeamList)
		{
			Log.V("[OnObserverCharacterSelectPhase] Start");
			foreach (MatchingTeam matchingTeam in matchingTeamList)
			{
				string text = string.Format("[OBSERVER] Team({0}) :", matchingTeam.teamNo);
				foreach (KeyValuePair<long, MatchingTeamMember> keyValuePair in matchingTeam.teamMembers)
				{
					text += string.Format(" {0}({1})", keyValuePair.Value.nickname, keyValuePair.Value.characterCode);
				}

				Log.V(text);
			}

			Log.V("[OnObserverCharacterSelectPhase] End");
			matchingAllTeamInfos.Clear();
			for (int i = 0; i < matchingTeamList.Count; i++)
			{
				MatchingTeam matchingTeam2 = matchingTeamList[i];
				if (!matchingAllTeamInfos.ContainsKey(matchingTeam2.teamNo))
				{
					matchingAllTeamInfos.Add(matchingTeam2.teamNo, new Dictionary<long, MatchingUser>());
				}

				int num = 1;
				foreach (KeyValuePair<long, MatchingTeamMember> keyValuePair2 in from x in matchingTeam2.teamMembers
					orderby x.Key
					select x)
				{
					long key = keyValuePair2.Key;
					MatchingTeamMember value = keyValuePair2.Value;
					matchingAllTeamInfos[matchingTeam2.teamNo].Add(key,
						new MatchingUser(num, MyUserNum == value.userNum, value.userNum, value.nickname,
							value.characterCode, value.weaponCode, value.pick));
					num++;
				}
			}

			MonoBehaviourInstance<LobbyUI>.inst.CharacterSelectWindow.Open();
		}


		public void OnCharacterSelect(long userNum, int characterCode, int startingDataCode, int skinCode)
		{
			int characterCode2;
			int startingDataCode2;
			if (IsObserver)
			{
				using (SortedDictionary<int, Dictionary<long, MatchingUser>>.Enumerator enumerator =
					matchingAllTeamInfos.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, Dictionary<long, MatchingUser>> keyValuePair = enumerator.Current;
						MatchingUser matchingUser;
						if (keyValuePair.Value.TryGetValue(userNum, out matchingUser))
						{
							characterCode2 = matchingUser.CharacterCode;
							startingDataCode2 = matchingUser.StartingDataCode;
							matchingUser.Update(characterCode, startingDataCode, matchingUser.Pick);
							OnCharacterSelectObserverEvent onCharacterSelectObserverEvent =
								this.onCharacterSelectObserverEvent;
							if (onCharacterSelectObserverEvent == null)
							{
								break;
							}

							onCharacterSelectObserverEvent(keyValuePair.Key, characterCode2, startingDataCode2,
								matchingUser);
							break;
						}
					}

					return;
				}
			}

			MatchingUser matchingUser2;
			if (!matchingTeamUserInfos.TryGetValue(userNum, out matchingUser2))
			{
				return;
			}

			characterCode2 = matchingUser2.CharacterCode;
			startingDataCode2 = matchingUser2.StartingDataCode;
			matchingUser2.Update(characterCode, startingDataCode, matchingUser2.Pick);
			matchingUser2.UpdateSkin(skinCode);
			OnCharacterSelectEvent onCharacterSelectEvent = this.onCharacterSelectEvent;
			if (onCharacterSelectEvent == null)
			{
				return;
			}

			onCharacterSelectEvent(characterCode2, startingDataCode2, matchingUser2, TeamMemberCount == 1);
		}


		public void OnSkinSelect(long userNum, int skinCode)
		{
			int skinCode2;
			if (IsObserver)
			{
				using (SortedDictionary<int, Dictionary<long, MatchingUser>>.Enumerator enumerator =
					matchingAllTeamInfos.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, Dictionary<long, MatchingUser>> keyValuePair = enumerator.Current;
						MatchingUser matchingUser;
						if (keyValuePair.Value.TryGetValue(userNum, out matchingUser))
						{
							skinCode2 = matchingUser.SkinCode;
							matchingUser.UpdateSkin(skinCode);
							OnSkinSelectObserverEvent onSkinSelectObserverEvent = this.onSkinSelectObserverEvent;
							if (onSkinSelectObserverEvent == null)
							{
								break;
							}

							onSkinSelectObserverEvent(keyValuePair.Key, skinCode2, matchingUser);
							break;
						}
					}

					return;
				}
			}

			MatchingUser matchingUser2;
			if (!matchingTeamUserInfos.TryGetValue(userNum, out matchingUser2))
			{
				return;
			}

			skinCode2 = matchingUser2.SkinCode;
			matchingUser2.UpdateSkin(skinCode);
			OnSkinSelectEvent onSkinSelectEvent = this.onSkinSelectEvent;
			if (onSkinSelectEvent == null)
			{
				return;
			}

			onSkinSelectEvent(skinCode2, matchingUser2, TeamMemberCount == 1);
		}


		public void OnCharacterPick(long userNum, int skinCode)
		{
			if (IsObserver)
			{
				using (SortedDictionary<int, Dictionary<long, MatchingUser>>.Enumerator enumerator =
					matchingAllTeamInfos.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, Dictionary<long, MatchingUser>> keyValuePair = enumerator.Current;
						MatchingUser matchingUser;
						if (keyValuePair.Value.TryGetValue(userNum, out matchingUser))
						{
							matchingUser.Update(matchingUser.CharacterCode, matchingUser.StartingDataCode, true);
							matchingUser.UpdateSkin(skinCode);
							OnCharacterPickObserverEvent onCharacterPickObserverEvent =
								this.onCharacterPickObserverEvent;
							if (onCharacterPickObserverEvent != null)
							{
								onCharacterPickObserverEvent(keyValuePair.Key, matchingUser);
							}

							OnSkinSelectObserverEvent onSkinSelectObserverEvent = this.onSkinSelectObserverEvent;
							if (onSkinSelectObserverEvent == null)
							{
								break;
							}

							onSkinSelectObserverEvent(keyValuePair.Key, -1, matchingUser);
							break;
						}
					}

					return;
				}
			}

			MatchingUser matchingUser2;
			if (matchingTeamUserInfos.TryGetValue(userNum, out matchingUser2))
			{
				matchingUser2.Update(matchingUser2.CharacterCode, matchingUser2.StartingDataCode, true);
				matchingUser2.UpdateSkin(skinCode);
				if (matchingUser2.IsMe)
				{
					SingletonMonoBehaviour<GameAnalytics>.inst.SetSelectCharacter(matchingUser2.CharacterCode);
					SingletonMonoBehaviour<GameAnalytics>.inst.SetSelectWeapon(matchingUser2.StartingDataCode);
					OnCharacterMyPickEvent onCharacterMyPickEvent = this.onCharacterMyPickEvent;
					if (onCharacterMyPickEvent != null)
					{
						onCharacterMyPickEvent(matchingUser2.CharacterCode, matchingUser2.SkinCode,
							TeamMemberCount == 1);
					}

					OnSkinSelectEvent onSkinSelectEvent = this.onSkinSelectEvent;
					if (onSkinSelectEvent != null)
					{
						onSkinSelectEvent(-1, matchingUser2, TeamMemberCount == 1);
					}
				}

				if (1 < TeamMemberCount)
				{
					OnCharacterPickEvent onCharacterPickEvent = this.onCharacterPickEvent;
					if (onCharacterPickEvent != null)
					{
						onCharacterPickEvent(matchingUser2);
					}

					OnSkinSelectEvent onSkinSelectEvent2 = onSkinSelectEvent;
					if (onSkinSelectEvent2 == null)
					{
						return;
					}

					onSkinSelectEvent2(-1, matchingUser2, TeamMemberCount == 1);
				}
			}
		}


		public void OnCancelCharacterPick(long cancelUserNum)
		{
			if (IsObserver)
			{
				using (SortedDictionary<int, Dictionary<long, MatchingUser>>.Enumerator enumerator =
					matchingAllTeamInfos.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, Dictionary<long, MatchingUser>> keyValuePair = enumerator.Current;
						MatchingUser matchingUser;
						if (keyValuePair.Value.TryGetValue(cancelUserNum, out matchingUser))
						{
							matchingUser.Update(matchingUser.CharacterCode, matchingUser.StartingDataCode, false);
							OnCharacterPickObserverEvent onCharacterPickObserverEvent =
								onCharacterCancelPickObserverEvent;
							if (onCharacterPickObserverEvent == null)
							{
								break;
							}

							onCharacterPickObserverEvent(keyValuePair.Key, matchingUser);
							break;
						}
					}

					return;
				}
			}

			MatchingUser matchingUser2;
			if (matchingTeamUserInfos.TryGetValue(cancelUserNum, out matchingUser2))
			{
				matchingUser2.Update(matchingUser2.CharacterCode, matchingUser2.StartingDataCode, false);
				OnCharacterCancelPickEvent onCharacterCancelPickEvent = this.onCharacterCancelPickEvent;
				if (onCharacterCancelPickEvent != null)
				{
					onCharacterCancelPickEvent(matchingUser2);
				}

				if (matchingUser2.IsMe)
				{
					OnCharacterCancelPickEvent onCharacterCancelPickEvent2 = onCharacterCancelMyPickEvent;
					if (onCharacterCancelPickEvent2 == null)
					{
						return;
					}

					onCharacterCancelPickEvent2(matchingUser2);
				}
			}
		}


		public void OnSelectSkinPhase() { }


		public void OnDodgeMatching()
		{
			LobbyService.UpdateLobbyState(LobbyState.Matching);
			CloseMatchingConfirmPopup();
			CloseCharacterSelectWindow();
			MonoBehaviourInstance<LobbyUI>.inst.ToastMessage.ShowMessage(Ln.Get("이탈자가 발생하여 매칭을 다시 시도합니다."), 5f);
		}


		public void OnKickOutMatching()
		{
			LobbyService.UpdateLobbyState(LobbyState.Ready);
			CloseMatchingConfirmPopup();
			CloseCharacterSelectWindow();
			MatchingMode matchingMode = GlobalUserData.matchingMode;
			if (matchingMode - MatchingMode.Normal <= 1)
			{
				if (GlobalUserData.matchingTeamMode.IsTeamMode())
				{
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("팀원 중 이탈자가 발생하여 매칭이 취소되었습니다."), new Popup.Button
					{
						text = Ln.Get("확인")
					});
				}
				else
				{
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭 취소 페널티 팝업"), new Popup.Button
					{
						text = Ln.Get("확인")
					});
				}

				RequestDelegate.request<MatchingApi.GetMatchingPenaltyResult>(
					MatchingApi.GetMatchingPenalty(GlobalUserData.matchingMode), delegate(RequestDelegateError err,
						MatchingApi.GetMatchingPenaltyResult res)
					{
						if (err != null)
						{
							return;
						}

						if (GlobalUserData.matchingMode == MatchingMode.Normal)
						{
							Lobby.inst.SetNormalMatchingPenaltyTime(res.matchingPenaltyTime);
						}

						if (GlobalUserData.matchingMode == MatchingMode.Rank)
						{
							Lobby.inst.SetRankMatchingPenaltyTime(res.matchingPenaltyTime);
						}
					});
				return;
			}

			if (matchingMode != MatchingMode.Custom)
			{
				return;
			}

			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭을 취소하여 사용자 설정 대전 참여자에서 제외되었습니다."), new Popup.Button
			{
				text = Ln.Get("확인")
			});
		}


		public void OnAcceptMatching()
		{
			OnCompleteMatchingEvent onCompleteMatchingEvent = onAcceptMatchingEvent;
			if (onCompleteMatchingEvent == null)
			{
				return;
			}

			onCompleteMatchingEvent();
		}


		public void OnKickOutDeclineMatching()
		{
			LobbyService.UpdateLobbyState(LobbyState.Ready);
			OnCompleteMatchingEvent onCompleteMatchingEvent = onDeclineMatchingEvent;
			if (onCompleteMatchingEvent == null)
			{
				return;
			}

			onCompleteMatchingEvent();
		}


		public void OnDodgeCustomMatching()
		{
			LobbyService.UpdateLobbyState(LobbyState.CustomGameRoom);
			CloseCharacterSelectWindow();
			MonoBehaviourInstance<LobbyUI>.inst.ToastMessage.ShowMessage(Ln.Get("이탈자가 발생하여 매칭이 취소되었습니다."), 5f);
		}


		public void OnCustomGameRoomEmpty()
		{
			LobbyService.UpdateLobbyState(LobbyState.Ready);
			CloseCharacterSelectWindow();
			CloseCustomModeWindow();
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("사용자 설정 대전 참여자가 없어 해산되었습니다."), new Popup.Button
			{
				text = Ln.Get("확인")
			});
		}


		public void OnStandBy()
		{
			OnStandByEvent onStandByEvent = this.onStandByEvent;
			if (onStandByEvent != null)
			{
				onStandByEvent();
			}

			Singleton<SoundControl>.inst.StopBGM();
			Singleton<SoundControl>.inst.Play2DSound("Lobby_5sec");
			if (GlobalUserData.IsStandaloneMode())
			{
				MatchingUser matchingUser;
				if (matchingTeamUserInfos.TryGetValue(MyUserNum, out matchingUser))
				{
					if (matchingUser.CharacterCode == 0)
					{
						OnKickOutMatching();
						return;
					}

					if (!matchingUser.Pick)
					{
						SingletonMonoBehaviour<GameAnalytics>.inst.SetSelectCharacter(matchingUser.CharacterCode);
						SingletonMonoBehaviour<GameAnalytics>.inst.SetSelectWeapon(matchingUser.StartingDataCode);
						matchingUser.Update(matchingUser.CharacterCode, matchingUser.StartingDataCode, true);
						OnCharacterMyPickEvent onCharacterMyPickEvent = this.onCharacterMyPickEvent;
						if (onCharacterMyPickEvent != null)
						{
							onCharacterMyPickEvent(matchingUser.CharacterCode, matchingUser.SkinCode, true);
						}
					}
				}

				StopCoroutineStartPracticeGame();
				startPracticeGame = this.StartThrowingCoroutine(
					CoroutineUtil.DelayedAction(GameConstants.GameStartStandByTime, OnStartPracticeGame),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][OnStandBy] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
				return;
			}

			if (IsObserver)
			{
				using (SortedDictionary<int, Dictionary<long, MatchingUser>>.Enumerator enumerator =
					matchingAllTeamInfos.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, Dictionary<long, MatchingUser>> keyValuePair = enumerator.Current;
						foreach (KeyValuePair<long, MatchingUser> keyValuePair2 in keyValuePair.Value)
						{
							if (!keyValuePair2.Value.Pick && 0 < keyValuePair2.Value.CharacterCode)
							{
								OnCharacterPick(keyValuePair2.Value.UserNum, keyValuePair2.Value.SkinCode);
							}
						}
					}

					return;
				}
			}

			foreach (KeyValuePair<long, MatchingUser> keyValuePair3 in matchingTeamUserInfos)
			{
				if (!keyValuePair3.Value.Pick && 0 < keyValuePair3.Value.CharacterCode)
				{
					OnCharacterPick(keyValuePair3.Value.UserNum, keyValuePair3.Value.SkinCode);
				}
			}
		}


		public void OnStartGame(MatchingResult matchingResult)
		{
			OnStartGameEvent onStartGameEvent = this.onStartGameEvent;
			if (onStartGameEvent != null)
			{
				onStartGameEvent();
			}

			MonoBehaviourInstance<LobbyUI>.inst.CharacterSelectWindow.SetIsInputLock(true);
			Singleton<SoundControl>.inst.StopBGM();
			bootstrap.LoadClient(matchingResult, MyUserNum, Singleton<LocalSetting>.inst.setting.accelerateChina);
			matchingSocket.RegisterCloseCallback(null);
			matchingSocket.Close();
			Destroy(gameObject);
			double totalSeconds = (DateTime.Now - matchMakingStartTime).TotalSeconds;
			Singleton<GameEventLogger>.inst.SetMatchmakingTime((int) totalSeconds);
			Singleton<GameEventLogger>.inst.SetRoomKey(matchingResult.battleTokenKey);
			Singleton<GameEventLogger>.inst.SetGameMode(matchingResult.matchingMode.GetGameMode().ToString());
		}


		public bool IsCustomGameRoomOwner(long userNum)
		{
			return MatchingMode == MatchingMode.Custom && customGameRoom != null &&
			       customGameRoom.ownerUserNum == userNum;
		}


		public bool IsFullObserverInCustomGameRoom()
		{
			if (MatchingMode != MatchingMode.Custom)
			{
				return false;
			}

			if (customGameRoom == null)
			{
				return false;
			}

			return customGameRoom.observerSlotList.All(x => !x.IsEmptySlot());
		}

		public event OnCompleteMatchingEvent onStartMatchingEvent = delegate { };
		public event OnCompleteMatchingEvent onStopMatchingEvent = delegate { };
		public event OnCompleteMatchingEvent onCompleteMatchingEvent = delegate { };
		public event OnCompleteMatchingEvent onAcceptMatchingEvent = delegate { };
		public event OnCompleteMatchingEvent onDeclineMatchingEvent = delegate { };
		public event OnCharacterSelectEvent onCharacterSelectEvent = delegate { };
		public event OnCharacterSelectObserverEvent onCharacterSelectObserverEvent = delegate { };
		public event OnSkinSelectEvent onSkinSelectEvent = delegate { };
		public event OnSkinSelectObserverEvent onSkinSelectObserverEvent = delegate { };
		public event OnCharacterMyPickEvent onCharacterMyPickEvent = delegate { };
		public event OnCharacterPickEvent onCharacterPickEvent = delegate { };
		public event OnCharacterCancelPickEvent onCharacterCancelPickEvent = delegate { };
		public event OnCharacterCancelPickEvent onCharacterCancelMyPickEvent = delegate { };
		public event OnCharacterPickObserverEvent onCharacterPickObserverEvent = delegate { };
		public event OnCharacterPickObserverEvent onCharacterCancelPickObserverEvent = delegate { };
		public event OnChatMessageEvent onChatMessageEvent = delegate { };
		public event OnStandByEvent onStandByEvent = delegate { };
		public event OnStartGameEvent onStartGameEvent = delegate { };
		public event OnCloseWindowEvent onCloseCharacterSelectWindowEvent = () => false;
		public event OnCloseWindowEvent onCloseCustomModeWindowEvent = () => false;

		protected override void _Awake()
		{
			base._Awake();
			GameUtil.BindOrAdd<MatchingSocket>(gameObject, ref matchingSocket);
		}

		public void RegisterServerEventHandler(BlisWebSocket.ServerErrorWithOutOfService OnServerErrorWithOutOfService)
		{
			MatchingSocket matchingSocket = this.matchingSocket;
			matchingSocket.OnServerErrorWithOutOfService =
				(BlisWebSocket.ServerErrorWithOutOfService) Delegate.Combine(
					matchingSocket.OnServerErrorWithOutOfService, OnServerErrorWithOutOfService);
		}


		protected override void _OnDestroy()
		{
			base._OnDestroy();
			MatchingSocket matchingSocket = this.matchingSocket;
			if (matchingSocket == null)
			{
				return;
			}

			matchingSocket.Close();
		}


		public void Init()
		{
			bootstrap = SingletonMonoBehaviour<Bootstrap>.inst;
		}


		public void CreateCustomGame(MatchingTeamMode customMatchingTeamMode)
		{
			try
			{
				ValidateMatchingRegion();
				ValidateMatchingMode(customMatchingTeamMode);
				RequestDelegate.request<MatchingApi.EnterCustomResult>(
					MatchingApi.EnterCustom(new MatchingParam(MatchingRegion, MatchingMode.Custom,
						customMatchingTeamMode, MyUserNum.ToString(), 1, SteamApi.GetStringSteamID(),
						Singleton<LocalSetting>.inst.setting.hideNameFromEnemy)),
					delegate(RequestDelegateError err, MatchingApi.EnterCustomResult res)
					{
						if (err != null)
						{
							MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
							LobbyService.UpdateLobbyState(LobbyState.Ready);
							return;
						}

						matchingSocket.Init(res.GetMatchingHost(), this);
						matchingSocket.RegisterErrorCallback(delegate(string msg)
						{
							MonoBehaviourInstance<Popup>.inst.Error(msg);
						});
						matchingSocket.RegisterCloseCallback(delegate
						{
							LobbyService.UpdateLobbyState(LobbyState.Ready);
							LobbyUI inst = MonoBehaviourInstance<LobbyUI>.inst;
							if (inst != null)
							{
								inst.CustomModeWindow.Close();
							}
						});
						matchingSocket.CreateCustomGameRoom(res.customGameTokenKey, MyUserNum,
							delegate(CustomGameRoom result)
							{
								customGameRoom = result;
								LobbyUI inst = MonoBehaviourInstance<LobbyUI>.inst;
								if (inst != null)
								{
									inst.CustomModeWindow.Open();
									inst.CustomModeWindow.InitCustomGameRoom(true);
									inst.CustomModeWindow.UpdateCustomGameRoom();
									inst.MainMenu.MatchingButton.HideAllButtons();
								}

								LobbyService.UpdateLobbyState(LobbyState.CustomGameRoom);
								GlobalUserData.customGameRoomKey = result.customGameKey;
							});
					});
			}
			catch (GameException ex)
			{
				Log.E(ex.msg);
				if (ex.errorType == ErrorType.MatchingServerNone)
				{
					MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("UnknownMatchingError"));
				}
			}
			catch (Exception)
			{
				MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("매칭서버 타임아웃"));
			}
		}


		public void EnterCustomGame(string roomKey)
		{
			try
			{
				ValidateMatchingRegion();
				RequestDelegate.request<MatchingApi.EnterCustomResult>(
					MatchingApi.EnterCustom(new MatchingParam(MatchingRegion, MatchingMode.Custom,
						MatchingTeamMode.None, MyUserNum.ToString(), 1, SteamApi.GetStringSteamID(),
						Singleton<LocalSetting>.inst.setting.hideNameFromEnemy)),
					(err, res) =>
					{
						if (err != null)
						{
							MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
							LobbyService.UpdateLobbyState(LobbyState.Ready);
						}
						else
						{
							matchingSocket.Init(res.GetMatchingHost(), this);
							matchingSocket.RegisterErrorCallback(msg =>
								MonoBehaviourInstance<Popup>.inst.Error(msg));
							matchingSocket.RegisterCloseCallback(() =>
							{
								LobbyService.UpdateLobbyState(LobbyState.Ready);
								MonoBehaviourInstance<LobbyUI>.inst.CustomModeWindow.Close();
							});
							matchingSocket.EnterCustomGameRoom(res.customGameTokenKey, roomKey, MyUserNum,
								result =>
								{
									customGameRoom = result;
									MonoBehaviourInstance<LobbyUI>.inst.CustomModeWindow.Open();
									MonoBehaviourInstance<LobbyUI>.inst.CustomModeWindow.InitCustomGameRoom(false);
									MonoBehaviourInstance<LobbyUI>.inst.CustomModeWindow.UpdateCustomGameRoom();
									MonoBehaviourInstance<LobbyUI>.inst.MainMenu.MatchingButton.HideAllButtons();
									LobbyService.UpdateLobbyState(LobbyState.CustomGameRoom);
									GlobalUserData.customGameRoomKey = result.customGameKey;
								});
						}
					});
			}
			catch (GameException ex)
			{
				Log.E(ex.msg);
				if (ex.errorType != ErrorType.MatchingServerNone)
				{
					return;
				}

				MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("UnknownMatchingError"));
			}
			catch (Exception ex)
			{
				MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("매칭서버 타임아웃"));
				Log.E(ex.Message);
			}
		}


		public void LeaveCustomGameRoom()
		{
			matchingSocket.LeaveCustomGameRoom(delegate { LobbyService.UpdateLobbyState(LobbyState.Ready); });
			GlobalUserData.customGameRoomKey = string.Empty;
		}


		public void LeaveCharacterSelectPhase()
		{
			matchingSocket.LeaveCustomGameRoom(delegate
			{
				LobbyService.UpdateLobbyState(LobbyState.Ready);
				CloseCharacterSelectWindow();
			});
		}


		public void KickUserCustomGameRoom(long kickUserNum)
		{
			matchingSocket.KickUserCustomGameRoom(kickUserNum);
		}


		public void MoveSlotCustomGameRoom(int destSlotIndex)
		{
			matchingSocket.MoveSlot(destSlotIndex);
		}


		public void MoveObserverSlotCustomGameRoom()
		{
			if (!customGameRoom.slotList.Any(x => 0L < x.userNum && !x.isBot && x.userNum != MyUserNum))
			{
				return;
			}

			if (customGameRoom.observerSlotList.Any(x => x.userNum == MyUserNum))
			{
				return;
			}

			matchingSocket.MoveObserverSlot();
		}


		public void MoveUserToObserverSlotCustomGameRoom(long moveUserNum)
		{
			matchingSocket.MoveUserToObserverSlot(moveUserNum);
		}


		public void AddBotCustomGameRoom(int slotIndex)
		{
			List<int> creatableBot = GameDB.bot.GetCreatableBot();
			int num = creatableBot.ElementAt(Random.Range(0, creatableBot.Count));
			string name = StringUtil.CreateBotNickname(GameDB.character.GetCharacterData(num).name,
				customGameRoom.botNameNum);
			matchingSocket.AddBotCustomGameRoom(slotIndex, num, name);
		}


		public void RemoveBotCustomGameRoom(int botUserNum)
		{
			matchingSocket.RemoveBotCustomGameRoom(botUserNum);
		}


		public void SendCustomChatting(string chatContent)
		{
			if (string.IsNullOrEmpty(chatContent))
			{
				return;
			}

			chatContent = ArchStringUtil.CutOverSizeANSI(chatContent, 100);
			matchingSocket.SendCustomChatting(chatContent);
		}


		public void UpdateAcceleration(bool isOn)
		{
			matchingSocket.UpdateAcceleration(isOn);
		}


		public void StartCustomGame()
		{
			matchingSocket.StartCustomGame();
		}


		public void BeforeMatching(List<long> teamMembers)
		{
			BeforeMatchingParam matchingParam = new BeforeMatchingParam(MatchingMode, MatchingTeamMode, teamMembers);
			
			RequestDelegate.request<MatchingApi.BeforeMatchingResult>(MatchingApi.BeforeMatching(matchingParam)
				, false,responseCallback);

			void responseCallback(RequestDelegateError err, MatchingApi.BeforeMatchingResult res)
			{
				if (err != null)
				{
					MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
					LobbyService.UpdateLobbyState(LobbyState.Ready);
					return;
				}

				int count = res.matchingPenaltyInfo.Count;
				if (count > 0)
				{
					DateTime d = DateTime.Now.ToUniversalTime();
					StringBuilder stringBuilder = GameUtil.StringBuilder;
					stringBuilder.Clear();
					stringBuilder.Append(Ln.Get("대기열 회피 팝업"));
					for (int i = 0; i < count; i++)
					{
						MatchingApi.MatchingPenaltyInfo matchingPenaltyInfo = res.matchingPenaltyInfo[i];
						if (MyUserNum == matchingPenaltyInfo.userNum)
						{
							if (MatchingMode == MatchingMode.Normal)
							{
								Lobby.inst.SetNormalMatchingPenaltyTime(matchingPenaltyInfo.until);
							}

							if (MatchingMode == MatchingMode.Rank)
							{
								Lobby.inst.SetRankMatchingPenaltyTime(matchingPenaltyInfo.until);
							}
						}

						stringBuilder.Append(matchingPenaltyInfo.nickname);
						stringBuilder.Append(": ");
						TimeSpan timeSpan = matchingPenaltyInfo.until - d;
						if ((int) timeSpan.TotalMinutes > 0)
						{
							stringBuilder.Append((int) timeSpan.TotalMinutes);
							stringBuilder.Append(Ln.Get("분"));
						}
						else
						{
							stringBuilder.Append((int) timeSpan.TotalSeconds);
							stringBuilder.Append(Ln.Get("초"));
						}

						if (i + 1 < count)
						{
							stringBuilder.Append(", ");
						}
					}

					MonoBehaviourInstance<Popup>.inst.Message(stringBuilder.ToString(), new Popup.Button
					{
						text = Ln.Get("확인")
					});
					LobbyService.UpdateLobbyState(LobbyState.Ready);
					return;
				}

				MonoBehaviourInstance<LobbyService>.inst.RequestStartMatching();
			}
		}


		public void StartMatching(string teamKey, int teamMemberCount)
		{
			try
			{
				MatchingParam matchingParam = new MatchingParam(MatchingRegion, MatchingMode, 
					MatchingTeamMode, teamKey,
					teamMemberCount, SteamApi.GetStringSteamID(),
					Singleton<LocalSetting>.inst.setting.hideNameFromEnemy);
				
				ValidateMatchingRegion();
				ValidateMatchingMode(MatchingTeamMode);
				
				RequestDelegate.request<MatchingApi.EnterResult>(MatchingApi.Enter(matchingParam),
					false, responseCallback);
			}
			catch (GameException ex)
			{
				Log.E(ex.msg);
				if (ex.errorType == ErrorType.MatchingServerNone)
				{
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("UnknownMatchingError"), new Popup.Button
					{
						type = Popup.ButtonType.Confirm,
						text = Ln.Get("확인")
					});
				}
			}
			catch (Exception)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭서버 타임아웃"), new Popup.Button
				{
					type = Popup.ButtonType.Confirm,
					text = Ln.Get("확인")
				});
			}

			void responseCallback(RequestDelegateError err, MatchingApi.EnterResult res)
			{
				if (err != null)
				{
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("ServerError/" + err.message),
						new Popup.Button
						{
							type = Popup.ButtonType.Confirm,
							text = Ln.Get("확인")
						});
					LobbyService.UpdateLobbyState(LobbyState.Ready);
					return;
				}

				StartMatching(res.GetMatchingHost(), res.matchingTokenKey);
			}
		}


		public void StartMatching(string matchingHost, string tokenKey)
		{
			matchingSocket.Init(matchingHost, this);
			matchingSocket.RegisterErrorCallback(delegate(string msg)
			{
				MonoBehaviourInstance<Popup>.inst.Error(msg,
					delegate { LobbyService.UpdateLobbyState(LobbyState.Ready); });
			});
			
			matchingSocket.EnterMatching(tokenKey, MyUserNum, null);
		}


		public void StopMatching()
		{
			matchingSocket.ExitMatching(null);
		}


		public void StopMatchingWithPopup()
		{
			matchingSocket.ExitMatching(delegate
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭이 취소 되었습니다."), new Popup.Button
				{
					type = Popup.ButtonType.Confirm,
					text = Ln.Get("확인")
				});
			});
		}


		public void AcceptMatching()
		{
			matchingSocket.AcceptMatching();
		}


		public void DeclineMatching()
		{
			matchingSocket.DeclineMatching();
		}


		public void SelectCharacter(int characterCode, int startingDataCode, int skinCode)
		{
			if (GlobalUserData.IsStandaloneMode())
			{
				MatchingUser matchingUser;
				if (matchingTeamUserInfos.TryGetValue(MyUserNum, out matchingUser) && !matchingUser.Pick)
				{
					OnCharacterSelect(MyUserNum, characterCode, startingDataCode, skinCode);
				}
			}
			else
			{
				if (matchingTeamUserInfos.Any(x => x.Key != MyUserNum && x.Value.CharacterCode == characterCode))
				{
					MonoBehaviourInstance<LobbyUI>.inst.ToastMessage.ShowMessage(Ln.Get("팀원이 이미 선택 중인 캐릭터입니다."), 5f);
					return;
				}

				matchingSocket.SelectCharacter(characterCode, startingDataCode, skinCode);
			}
		}


		public void SelectSkin(int skinCode)
		{
			if (GlobalUserData.IsStandaloneMode())
			{
				MatchingUser matchingUser;
				if (matchingTeamUserInfos.TryGetValue(MyUserNum, out matchingUser) && matchingUser.Pick)
				{
					OnSkinSelect(MyUserNum, skinCode);
				}
			}
			else
			{
				matchingSocket.SelectSkin(skinCode);
			}
		}


		public void SelectWeapon(int startingDataCode)
		{
			MatchingUser matchingUser;
			if (matchingTeamUserInfos.TryGetValue(MyUserNum, out matchingUser))
			{
				if (GlobalUserData.IsStandaloneMode())
				{
					if (!matchingUser.Pick)
					{
						OnCharacterSelect(MyUserNum, matchingUser.CharacterCode, startingDataCode,
							matchingUser.SkinCode);
					}
				}
				else
				{
					matchingSocket.SelectCharacter(matchingUser.CharacterCode, startingDataCode,
						matchingUser.GetSkinDataCode());
				}
			}
		}


		public void PickCharacter()
		{
			MatchingUser matchingUser;
			if (matchingTeamUserInfos.TryGetValue(MyUserNum, out matchingUser))
			{
				if (matchingUser.Pick)
				{
					if (!GlobalUserData.IsStandaloneMode())
					{
						matchingSocket.CancelPickCharacter();
					}
				}
				else
				{
					if (matchingUser.Pick)
					{
						return;
					}

					if (matchingUser.CharacterCode == 0)
					{
						return;
					}

					if (matchingUser.StartingDataCode == 0)
					{
						return;
					}

					if (GlobalUserData.IsStandaloneMode())
					{
						OnCharacterPick(MyUserNum, matchingUser.SkinCode);
						OnStandBy();
						return;
					}

					matchingSocket.PickCharacter();
				}
			}
		}


		public void SelectRoute(int route)
		{
			if (!GlobalUserData.IsStandaloneMode())
			{
				matchingSocket.SelectRoute(route);
			}
		}


		public void SendChatting(string chatContent)
		{
			if (string.IsNullOrEmpty(chatContent))
			{
				return;
			}

			chatContent = ArchStringUtil.CutOverSizeANSI(chatContent, 100);
			if (GlobalUserData.IsStandaloneMode())
			{
				OnChatMessage(new MatchingChatMessage
				{
					sender = Lobby.inst.User.Nickname,
					message = chatContent
				});
				return;
			}

			matchingSocket.SendChatMessage(chatContent);
		}


		public SortedDictionary<int, Dictionary<long, MatchingUser>> GetAllTeamInfo()
		{
			return matchingAllTeamInfos;
		}


		public int GetSlotIndex(MatchingUser userInfo)
		{
			if (userInfo.IsMe)
			{
				return 0;
			}

			MatchingUser matchingUser;
			if (!matchingTeamUserInfos.TryGetValue(MyUserNum, out matchingUser))
			{
				return -1;
			}

			if (matchingUser.TeamSlot < userInfo.TeamSlot)
			{
				return userInfo.TeamSlot - 1;
			}

			return userInfo.TeamSlot;
		}


		public MatchingUser GetTeamUser(int slotIndex)
		{
			MatchingUser matchingUser;
			if (matchingTeamUserInfos.TryGetValue(MyUserNum, out matchingUser))
			{
				if (slotIndex == 0)
				{
					return matchingUser;
				}

				foreach (KeyValuePair<long, MatchingUser> keyValuePair in matchingTeamUserInfos)
				{
					if (!keyValuePair.Value.IsMe)
					{
						if (matchingUser.TeamSlot < keyValuePair.Value.TeamSlot)
						{
							if (slotIndex == keyValuePair.Value.TeamSlot - 1)
							{
								return keyValuePair.Value;
							}
						}
						else if (slotIndex == keyValuePair.Value.TeamSlot)
						{
							return keyValuePair.Value;
						}
					}
				}
			}

			return null;
		}


		private void CloseMatchingConfirmPopup()
		{
			MonoBehaviourInstance<LobbyUI>.inst.MatchingConfirmPopup.Close();
		}


		private void CloseCharacterSelectWindow()
		{
			OnCloseWindowEvent onCloseWindowEvent = onCloseCharacterSelectWindowEvent;
			bool? flag = onCloseWindowEvent != null ? new bool?(onCloseWindowEvent()) : null;
			if (flag != null && flag.Value)
			{
				Singleton<SoundControl>.inst.PlayUISound("oui_cancel_v1");
			}

			StopCoroutineStartPracticeGame();
		}


		private void CloseCustomModeWindow()
		{
			OnCloseWindowEvent onCloseWindowEvent = onCloseCustomModeWindowEvent;
			if (onCloseWindowEvent == null)
			{
				return;
			}

			onCloseWindowEvent();
		}


		private void StopCoroutineStartPracticeGame()
		{
			if (startPracticeGame != null)
			{
				StopCoroutine(startPracticeGame);
				startPracticeGame = null;
			}
		}


		private void ValidateMatchingRegion()
		{
			if (MatchingRegion == MatchingRegion.None)
			{
				throw new GameException(ErrorType.MatchingServerNone);
			}
		}


		private void ValidateMatchingMode(MatchingTeamMode matchingTeamMode)
		{
			if (matchingTeamMode == MatchingTeamMode.None)
			{
				throw new GameException(ErrorType.MatchingTeamModeNone);
			}
		}


		private void ValidateTutorial()
		{
			if (MatchingRegion == MatchingRegion.None)
			{
				throw new GameException(ErrorType.MatchingServerNone);
			}
		}


		/// <summary>
		///  AI 모드 매칭
		/// </summary>
		public void StartPracticeMatching()
		{
			RequestDelegate.request<MatchingApi.EnterSingleResult>(MatchingApi.EnterSingle(),
				delegate(RequestDelegateError err, MatchingApi.EnterSingleResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
						LobbyService.UpdateLobbyState(LobbyState.Ready);
						CloseCharacterSelectWindow();
						return;
					}

					SetFreeCharacters(JsonConvert.DeserializeObject<List<long>>(res.freeCharacters));
					SetBanCharacter(JsonConvert.DeserializeObject<List<long>>(res.banCharacters));
					
					matchingTeamUserInfos.Clear();
					matchingTeamUserInfos.Add(MyUserNum,
						new MatchingUser(1, true, MyUserNum, Lobby.inst.User.Nickname, 0, 0, false));
					
					MonoBehaviourInstance<LobbyUI>.inst.CharacterSelectWindow.Open();
					StopCoroutineStartPracticeGame();
					
					startPracticeGame = this.StartThrowingCoroutine(
						CoroutineUtil.DelayedAction(GameConstants.MatchingCharacterSelectTime, OnStandBy),
						delegate(Exception exception)
						{
							Log.E("[EXCEPTION][StartPracticeMatching] Message:" + exception.Message + ", StackTrace:" +
							      exception.StackTrace);
						});
				});
		}


		private void OnStartPracticeGame()
		{
			try
			{
				RequestDelegate.request<InventoryApi.EmotionResult>(InventoryApi.GetInventoryEmotion(),
					delegate(RequestDelegateError err, InventoryApi.EmotionResult res)
					{
						Dictionary<EmotionPlateType, int> emotion;
						if (err != null)
						{
							emotion = new Dictionary<EmotionPlateType, int>();
						}
						else
						{
							emotion = res.userEmotionSlots.ToDictionary(e => e.slotType, e => e.emotionCode);
						}

						ValidateMatchingRegion();
						MatchingUser matchingUser;
						if (!matchingTeamUserInfos.TryGetValue(MyUserNum, out matchingUser) ||
						    matchingUser.CharacterCode == 0 || matchingUser.StartingDataCode == 0)
						{
							LobbyService.UpdateLobbyState(LobbyState.Ready);
							CloseCharacterSelectWindow();
							return;
						}

						Singleton<SoundControl>.inst.StopBGM();
						
						RecommendStarting recommendStarting =
							GameDB.recommend.FindRecommendStarting(matchingUser.StartingDataCode);
						
						MatchingToken matchingToken = new MatchingToken(GlobalUserData.matchingMode,
							MatchingTeamMode.Solo, MatchingRegion.None, "", Lobby.inst.User.UserNum,
							Lobby.inst.User.MMR, Lobby.inst.User.Nickname, matchingUser.CharacterCode,
							matchingUser.SkinCode, recommendStarting.startWeapon, new List<int>(), 1, false, emotion);
						
						bootstrap.LoadTest(Bootstrap.Mode.Standalone,
							string.Format("localhost:{0}", GameConstants.DefaultPort),
							MatchingTeamMode.Solo.MaxUserCapacity() - 1, GlobalUserData.botDifficulty, 0,
							matchingToken);
						
						matchingSocket.RegisterCloseCallback(null);
						matchingSocket.Close();
						Singleton<GameEventLogger>.inst.SetGameMode(GameMode.SINGLE.ToString());
						
						Destroy(gameObject);
					});
			}
			catch (GameException ex)
			{
				MonoBehaviourInstance<Popup>.inst.Error(Ln.Get(ex.errorType.ToString()));
				LobbyService.UpdateLobbyState(LobbyState.Ready);
				CloseCharacterSelectWindow();
			}
		}


		public void StartTutorial(TutorialType tutorialType)
		{
			try
			{
				SingletonMonoBehaviour<GameAnalytics>.inst.CustomEvent("Tutorial Start", new Dictionary<string, object>
				{
					{
						"server",
						"release"
					},
					{
						"tutorial_Type",
						(int) tutorialType
					}
				});
				ValidateTutorial();
				TutorialSettingData tutorialSettingData = GameDB.tutorial.GetTutorialSettingData(tutorialType);
				CharacterSettingData characterSettingData =
					GameDB.tutorial.GetCharacterSettingData(tutorialSettingData.playerSettingDataCode);
				SingletonMonoBehaviour<AnimationEventService>.inst.AnimationCollection.LoadLobbyCharacter(
					characterSettingData.characterCode);
				Singleton<SoundControl>.inst.StopBGM();
				MatchingToken matchingToken = new MatchingToken(tutorialType.ConvertToMatchingMode(),
					MatchingTeamMode.Solo, MatchingRegion.None, "", Lobby.inst.User.UserNum, Lobby.inst.User.MMR,
					Lobby.inst.User.Nickname, characterSettingData.characterCode, 0,
					characterSettingData.equipmentWeapon, new List<int>(), 1, false,
					new Dictionary<EmotionPlateType, int>());
				bootstrap.LoadTutorial(tutorialType, string.Format("localhost:{0}", GameConstants.DefaultPort),
					tutorialSettingData.BotCount, tutorialSettingData.botDifficulty, matchingToken);
				matchingSocket.RegisterCloseCallback(null);
				matchingSocket.Close();
				LobbyService.UpdateLobbyState(LobbyState.MatchCompleted);
				Singleton<GameEventLogger>.inst.SetGameMode(matchingToken.matchingMode.GetGameMode().ToString());
				CommunityService.SetRichPresence(CommunityRichPresenceType.GAME_STATUS, "InTutorialGame");
				Destroy(gameObject);
			}
			catch (GameException ex)
			{
				MonoBehaviourInstance<Popup>.inst.Error(Ln.Get(ex.errorType.ToString()));
			}
		}


		public int IsSelectedCharacter(int characterCode)
		{
			foreach (KeyValuePair<long, MatchingUser> keyValuePair in matchingTeamUserInfos)
			{
				if (keyValuePair.Value.CharacterCode == characterCode)
				{
					return keyValuePair.Value.TeamSlot;
				}
			}

			return 0;
		}


		public bool IsPicked(int characterCode)
		{
			foreach (KeyValuePair<long, MatchingUser> keyValuePair in matchingTeamUserInfos)
			{
				if (keyValuePair.Value.CharacterCode == characterCode)
				{
					return keyValuePair.Value.Pick;
				}
			}

			return false;
		}


		private List<int> ConvertCharactersToIntList(List<long> characterList)
		{
			List<int> list = new List<int>();
			foreach (int num in (from c in GameDB.character.GetAllCharacterData()
				select c.code).ToList<int>())
			{
				int num2 = num / 64;
				long num3 = characterList.ElementAt(num2);
				int num4 = num - 64 * num2;
				long num5 = 1L << (num4 & 31);
				if (0L < (num3 & num5))
				{
					list.Add(num);
				}
			}

			return list;
		}


		private void SetFreeCharacters(List<long> freeCharacterList)
		{
			freeCharacters.Clear();
			if (freeCharacterList == null || freeCharacterList.Count == 0)
			{
				return;
			}

			foreach (int item in ConvertCharactersToIntList(freeCharacterList))
			{
				freeCharacters.Add(item);
			}
		}


		private void SetBanCharacter(List<long> banCharacterList)
		{
			banCharacters.Clear();
			if (banCharacterList == null || banCharacterList.Count == 0)
			{
				return;
			}

			foreach (int item in ConvertCharactersToIntList(banCharacterList))
			{
				banCharacters.Add(item);
			}
		}


		public bool IsFreeCharacter(int characterCode)
		{
			return freeCharacters.Exists(x => x == characterCode);
		}


		public bool IsBanCharacter(int characterCode)
		{
			return banCharacters.Exists(x => x == characterCode);
		}


		public void Exit()
		{
			if (!GlobalUserData.IsStandaloneMode())
			{
				return;
			}

			LobbyService.UpdateLobbyState(LobbyState.Ready);
			CloseCharacterSelectWindow();
		}


		public class MatchingUser
		{
			private int characterCode;
			private bool pick;
			private int skinCode;
			private int startingDataCode;

			public MatchingUser(int teamSlot, bool isMe, long userNum, string nickName, int characterCode,
				int startingDataCode, bool pick)
			{
				TeamSlot = teamSlot;
				IsMe = isMe;
				UserNum = userNum;
				NickName = nickName;
				Update(characterCode, startingDataCode, pick);
			}

			public int TeamSlot { get; }
			public bool IsMe { get; }
			public long UserNum { get; }
			public string NickName { get; }
			public int CharacterCode => characterCode;
			public int SkinCode {
				get
				{
					if (skinCode != 0)
					{
						return skinCode;
					}

					CharacterSkinData skinData = GameDB.character.GetSkinData(characterCode, 0);
					if (skinData != null)
					{
						return skinData.code;
					}

					return 0;
				}
			}

			public int StartingDataCode => startingDataCode;
			public bool Pick => pick;
			public int GetSkinDataCode()
			{
				return skinCode;
			}

			public void Update(int characterCode, int startingDataCode, bool pick)
			{
				this.characterCode = characterCode;
				this.startingDataCode = startingDataCode;
				this.pick = pick;
			}

			public void UpdateSkin(int skinCode)
			{
				this.skinCode = skinCode;
			}
		}
	}
}