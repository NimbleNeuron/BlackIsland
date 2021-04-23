using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

namespace Blis.Client
{
	public static class CommunityService
	{
		public delegate void OnCommunityEvent();
		public delegate void OnCompleteMatching(MatchingResult result);
		public delegate void OnUpdateFriendInfo(CSteamID steamdID);

		private const int MaxLobbyMemberCount = 3;

		private static readonly Dictionary<CommunityRichPresenceType, string> richPresenceKeyMap =
			new Dictionary<CommunityRichPresenceType, string>
			{
				{
					CommunityRichPresenceType.STEAM_DISPLAY,
					"steam_display"
				},
				{
					CommunityRichPresenceType.STEAM_PLAYER_GROUP,
					"steam_player_group"
				},
				{
					CommunityRichPresenceType.STEAM_PLAYER_GROUP_SIZE,
					"steam_player_group_size"
				},
				{
					CommunityRichPresenceType.NICK_NAME,
					"nickname"
				},
				{
					CommunityRichPresenceType.GAME_STATUS,
					"gamestatus"
				},
				{
					CommunityRichPresenceType.GAME_MODE,
					"gamemode"
				}
			};


		private static CSteamID steamIDLobby;
		private static SteamFriendInfo myInfo;

		private static List<SteamFriendInfo> friendInfoList = new List<SteamFriendInfo>();
		private static List<SteamFriendInfo> onlineFriendInfoList = new List<SteamFriendInfo>();
		private static readonly List<SteamLobbyMemberInfo> lobbyMemberInfoList = new List<SteamLobbyMemberInfo>();
		private static readonly List<SteamFriendInfo> requestUserInfoList = new List<SteamFriendInfo>();
		private static readonly List<ChattingUI.ChattingInfo> lobbyMemberChat = new List<ChattingUI.ChattingInfo>();

		private static Dictionary<ulong, Texture2D> steamAvatarMap = new Dictionary<ulong, Texture2D>();

		private static readonly Queue<LobbyInvite_t> inviteRequests = new Queue<LobbyInvite_t>();

		private static CSteamID inviteSteamID;
		private static float lastRefreshTime = float.MinValue;
		private static bool initialized;

		// Note: this type is marked as 'beforefieldinit'.
		static CommunityService()
		{
			inviteLobbyEvent = delegate { };
			updateGroupEvent = delegate { };
			enterLobbyEvent = delegate { };
			updateChatMsgEvent = delegate { };
			onCompleteMatching = delegate { };
			onUpdateRichPresence = delegate { };
			onUpdatePersonaState = delegate { };
			initialized = false;
		}


		private static SteamFriendInfo MyInfo {
			get
			{
				if (myInfo == null)
				{
					myInfo = SteamApi.GetFriendInfo(SteamApi.GetSteamID(), richPresenceKeyMap);
					SteamApi.GetAvatar(myInfo.steamID, ref steamAvatarMap);
				}

				return myInfo;
			}
		}


		public static CSteamID MySteamID {
			get
			{
				SteamFriendInfo steamFriendInfo = MyInfo;
				if (steamFriendInfo == null)
				{
					return CSteamID.Nil;
				}

				return steamFriendInfo.steamID;
			}
		}


		public static int LobbyMemberCount {
			get
			{
				List<SteamLobbyMemberInfo> list = lobbyMemberInfoList;
				if (list == null)
				{
					return 0;
				}

				return list.Count;
			}
		}


		
		
		public static event OnCommunityEvent inviteLobbyEvent;
		public static event OnCommunityEvent updateGroupEvent;
		public static event OnCommunityEvent enterLobbyEvent;
		public static event OnCommunityEvent updateChatMsgEvent;
		public static event OnCompleteMatching onCompleteMatching;
		public static event OnUpdateFriendInfo onUpdateRichPresence;
		public static event OnUpdateFriendInfo onUpdatePersonaState;

		public static void Init()
		{
			if (initialized)
			{
				return;
			}

			initialized = true;
			SteamApi.callback_LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
			SteamApi.callback_LobbyInvite = Callback<LobbyInvite_t>.Create(OnLobbyInvite);
			SteamApi.callback_LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
			SteamApi.callback_LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
			SteamApi.callback_FriendRichPresenceUpdate =
				Callback<FriendRichPresenceUpdate_t>.Create(OnFriendRichPresenceUpdate);
			SteamApi.callback_PersonaStateChange = Callback<PersonaStateChange_t>.Create(OnPersonaStateChange);
			SteamApi.callback_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
			SteamApi.callback_LobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMsg);
			RefreshFriendInfo();
		}


