using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.SightObject)]
	public class LocalSightObject : LocalObject
	{
		private DefaultColliderAgent colliderAgent;


		private LocalCharacter owner;


		private LocalSightAgent sightAgent;


		public LocalCharacter Owner => owner;

		protected override ObjectType GetObjectType()
		{
			return ObjectType.SightObject;
		}


		protected override int GetTeamNumber()
		{
			LocalCharacter localCharacter = owner;
			if (localCharacter == null)
			{
				return 0;
			}

			return localCharacter.TeamNumber;
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
			SightObjectSnapshot sightObjectSnapshot = serializer.Deserialize<SightObjectSnapshot>(snapshotData);
			owner = MonoBehaviourInstance<ClientService>.inst.World.Find<LocalMovableCharacter>(sightObjectSnapshot
				.ownerId);
			GameUtil.BindOrAdd<DefaultColliderAgent>(gameObject, ref colliderAgent);
			GameUtil.BindOrAdd<LocalSightAgent>(gameObject, ref sightAgent);
			sightAgent.InitAttachSight(this, sightObjectSnapshot.attachSightId);
			sightAgent.SetOwner(owner.SightAgent);
			sightAgent.UpdateSightRange(sightObjectSnapshot.sightRange);
			sightAgent.UpdateSightAngle(sightObjectSnapshot.sightAngle);
			sightAgent.SetDetect(sightObjectSnapshot.isDetectShare, sightObjectSnapshot.isDetect);
		}


		public override void DestroySelf()
		{
			base.DestroySelf();
			SightAgent sightAgent = this.sightAgent.GetOwner();
			if (sightAgent != null)
			{
				sightAgent.RemoveAttachSight(this.sightAgent);
			}

			RemoveAttachedSight(this.sightAgent);
		}
	}
}