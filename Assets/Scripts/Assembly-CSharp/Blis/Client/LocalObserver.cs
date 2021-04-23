using BIFog;
using Blis.Common;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.Observer)]
	public class LocalObserver : LocalObject
	{
		private CharacterColliderAgent colliderAgent;


		private ObserverHostileAgent hostileAgent;


		private MyObserverContext myObserver;


		private ObserverContext observer;


		private LocalSightAgent sightAgent;


		public LocalSightAgent SightAgent => sightAgent;


		public ObserverHostileAgent HostileAgent => hostileAgent;


		protected override ObjectType GetObjectType()
		{
			return ObjectType.Observer;
		}


		protected override int GetTeamNumber()
		{
			ObserverHostileAgent observerHostileAgent = hostileAgent;
			if (observerHostileAgent == null)
			{
				return 0;
			}

			return observerHostileAgent.SelectedTeamNumber;
		}


		protected override ColliderAgent GetColliderAgent()
		{
			return colliderAgent;
		}


		protected override bool GetIsOutSight()
		{
			return false;
		}


		public override void Init(byte[] snapshotData)
		{
			GameUtil.BindOrAdd<CharacterColliderAgent>(gameObject, ref colliderAgent);
			colliderAgent.Init(0f);
			GameUtil.BindOrAdd<LocalSightAgent>(gameObject, ref sightAgent);
			sightAgent.InitCharacterSight(this);
			sightAgent.SetDetect(true, true);
			hostileAgent = new ObserverHostileAgent(this);
		}


		public void SetObserverContext(ObserverContext observer)
		{
			this.observer = observer;
		}


		public void SetMyObserverContext(MyObserverContext observer)
		{
			SetObserverContext(observer);
			myObserver = observer;
			sightAgent.SetSightQuality(SightQuality.High);
			sightAgent.EnableFogSight();
		}
	}
}