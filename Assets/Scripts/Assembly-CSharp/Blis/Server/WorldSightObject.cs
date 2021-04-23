using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.SightObject)]
	public class WorldSightObject : WorldObject
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.SightObject;
		}

		
		protected override int GetTeamNumber()
		{
			WorldCharacter worldCharacter = this.owner;
			if (worldCharacter == null)
			{
				return 0;
			}
			return worldCharacter.TeamNumber;
		}

		
		protected override ColliderAgent GetColliderAgent()
		{
			return this.colliderAgent;
		}

		
		
		public WorldCharacter Owner
		{
			get
			{
				return this.owner;
			}
		}

		
		
		public ServerSightAgent SightAgent
		{
			get
			{
				return this.sightAgent;
			}
		}

		
		
		public float DestroyTime
		{
			get
			{
				return this.destroyTime;
			}
		}

		
		public void Init(WorldCharacter owner, float sightRange, bool isRemoveWhenInvisibleStart)
		{
			this.owner = owner;
			GameUtil.BindOrAdd<DefaultColliderAgent>(base.gameObject, ref this.colliderAgent);
			GameUtil.BindOrAdd<ServerSightAgent>(base.gameObject, ref this.sightAgent);
			this.sightAgent.InitAttachSight(this, MonoBehaviourInstance<GameService>.inst.World.GetSightId());
			this.sightAgent.SetOwner(owner.SightAgent);
			this.sightAgent.SetDetect(true, false);
			this.sightAgent.UpdateSightRange(sightRange);
			this.sightAgent.UpdateSightAngle(360);
			this.sightAgent.SetIsRemoveWhenInvisibleStart(isRemoveWhenInvisibleStart);
		}

		
		public void DelayDestroySelf(float delay)
		{
			if (this.coroutine != null)
			{
				base.StopCoroutine(this.coroutine);
			}
			this.destroyTime = delay + MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			this.coroutine = base.StartCoroutine(CoroutineUtil.DelayedAction(delay, new Action(this.DestroySelf)));
		}

		
		public override void DestroySelf()
		{
			this.coroutine = null;
			SightAgent sightAgent = this.sightAgent.GetOwner();
			if (sightAgent != null)
			{
				sightAgent.RemoveAttachSight(this.sightAgent);
			}
			base.RemoveAttachedSight(this.sightAgent);
			MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(this);
		}

		
		protected override SkillAgent GetSkillAgent()
		{
			return null;
		}

		
		public override byte[] CreateSnapshot()
		{
			return WorldObject.serializer.Serialize<SightObjectSnapshot>(new SightObjectSnapshot
			{
				ownerId = this.owner.ObjectId,
				attachSightId = this.sightAgent.AttachSightId,
				sightRange = this.sightAgent.SightRange,
				sightAngle = this.sightAgent.SightAngle,
				isDetectShare = this.sightAgent.IsDetectShare,
				isDetect = this.sightAgent.IsDetect
			});
		}

		
		private DefaultColliderAgent colliderAgent;

		
		private WorldCharacter owner;

		
		protected ServerSightAgent sightAgent;

		
		private float destroyTime;

		
		private Coroutine coroutine;
	}
}
