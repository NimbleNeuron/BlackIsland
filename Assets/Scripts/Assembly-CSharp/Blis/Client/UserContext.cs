namespace Blis.Client
{
	public abstract class UserContext
	{
		public readonly long userId;


		protected UserContext(long userId)
		{
			this.userId = userId;
		}

		public virtual int GetTeamSlot()
		{
			return 0;
		}
	}
}