		public static void RefreshFriendInfo()
		{
			if (!SteamManager.Initialized)
			{
				return;
			}

			if (Time.time - lastRefreshTime < 5f)
			{
				myInfo = SteamApi.GetFriendInfo(SteamApi.GetSteamID(), richPresenceKeyMap);
				SteamApi.GetAvatar(myInfo.steamID, ref steamAvatarMap);
				return;
			}

			lastRefreshTime = Time.time;
			friendInfoList = SteamApi.GetFriends(richPresenceKeyMap);
			friendInfoList.Sort(delegate(SteamFriendInfo x, SteamFriendInfo y)
			{
				int num = x.personaState.CompareTo(y.personaState);
				if (num != 0)
				{
					return num;
				}

				bool flag = x.GameStatus.IsEmpty<char>();
				bool flag2 = y.GameStatus.IsEmpty<char>();
				if (flag == flag2)
				{
					return 0;
				}

				if (!flag)
				{
					return -1;
				}

				return 1;
			});
			RefreshOnlineFriendInfo();
			SteamApi.GetFriendAvatars(false, ref steamAvatarMap);
		}


		public static void RefreshOnlineFriendInfo()
		{
			if (friendInfoList != null && friendInfoList.Count > 0)
			{
				onlineFriendInfoList = friendInfoList.FindAll(x => x.personaState != 2);
			}

			if (onlineFriendInfoList == null || onlineFriendInfoList.Count == 0)
			{
				return;
			}

			onlineFriendInfoList.Sort(delegate(SteamFriendInfo x, SteamFriendInfo y)
			{
				int num = x.personaState.CompareTo(y.personaState);
				if (num != 0)
				{
					return num;
				}

				bool flag = x.GameStatus.IsEmpty<char>();
				bool flag2 = y.GameStatus.IsEmpty<char>();
				if (flag == flag2)
				{
					return 0;
				}

				if (!flag)
				{
					return -1;
				}

				return 1;
			});
		}


		public static bool IsMe(CSteamID steamID)
		{
			return SteamManager.Initialized && steamID.IsValid() && steamID.Equals(MySteamID);
		}


		private static bool IsValidSteamLobbyID()
		{
			return steamIDLobby.IsValid();
		}


		public static bool HasLobby()
		{
			return IsValidSteamLobbyID() && 0 < lobbyMemberInfoList.Count;
		}


		public static bool IsLobbyOwner()
		{
			return SteamManager.Initialized && HasLobby() && IsLobbyOwner(MySteamID);
		}


		public static bool IsLobbyMember(CSteamID steamId)
		{
			return HasLobby() && GetLobbyMemberInfo(steamId) != null;
		}


		public static bool IsLobbyOwner(CSteamID steamID)
		{
			return SteamManager.Initialized && steamID.IsValid() && HasLobby() &&
			       steamID.Equals(SteamApi.GetLobbyOwner(steamIDLobby));
		}


		public static bool IsAllMemberReady()
		{
			foreach (SteamLobbyMemberInfo steamLobbyMemberInfo in lobbyMemberInfoList)
			{
				if (!IsMe(steamLobbyMemberInfo.steamID))
				{
					if (SteamApi.RequestUserInformation(steamLobbyMemberInfo.steamID, true))
					{
						return false;
					}

					if (!SteamApi.GetFriendRichPresence(steamLobbyMemberInfo.steamID,
						richPresenceKeyMap[CommunityRichPresenceType.GAME_STATUS]).Equals("InLobby"))
					{
						return false;
					}
				}
			}

			return true;
		}


		public static int GetFriendCount()
		{
			List<SteamFriendInfo> list = friendInfoList;
			if (list == null)
			{
				return 0;
			}

			return list.Count;
		}


		private static SteamFriendInfo GetFriendInfo(CSteamID steamID)
		{
			if (!SteamManager.Initialized)
			{
				return null;
			}

			if (IsMe(steamID))
			{
				return MyInfo;
			}

			return friendInfoList.Find(x => x.steamID.Equals(steamID));
		}


		private static SteamFriendInfo GetRequestUserInfo(CSteamID steamID)
		{
			if (!SteamManager.Initialized)
			{
				return null;
			}

			return requestUserInfoList.Find(x => x.steamID.Equals(steamID));
		}


		public static string GetFriendSteamName(CSteamID steamID)
		{
			SteamFriendInfo friendInfo = GetFriendInfo(steamID);
			if (friendInfo != null)
			{
				return friendInfo.name;
			}

			SteamLobbyMemberInfo lobbyMemberInfo = GetLobbyMemberInfo(steamID);
			if (lobbyMemberInfo != null)
			{
				return lobbyMemberInfo.name;
			}

			SteamFriendInfo requestUserInfo = GetRequestUserInfo(steamID);
			if (requestUserInfo != null)
			{
				return requestUserInfo.name;
			}

			if (!SteamApi.RequestUserInformation(steamID, true))
			{
				return SteamApi.GetFriendInfo(steamID, richPresenceKeyMap).name;
			}

			return string.Empty;
		}


		public static string GetFriendNickName(CSteamID steamID)
		{
			SteamFriendInfo friendInfo = GetFriendInfo(steamID);
			if (friendInfo != null)
			{
				return friendInfo.NickName;
			}

			SteamLobbyMemberInfo lobbyMemberInfo = GetLobbyMemberInfo(steamID);
			if (lobbyMemberInfo != null)
			{
				return lobbyMemberInfo.nickname;
			}

			SteamFriendInfo requestUserInfo = GetRequestUserInfo(steamID);
			if (requestUserInfo != null)
			{
				return requestUserInfo.NickName;
			}

			if (!SteamApi.RequestUserInformation(steamID, true))
			{
				return SteamApi.GetFriendInfo(steamID, richPresenceKeyMap).NickName;
			}

			return string.Empty;
		}


