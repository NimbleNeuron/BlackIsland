using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Blis.Common;
using Blis.Common.Utils;
using Microsoft.Win32;
using Neptune.Http;
using UnityEngine;

namespace Blis.Client
{
	public class LobbyClient : MonoBehaviourInstance<LobbyClient>
	{
		private static DateTime lastLoginTime;
		public TimeZoneInfo timezoneInfo;

		[SerializeField] private bool useDevLogin = default;
		[HideInInspector] public bool doneDevLogin = default;

		private readonly List<MatchingRegionSchedule> matchingRegionSchedules = new List<MatchingRegionSchedule>();
		private MatchingResult reconnectMatchingResult;

		private bool RegisterProxySetting_Internal()
		{
			try
			{
				RegistryKey registryKey =
					Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings",
						true);
				if (registryKey == null)
				{
					registryKey =
						Registry.CurrentUser.CreateSubKey(
							"Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings");
				}

				if (registryKey != null)
				{
					registryKey.SetValue("ProxyEnable", 0, RegistryValueKind.DWord);
				}
			}
			catch (Exception ex)
			{
				Log.E("Registry Key Setting Failed. " + ex.Message);
				return false;
			}
			
			return true;
		}

		private bool ConfigureTimeZone_Internal()
		{
			try
			{
				timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById("ER Standard Time");
			}
			catch (TimeZoneNotFoundException)
			{
				string displayName = "(GMT+09:00) ER Standard Time";
				string text = "ER Standard Time";
				TimeSpan baseUtcOffset = new TimeSpan(9, 0, 0);
				timezoneInfo = TimeZoneInfo.CreateCustomTimeZone(text, baseUtcOffset, displayName, text);
			}

			return true;
		}

		private void Start()
		{
			if (!RegisterProxySetting_Internal()) return;
			if (!ConfigureTimeZone_Internal()) return;

			// 기본 언어 설정 가져오기
			SingletonMonoBehaviour<LnLoader>.inst.LoadDefaultLangaugeData();
			SingletonMonoBehaviour<LnLoader>.inst.LoadData();
			
			// 로비 시작
			this.StartThrowingCoroutine(StartLobby(), OnExeption);

			void OnExeption(Exception exception)
			{
				Log.E("[EXCEPTION][StartLobby] Message:" + exception.Message + ", StackTrace:" +
				      exception.StackTrace);
			}
		}

		protected override void _Awake()
		{
			base._Awake();
			
			RequestDelegate request = SingletonMonoBehaviour<RequestDelegate>.inst;
			request.OnServerDisruption += OnServerCommunicateDisruption;
			
			Log.V($"[GameVersion] {BSERVersion.VERSION}");
			
			// 윈도우 api 컨트롤
			WindowController.Init();
		}

		protected override void _OnDestroy()
		{
			base._OnDestroy();
			
			RequestDelegate request = SingletonMonoBehaviour<RequestDelegate>.inst;
			request.OnServerDisruption -= OnServerCommunicateDisruption;
		}
		
		private IEnumerator StartLobby()
		{
			LocalSetting localSetting = Singleton<LocalSetting>.inst;
			
			SingletonMonoBehaviour<ArchCrashlytics>.inst.Init(BSERVersion.VERSION);
			// SingletonMonoBehaviour<GameAnalytics>.inst.Init(BSERVersion.VERSION);
			
			float maxWaitTime = 0f;
			while (!SteamManager.Initialized)
			{
				yield return new WaitForSeconds(0.1f);
				maxWaitTime += 0.1f;
				if (maxWaitTime >= 10f)
				{
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("ServerError/2001"), new Popup.Button
					{
						text = Ln.Get("닫기"),
						callback = Application.Quit,
						type = Popup.ButtonType.Cancel
					});
					
					yield break;
				}
			}

			SingletonMonoBehaviour<GameAnalytics>.inst.CustomEvent("Login", new Dictionary<string, object>
			{
				{
					"server",
					"release"
				}
			});
			
			HttpRequestFactory.SetHeader("X-BSER-Version", BSERVersion.VERSION);
			
			yield return Login();
			
			// 게임 심의 등급 보이기
			yield return SingletonMonoBehaviour<KoreaGameGradeUtil>.inst.ShowLobbyStart();
			
			Singleton<ItemService>.inst.SetLevelData(GameDB.level.DefaultLevel);
			
			// 비속어 필터
			SingletonMonoBehaviour<SwearWordManager>.inst.LoadSwearWords();
			
			// 사운드 모듈
			Singleton<SoundControl>.inst.Init();
			
