using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.HookLineProjectile)]
	public class WorldHookLineProjectile : WorldProjectile
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.HookLineProjectile;
		}

		
		
		public WorldCharacter LinkFromCharacter
		{
			get
			{
				return this.linkFromCharacter;
			}
		}

		
		
		public WorldCharacter LinkToCharacter
		{
			get
			{
				return this.linkToCharacter;
			}
		}

		
		
		public Vector3? LinkToPoint
		{
			get
			{
				return this.linkToPoint;
			}
		}

		
		
		public bool IsLinked
		{
			get
			{
				return this.isLinked;
			}
		}

		
		
		public float LinkedTimeStack
		{
			get
			{
				return this.linkedTimeStack;
			}
		}

		
		public override byte[] CreateSnapshot()
		{
			HookLineProjectileSnapshot hookLineProjectileSnapshot = new HookLineProjectileSnapshot
			{
				code = base.ProjectileData.code,
				ownerId = base.Owner.ObjectId,
				createdPosition = new BlisVector(base.CreatedPosition),
				collisionCount = base.CollisionCount
			};
			base.OverwriteSnapShotByType(hookLineProjectileSnapshot);
			hookLineProjectileSnapshot.hookLineInfoSnapshot = WorldObject.serializer.Serialize<HookLineInfoSnapshot>(new HookLineInfoSnapshot
			{
				hookLineCode = this.hookLineProperty.HookLineCode,
				linkFromObjectId = ((this.LinkFromCharacter == null) ? 0 : this.LinkFromCharacter.ObjectId),
				linkToObjectId = ((this.LinkToCharacter == null) ? 0 : this.LinkToCharacter.ObjectId),
				linkToPoint = this.LinkToPoint,
				linkedTimeStack = new BlisFixedPoint(this.linkedTimeStack)
			});
			return WorldObject.serializer.Serialize<HookLineProjectileSnapshot>(hookLineProjectileSnapshot);
		}

		
		public void Init(HookLineProperty property)
		{
			base.Init(property);
			this.playerOwner = (base.Owner as WorldPlayerCharacter);
			this.hookLineProperty = property;
			this.linkFromCharacter = property.LinkFromCharacter;
			this.linkFromPlayerCharacter = (this.linkFromCharacter as WorldPlayerCharacter);
			this.linkToCharacter = null;
			this.linkToPlayerCharacter = null;
			this.linkToPoint = null;
			this.linkedTimeStack = 0f;
			this.isLinked = false;
			if (this.linkFromCharacter.ObjectId != base.Owner.ObjectId)
			{
				this.sharedSightAgentFrom = base.Owner.SkillAgent.AttachSight(this.linkFromCharacter, 1f, 0f, false);
				this.sightRangeLinkFrom = new SightRangeLink(this.linkFromCharacter, base.Owner);
			}
			if (!this.hookLineProperty.IsImmediatelyLinkHookLine)
			{
				return;
			}
			if (this.hookLineProperty.LinkToCharacter != null)
			{
				this.SetLinkToCharacter(this.hookLineProperty.LinkToCharacter, base.Owner);
				return;
			}
			if (this.hookLineProperty.LinkToPoint != null)
			{
				this.SetLinkToPoint(this.hookLineProperty.LinkToPoint);
				return;
			}
			Log.E(string.Format("HookLineProjectile Error : IsImmediatelyLineHookLine is {0} but Target is NULL", this.hookLineProperty.IsImmediatelyLinkHookLine));
		}

		
		protected override void OnFrameUpdate()
		{
			if (!base.IsAlive)
			{
				return;
			}
			if (this.isLinked)
			{
				this.UpdateHookLine();
				return;
			}
			if (this.IsOwnerInvalid())
			{
				this.DestroySelf();
				return;
			}
			base.OnFrameUpdate();
		}

		
		private void UpdateHookLine()
		{
			if (this.IsOwnerInvalid())
			{
				this.DestroySelf();
				return;
			}
			if (this.IsLinkedCharacterInvalid())
			{
				this.DestroySelf();
				return;
			}
			Vector3 vector = (this.linkToPoint != null) ? this.linkToPoint.Value : this.linkToCharacter.GetPosition();
			this.SetPosition(vector);
			if (GameUtil.DistanceOnPlane(vector, this.linkFromCharacter.GetPosition()) > this.hookLineProperty.LinkedDistance)
			{
				Action<WorldCharacter, WorldHookLineProjectile> onDisconnectionRangeOut = this.hookLineProperty.OnDisconnectionRangeOut;
				if (onDisconnectionRangeOut != null)
				{
					onDisconnectionRangeOut(this.linkToCharacter, this);
				}
				this.DestroySelf();
				return;
			}
			this.linkedTimeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
			if (this.linkedTimeStack > this.hookLineProperty.LinkedDuration)
			{
				Action<WorldCharacter, WorldHookLineProjectile> onDisconnectionTimeOut = this.hookLineProperty.OnDisconnectionTimeOut;
				if (onDisconnectionTimeOut != null)
				{
					onDisconnectionTimeOut(this.linkToCharacter, this);
				}
				this.DestroySelf();
			}
		}

		
		private bool IsOwnerInvalid()
		{
			return base.Owner == null || !base.Owner.IsAlive || (this.playerOwner != null && this.playerOwner.IsDyingCondition);
		}

		
		private bool IsLinkedCharacterInvalid()
		{
			return this.linkFromCharacter == null || !this.linkFromCharacter.IsAlive || (this.linkFromPlayerCharacter != null && this.linkFromPlayerCharacter.IsDyingCondition) || (this.hookLineProperty.LinkToPoint == null && (this.linkToCharacter == null || !this.linkToCharacter.IsAlive || (this.linkToPlayerCharacter != null && this.linkToPlayerCharacter.IsDyingCondition)));
		}

		
		protected override void ArrivedDestination(bool isCollision)
		{
			if (this.isArrived)
			{
				return;
			}
			this.isArrived = true;
			Action<Vector3, bool, WorldProjectile> onArrive = base.Property.OnArrive;
			if (onArrive != null)
			{
				onArrive(base.GetPosition(), isCollision, this);
			}
			if (!isCollision)
			{
				this.DestroySelf();
			}
		}

		
		public override void DestroySelf()
		{
			if (this.linkToCharacter != null)
			{
				base.Owner.SkillAgent.RemoveSight(this.sharedSightAgentTo, this.linkToCharacter.ObjectId);
			}
			if (this.linkFromCharacter != null && this.linkFromCharacter.ObjectId != this.playerOwner.ObjectId)
			{
				base.Owner.SkillAgent.RemoveSight(this.sharedSightAgentFrom, this.linkFromCharacter.ObjectId);
			}
			if (this.sightRangeLinkTo != null)
			{
				this.sightRangeLinkTo.RemoveSightRangeLink();
			}
			if (this.sightRangeLinkFrom != null)
			{
				this.sightRangeLinkFrom.RemoveSightRangeLink();
			}
			base.DestroySelf();
		}

		
		protected override void DestroyAfterAction()
		{
			SkillScript playingScripts = this.playingScripts;
			if (playingScripts != null)
			{
				playingScripts.Stop(false);
			}
			MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(this);
		}

		
		protected void SetLinkToCharacter(WorldCharacter target, WorldCharacter caster)
		{
			this.linkToCharacter = target;
			this.isLinked = true;
			this.linkToPlayerCharacter = (this.linkToCharacter as WorldPlayerCharacter);
			this.sharedSightAgentTo = base.Owner.SkillAgent.AttachSight(target, 1f, 0f, false);
			this.sightRangeLinkTo = new SightRangeLink(target, caster);
		}

		
		protected void SetLinkToPoint(Vector3? targetPoint)
		{
			this.linkToPoint = targetPoint;
			this.isLinked = true;
		}

		
		protected override void OnCollisionCharacter(WorldCharacter target, WorldCharacter caster)
		{
			if (target.ObjectId == this.linkFromCharacter.ObjectId)
			{
				return;
			}
			base.OnCollisionCharacter(target, caster);
			this.SetLinkToCharacter(target, caster);
		}

		
		protected override void OnCollisionWall(Vector3 damagePoint)
		{
			if (!this.hookLineProperty.IsLinkPoint)
			{
				return;
			}
			base.OnCollisionWall(damagePoint);
			this.SetLinkToPoint(new Vector3?(damagePoint));
		}

		
		private HookLineProperty hookLineProperty;

		
		private WorldCharacter linkFromCharacter;

		
		private WorldPlayerCharacter linkFromPlayerCharacter;

		
		private WorldCharacter linkToCharacter;

		
		private WorldPlayerCharacter linkToPlayerCharacter;

		
		private Vector3? linkToPoint;

		
		private WorldPlayerCharacter playerOwner;

		
		private bool isLinked;

		
		private float linkedTimeStack;

		
		private ServerSightAgent sharedSightAgentTo;

		
		private SightRangeLink sightRangeLinkTo;

		
		private ServerSightAgent sharedSightAgentFrom;

		
		private SightRangeLink sightRangeLinkFrom;
	}
}