		public static int GetPersonaState(CSteamID steamID)
		{
			SteamFriendInfo friendInfo = GetFriendInfo(steamID);
			if (friendInfo != null)
			{
				return friendInfo.personaState;
			}

			SteamFriendInfo requestUserInfo = GetRequestUserInfo(steamID);
			if (requestUserInfo != null)
			{
				return requestUserInfo.personaState;
			}

			if (!SteamApi.RequestUserInformation(steamID, true))
			{
				return SteamApi.GetFriendInfo(steamID, richPresenceKeyMap).personaState;
			}

			return SteamApi.GetFriendPersonaState(steamID);
		}


		public static string GetGameStatus(CSteamID steamID)
		{
			SteamFriendInfo friendInfo = GetFriendInfo(steamID);
			if (friendInfo != null)
			{
				return friendInfo.GameStatus;
			}

			SteamFriendInfo requestUserInfo = GetRequestUserInfo(steamID);
			if (requestUserInfo != null)
			{
				return requestUserInfo.GameStatus;
			}

			if (!SteamApi.RequestUserInformation(steamID, true))
			{
				return SteamApi.GetFriendInfo(steamID, richPresenceKeyMap).GameStatus;
			}

			return SteamApi.GetFriendRichPresence(steamID, richPresenceKeyMap[CommunityRichPresenceType.NICK_NAME]);
		}


		public static Texture2D GetAvatar(CSteamID steamID)
		{
			if (!SteamManager.Initialized)
			{
				return null;
			}

			if (steamAvatarMap.ContainsKey(steamID.m_SteamID))
			{
				return steamAvatarMap[steamID.m_SteamID];
			}

			if (!SteamApi.RequestUserInformation(steamID, true))
			{
				SteamApi.GetAvatar(steamID, ref steamAvatarMap);
				return steamAvatarMap[steamID.m_SteamID];
			}

			return null;
		}


		public static SteamFriendInfo GetFriendInfo(int index)
		{
			if (friendInfoList == null)
			{
				return null;
			}

			if (friendInfoList.Count > index)
			{
				return friendInfoList[index];
			}

			return null;
		}


		public static SteamFriendInfo GetOnlineFriendInfo(int index)
		{
			if (onlineFriendInfoList == null)
			{
				return null;
			}

			if (onlineFriendInfoList.Count > index)
			{
				return onlineFriendInfoList[index];
			}

			return null;
		}


		public static int GetOnlineFriendCount()
		{
			List<SteamFriendInfo> list = onlineFriendInfoList;
			if (list == null)
			{
				return 0;
			}

			return list.Count;
		}


		private static SteamLobbyMemberInfo GetLobbyMemberInfo(CSteamID steamID)
		{
			return lobbyMemberInfoList.Find(x => x.steamID.Equals(steamID));
		}


		public static List<ChattingUI.ChattingInfo> GetLobbyChatMsg()
		{
			return lobbyMemberChat;
		}


		public static void InviteMyGroup(CSteamID steamID)
		{
			if (!SteamManager.Initialized)
			{
				return;
			}

			if (!HasLobby())
			{
				inviteSteamID = steamID;
				SteamApi.CreateLobby();
				return;
			}

			if (!IsMe(SteamApi.GetLobbyOwner(steamIDLobby)))
			{
				return;
			}

			inviteSteamID = steamID;
			InviteFriend();
		}


		private static void InviteFriend()
		{
			if (!SteamManager.Initialized)
			{
				return;
			}

			if (!inviteSteamID.IsValid())
			{
				return;
			}

			SteamFriendInfo friendInfo = GetFriendInfo(inviteSteamID);
			if (friendInfo == null)
			{
				NotFoundFriend();
				return;
			}

			string gameStatus = friendInfo.GameStatus;
			if (string.IsNullOrEmpty(gameStatus))
			{
				NotFoundFriend();
				return;
			}

			if (gameStatus == "InLobby")
			{
				SteamApi.InviteUserToLobby(steamIDLobby, inviteSteamID);
				inviteSteamID = CSteamID.Nil;
				AddNewChatMessage(new ChattingUI.ChattingInfo
				{
					Content = Ln.Format("{0}님을 초대했습니다.", friendInfo.name),
					IsNotice = true,
					noticeColor = true
				});
				return;
			}

			Popup inst = MonoBehaviourInstance<Popup>.inst;
			string msg = Ln.Get("이미 게임 중인 유저입니다.");
			Popup.Button[] array = new Popup.Button[1];
			int num = 0;
			Popup.Button button = new Popup.Button();
			button.type = Popup.ButtonType.Confirm;
			button.text = Ln.Get("확인");
			button.callback = delegate
			{
				if (IsRemainInviteRequest())
				{
					OnCommunityEvent onCommunityEvent = inviteLobbyEvent;
					if (onCommunityEvent == null)
					{
						return;
					}

					onCommunityEvent();
				}
			};
			array[num] = button;
			inst.Message(msg, array);
		}


