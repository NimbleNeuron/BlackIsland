using Steamworks;

namespace Blis.Common
{
	
	public class AuthTokenAcquirer_Steam : AuthTokenAcquirer
	{
		private readonly int SESSION_TICKET_MAX_LEN = 1024;
		private Callback<GetAuthSessionTicketResponse_t> getAuthSessionTicketResponse;
		private readonly byte[] sessionTicket;
		private uint ticketLen;
		
		public AuthTokenAcquirer_Steam() : base(AuthProvider.STEAM)
		{
			getAuthSessionTicketResponse =
				Callback<GetAuthSessionTicketResponse_t>.Create(GetAuthSessionTicketResponse);
			sessionTicket = new byte[SESSION_TICKET_MAX_LEN];
		}

		protected override void FetchTokenInternal()
		{
			if (!SteamManager.Initialized)
			{
				Finish("SteamManager is not initialized.", null);
			}

			if (SteamUser.GetAuthSessionTicket(sessionTicket, SESSION_TICKET_MAX_LEN, out ticketLen) ==
			    HAuthTicket.Invalid)
			{
				Finish("SessionTicket is not valid! I think session ticket array overflow.", null);
			}
		}

		private void GetAuthSessionTicketResponse(GetAuthSessionTicketResponse_t pCallback)
		{
			if (pCallback.m_eResult != EResult.k_EResultOK)
			{
				Log.E("SessionTicket is not valid!");
				Finish("SessionTicket is not valid!", null);
				return;
			}

			Finish(null, new AuthToken_Steam(sessionTicket, (int) ticketLen));
		}
	}
}