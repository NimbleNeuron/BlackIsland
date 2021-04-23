using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class WorldSummonSkillAgent : WorldCreatedObjectByMovableCharacter
	{
		
		public WorldSummonSkillAgent(WorldObject worldObject) : base(worldObject)
		{
			this._worldSummonBase = worldObject.GetComponent<WorldSummonBase>();
		}

		
		protected override WorldMovableCharacter GetOwner()
		{
			return this._worldSummonBase.Owner;
		}

		
		protected override WorldObject GetWorldObject()
		{
			return this._worldSummonBase;
		}

		
		protected override WorldCharacter GetWorldCharacter()
		{
			return this._worldSummonBase;
		}

		
		protected override bool GetIsAlive()
		{
			return this._worldSummonBase.IsAlive;
		}

		
		protected override CollisionObject3D GetCollisionObject()
		{
			return this._worldSummonBase.GetCollisionObject();
		}

		
		public override WorldObject GetTarget()
		{
			return null;
		}

		
		public override void PlaySkillAction(SkillId skillId, int actionNo, int targetId, BlisVector targetPosition)
		{
			this._worldSummonBase.PlaySkillAction(skillId, actionNo, targetId, targetPosition);
		}

		
		public override void PlaySkillAction(SkillId skillId, int actionNo, List<SkillActionTarget> targets)
		{
			this._worldSummonBase.PlaySkillAction(skillId, actionNo, targets);
		}

		
		public override void BlockAllySight(BlockedSightType blockedSightType, bool block)
		{
			this._worldSummonBase.SightAgent.BlockAllySight(blockedSightType, block);
		}

		
		public override bool IsInAllySight(SightAgent targetSightAgent, Vector3 targetPosition, float targetRadius, bool targetIsInvisible)
		{
			return this._worldSummonBase.SightAgent.IsInAllySight(targetSightAgent, targetPosition, targetRadius, targetIsInvisible);
		}

		
		public override bool IsInSight(SightAgent targetSightAgent, Vector3 targetPosition, float targetRadius, bool targetIsInvisible)
		{
			return this._worldSummonBase.SightAgent.IsInSight(targetSightAgent, targetPosition, targetRadius, targetIsInvisible);
		}

		
		public override void AddState(CharacterState state, int casterObjectId)
		{
			this._worldSummonBase.AddState(state, casterObjectId);
		}

		
		public override void LookAt(Vector3 direction, float localAngularSpeed = 0f, bool isServerRotateInstant = false)
		{
			this._worldSummonBase.LookAt(direction, 0f, false);
		}

		
		private readonly WorldSummonBase _worldSummonBase;
	}
}
