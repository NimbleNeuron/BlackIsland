using System.Collections.Generic;
using System.Text;
using Steamworks;
using UnityEngine;

namespace Blis.Client
{
	public static class SteamApi
	{
		public static Callback<LobbyCreated_t> callback_LobbyCreated;
		public static Callback<LobbyInvite_t> callback_LobbyInvite;
		public static Callback<LobbyEnter_t> callback_LobbyEnter;
		public static Callback<LobbyDataUpdate_t> callback_LobbyDataUpdate;
		public static Callback<FriendRichPresenceUpdate_t> callback_FriendRichPresenceUpdate;
		public static Callback<PersonaStateChange_t> callback_PersonaStateChange;
		public static Callback<LobbyChatUpdate_t> callback_LobbyChatUpdate;
		public static Callback<LobbyChatMsg_t> callback_LobbyChatMsg;
		public static Callback<MicroTxnAuthorizationResponse_t> microTxnAuthorizationResponse;


		private static bool initializedMicroTxn;

		public static CSteamID GetSteamID()
		{
			return SteamUser.GetSteamID();
		}


		public static string GetStringSteamID()
		{
			if (!SteamManager.Initialized)
			{
				return string.Empty;
			}

			return SteamUser.GetSteamID().ToString();
		}


		public static string GetFriendPersonaName(CSteamID steamIDFriend)
		{
			return SteamFriends.GetFriendPersonaName(steamIDFriend);
		}


		public static CGameID GetFriendGamePlayed(CSteamID steamIDFriend)
		{
			FriendGameInfo_t friendGameInfo_t;
			if (!SteamFriends.GetFriendGamePlayed(steamIDFriend, out friendGameInfo_t))
			{
				return new CGameID(0UL);
			}

			return friendGameInfo_t.m_gameID;
		}


		public static void CreateLobby()
		{
			SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, 3);
		}


		public static void InviteUserToLobby(CSteamID steamIDLobby, CSteamID steamIDInvitee)
		{
			SteamMatchmaking.InviteUserToLobby(steamIDLobby, steamIDInvitee);
		}


		public static void JoinLobby(CSteamID steamIDLobby)
		{
			SteamMatchmaking.JoinLobby(steamIDLobby);
		}


		public static void LeaveLobby(CSteamID steamIDLobby)
		{
			SteamMatchmaking.LeaveLobby(steamIDLobby);
		}


		public static void SetLobbyData(CSteamID steamIDLobby, string key, string value)
		{
			SteamMatchmaking.SetLobbyData(steamIDLobby, key, value);
		}


		public static string GetLobbyData(CSteamID steamIDLobby, string key)
		{
			return SteamMatchmaking.GetLobbyData(steamIDLobby, key);
		}


		public static string GetLobbyMemberData(CSteamID steamIDLobby, CSteamID steamIDUser, string key)
		{
			return SteamMatchmaking.GetLobbyMemberData(steamIDLobby, steamIDUser, key);
		}


		public static void SetLobbyMemberData(CSteamID steamIDLobby, string key, string value)
		{
			SteamMatchmaking.SetLobbyMemberData(steamIDLobby, key, value);
		}


		public static CSteamID GetLobbyMemberByIndex(CSteamID steamIDLobby, int iMember)
		{
			return SteamMatchmaking.GetLobbyMemberByIndex(steamIDLobby, iMember);
		}


		public static int GetNumLobbyMembers(CSteamID steamIDLobby)
		{
			return SteamMatchmaking.GetNumLobbyMembers(steamIDLobby);
		}