		private static void NotFoundFriend()
		{
			Popup inst = MonoBehaviourInstance<Popup>.inst;
			string msg = Ln.Get("상대방을 찾을 수 없습니다.");
			Popup.Button[] array = new Popup.Button[1];
			int num = 0;
			Popup.Button button = new Popup.Button();
			button.type = Popup.ButtonType.Confirm;
			button.text = Ln.Get("확인");
			button.callback = delegate
			{
				if (IsRemainInviteRequest())
				{
					OnCommunityEvent onCommunityEvent = inviteLobbyEvent;
					if (onCommunityEvent == null)
					{
						return;
					}

					onCommunityEvent();
				}
			};
			array[num] = button;
			inst.Message(msg, array);
		}

		public static void LeaveLobby()
		{
			if (!SteamManager.Initialized)
			{
				return;
			}

			if (!HasLobby())
			{
				return;
			}

			SteamApi.LeaveLobby(steamIDLobby);
			LeaveMember(MySteamID);
		}


		public static void KickLobby(CSteamID steamID)
		{
			if (!SteamManager.Initialized)
			{
				return;
			}

			if (!steamID.IsValid())
			{
				return;
			}

			if (!IsLobbyOwner())
			{
				return;
			}

			if (IsMe(steamID))
			{
				return;
			}

			LobbyChatData lobbyChatData = new LobbyChatData();
			lobbyChatData.chatType = ChatMessageType.KICK_MEMBER;
			lobbyChatData.contents = steamID.ToString();
			SteamApi.SendLobbyChatMsg(steamIDLobby, JsonConvert.SerializeObject(lobbyChatData));
		}


		private static void OnLobbyCreated(LobbyCreated_t param)
		{
			if (param.m_eResult == EResult.k_EResultOK)
			{
				steamIDLobby = new CSteamID(param.m_ulSteamIDLobby);
				InviteFriend();
				return;
			}

			MonoBehaviourInstance<Popup>.inst.Error("팀을 찾을 수 없습니다.");
		}


		private static void OnLobbyInvite(LobbyInvite_t param)
		{
			if (SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene)
			{
				return;
			}

			inviteRequests.Enqueue(param);
			OnCommunityEvent onCommunityEvent = inviteLobbyEvent;
			if (onCommunityEvent == null)
			{
				return;
			}

			onCommunityEvent();
		}


		public static bool IsRemainInviteRequest()
		{
			return 0 < inviteRequests.Count;
		}


		public static bool GetInviteRequest(ref LobbyInvite_t param)
		{
			if (0 < inviteRequests.Count)
			{
				param = inviteRequests.Dequeue();
				return true;
			}

			return false;
		}


		public static void SetRichPresence(CommunityRichPresenceType type, string value)
		{
			if (!SteamManager.Initialized)
			{
				return;
			}

			if (type == CommunityRichPresenceType.GAME_STATUS)
			{
				SingletonMonoBehaviour<GameAnalytics>.inst.SetGameStatus(value);
			}

			SteamApi.SetRichPresence(richPresenceKeyMap[type], value);
			SteamFriendInfo steamFriendInfo = MyInfo;
			if (steamFriendInfo != null)
			{
				steamFriendInfo.SetRichPresence(type, value);
			}

			OnUpdateFriendInfo onUpdateFriendInfo = onUpdateRichPresence;
			if (onUpdateFriendInfo == null)
			{
				return;
			}

			onUpdateFriendInfo(MySteamID);
		}


		public static void AcceptInvite(CSteamID steamIDLobby, CSteamID steamIDUser)
		{
			if (!SteamManager.Initialized)
			{
				return;
			}

			SteamFriendInfo friendInfo = GetFriendInfo(steamIDUser);
			if (friendInfo == null)
			{
				return;
			}

			if (HasLobby())
			{
				Popup inst = MonoBehaviourInstance<Popup>.inst;
				string msg = Ln.Get("팀초대수락불가팝업");
				Popup.Button[] array = new Popup.Button[1];
				int num = 0;
				Popup.Button button = new Popup.Button();
				button.type = Popup.ButtonType.Confirm;
				button.text = Ln.Get("확인");
				button.callback = delegate
				{
					if (IsRemainInviteRequest())
					{
						OnCommunityEvent onCommunityEvent = inviteLobbyEvent;
						if (onCommunityEvent == null)
						{
							return;
						}

						onCommunityEvent();
					}
				};
				array[num] = button;
				inst.Message(msg, array);
				return;
			}

			SteamFriendInfo steamFriendInfo = MyInfo;
			if (steamFriendInfo != null && !steamFriendInfo.CanJoinTeam() || !friendInfo.CanJoinTeam())
			{
				Popup inst2 = MonoBehaviourInstance<Popup>.inst;
				string msg2 = Ln.Get("초대를 받을 수 없는 상태입니다.");
				Popup.Button[] array2 = new Popup.Button[1];
				int num2 = 0;
				Popup.Button button2 = new Popup.Button();
				button2.type = Popup.ButtonType.Confirm;
				button2.text = Ln.Get("확인");
				button2.callback = delegate
				{
					if (IsRemainInviteRequest())
					{
						OnCommunityEvent onCommunityEvent = inviteLobbyEvent;
						if (onCommunityEvent == null)
						{
							return;
						}

						onCommunityEvent();
					}
				};
				array2[num2] = button2;
				inst2.Message(msg2, array2);
				return;
			}

			SteamApi.JoinLobby(steamIDLobby);
		}


