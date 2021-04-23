namespace Blis.Common
{
	
	public class AuthTokenAcquirer_Guest : AuthTokenAcquirer
	{
		
		public AuthTokenAcquirer_Guest() : base(AuthProvider.NONE) { }

		
		protected override void FetchTokenInternal()
		{
			Finish(null, new AuthToken_Guest());
		}
	}
}