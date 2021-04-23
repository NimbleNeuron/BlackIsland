namespace Blis.Client
{
	public class ObserverContext : UserContext
	{
		private LocalObserver observer;


		public ObserverContext(long userId) : base(userId) { }


		public LocalObserver Observer => observer;


		public void SetObserver(LocalObserver observer)
		{
			this.observer = observer;
			this.observer.SetObserverContext(this);
		}
	}
}