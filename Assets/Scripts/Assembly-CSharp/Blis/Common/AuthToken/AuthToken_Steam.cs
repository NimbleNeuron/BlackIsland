using System;
using System.Collections.Generic;
using Steamworks;

namespace Blis.Common
{
	public class AuthToken_Steam : AuthToken
	{
		public readonly string sessionTicket;


		public AuthToken_Steam(byte[] sessionTicketBytes, int ticketLen)
		{
			sessionTicket = BitConverter.ToString(sessionTicketBytes, 0, ticketLen).Replace("-", "");
		}


		public string GetPlayerId()
		{
			if (!SteamManager.Initialized)
			{
				return "";
			}

			return SteamFriends.GetPersonaName();
		}


		public Dictionary<string, string> GetAttributesMap()
		{
			return new Dictionary<string, string>
			{
				{
					"authorizationCode",
					sessionTicket
				}
			};
		}
	}
}