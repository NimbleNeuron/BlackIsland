using Blis.Common;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.Observer)]
	public class WorldObserver : WorldObject
	{
		
		
		public ServerSightAgent SightAgent
		{
			get
			{
				return this.sightAgent;
			}
		}

		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.Observer;
		}

		
		protected override int GetTeamNumber()
		{
			return this.teamNumber;
		}

		
		protected override ColliderAgent GetColliderAgent()
		{
			return this.colliderAgent;
		}

		
		protected override SkillAgent GetSkillAgent()
		{
			return null;
		}

		
		public override byte[] CreateSnapshot()
		{
			return null;
		}

		
		public void SetSession(ObserverSession session)
		{
			this.session = session;
		}

		
		public void Init()
		{
			GameUtil.BindOrAdd<ServerSightAgent>(base.gameObject, ref this.sightAgent);
			this.sightAgent.InitCharacterSight(this);
			this.sightAgent.SetDetect(true, true);
		}

		
		public byte[] CreateMyPlayerSnapshot()
		{
			return WorldObject.serializer.Serialize<ObserverSnapshot>(new ObserverSnapshot());
		}

		
		private ObserverSession session;

		
		private int teamNumber = default;

		
		private CharacterColliderAgent colliderAgent = default;

		
		private ServerSightAgent sightAgent = default;
	}
}
