using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using Neptune.Http;

namespace Blis.Client
{
	public static class CurrencyApi
	{
		public const string HEADER_KEY_NP = "X-BSER-NP";


		public const string HEADER_KEY_ACOIN = "X-BSER-ACOIN";

		public static Func<HttpRequest> GetUserCurrency()
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/users/currency", Array.Empty<object>()), null);
		}


		public static void CheckChangeCurrency(Dictionary<string, string> responseHeader)
		{
			if (Lobby.inst == null || Lobby.inst.User == null)
			{
				return;
			}

			bool flag = false;
			if (responseHeader.ContainsKey("X-BSER-NP"))
			{
				Lobby.inst.User.np = Convert.ToInt32(responseHeader["X-BSER-NP"]);
				flag = true;
			}

			if (responseHeader.ContainsKey("X-BSER-ACOIN"))
			{
				Lobby.inst.User.aCoin = Convert.ToInt32(responseHeader["X-BSER-ACOIN"]);
				flag = true;
			}

			if (flag)
			{
				MonoBehaviourInstance<LobbyUI>.inst.MainMenu.UpdateUserCurrency();
			}
		}


		public class UserCurrencyResult
		{
			public string userCurrency;
		}
	}
}