			// 아틀라스 로드
			SingletonMonoBehaviour<ResourceManager>.inst.LoadAtlas("UI/Atlas");
			
			// 로컬 데이터베이스 로드, 업데이트
			yield return SingletonMonoBehaviour<GameDBLoader>.inst.Load(GameConstants.GetDataCacheFilePath());
			if (!SingletonMonoBehaviour<GameDBLoader>.inst.Result.success)
			{
				Log.E("Failed to load GameDB, Error: {0}", SingletonMonoBehaviour<GameDBLoader>.inst.Result.reason);
				MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("게임 데이터를 불러오는데 실패했습니다."), GameUtil.ExitGame);
				yield break;
			}

			// 리포팅용 플레이어 메타 데이터
			SingletonMonoBehaviour<GameAnalytics>.inst.SetUserMetaData();
			SingletonMonoBehaviour<GameAnalytics>.inst.UpdateGraphicsMetaData();
			SingletonMonoBehaviour<GameAnalytics>.inst.UpdateActiveScene();
			
			// 배너 로딩
			yield return SingletonMonoBehaviour<BannerLoader>.inst.Load(GameConstants.GetDataCacheFilePath());
			
			// 게임 결과 - 있을때만
			BattleApi.BattleResult battleResult = null;
			if (!string.IsNullOrEmpty(GlobalUserData.battleResultKey))
			{
				string battleResultKey = GlobalUserData.battleResultKey;
				yield return RequestDelegate.requestCoroutine<BattleApi.BattleResult>(
					BattleApi.GetBattleResult(battleResultKey),
					delegate(RequestDelegateError err, BattleApi.BattleResult res)
					{
						if (err != null)
						{
							MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
							return;
						}

						Log.V("[BattleResult] mmr:{0} | mmrGain: {1}", res.mmr, res.mmrGain);
						battleResult = res;
						SingletonMonoBehaviour<KakaoPcService>.inst.SetBenefitByKakaoPcCafe(res.benefitByKakaoPcCafe);
					});
			}

			// 로비 입장
			yield return RequestDelegate.requestCoroutine<LobbyApi.EnterLobbyResult>(LobbyApi.EnterLobby(),
				delegate(RequestDelegateError err, LobbyApi.EnterLobbyResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					// 유저 세팅
					SingletonMonoBehaviour<UserService>.inst.SetUser(res.user);
					
					Log.V(string.Format("[User] {0}({1})", SingletonMonoBehaviour<UserService>.inst.User.Nickname,
						SingletonMonoBehaviour<UserService>.inst.User.UserNum));
					
					// 유저 정보로 로비 초기화
					Lobby.Init(SingletonMonoBehaviour<UserService>.inst.User);
					
					// 스팀 스테이트 설정
					CommunityService.SetRichPresence(CommunityRichPresenceType.STEAM_DISPLAY, "#StatusSimple");
					CommunityService.SetRichPresence(CommunityRichPresenceType.NICK_NAME,
						SingletonMonoBehaviour<UserService>.inst.User.Nickname);
					
					if (GlobalUserData.myLevel == 0)
					{
						GlobalUserData.myLevel = Lobby.inst.User.Level;
					}

					if (battleResult != null)
					{
						Lobby.inst.User.SetUserMMR(battleResult.mmr);
						Lobby.inst.User.SetUserGainMMR(battleResult.mmrGain);
						Lobby.inst.User.SetUserGainAP(battleResult.rewardCoin);
						Lobby.inst.User.SetUserGainXP(battleResult.gainExp);
						Lobby.inst.User.SetUserNeedExp(battleResult.userNeedExp);
						Lobby.inst.User.SetUserLevel(battleResult.userLevel);
						Lobby.inst.User.SetLastBatchMode(battleResult.lastBatchMode);
						Lobby.inst.User.SetBatchMode(battleResult.batchMode);
						Lobby.inst.User.SetBeforeTierChangeType(battleResult.beforeTierChangeType);
						Lobby.inst.User.SetAfterTierChangeType(battleResult.afterTierChangeType);
						Lobby.inst.User.SetBeforeTierType(battleResult.beforeTierType);
						Lobby.inst.User.SetBeforeTierGrade(battleResult.beforeTierGrade);
						Lobby.inst.User.SetAfterTierType(battleResult.afterTierType);
						Lobby.inst.User.SetAfterTierGrade(battleResult.afterTierGrade);
					}

					Lobby.inst.SetCharacterList(res.characterList);
					Lobby.inst.SetSkinList(res.skinList);
					Lobby.inst.SetEmotionList(res.emotionList);
					Lobby.inst.SetFreeRotation(res.freeRotation);
					Lobby.inst.SetRankPromotion(res.battleUsers);
					Lobby.inst.SetNormalMatchingPenaltyTime(res.normalMatchingPenaltyTime);
					Lobby.inst.SetCurrentRankingSeason(res.rankingSeason);
					Lobby.inst.SetRankMatchingPenaltyTime(res.rankMatchingPenaltyTime);
					Lobby.inst.SetServerTime(res.serverTime);
					Lobby.inst.SetClientTime();
					Lobby.inst.User.SetFreeNicknameChange(res.freeNicknameChange);
					Lobby.inst.User.SetCurrency(Ln.GetCurrentLanguage().GetCurrency());
					
					SingletonMonoBehaviour<KakaoPcService>.inst.SetBenefitByKakaoPcCafe(res.benefitByKakaoPcCafe);
					
					// 로비 초기화
					MonoBehaviourInstance<LobbyService>.inst.InitLobby();
					// 매칭 서비스 초기화
					MonoBehaviourInstance<MatchingService>.inst.Init();
					
					matchingRegionSchedules.Clear();
					matchingRegionSchedules.AddRange(res.matchingRegionSchedule);
					reconnectMatchingResult = res.matchingResult;
					
					if (reconnectMatchingResult != null &&
					    GlobalUserData.finishedBattleTokenKeys.Contains(reconnectMatchingResult.battleTokenKey))
					{
						Log.W("[Login] Finished MatchingResult. {0}", reconnectMatchingResult.battleTokenKey);
						reconnectMatchingResult = null;
					}

					if (reconnectMatchingResult != null)
					{
						MonoBehaviourInstance<LobbyUI>.inst.ReconnectPopup.Open();
					}
				});
			
			// 일일 미션 결과 업데이트
			yield return RequestDelegate.requestCoroutine<MissionApi.DailyMissionsResult>(MissionApi.GetDailyMissions(),
				delegate(RequestDelegateError err, MissionApi.DailyMissionsResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					GlobalUserData.userDailyMissions = res.userMissions;
					Lobby.inst.SetDailyMissionRefreshFlag(res.changeable);
					MonoBehaviourInstance<LobbyService>.inst.InitDailyMissions();
				});
			
			// 알림 서비스 초기화
			NoticeService.Init();
			NoticeService.RequestAll(delegate
			{
				MonoBehaviourInstance<LobbyUI>.inst.MainMenu.EnableNoticeRedDot(NoticeService.AnyNewNotice() ||
					NoticeService.AnyNewGiftMail());
			});
			
			// 로비 채팅
			MonoBehaviourInstance<LobbyUI>.inst.MainMenu.CommunityHud.AddSystemChatting(Ln.Get("게임문화개선캠페인"));
			
			// 게임 심의 등급 로고 재생
			SingletonMonoBehaviour<KoreaGameGradeUtil>.inst.GameGradeLogoPlay();
			
			// 데이터 클리어
			GlobalUserData.dicPlayerResults.Clear();
			GlobalUserData.battleResultKey = string.Empty;
			GlobalUserData.customGameRoomKey = string.Empty;
		}


		public static void ResetUser()
		{
			SingletonMonoBehaviour<UserService>.inst.SetUser(null);
		}

		private IEnumerator Login()
		{
			// 이미 유저 정보가 있으면 로그인 안 함
			if (SingletonMonoBehaviour<UserService>.inst.User != null)
			{
				Lobby.Init(SingletonMonoBehaviour<UserService>.inst.User);
				yield break;
			}

			// 스팀 로그인
			AuthProvider authProvider;
			if (SteamManager.Initialized && !useDevLogin)
			{
				authProvider = AuthProvider.STEAM;
				CommunityService.Init();
				
				Log.V("STEAM Login");
			}
			else
			{
				authProvider = AuthProvider.GUEST;
				Log.V("GUEST Login");
			}

			Log.H(string.Format("AuthProvider : {0}", authProvider));
			
			// 인증 토큰
			AuthToken authToken = null;
			do
			{
				AuthTokenAcquirer authTokenAcquirer = AuthTokenAcquirer.Create(authProvider);
				yield return authTokenAcquirer.FetchToken();
				
				if (authTokenAcquirer.HasError() || authTokenAcquirer.IsTimeOut())
				{
					Log.E("토큰 발급 실패 : " + authTokenAcquirer.GetError());
					yield return MonoBehaviourInstance<Popup>.inst.Message(
						Ln.Get("로그인 실패"),
						new Popup.Button
						{
							text = Ln.Get("재시도")
						});
				}
				else
				{
					authToken = authTokenAcquirer.GetToken();
				}
			} while (authToken == null);

			// 로그인 시도
			bool loginSuccess = false;
			do
			{
				WaitForTrigger loginTrigger = new WaitForTrigger();

				RequestDelegate.request<AuthResult>(
					LobbyApi.authenticate(GetAuthParam(authProvider, authToken)), false,
					(err, res) =>
					{
						// 뭔가 에러
						if (err != null)
						{
							Log.V("[Login] Failed. {0}", (object) err.errorType);

							MonoBehaviourInstance<Popup>.inst.Message(
								Ln.Get(string.Format("ServerError/{0}", (int) err.errorType)),
								new Popup.Button
								{
									text = Ln.Get("확인"),
									callback = (Action) (() => loginTrigger.ActiveTrigger())
								});
						}
						else
						{
							// 성공
							Log.V("[Login] SUCCESS");

							loginSuccess = true;
							lastLoginTime = DateTime.Now;

							HttpRequestFactory.SetHeader("X-BSER-SessionKey", res.sessionKey);

							GlobalUserData.gaap = res.gaap;
							GlobalUserData.showGrade = res.showGrade;

							loginTrigger.ActiveTrigger();
						}
					});

				yield return loginTrigger;
			} while (!loginSuccess);
		}


		private AuthParam GetAuthParam(AuthProvider authProvider, AuthToken authToken)
		{
			return new AuthParam
			{
				authProvider = authProvider.ToString(),
				idToken = useDevLogin
					? PlayerPrefs.GetString("_dev_id_token")
					: authToken.GetAttributesMap()["authorizationCode"],
				machineNum = SystemInfo.deviceUniqueIdentifier,
				paramMap = authToken.GetAttributesMap(),
				appVersion = BSERVersion.VERSION,
				appLanguageCode = Ln.GetCurrentLanguage().GetISO639_1(),
				deviceLanguageCode = Application.systemLanguage.ToSupportLanguage().GetAppLanguageCode(),
				geoLocationCode = "ko"
			};
		}

		private void OnServerCommunicateDisruption(Action retryAction)
		{
			RequestDelegate.request<Maintenance>(HttpRequestFactory.Get(ApiConstants.MaintenaceUrl), true,
				(serverError, serverResult) =>
				{
					if (serverResult != null && serverResult.start)
					{
						MonoBehaviourInstance<Popup>.inst.Message("OutOfService_Temp", serverResult.endDtm,
							new Popup.Button
							{
								text = Ln.Get("기본 커뮤니티"),
								callback = (Action) (() => Application.OpenURL(Ln.Get("점검 중 링크"))),
								type = Popup.ButtonType.Link
							}, new Popup.Button
							{
								text = Ln.Get("닫기"),
								callback = Exit,
								type = Popup.ButtonType.Cancel
							});
					}
					else
					{
						MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("서버와연결할수없음"), () =>
						{
							if (Lobby.inst == null ||
							    MonoBehaviourInstance<LobbyService>.inst.LobbyState == LobbyState.None)
							{
								return;
							}

							LobbyService.UpdateLobbyState(LobbyState.Ready);
						}, new Popup.Button
						{
							text = Ln.Get("해결방법"),
							callback = (Action) (() => Application.OpenURL(Ln.Get("해결방법링크"))),
							type = Popup.ButtonType.Link
						}, new Popup.Button
						{
							text = Ln.Get("재시도"),
							callback = (Action) (() =>
							{
								Action action = retryAction;
								if (action == null)
								{
									return;
								}

								action();
							}),
							type = Popup.ButtonType.Confirm
						});
					}
				}, false);
		}

		private static void Exit()
		{
			Application.Quit();
		}


		public void Reconnect()
		{
			if (reconnectMatchingResult != null)
			{
				Singleton<SoundControl>.inst.StopBGM();
				SingletonMonoBehaviour<Bootstrap>.inst.LoadClient(reconnectMatchingResult,
					SingletonMonoBehaviour<UserService>.inst.User.UserNum,
					Singleton<LocalSetting>.inst.setting.accelerateChina, true);
				
				Singleton<GameEventLogger>.inst.SetRoomKey(reconnectMatchingResult.battleTokenKey);
				Singleton<GameEventLogger>.inst.SetGameMode(reconnectMatchingResult.matchingMode.GetGameMode()
					.ToString());
				
				GlobalUserData.matchingMode = reconnectMatchingResult.matchingMode;
				GlobalUserData.matchingTeamMode = reconnectMatchingResult.matchingTeamMode;
				
				reconnectMatchingResult = null;
			}
		}


		public bool CheckRegionSchedule(MatchingMode matchingMode, string teamModeName, int modeIndex,
			Action<string, List<bool>> impossibleExistCallback)
		{
			List<bool> matchingRegionSchedule = GetMatchingRegionSchedule(matchingMode);
			bool flag = false;
			if (teamModeName == null)
			{
				flag = true;
			}
			else if ((teamModeName.Equals("Solo") || teamModeName.Equals("Duo") || teamModeName.Equals("Squad")) &&
			         !matchingRegionSchedule[modeIndex])
			{
				flag = true;
			}

			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder(Ln.Get("잘못된 매칭 모드"));
				List<string> list = new List<string>();
				if (0 < matchingRegionSchedule.Count && matchingRegionSchedule[0])
				{
					list.Add("Solo");
				}

				if (1 < matchingRegionSchedule.Count && matchingRegionSchedule[1])
				{
					list.Add("Duo");
				}

				if (2 < matchingRegionSchedule.Count && matchingRegionSchedule[2])
				{
					list.Add("Squad");
				}

				if (list.Count == 0)
				{
					stringBuilder.Append(Ln.Get("없음"));
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (i > 0)
						{
							stringBuilder.Append(", ");
						}
						else
						{
							stringBuilder.Append(" ");
						}

						list[i] = LnUtil.GetMatchingTeamModeName(list[i]);
						stringBuilder.Append(list[i]);
					}
				}

				if (impossibleExistCallback != null)
				{
					impossibleExistCallback(stringBuilder.ToString(), matchingRegionSchedule);
				}
			}

			return !flag;
		}


		public List<bool> GetMatchingRegionSchedule(MatchingMode matchingMode)
		{
			List<bool> list = new List<bool>();
			bool item = false;
			bool item2 = false;
			bool item3 = false;
			foreach (MatchingRegionSchedule matchingRegionSchedule in matchingRegionSchedules)
			{
				if (Singleton<LocalSetting>.inst.setting.serverRegion != null &&
				    Singleton<LocalSetting>.inst.setting.serverRegion.Equals(matchingRegionSchedule.matchingRegion
					    .ToString()) && matchingMode == matchingRegionSchedule.matchingMode)
				{
					if (matchingRegionSchedule.matchingTeamMode == MatchingTeamMode.Solo)
					{
						item = IsEnableMatchingTime(matchingRegionSchedule);
					}
					else if (matchingRegionSchedule.matchingTeamMode == MatchingTeamMode.Duo)
					{
						item2 = IsEnableMatchingTime(matchingRegionSchedule);
					}
					else if (matchingRegionSchedule.matchingTeamMode == MatchingTeamMode.Squad)
					{
						item3 = IsEnableMatchingTime(matchingRegionSchedule);
					}
				}
			}

			list.Add(item);
			list.Add(item2);
			list.Add(item3);
			return list;
		}


		private bool IsEnableMatchingTime(MatchingRegionSchedule schedule)
		{
			DateTime target = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), timezoneInfo);
			return
				schedule.mon && target.DayOfWeek == DayOfWeek.Monday &&
				IsInTimeBetween(target, schedule.monStartTime, schedule.monEndTime) ||
				schedule.tue && target.DayOfWeek == DayOfWeek.Tuesday &&
				IsInTimeBetween(target, schedule.tueStartTime, schedule.tueEndTime) ||
				schedule.wed && target.DayOfWeek == DayOfWeek.Wednesday &&
				IsInTimeBetween(target, schedule.wedStartTime, schedule.wedEndTime) ||
				schedule.thu && target.DayOfWeek == DayOfWeek.Thursday &&
				IsInTimeBetween(target, schedule.thuStartTime, schedule.thuEndTime) ||
				schedule.fri && target.DayOfWeek == DayOfWeek.Friday &&
				IsInTimeBetween(target, schedule.friStartTime, schedule.friEndTime) ||
				schedule.sat && target.DayOfWeek == DayOfWeek.Saturday &&
				IsInTimeBetween(target, schedule.satStartTime, schedule.satEndTime) || schedule.sun &&
				target.DayOfWeek == DayOfWeek.Sunday &&
				IsInTimeBetween(target, schedule.sunStartTime, schedule.sunEndTime);
		}


		private bool IsInTimeBetween(DateTime target, TimeSpan start, TimeSpan end)
		{
			DateTime dateTime = target.Date + start;
			DateTime dateTime2 = target.Date + end;
			if (dateTime2 < dateTime)
			{
				if (target < dateTime2)
				{
					dateTime = dateTime.AddDays(-1.0);
				}
				else
				{
					dateTime2 = dateTime2.AddDays(1.0);
				}
			}

			return dateTime <= target && target <= dateTime2;
		}
	}
}