		public static void StartMatching()
		{
			if (!SteamManager.Initialized)
			{
				return;
			}

			if (!IsLobbyOwner())
			{
				return;
			}

			if (!IsAllMemberReady())
			{
				return;
			}

			LobbyChatData lobbyChatData = new LobbyChatData();
			lobbyChatData.chatType = ChatMessageType.MATCHING_START;
			lobbyChatData.contents = string.Format("{0}/{1}/{2}/{3}", GlobalUserData.matchingRegion,
				GlobalUserData.matchingMode, GlobalUserData.matchingTeamMode,
				MonoBehaviourInstance<LobbyService>.inst.SecretMatching);
			SteamApi.SendLobbyChatMsg(steamIDLobby, JsonConvert.SerializeObject(lobbyChatData));
		}


		public static void SendChatContent(string chatContent)
		{
			if (!SteamManager.Initialized)
			{
				return;
			}

			if (!HasLobby())
			{
				return;
			}

			if (string.IsNullOrEmpty(chatContent))
			{
				return;
			}

			chatContent = ArchStringUtil.CutOverSizeANSI(chatContent, 100);
			LobbyChatData lobbyChatData = new LobbyChatData();
			lobbyChatData.chatType = ChatMessageType.CHATTING;
			lobbyChatData.contents = chatContent;
			SteamApi.SendLobbyChatMsg(steamIDLobby, JsonConvert.SerializeObject(lobbyChatData));
		}


