using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class LobbyService : MonoBehaviourInstance<LobbyService>
	{
		private bool secretMatching;
		public LobbyState LobbyState => Lobby.inst.LobbyContext.lobbyState;
		public bool SecretMatching => secretMatching;

		private void LoadRegionSetting()
		{
			UpdateAccelerateChina(Singleton<LocalSetting>.inst.setting.accelerateChina);
			if (Enum.TryParse<MatchingRegion>(Singleton<LocalSetting>.inst.setting.serverRegion, out MatchingRegion region) &&
			    region != MatchingRegion.None && GameDB.platform.serverRegions.Exists(x => x.region == region))
			{
				SelectServerRegion(region);
				Log.H(string.Format("Selected default ServerRegion by LocalSetting : {0}", region));
				return;
			}

			SingletonMonoBehaviour<PingUtil>.inst.GetBestRegion(from x in GameDB.platform.serverRegions
				select x.region, delegate(MatchingRegion serverRegion, int bestMs)
				{
					if (serverRegion == MatchingRegion.None)
					{
						serverRegion = MatchingRegion.Seoul;
					}

					SelectServerRegion(serverRegion);
					SingletonMonoBehaviour<PingUtil>.inst.GetPingMs("49.234.245.133",
						delegate(int gaapPing)
						{
							UpdateAccelerateChina(gaapPing < bestMs &&
							                      Ln.GetCurrentLanguage() == SupportLanguage.ChineseSimplified);
						});
				});
			
			Log.H("Selected default ServerRegion by PingUtil.GetBestRegion");
		}


		public void InitLobby()
		{
			UpdateLobbyState(LobbyState.Ready);
			MonoBehaviourInstance<LobbyUI>.inst.InitLobbyUI();
			
			LoadRegionSetting();
			InitLobbyCursor();
			
			MonoBehaviourInstance<LobbyUI>.inst.UIEvent.SetAccountInfo(Lobby.inst.User.Nickname, Lobby.inst.User.Level,
				Lobby.inst.User.NeedXP);
			
			MonoBehaviourInstance<LobbyUI>.inst.UIEvent.SetBenefitByKakaoPcCafe(SingletonMonoBehaviour<KakaoPcService>
				.inst.BenefitByKakaoPcCafe);
			
			MonoBehaviourInstance<LobbyUI>.inst.UIEvent.OnNicknameChange(Lobby.inst.User.Nickname);
			MonoBehaviourInstance<LobbyUI>.inst.MainMenu.UpdateUserCurrency();
			
			Singleton<SoundControl>.inst.SetInGame(false);
			Singleton<SoundControl>.inst.PlayBGM("BGM_Lobby", true);
			
			Cursor.lockState = CursorLockMode.None;
			if (GlobalUserData.matchingMode != MatchingMode.Rank && GlobalUserData.matchingMode != MatchingMode.Normal)
			{
				GlobalUserData.matchingMode = MatchingMode.Normal;
			}

			GlobalUserData.botDifficulty = BotDifficulty.NORMAL;
		}


		public void InitDailyMissions()
		{
			MonoBehaviourInstance<LobbyUI>.inst.UIEvent.SetDailyMissions();
		}


		public void RequestStartMatching()
		{
			if (CommunityService.HasLobby())
			{
				CommunityService.StartMatching();
				return;
			}

			MonoBehaviourInstance<MatchingService>.inst.StartMatching(Lobby.inst.User.UserNum.ToString(), 1);
		}
		
		public void SelectCharacter(int characterCode)
		{
			SingletonMonoBehaviour<AnimationEventService>.inst.AnimationCollection.LoadLobbyCharacter(characterCode);
		}
		
		public void ChangeNickname(string nicknameText, Action<RestErrorType, string, string> callback)
		{
			if (LobbyState == LobbyState.Ready)
			{
				RequestDelegate.request<NicknameResult>(LobbyApi.setupNickname(nicknameText), false,
					delegate(RequestDelegateError err, NicknameResult res)
					{
						if (err != null)
						{
							callback(err.errorType, err.message, null);
							return;
						}

						Lobby.inst.User.SetNickname(res.nickname);
						PlayerPrefs.SetString("_dev_nickname", Lobby.inst.User.Nickname);
						MonoBehaviourInstance<LobbyUI>.inst.UIEvent.OnNicknameChange(nicknameText);
						callback(RestErrorType.SUCCESS, "", res.nickname);
					});
			}
		}

		public void PurchaseChangeNickname(string newNickname, Action<RestErrorType, string, string> callback)
		{
			if (LobbyState == LobbyState.Ready)
			{
				RequestDelegate.request<NicknameResult>(ProductApi.PurchaseChangeNickname(newNickname), false,
					delegate(RequestDelegateError err, NicknameResult res)
					{
						if (err != null)
						{
							callback(err.errorType, err.message, null);
							return;
						}

						Lobby.inst.User.SetNickname(res.nickname);
						PlayerPrefs.SetString("_dev_nickname", Lobby.inst.User.Nickname);
						MonoBehaviourInstance<LobbyUI>.inst.UIEvent.OnNicknameChange(newNickname);
						callback(RestErrorType.SUCCESS, "", res.nickname);
					});
			}
		}

		public void SelectServerRegion(MatchingRegion region)
		{
			GlobalUserData.matchingRegion = region;
			MonoBehaviourInstance<LobbyUI>.inst.UIEvent.OnChangeMatchingRegion(region);
			Singleton<LocalSetting>.inst.setting.serverRegion = region.ToString();
			Singleton<LocalSetting>.inst.Save();
		}
		
		public void UpdateAccelerateChina(bool isOn)
		{
			if (Lobby.inst.LobbyContext.accelerateChina != isOn)
			{
				Lobby.inst.LobbyContext.accelerateChina = isOn;
				MonoBehaviourInstance<LobbyUI>.inst.UIEvent.OnChangeAccelerateChina(isOn);
				Singleton<LocalSetting>.inst.setting.accelerateChina = isOn;
				Singleton<LocalSetting>.inst.Save();
			}
		}

		private void InitLobbyCursor()
		{
			Cursor.SetCursor(Resources.Load<Texture2D>("UI/Images/Cursor/Cursor_01"), Vector2.zero,
				UnityEngine.CursorMode.Auto);
		}

		public static void UpdateLobbyState(LobbyState lobbyState)
		{
			Lobby.inst.LobbyContext.lobbyState = lobbyState;
			LobbyUI lobbyUi = MonoBehaviourInstance<LobbyUI>.inst;
			if (lobbyUi != null)
			{
				lobbyUi.UIEvent.OnLobbyStateUpdate(lobbyState);
			}

			SetLobbyRichPresence(lobbyState);
		}


		public static void SetLobbyRichPresence(LobbyState lobbyState)
		{
			if (Lobby.inst.User == null)
			{
				return;
			}

			if (!Lobby.inst.User.GetTutorialClearState(TutorialType.BasicGuide))
			{
				CommunityService.SetRichPresence(CommunityRichPresenceType.GAME_STATUS, "LumiaIsland");
				return;
			}

			switch (lobbyState)
			{
				case LobbyState.None:
				case LobbyState.TutorialStart:
					break;
				case LobbyState.Ready:
					CommunityService.SetRichPresence(CommunityRichPresenceType.GAME_STATUS, "InLobby");
					return;
				case LobbyState.Matching:
					if (inst.SecretMatching)
					{
						return;
					}

					CommunityService.SetRichPresence(CommunityRichPresenceType.GAME_STATUS, "InMatching");
					return;
				case LobbyState.MatchCompleted:
					CommunityService.SetRichPresence(CommunityRichPresenceType.GAME_STATUS, "MatchCompleted");
					return;
				case LobbyState.CustomGameRoom:
					CommunityService.SetRichPresence(CommunityRichPresenceType.GAME_STATUS, "InCustomLobby");
					break;
				default:
					return;
			}
		}

		public void StartSecretMatching()
		{
			secretMatching = true;
		}

		public void CancelSecretMatching()
		{
			secretMatching = false;
		}

		public bool StartSecretMatching(MatchingMode matchingMode, MatchingTeamMode matchingTeamMode,
			Action<string, List<bool>> impossibleExistCallback)
		{
			if (Lobby.inst.LobbyContext.lobbyState != LobbyState.Ready)
			{
				if (Lobby.inst.LobbyContext.lobbyState == LobbyState.Matching)
				{
					secretMatching = false;
					if (CommunityService.HasLobby() && !CommunityService.IsLobbyOwner())
					{
						return false;
					}

					MonoBehaviourInstance<MatchingService>.inst.StopMatchingWithPopup();
				}

				return false;
			}

			if (MonoBehaviourInstance<Popup>.inst.IsOpen)
			{
				return false;
			}

			if (matchingMode == MatchingMode.Rank && Lobby.inst.CheckRankPenalty())
			{
				return false;
			}

			if (CommunityService.HasLobby())
			{
				if (!CommunityService.IsLobbyOwner())
				{
					return false;
				}

				switch (matchingTeamMode)
				{
					case MatchingTeamMode.Solo:
						if (1 < CommunityService.LobbyMemberCount)
						{
							return false;
						}

						break;
					case MatchingTeamMode.Duo:
						if (2 < CommunityService.LobbyMemberCount)
						{
							return false;
						}

						break;
					case MatchingTeamMode.Squad:
						if (3 < CommunityService.LobbyMemberCount)
						{
							return false;
						}

						break;
					default:
						return false;
				}

				if (!CommunityService.IsAllMemberReady())
				{
					return false;
				}
			}

			string teamModeName = null;
			int modeIndex = 0;
			if (matchingTeamMode - MatchingTeamMode.Solo <= 2)
			{
				teamModeName = matchingTeamMode.ToString();
				modeIndex = matchingTeamMode - MatchingTeamMode.Solo;
			}

			if (!MonoBehaviourInstance<LobbyClient>.inst.CheckRegionSchedule(matchingMode, teamModeName, modeIndex,
				impossibleExistCallback))
			{
				return false;
			}

			secretMatching = true;
			
			GlobalUserData.matchingMode = matchingMode;
			GlobalUserData.matchingTeamMode = matchingTeamMode;
			
			MonoBehaviourInstance<MatchingService>.inst.BeforeMatching(CommunityService.GetMemberUserNumList());
			return true;
		}
	}
}