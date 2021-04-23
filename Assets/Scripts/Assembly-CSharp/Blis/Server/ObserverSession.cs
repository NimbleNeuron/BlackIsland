namespace Blis.Server
{
	
	public class ObserverSession : Session
	{
		
		
		public WorldObserver Observer
		{
			get
			{
				return this.observer;
			}
		}

		
		
		public override bool IsObserverSession
		{
			get
			{
				return true;
			}
		}

		
		public ObserverSession(int connectionId, long userId, string nickname) : base(connectionId, userId, nickname)
		{
		}

		
		public void SetObserver(WorldObserver observer)
		{
			base.SetWorldObject(observer);
			this.observer = observer;
			this.observer.SetSession(this);
		}

		
		private WorldObserver observer;
	}
}