		private static void OnLobbyEnter(LobbyEnter_t param)
		{
			EChatRoomEnterResponse echatRoomEnterResponse = (EChatRoomEnterResponse) param.m_EChatRoomEnterResponse;
			if (echatRoomEnterResponse != EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
			{
				if (echatRoomEnterResponse != EChatRoomEnterResponse.k_EChatRoomEnterResponseFull)
				{
					Popup inst = MonoBehaviourInstance<Popup>.inst;
					string msg = Ln.Get("초대를 받을 수 없는 상태입니다.");
					Popup.Button[] array = new Popup.Button[1];
					int num = 0;
					Popup.Button button = new Popup.Button();
					button.type = Popup.ButtonType.Confirm;
					button.text = Ln.Get("확인");
					button.callback = delegate
					{
						if (IsRemainInviteRequest())
						{
							OnCommunityEvent onCommunityEvent3 = inviteLobbyEvent;
							if (onCommunityEvent3 == null)
							{
								return;
							}

							onCommunityEvent3();
						}
					};
					array[num] = button;
					inst.Message(msg, array);
					return;
				}

				Popup inst2 = MonoBehaviourInstance<Popup>.inst;
				string msg2 = Ln.Get("최대 인원을 초과하여 입장할 수 없습니다.");
				Popup.Button[] array2 = new Popup.Button[1];
				int num2 = 0;
				Popup.Button button2 = new Popup.Button();
				button2.type = Popup.ButtonType.Confirm;
				button2.text = Ln.Get("확인");
				button2.callback = delegate
				{
					if (IsRemainInviteRequest())
					{
						OnCommunityEvent onCommunityEvent3 = inviteLobbyEvent;
						if (onCommunityEvent3 == null)
						{
							return;
						}

						onCommunityEvent3();
					}
				};
				array2[num2] = button2;
				inst2.Message(msg2, array2);
			}
			else
			{
				steamIDLobby = new CSteamID(param.m_ulSteamIDLobby);
				inviteRequests.Clear();
				int numLobbyMembers = SteamApi.GetNumLobbyMembers(steamIDLobby);
				for (int i = 0; i < numLobbyMembers; i++)
				{
					CSteamID lobbyMemberByIndex = SteamApi.GetLobbyMemberByIndex(steamIDLobby, i);
					string lobbyMemberData =
						SteamApi.GetLobbyMemberData(steamIDLobby, lobbyMemberByIndex, "memberInfo");
					if (!lobbyMemberData.Equals(""))
					{
						SteamLobbyMemberInfo steamLobbyMemberInfo =
							JsonConvert.DeserializeObject<SteamLobbyMemberInfo>(lobbyMemberData);
						SteamLobbyMemberInfo lobbyMemberInfo = GetLobbyMemberInfo(steamLobbyMemberInfo.steamID);
						if (lobbyMemberInfo != null)
						{
							lobbyMemberInfoList.Remove(lobbyMemberInfo);
						}

						lobbyMemberInfoList.Add(steamLobbyMemberInfo);
					}
				}

				SteamLobbyMemberInfo steamLobbyMemberInfo2 = new SteamLobbyMemberInfo();
				steamLobbyMemberInfo2.steamID = SteamApi.GetSteamID();
				steamLobbyMemberInfo2.userNum = Lobby.inst.User.UserNum;
				SteamFriendInfo steamFriendInfo = MyInfo;
				steamLobbyMemberInfo2.name = steamFriendInfo != null ? steamFriendInfo.name : null;
				steamLobbyMemberInfo2.nickname = Lobby.inst.User.Nickname;
				SteamLobbyMemberInfo value = steamLobbyMemberInfo2;
				SteamApi.SetLobbyMemberData(steamIDLobby, "memberInfo", JsonConvert.SerializeObject(value));
				OnCommunityEvent onCommunityEvent = updateGroupEvent;
				if (onCommunityEvent != null)
				{
					onCommunityEvent();
				}

				OnCommunityEvent onCommunityEvent2 = enterLobbyEvent;
				if (onCommunityEvent2 == null)
				{
					return;
				}

				onCommunityEvent2();
			}
		}


		private static void OnLobbyDataUpdate(LobbyDataUpdate_t param)
		{
			if (param.m_ulSteamIDLobby != param.m_ulSteamIDMember)
			{
				SteamLobbyMemberInfo steamLobbyMemberInfo =
					JsonConvert.DeserializeObject<SteamLobbyMemberInfo>(SteamApi.GetLobbyMemberData(steamIDLobby,
						new CSteamID(param.m_ulSteamIDMember), "memberInfo"));
				SteamLobbyMemberInfo lobbyMemberInfo = GetLobbyMemberInfo(steamLobbyMemberInfo.steamID);
				if (lobbyMemberInfo != null)
				{
					lobbyMemberInfoList.Remove(lobbyMemberInfo);
				}

				lobbyMemberInfoList.Add(steamLobbyMemberInfo);
				OnCommunityEvent onCommunityEvent = updateGroupEvent;
				if (onCommunityEvent != null)
				{
					onCommunityEvent();
				}

				AddNewChatMessage(new ChattingUI.ChattingInfo
				{
					Content = Ln.Format("{0}님이 팀에 들어왔습니다.", steamLobbyMemberInfo.name),
					IsNotice = true,
					noticeColor = true
				});
			}
		}


		private static void OnFriendRichPresenceUpdate(FriendRichPresenceUpdate_t update)
		{
			SteamFriendInfo friendInfo = GetFriendInfo(update.m_steamIDFriend);
			if (friendInfo == null)
			{
				SteamFriendInfo requestUserInfo = GetRequestUserInfo(update.m_steamIDFriend);
				if (requestUserInfo != null)
				{
					requestUserInfo.SetRichPresence(CommunityRichPresenceType.GAME_STATUS,
						SteamApi.GetFriendRichPresence(update.m_steamIDFriend,
							richPresenceKeyMap[CommunityRichPresenceType.GAME_STATUS]));
					requestUserInfo.SetRichPresence(CommunityRichPresenceType.NICK_NAME,
						SteamApi.GetFriendRichPresence(update.m_steamIDFriend,
							richPresenceKeyMap[CommunityRichPresenceType.NICK_NAME]));
					OnUpdateFriendInfo onUpdateFriendInfo = onUpdateRichPresence;
					if (onUpdateFriendInfo == null)
					{
						return;
					}

					onUpdateFriendInfo(update.m_steamIDFriend);
				}

				return;
			}

			friendInfo.SetRichPresence(CommunityRichPresenceType.GAME_STATUS,
				SteamApi.GetFriendRichPresence(update.m_steamIDFriend,
					richPresenceKeyMap[CommunityRichPresenceType.GAME_STATUS]));
			friendInfo.SetRichPresence(CommunityRichPresenceType.NICK_NAME,
				SteamApi.GetFriendRichPresence(update.m_steamIDFriend,
					richPresenceKeyMap[CommunityRichPresenceType.NICK_NAME]));
			OnUpdateFriendInfo onUpdateFriendInfo2 = onUpdateRichPresence;
			if (onUpdateFriendInfo2 == null)
			{
				return;
			}

			onUpdateFriendInfo2(update.m_steamIDFriend);
		}


		private static void OnPersonaStateChange(PersonaStateChange_t param)
		{
			CSteamID csteamID = (CSteamID) param.m_ulSteamID;
			SteamFriendInfo steamFriendInfo = GetFriendInfo(csteamID);
			if (steamFriendInfo != null)
			{
				steamFriendInfo.personaState =
					IsMe(csteamID) ? SteamApi.GetPersonaState() : SteamApi.GetFriendPersonaState(csteamID);
			}
			else
			{
				steamFriendInfo = GetRequestUserInfo(csteamID);
				if (steamFriendInfo == null)
				{
					steamFriendInfo = SteamApi.GetFriendInfo(csteamID, richPresenceKeyMap);
					SteamApi.GetAvatar(csteamID, ref steamAvatarMap);
					requestUserInfoList.Add(steamFriendInfo);
				}
			}

			if (steamFriendInfo == null)
			{
				return;
			}

			RefreshOnlineFriendInfo();
			if (param.m_nChangeFlags.HasFlag(EPersonaChange.k_EPersonaChangeName))
			{
				steamFriendInfo.name = SteamApi.GetFriendPersonaName(csteamID);
				OnUpdateFriendInfo onUpdateFriendInfo = onUpdatePersonaState;
				if (onUpdateFriendInfo == null)
				{
					return;
				}

				onUpdateFriendInfo(csteamID);
			}
			else
			{
				if (!param.m_nChangeFlags.HasFlag(EPersonaChange.k_EPersonaChangeStatus) &&
				    !param.m_nChangeFlags.HasFlag(EPersonaChange.k_EPersonaChangeComeOnline) &&
				    !param.m_nChangeFlags.HasFlag(EPersonaChange.k_EPersonaChangeGoneOffline))
				{
					if (param.m_nChangeFlags.HasFlag(EPersonaChange.k_EPersonaChangeAvatar))
					{
						SteamApi.GetAvatar(csteamID, ref steamAvatarMap);
						OnUpdateFriendInfo onUpdateFriendInfo2 = onUpdatePersonaState;
						if (onUpdateFriendInfo2 == null)
						{
							return;
						}

						onUpdateFriendInfo2(csteamID);
					}

					return;
				}

				steamFriendInfo.personaState =
					IsMe(csteamID) ? SteamApi.GetPersonaState() : SteamApi.GetFriendPersonaState(csteamID);
				OnUpdateFriendInfo onUpdateFriendInfo3 = onUpdatePersonaState;
				if (onUpdateFriendInfo3 == null)
				{
					return;
				}

				onUpdateFriendInfo3(csteamID);
			}
		}


		private static void OnLobbyChatUpdate(LobbyChatUpdate_t update)
		{
			CSteamID userChangedID = (CSteamID) update.m_ulSteamIDUserChanged;
			EChatMemberStateChange rgfChatMemberStateChange =
				(EChatMemberStateChange) update.m_rgfChatMemberStateChange;
			switch (rgfChatMemberStateChange)
			{
				case EChatMemberStateChange.k_EChatMemberStateChangeEntered:
					return;
				case EChatMemberStateChange.k_EChatMemberStateChangeLeft:
					LeaveMember(userChangedID);
					return;
				case EChatMemberStateChange.k_EChatMemberStateChangeEntered |
				     EChatMemberStateChange.k_EChatMemberStateChangeLeft:
					break;
				case EChatMemberStateChange.k_EChatMemberStateChangeDisconnected:
					LeaveMember(userChangedID);
					return;
				default:
					if (rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeKicked)
					{
						LeaveMember(userChangedID);
						return;
					}

					if (rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeBanned)
					{
						LeaveMember(userChangedID);
						return;
					}

					break;
			}

			throw new ArgumentOutOfRangeException();
		}


		private static void LeaveMember(CSteamID userChangedID)
		{
			string content = string.Empty;
			if (IsMe(userChangedID))
			{
				steamIDLobby = CSteamID.Nil;
				lobbyMemberInfoList.Clear();
				content = Ln.Get("팀에서 나왔습니다.");
			}
			else
			{
				SteamLobbyMemberInfo lobbyMemberInfo = GetLobbyMemberInfo(userChangedID);
				if (lobbyMemberInfo == null)
				{
					return;
				}

				content = Ln.Format("{0}님이 팀에서 나갔습니다.", lobbyMemberInfo.name);
				lobbyMemberInfoList.Remove(lobbyMemberInfo);
			}

			OnCommunityEvent onCommunityEvent = updateGroupEvent;
			if (onCommunityEvent != null)
			{
				onCommunityEvent();
			}

			AddNewChatMessage(new ChattingUI.ChattingInfo
			{
				Content = content,
				IsNotice = true,
				noticeColor = true
			});
		}


		private static void OnLobbyChatMsg(LobbyChatMsg_t msg)
		{
			CSteamID csteamID;
			EChatEntryType echatEntryType;
			string lobbyChatMsg = SteamApi.GetLobbyChatMsg((CSteamID) msg.m_ulSteamIDLobby, (int) msg.m_iChatID,
				out csteamID, out echatEntryType);
			if (echatEntryType == EChatEntryType.k_EChatEntryTypeChatMsg)
			{
				LobbyChatData lobbyChatData = JsonConvert.DeserializeObject<LobbyChatData>(lobbyChatMsg);
				switch (lobbyChatData.chatType)
				{
					case ChatMessageType.CHATTING:
					{
						SteamLobbyMemberInfo lobbyMemberInfo = GetLobbyMemberInfo(csteamID);
						string friendPersonaName = SteamFriends.GetFriendPersonaName(csteamID);
						string content =
							SingletonMonoBehaviour<SwearWordManager>.inst.CheckAndReplaceChat(lobbyChatData.contents);
						AddNewChatMessage(new ChattingUI.ChattingInfo
						{
							NickName = friendPersonaName,
							CharacterName = lobbyMemberInfo.nickname,
							Content = content
						});
						return;
					}
					case ChatMessageType.MATCHING_START:
					{
						string[] array = lobbyChatData.contents.Split('/');
						GlobalUserData.matchingRegion = (MatchingRegion) Enum.Parse(typeof(MatchingRegion), array[0]);
						GlobalUserData.matchingMode = (MatchingMode) Enum.Parse(typeof(MatchingMode), array[1]);
						GlobalUserData.matchingTeamMode =
							(MatchingTeamMode) Enum.Parse(typeof(MatchingTeamMode), array[2]);
						if (bool.Parse(array[3]))
						{
							MonoBehaviourInstance<LobbyService>.inst.StartSecretMatching();
						}
						else
						{
							MonoBehaviourInstance<LobbyService>.inst.CancelSecretMatching();
						}

						if (MonoBehaviourInstance<MatchingService>.inst != null)
						{
							MonoBehaviourInstance<MatchingService>.inst.StartMatching(GetLobbyID(), GetMemberCount());
						}

						break;
					}
					case ChatMessageType.KICK_MEMBER:
					{
						CSteamID steamID = new CSteamID(ulong.Parse(lobbyChatData.contents));
						if (IsMe(steamID))
						{
							LeaveLobby();
							MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("팀에서 추방되었습니다."), new Popup.Button
							{
								type = Popup.ButtonType.Confirm,
								text = Ln.Get("확인")
							});
							return;
						}

						SteamLobbyMemberInfo lobbyMemberInfo2 = GetLobbyMemberInfo(steamID);
						if (lobbyMemberInfo2 != null)
						{
							AddNewChatMessage(new ChattingUI.ChattingInfo
							{
								Content = Ln.Format("{0}님이 추방되었습니다.", lobbyMemberInfo2.name),
								IsNotice = true,
								noticeColor = true
							});
						}

						break;
					}
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}


		private static void AddNewChatMessage(ChattingUI.ChattingInfo chatInfo)
		{
			if (string.IsNullOrEmpty(chatInfo.Content))
			{
				return;
			}

			lobbyMemberChat.Insert(0, chatInfo);
			if (100 < lobbyMemberChat.Count)
			{
				lobbyMemberChat.RemoveAt(100);
			}

			OnCommunityEvent onCommunityEvent = updateChatMsgEvent;
			if (onCommunityEvent == null)
			{
				return;
			}

			onCommunityEvent();
		}


		public static string GetLobbyID()
		{
			if (!HasLobby())
			{
				return Lobby.inst.User.UserNum.ToString();
			}

			return steamIDLobby.ToString();
		}


		public static int GetMemberCount()
		{
			if (!HasLobby())
			{
				return 1;
			}

			return lobbyMemberInfoList.Count;
		}


		public static List<long> GetMemberUserNumList()
		{
			if (!HasLobby())
			{
				return new List<long>
				{
					Lobby.inst.User.UserNum
				};
			}

			return (from m in lobbyMemberInfoList
				select m.userNum).ToList<long>();
		}


		public static Color GetStateImgColor(int personaState, string gameStatus)
		{
			if (personaState == 2)
			{
				return GameConstants.Community.OfflineColor;
			}

			if (gameStatus == "InTutorialGame" || gameStatus == "InGame" || gameStatus == "InCustomGame")
			{
				return GameConstants.Community.PlayingColor;
			}

			if (personaState != 0)
			{
				if (personaState != 1)
				{
					return GameConstants.Community.OfflineColor;
				}

				return GameConstants.Community.AwayColor;
			}

			if (!string.IsNullOrEmpty(gameStatus))
			{
				return GameConstants.Community.OnlineColor;
			}

			return GameConstants.Community.NotPlayingColor;
		}


		public static Color GetStateTextColor(int personaState, string gameStatus)
		{
			if (personaState != 2)
			{
				return GetStateImgColor(personaState, gameStatus);
			}

			return GameConstants.Community.OfflineTextColor;
		}


		public static string GetStateText(int personaState, string gameStatus)
		{
			if (personaState == 2)
			{
				return Ln.Get("오프라인");
			}

			if (gameStatus == "InTutorialGame" || gameStatus == "InGame" || gameStatus == "InCustomGame")
			{
				if (!string.IsNullOrEmpty(gameStatus))
				{
					return Ln.Get(gameStatus);
				}

				return Ln.Get("플레이 중");
			}

			if (personaState != 0)
			{
				if (personaState != 1)
				{
					return Ln.Get("오프라인");
				}

				if (!string.IsNullOrEmpty(gameStatus))
				{
					return Ln.Get(gameStatus);
				}

				return Ln.Get("자리비움");
			}

			if (!string.IsNullOrEmpty(gameStatus))
			{
				return Ln.Get(gameStatus);
			}

			return Ln.Get("온라인");
		}


		public static CSteamID GetLobbyMemberSteamID(int slotIndex)
		{
			int num = slotIndex - 1;
			for (int i = 0; i < lobbyMemberInfoList.Count; i++)
			{
				bool flag = IsLobbyOwner(lobbyMemberInfoList[i].steamID);
				if (slotIndex == 0)
				{
					if (flag)
					{
						return lobbyMemberInfoList[i].steamID;
					}
				}
				else if (!flag)
				{
					if (num == 0)
					{
						return lobbyMemberInfoList[i].steamID;
					}

					num--;
				}
			}

			return CSteamID.Nil;
		}
	}
}