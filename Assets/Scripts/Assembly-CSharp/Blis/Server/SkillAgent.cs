using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class SkillAgent
	{
		
		protected SkillAgent(WorldObject worldObject)
		{
		}

		
		
		public WorldObject WorldObject
		{
			get
			{
				return this.GetWorldObject();
			}
		}

		
		protected abstract WorldObject GetWorldObject();

		
		
		public Vector3 Position
		{
			get
			{
				if (!(this.WorldObject != null))
				{
					return Vector3.zero;
				}
				return this.WorldObject.GetPosition();
			}
		}

		
		
		public Vector3 Forward
		{
			get
			{
				if (!(this.WorldObject != null))
				{
					return Vector3.forward;
				}
				return this.WorldObject.transform.forward;
			}
		}

		
		
		public Vector3 Right
		{
			get
			{
				if (!(this.WorldObject != null))
				{
					return Vector3.right;
				}
				return this.WorldObject.transform.right;
			}
		}

		
		
		public int ObjectId
		{
			get
			{
				if (!(this.WorldObject != null))
				{
					return 0;
				}
				return this.WorldObject.ObjectId;
			}
		}

		
		
		public ObjectType ObjectType
		{
			get
			{
				if (!(this.WorldObject != null))
				{
					return ObjectType.None;
				}
				return this.WorldObject.ObjectType;
			}
		}

		
		
		public ColliderAgent ColliderAgent
		{
			get
			{
				if (!(this.WorldObject != null))
				{
					return null;
				}
				return this.WorldObject.ColliderAgent;
			}
		}

		
		
		public virtual int WalkableNavMask
		{
			get
			{
				if (!(this.WorldObject != null))
				{
					return 0;
				}
				return this.WorldObject.WalkableNavMask;
			}
		}

		
		
		public WorldCharacter Character
		{
			get
			{
				return this.GetWorldCharacter();
			}
		}

		
		protected abstract WorldCharacter GetWorldCharacter();

		
		
		public WorldMovableCharacter Owner
		{
			get
			{
				return this.GetOwner();
			}
		}

		
		protected abstract WorldMovableCharacter GetOwner();

		
		
		public bool IsAlive
		{
			get
			{
				return this.GetIsAlive();
			}
		}

		
		protected abstract bool GetIsAlive();

		
		
		public bool IsDyingCondition
		{
			get
			{
				return this.Character.IsDyingCondition;
			}
		}

		
		
		public CharacterStat Stat
		{
			get
			{
				return this.GetStat();
			}
		}

		
		protected abstract CharacterStat GetStat();

		
		
		public CharacterStatus Status
		{
			get
			{
				return this.GetStatus();
			}
		}

		
		protected abstract CharacterStatus GetStatus();

		
		
		public CollisionObject3D CollisionObject
		{
			get
			{
				return this.GetCollisionObject();
			}
		}

		
		protected abstract CollisionObject3D GetCollisionObject();

		
		public abstract HostileType GetHostileType(WorldCharacter target);

		
		public abstract WorldObject GetTarget();

		
		public abstract void PlaySkillAction(SkillId skillId, int actionNo, int targetId, BlisVector targetPosition);

		
		public abstract void PlaySkillAction(SkillId skillId, int actionNo, List<SkillActionTarget> targets);

		
		public abstract DamageInfo DirectDamageTo(SkillAgent target, AttackerInfo attackerInfo, DamageType type, DamageSubType subType, int damageDataCode, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, int minRemain, float damageMasteryModifier, int effectAndSoundCode, bool isCheckAlly, bool targetInCombat);

		
		public abstract DamageInfo DamageTo(SkillAgent target, AttackerInfo attackerInfo, DamageType type, DamageSubType subType, int damageDataCode, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, Vector3 damagePoint, Vector3 damageDirection, bool isCheckAlly, int minRemain, float damageMasteryModifier, int effectAndSoundCode, bool targetInCombat);

		
		public abstract void HealTo(SkillAgent target, int hpBaseAmount, float hpCoefficient, int hpFixAmount, int spBaseAmount, float spCoefficient, int spFixAmount, bool showUI, int effectAndSoundCode);

		
		public abstract void HpHealTo(SkillAgent target, int baseAmount, float coefficient, int fixAmount, bool showUI, int effectAndSoundCode);

		
		public abstract void LostHpHealTo(SkillAgent target, float coefficient, int fixAmount, bool showUI, int effectAndSoundCode);

		
		public abstract void SpHealTo(SkillAgent target, int baseAmount, float coefficient, int fixAmount, bool showUI, int effectAndSoundCode);

		
		public abstract void ExtraPointModifyTo(SkillAgent target, int deltaAmount);

		
		public abstract void ModifySkillCooldown(SkillSlotSet skillSlotSetFlag, float time);

		
		public virtual SkillData GetSkillData(SkillSlotIndex skillSlotIndex)
		{
			return null;
		}

		
		public virtual SkillData GetSkillData(SkillSlotSet skillSlotSet)
		{
			return null;
		}

		
		public virtual int GetSkillLevel(SkillSlotIndex skillSlotIndex)
		{
			return 0;
		}

		
		public virtual int GetSkillLevel(MasteryType masteryType)
		{
			return 0;
		}

		
		public virtual float GetSkillCooldown(SkillSlotSet skillSlotSet)
		{
			return 0f;
		}

		
		public virtual bool IsSkillEvolution(SkillSlotIndex skillSlotIndex)
		{
			return false;
		}

		
		public virtual void StartConcentration(SkillData skillData)
		{
		}

		
		public virtual void FinishConcentration(SkillSlotSet skillSlotSet, MasteryType masteryType, SkillData skillData, bool cancel)
		{
		}

		
		public virtual bool IsConcentration()
		{
			return false;
		}

		
		public virtual void Invisible(bool isInvisible)
		{
		}

		
		public virtual void PlayPassiveSkill(SkillUseInfo skillUseInfo, int actionNo, int targetId, BlisVector targetPosition)
		{
		}

		
		public virtual bool IsReadySkill(SkillSlotSet skillSlotSet)
		{
			return true;
		}

		
		public virtual void InjectSkill(SkillUseInfo skillUseInfo)
		{
		}

		
		public virtual void CancelNormalAttack()
		{
		}

		
		public virtual void ReadyNormalAttack()
		{
		}

		
		public virtual void MountNormalAttack(int skillCode)
		{
		}

		
		public virtual void UnmountNormalAttack()
		{
		}

		
		public virtual MasteryType GetEquipWeaponMasteryType()
		{
			return MasteryType.None;
		}

		
		public virtual bool IsEnoughBullet()
		{
			return true;
		}

		
		public virtual bool IsFullBullet()
		{
			return false;
		}

		
		public virtual void ConsumeBullet(ProjectileData projectileData)
		{
		}

		
		public virtual void GunReload(bool playReloadAnimation)
		{
		}

		
		public virtual void GunReload(bool playReloadAnimation, float reloadTime)
		{
		}

		
		public virtual bool CanActionDuringSkillPlaying()
		{
			return true;
		}

		
		public virtual bool IsPlayingScript(SkillSlotSet skillSlotSet)
		{
			return false;
		}

		
		public virtual bool IsPlayingScript(SkillId SkillId)
		{
			return false;
		}

		
		public virtual bool IsPlayingStateSkillScript(SkillId SkillId)
		{
			return false;
		}

		
		public virtual bool IsLockSkillSlot(SkillSlotSet skillSlotSet)
		{
			return false;
		}

		
		public virtual void LockSkillSlot(SkillSlotSet skillSlotSet, bool isLock)
		{
		}

		
		public virtual void LockSkillSlot(SpecialSkillId specialSkillId, bool isLock)
		{
		}

		
		public virtual void LockSkillSlotsWithPacket(SkillSlotSet skillSlotSet, bool isLock)
		{
		}

		
		public virtual bool SwitchSkillSet(SkillSlotIndex skillSlotIndex, SkillSlotSet skillSlotSet)
		{
			return false;
		}

		
		public virtual void OnTimeOver(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
		}

		
		public virtual void ConsumeSkillResources(SkillCostType infoSkillCostType, int infoSkillCostKey, int cost)
		{
		}

		
		public virtual void SetInCombatTime(float duration)
		{
		}

		
		public virtual WorldSummonTrap FindLifeLinkPairSummonObject(WorldSummonTrap worldSummonTrap, int summonCode, float summonData2Radius, float getFloat)
		{
			return null;
		}

		
		public virtual void PushLifeLinkPool(WorldSummonTrap worldSummonTrap)
		{
		}

		
		public virtual ServerSightAgent AttachSight(WorldObject target, float range, float duration, bool isRemoveWhenInvisibleStart)
		{
			return null;
		}

		
		public virtual ServerSightAgent AttachSightPosition(Vector3 position, float range, float duration, bool isRemoveWhenInvisibleStart)
		{
			return null;
		}

		
		public virtual void BlockAllySight(BlockedSightType blockedSightType, bool block)
		{
		}

		
		public virtual void RemoveSight(ServerSightAgent target, int targetId)
		{
		}

		
		public virtual void ResetSightDestroyTime(ServerSightAgent target, float duration)
		{
		}

		
		public virtual void EnableAllySight(bool enable)
		{
		}

		
		public virtual bool IsInAllySight(SightAgent targetSightAgent, Vector3 targetPosition, float targetRadius, bool targetIsInvisible)
		{
			return false;
		}

		
		public virtual bool IsInSight(SightAgent targetSightAgent, Vector3 targetPosition, float targetRadius, bool targetIsInvisible)
		{
			return false;
		}

		
		public virtual int GetSkillSequence(SkillSlotSet skillSlotSet)
		{
			return 0;
		}

		
		public virtual bool AnyHaveStateByGroup(int stateGroup)
		{
			return false;
		}

		
		public virtual bool IsHaveStateByGroup(int stateGroup, int casterObjectId)
		{
			return false;
		}

		
		public virtual bool AnyHaveStateByType(StateType stateType)
		{
			return false;
		}

		
		public virtual bool AnyHaveNegativelyAffectsMovementState()
		{
			return false;
		}

		
		public virtual CharacterState FindStateByGroup(int stateGroup, int casterObjectId)
		{
			return null;
		}

		
		public virtual void DurationPauseState(int stateGroup, int casterId, float deltaPauseTime)
		{
		}

		
		public virtual int GetStackByGroup(int stateGroup, int casterObjectId)
		{
			return 0;
		}

		
		public virtual void SetExternalNonCalculateStatValue(int stateGroup, int casterObjectId, float coef)
		{
		}

		
		public virtual void ActivateStatCalculator(int stateGroup, int casterObjectId, bool activate)
		{
		}

		
		public virtual void AddState(CharacterState state, int casterObjectId)
		{
		}

		
		public virtual void OverwriteState(int stateCode, int casterObjectId)
		{
		}

		
		public virtual bool RemoveStateByGroup(int stateGroup, int casterObjectId)
		{
			return false;
		}

		
		public virtual void RemoveStateByType(StateType stateType)
		{
		}

		
		public virtual void ModifyStateValue(int stateGroup, int casterId, float durationChangeAmount, int changeStackCount, bool isResetCreateedTime)
		{
		}

		
		public virtual bool IsMoving()
		{
			return false;
		}

		
		public virtual bool IsStopped()
		{
			return true;
		}

		
		public virtual void StopMove()
		{
		}

		
		public virtual void LockRotation(bool isLock)
		{
		}

		
		public virtual bool IsLockRotation()
		{
			return false;
		}

		
		public virtual void LookAt(Vector3 direction, float duration = 0f, bool isServerRotateInstant = false)
		{
		}

		
		public virtual float MoveToDestinationAtSpeed(Vector3 destination, float speed, EasingFunction.Ease ease, bool passingWall, out Vector3 finalDestination, out bool canMoveToDestination, out float finalDuration, bool canRotate = false)
		{
			finalDestination = this.Position;
			canMoveToDestination = false;
			finalDuration = 0f;
			return 0f;
		}

		
		public virtual void MoveToDestinationForTime(Vector3 destination, float duration, EasingFunction.Ease ease, bool passingWall, out Vector3 finalDestination, out bool canMoveToDestination, out float finalDuration, bool canRotate = false)
		{
			finalDestination = this.Position;
			canMoveToDestination = false;
			finalDuration = duration;
		}

		
		public virtual void MoveToDirectionForTime(Vector3 direction, float distance, float duration, EasingFunction.Ease ease, bool passingWall, out Vector3 finalDestination, out bool canMoveToDestination, out float finalDuration, bool canRotate = false)
		{
			finalDestination = this.Position;
			canMoveToDestination = false;
			finalDuration = duration;
		}

		
		public virtual void MoveStraightWithoutNavSpeed(Vector3 moveStartPos, Vector3 moveEndPos, float moveSpeed)
		{
		}

		
		public virtual void MoveStraightWithoutNavDuration(Vector3 moveStartPos, Vector3 moveEndPos, float duration)
		{
		}

		
		public virtual void MoveToTargetWithoutNavSpeed(Vector3 moveStartPos, WorldCharacter target, float moveSpeed, float arriveRadius)
		{
		}

		
		public virtual void WarpTo(Vector3 destination, bool needCheckNavMesh = true)
		{
		}

		
		public virtual void MoveInDirection(Vector3 direction)
		{
		}

		
		public virtual void MoveInCurve(float angularSpeed)
		{
		}

		
		public virtual void Airborne(float airborneDuration, float? power = null)
		{
		}
	}
}
