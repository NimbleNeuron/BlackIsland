using System.Collections.Generic;
using Steamworks;

namespace Blis.Client
{
	public class SteamFriendInfo
	{
		private readonly Dictionary<CommunityRichPresenceType, string> richPresence =
			new Dictionary<CommunityRichPresenceType, string>();
		public string name;
		public int personaState;
		public CSteamID steamID;
		public string GameStatus => GetRichPresence(CommunityRichPresenceType.GAME_STATUS);
		public string NickName => GetRichPresence(CommunityRichPresenceType.NICK_NAME);
		public void SetRichPresence(CommunityRichPresenceType type, string value)
		{
			richPresence[type] = value;
		}


		private string GetRichPresence(CommunityRichPresenceType type)
		{
			if (!richPresence.ContainsKey(type))
			{
				return string.Empty;
			}

			return richPresence[type];
		}


		public bool CanJoinTeam()
		{
			string a = GetRichPresence(CommunityRichPresenceType.GAME_STATUS);
			return a == "InLobby";
		}
	}
}