using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class WorldProjectileSkillAgent : WorldCreatedObjectByMovableCharacter
	{
		
		public WorldProjectileSkillAgent(WorldObject worldObject) : base(worldObject)
		{
			this.worldProjectile = worldObject.GetComponent<WorldProjectile>();
		}

		
		protected override WorldMovableCharacter GetOwner()
		{
			if (this.worldProjectile == null)
			{
				return null;
			}
			return this.worldProjectile.Owner as WorldMovableCharacter;
		}

		
		protected override WorldObject GetWorldObject()
		{
			return this.worldProjectile;
		}

		
		protected override WorldCharacter GetWorldCharacter()
		{
			if (this.worldProjectile == null)
			{
				return null;
			}
			return this.worldProjectile.Owner;
		}

		
		protected override bool GetIsAlive()
		{
			return this.worldProjectile.IsAlive;
		}

		
		protected override CollisionObject3D GetCollisionObject()
		{
			return this.worldProjectile.GetCollisionObject();
		}

		
		public override WorldObject GetTarget()
		{
			return this.worldProjectile.Target;
		}

		
		public override void PlaySkillAction(SkillId skillId, int actionNo, int targetId, BlisVector targetPosition)
		{
			this.worldProjectile.PlaySkillAction(skillId, actionNo, targetId, targetPosition);
		}

		
		public override void PlaySkillAction(SkillId skillId, int actionNo, List<SkillActionTarget> targets)
		{
			this.worldProjectile.PlaySkillAction(skillId, actionNo, targets);
		}

		
		public override bool IsMoving()
		{
			return !this.worldProjectile.IsArrived;
		}

		
		public override bool IsStopped()
		{
			return this.worldProjectile.IsArrived;
		}

		
		public override void LookAt(Vector3 direction, float duration = 0f, bool isServerRotateInstant = false)
		{
			this.worldProjectile.LookAt(direction, duration, isServerRotateInstant);
		}

		
		private readonly WorldProjectile worldProjectile;
	}
}