		public static void SendLobbyChatMsg(CSteamID steamIDLobby, string msg)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(msg);
			SteamMatchmaking.SendLobbyChatMsg(steamIDLobby, bytes, bytes.Length);
		}


		public static string GetFriendRichPresence(CSteamID steamIDFriend, string key)
		{
			return SteamFriends.GetFriendRichPresence(steamIDFriend, key);
		}


		public static int GetPersonaState()
		{
			return ConvertPersonaState(SteamFriends.GetPersonaState());
		}


		public static int GetFriendPersonaState(CSteamID steamIDFriend)
		{
			return ConvertPersonaState(SteamFriends.GetFriendPersonaState(steamIDFriend));
		}


		private static int ConvertPersonaState(EPersonaState state)
		{
			switch (state)
			{
				case EPersonaState.k_EPersonaStateOffline:
					return 2;
				case EPersonaState.k_EPersonaStateOnline:
				case EPersonaState.k_EPersonaStateLookingToTrade:
				case EPersonaState.k_EPersonaStateLookingToPlay:
					return 0;
				case EPersonaState.k_EPersonaStateBusy:
				case EPersonaState.k_EPersonaStateAway:
				case EPersonaState.k_EPersonaStateSnooze:
					return 1;
				default:
					return 2;
			}
		}


		public static void SetRichPresence(string key, string value)
		{
			if (string.IsNullOrEmpty(key) || value == null)
			{
				return;
			}

			SteamFriends.SetRichPresence(key, value);
		}


		public static bool RequestUserInformation(CSteamID steamID, bool bRequireNameOnly)
		{
			return SteamFriends.RequestUserInformation(steamID, bRequireNameOnly);
		}


		public static CSteamID GetLobbyOwner(CSteamID steamIDLobby)
		{
			return SteamMatchmaking.GetLobbyOwner(steamIDLobby);
		}


		public static bool SetLobbyOwner(CSteamID steamIDLobby, CSteamID steamIDNewOwner)
		{
			return SteamMatchmaking.SetLobbyOwner(steamIDLobby, steamIDNewOwner);
		}


		public static string GetLobbyChatMsg(CSteamID steamIDLobby, int iChatID, out CSteamID steamIDUser,
			out EChatEntryType chatEntryType)
		{
			byte[] array = new byte[4096];
			SteamMatchmaking.GetLobbyChatEntry(steamIDLobby, iChatID, out steamIDUser, array, array.Length,
				out chatEntryType);
			return Encoding.Default.GetString(array);
		}


		public static List<SteamFriendInfo> GetFriends(Dictionary<CommunityRichPresenceType, string> richPresenceKeyMap)
		{
			List<SteamFriendInfo> list = new List<SteamFriendInfo>();
			int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
			for (int i = 0; i < friendCount; i++)
			{
				CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
				SteamFriends.GetMediumFriendAvatar(friendByIndex);
				SteamFriendInfo steamFriendInfo = new SteamFriendInfo
				{
					steamID = friendByIndex,
					name = SteamFriends.GetFriendPersonaName(friendByIndex),
					personaState = GetFriendPersonaState(friendByIndex)
				};
				SetRichPresence(steamFriendInfo, richPresenceKeyMap);
				list.Add(steamFriendInfo);
			}

			return list;
		}


		private static void SetRichPresence(SteamFriendInfo info,
			Dictionary<CommunityRichPresenceType, string> richPresenceKeyMap)
		{
			foreach (KeyValuePair<CommunityRichPresenceType, string> keyValuePair in richPresenceKeyMap)
			{
				info.SetRichPresence(keyValuePair.Key, GetFriendRichPresence(info.steamID, keyValuePair.Value));
			}
		}


		public static SteamFriendInfo GetFriendInfo(CSteamID steamIdFriend,
			Dictionary<CommunityRichPresenceType, string> richPresenceKeyMap)
		{
			CSteamID steamID = GetSteamID();
			SteamFriendInfo steamFriendInfo = new SteamFriendInfo();
			steamFriendInfo.steamID = steamIdFriend;
			steamFriendInfo.name = SteamFriends.GetFriendPersonaName(steamIdFriend);
			steamFriendInfo.personaState = steamID.Equals(steamIdFriend)
				? GetPersonaState()
				: GetFriendPersonaState(steamIdFriend);
			SetRichPresence(steamFriendInfo, richPresenceKeyMap);
			return steamFriendInfo;
		}


		public static void GetFriendAvatars(bool allRefresh, ref Dictionary<ulong, Texture2D> avatarMap)
		{
			int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
			for (int i = 0; i < friendCount; i++)
			{
				CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
				if (avatarMap.ContainsKey(friendByIndex.m_SteamID))
				{
					if (allRefresh)
					{
						avatarMap[friendByIndex.m_SteamID] =
							GetAvatarTexture(SteamFriends.GetMediumFriendAvatar(friendByIndex));
					}
				}
				else
				{
					avatarMap.Add(friendByIndex.m_SteamID,
						GetAvatarTexture(SteamFriends.GetMediumFriendAvatar(friendByIndex)));
				}
			}
		}


		public static void GetAvatar(CSteamID steamID, ref Dictionary<ulong, Texture2D> avatarMap)
		{
			avatarMap[steamID.m_SteamID] = GetAvatarTexture(SteamFriends.GetMediumFriendAvatar(steamID));
		}


		private static Texture2D GetAvatarTexture(int iImage)
		{
			int num = 64;
			int num2 = 64;
			byte[] array = new byte[num * num2 * 4];
			SteamUtils.GetImageRGBA(iImage, array, num * num2 * 4);
			Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGBA32, false, true);
			texture2D.LoadRawTextureData(array);
			texture2D.Apply();
			return FlipTexture(texture2D);
		}


		private static Texture2D FlipTexture(Texture2D original)
		{
			Texture2D texture2D = new Texture2D(original.width, original.height);
			int width = original.width;
			int height = original.height;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					texture2D.SetPixel(i, height - j - 1, original.GetPixel(i, j));
				}
			}

			texture2D.Apply();
			return texture2D;
		}


		public static void InitMicroTxn(Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate callback)
		{
			if (initializedMicroTxn)
			{
				return;
			}

			microTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(callback);
			initializedMicroTxn = true;
		}


		public static int GetDlcCount()
		{
			return SteamApps.GetDLCCount();
		}


		public static List<DLCData> GetDLCData()
		{
			List<DLCData> list = new List<DLCData>();
			for (int i = 0; i < GetDlcCount(); i++)
			{
				DLCData dlcdata = new DLCData();
				if (SteamApps.BGetDLCDataByIndex(i, out dlcdata.appId, out dlcdata.available, out dlcdata.name, 128))
				{
					list.Add(dlcdata);
				}
			}

			return list;
		}


		public static bool UserSubscribedDLC(AppId_t appId)
		{
			return SteamApps.BIsSubscribedApp(appId);
		}


		public static bool UserSubscribedAndInstalledDLC(AppId_t appId)
		{
			return SteamApps.BIsDlcInstalled(appId);
		}


		public class DLCData
		{
			public AppId_t appId;


			public bool available;


			public string name;
		}
	}
}