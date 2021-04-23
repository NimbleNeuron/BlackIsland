using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Server
{
	
	public class WorldMovableCharacterSkillAgent : WorldCharacterSkillAgent
	{
		
		
		public override int WalkableNavMask
		{
			get
			{
				return this.worldMovableCharacter.WalkableNavMask;
			}
		}

		
		public WorldMovableCharacterSkillAgent(WorldObject worldObject) : base(worldObject)
		{
			this.worldMovableCharacter = worldObject.GetComponent<WorldMovableCharacter>();
		}

		
		protected override WorldMovableCharacter GetOwner()
		{
			return this.worldMovableCharacter;
		}

		
		public override WorldObject GetTarget()
		{
			return this.worldMovableCharacter.Controller.GetTarget();
		}

		
		public override void PlaySkillAction(SkillId skillId, int actionNo, int targetId, BlisVector targetPosition)
		{
			this.worldMovableCharacter.PlaySkillAction(skillId, actionNo, targetId, targetPosition);
		}

		
		public override void PlaySkillAction(SkillId skillId, int actionNo, List<SkillActionTarget> targets)
		{
			this.worldMovableCharacter.PlaySkillAction(skillId, actionNo, targets);
		}

		
		public override DamageInfo DirectDamageTo(SkillAgent target, AttackerInfo attackerInfo, DamageType type, DamageSubType subType, int damageDataCode, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, int minRemain, float damageMasteryModifier, int effectAndSoundCode, bool isCheckAlly, bool targetInCombat)
		{
			if (target.Character == null)
			{
				return null;
			}
			if (!target.IsAlive)
			{
				return null;
			}
			parameters.Merge(this.worldMovableCharacter.SkillController.GetPlayingScriptParameterCollection(target.Character, type, subType, damageId));
			int baseDamage = (int)parameters.Get(SkillScriptParameterType.Damage);
			DirectDamageCalculator calculator = new DirectDamageCalculator(WeaponType.None, type, subType, damageDataCode, damageId, baseDamage, minRemain, damageMasteryModifier);
			calculator.SetAttackerSkillScriptParameter(parameters);
			target.Character.IfTypeOf<WorldMovableCharacter>(delegate(WorldMovableCharacter targetCharacter)
			{
				SkillScriptParameterCollection playingScriptParameterCollection = targetCharacter.SkillController.GetPlayingScriptParameterCollection(this.GetOwner(), type, subType, damageId);
				calculator.SetVictimSkillScriptParameter(playingScriptParameterCollection);
			});
			return Singleton<DamageService>.inst.DamageTo(target.Character, attackerInfo, calculator, new Vector3?(target.Position), skillSlotSet, isCheckAlly, effectAndSoundCode, targetInCombat);
		}

		
		public override DamageInfo DamageTo(SkillAgent target, AttackerInfo attackerInfo, DamageType type, DamageSubType subType, int damageDataCode, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, Vector3 damagePoint, Vector3 damageDirection, bool isCheckAlly, int minRemain, float damageMasteryModifier, int effectAndSoundCode, bool targetInCombat)
		{
			if (target.Character == null)
			{
				return null;
			}
			if (!target.IsAlive)
			{
				return null;
			}
			parameters.Merge(this.worldMovableCharacter.SkillController.GetPlayingScriptParameterCollection(target.Character, type, subType, damageId));
			int baseDamage = (int)parameters.Get(SkillScriptParameterType.Damage);
			DamageCalculator calculator = new BasicDamageCalculator(WeaponType.None, type, subType, damageDataCode, damageId, baseDamage, minRemain, damageMasteryModifier);
			calculator.SetAttackerSkillScriptParameter(parameters);
			target.Character.IfTypeOf<WorldMovableCharacter>(delegate(WorldMovableCharacter targetCharacter)
			{
				SkillScriptParameterCollection playingScriptParameterCollection = targetCharacter.SkillController.GetPlayingScriptParameterCollection(this.GetOwner(), type, subType, damageId);
				calculator.SetVictimSkillScriptParameter(playingScriptParameterCollection);
			});
			if (this.worldMovableCharacter != null && isCheckAlly && target.GetHostileType(this.worldMovableCharacter) == HostileType.Ally)
			{
				return null;
			}
			return Singleton<DamageService>.inst.DamageTo(target.Character, attackerInfo, calculator, new Vector3?(damagePoint), skillSlotSet, isCheckAlly, effectAndSoundCode, targetInCombat);
		}

		
		public override void ModifySkillCooldown(SkillSlotSet skillSlotSetFlag, float time)
		{
			if (!base.IsAlive)
			{
				return;
			}
			this.worldMovableCharacter.ModifyCooldown(skillSlotSetFlag, time);
		}

		
		public override SkillData GetSkillData(SkillSlotSet skillSlotSet)
		{
			return this.worldMovableCharacter.GetSkillData(skillSlotSet, -1);
		}

		
		public override SkillData GetSkillData(SkillSlotIndex skillSlotIndex)
		{
			return this.worldMovableCharacter.GetSkillData(skillSlotIndex, -1);
		}

		
		public override int GetSkillLevel(SkillSlotIndex skillSlotIndex)
		{
			return this.worldMovableCharacter.CharacterSkill.GetSkillLevel(skillSlotIndex);
		}

		
		public override int GetSkillLevel(MasteryType masteryType)
		{
			return this.worldMovableCharacter.CharacterSkill.GetSkillLevel(masteryType);
		}

		
		public override float GetSkillCooldown(SkillSlotSet skillSlotSet)
		{
			return this.worldMovableCharacter.CharacterSkill.GetCooldown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
		}

		
		public override bool IsSkillEvolution(SkillSlotIndex skillSlotIndex)
		{
			return 0 < this.worldMovableCharacter.CharacterSkill.GetSkillEvolutionLevel(skillSlotIndex);
		}

		
		public override void StartConcentration(SkillData skillData)
		{
			this.worldMovableCharacter.StartConcentration(skillData);
		}

		
		public override void FinishConcentration(SkillSlotSet skillSlotSet, MasteryType masteryType, SkillData skillData, bool cancel)
		{
			this.worldMovableCharacter.EndConcentration(skillData, cancel);
			if (skillData.CooldownType == SkillCooldownType.ConcentrationEnd)
			{
				this.worldMovableCharacter.StartSkillCooldown(skillSlotSet, masteryType, cancel ? skillData.concentrationCancelCooldown : skillData.cooldown);
			}
		}

		
		public override bool IsConcentration()
		{
			return this.worldMovableCharacter.IsConcentration();
		}

		
		public override void Invisible(bool isInvisible)
		{
			this.worldMovableCharacter.Invisible(isInvisible);
		}

		
		public override void PlayPassiveSkill(SkillUseInfo skillUseInfo, int actionNo, int targetId, BlisVector targetPosition)
		{
			if (!base.IsAlive)
			{
				return;
			}
			if (skillUseInfo.skillSlotSet.SlotSet2Index() != SkillSlotIndex.Passive)
			{
				return;
			}
			this.worldMovableCharacter.UpdateSkillCooldown(skillUseInfo, skillUseInfo.skillData);
			this.worldMovableCharacter.PlayPassiveSkill(skillUseInfo.skillData.PassiveSkillId, actionNo, targetId, targetPosition);
		}

		
		public override bool IsReadySkill(SkillSlotSet skillSlotSet)
		{
			return this.worldMovableCharacter.IsReadySkill(skillSlotSet);
		}

		
		public override void InjectSkill(SkillUseInfo skillUseInfo)
		{
			if (!base.IsAlive)
			{
				return;
			}
			if (this.worldMovableCharacter.StateEffector.CanUseSkill(skillUseInfo.skillData))
			{
				this.worldMovableCharacter.IfTypeOf<WorldPlayerCharacter>(delegate(WorldPlayerCharacter player)
				{
					if (player.IsRunningCastingAction())
					{
						player.CancelActionCasting(CastingCancelType.Action);
					}
				});
				this.worldMovableCharacter.SkillController.Inject(skillUseInfo);
			}
		}

		
		public override void CancelNormalAttack()
		{
			this.worldMovableCharacter.SkillController.CancelNormalAttack();
		}

		
		public override void ReadyNormalAttack()
		{
			this.worldMovableCharacter.SkillController.ReadyNormalAttack();
		}

		
		public override void MountNormalAttack(int skillCode)
		{
			this.worldMovableCharacter.MountNormalAttack(skillCode);
		}

		
		public override void UnmountNormalAttack()
		{
			this.worldMovableCharacter.UnmountNormalAttack();
		}

		
		public override bool CanActionDuringSkillPlaying()
		{
			return this.worldMovableCharacter.SkillController.CanActionDuringSkillPlaying();
		}

		
		public override bool IsPlayingScript(SkillSlotSet skillSlotSet)
		{
			return this.worldMovableCharacter.SkillController.IsPlaying(skillSlotSet);
		}

		
		public override bool IsPlayingScript(SkillId SkillId)
		{
			return this.worldMovableCharacter.SkillController.IsPlaying(SkillId);
		}

		
		public override bool IsPlayingStateSkillScript(SkillId SkillId)
		{
			return this.worldMovableCharacter.SkillController.IsPlayingStateSkill(SkillId);
		}

		
		public override bool IsLockSkillSlot(SkillSlotSet skillSlotSet)
		{
			return this.worldMovableCharacter.SkillController.IsLockSlot(skillSlotSet);
		}

		
		public override void OnTimeOver(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			this.worldMovableCharacter.SequenceTimeOver(skillSlotSet, masteryType);
		}

		
		public override void SetInCombatTime(float duration)
		{
			this.worldMovableCharacter.SetInCombatTime(duration);
		}

		
		public override WorldSummonTrap FindLifeLinkPairSummonObject(WorldSummonTrap summonTrap, int lifeLinkSummonDataCode, float minRange, float maxRange)
		{
			return this.worldMovableCharacter.MySummonController.FindLifeLinkPairSummonObject(summonTrap, lifeLinkSummonDataCode, minRange, maxRange);
		}

		
		public override void PushLifeLinkPool(WorldSummonTrap worldSummonTrap)
		{
			this.worldMovableCharacter.MySummonController.PushLifeLinkPool(worldSummonTrap);
		}

		
		public override ServerSightAgent AttachSight(WorldObject target, float range, float duration, bool isRemoveWhenInvisibleStart)
		{
			if (target != null)
			{
				return this.worldMovableCharacter.AttachSight(target, range, duration, false, isRemoveWhenInvisibleStart);
			}
			return null;
		}

		
		public override ServerSightAgent AttachSightPosition(Vector3 position, float range, float duration, bool isRemoveWhenInvisibleStart)
		{
			return this.worldMovableCharacter.AttachSightPosition(position, range, duration, true, isRemoveWhenInvisibleStart);
		}

		
		public override void RemoveSight(ServerSightAgent target, int targetId)
		{
			this.worldMovableCharacter.RemoveSight(target, targetId);
		}

		
		public override void ResetSightDestroyTime(ServerSightAgent target, float duration)
		{
			this.worldMovableCharacter.ResetSightDestroyTime(target, duration);
		}

		
		public override void BlockAllySight(BlockedSightType blockedSightType, bool block)
		{
			this.worldMovableCharacter.SightAgent.BlockAllySight(blockedSightType, block);
		}

		
		public override bool AnyHaveStateByGroup(int stateGroup)
		{
			return this.worldMovableCharacter.StateEffector.AnyHaveStateByGroup(stateGroup);
		}

		
		public override bool IsHaveStateByGroup(int stateGroup, int casterObjectId)
		{
			return this.worldMovableCharacter.StateEffector.IsHaveStateByGroup(stateGroup, casterObjectId);
		}

		
		public override bool AnyHaveStateByType(StateType stateType)
		{
			return this.worldMovableCharacter.StateEffector.IsHaveStateByType(stateType);
		}

		
		public override bool AnyHaveNegativelyAffectsMovementState()
		{
			return this.worldMovableCharacter.StateEffector.AnyNegativelyAffectsMovementState();
		}

		
		public override CharacterState FindStateByGroup(int stateGroup, int casterObjectId)
		{
			return this.worldMovableCharacter.StateEffector.FindStateByGroup(stateGroup, casterObjectId);
		}

		
		public override void DurationPauseState(int stateGroup, int casterId, float deltaPauseTime)
		{
			this.worldMovableCharacter.StateEffector.DurationPauseState(stateGroup, casterId, deltaPauseTime);
		}

		
		public override int GetStackByGroup(int stateGroup, int casterObjectId)
		{
			return this.worldMovableCharacter.StateEffector.GetStackByGroup(stateGroup, casterObjectId);
		}

		
		public override void SetExternalNonCalculateStatValue(int stateGroup, int casterObjectId, float statValue)
		{
			this.worldMovableCharacter.StateEffector.SetExternalNonCalculateStatValue(stateGroup, casterObjectId, statValue);
		}

		
		public override void ActivateStatCalculator(int stateGroup, int casterObjectId, bool activate)
		{
			this.worldMovableCharacter.StateEffector.ActivateStatCalculator(stateGroup, casterObjectId, activate);
		}

		
		public override void AddState(CharacterState state, int casterObjectId)
		{
			this.worldMovableCharacter.AddState(state, casterObjectId);
		}

		
		public override void OverwriteState(int stateCode, int casterObjectId)
		{
			this.worldMovableCharacter.OverwriteState(stateCode, casterObjectId);
		}

		
		public override bool RemoveStateByGroup(int stateGroup, int casterObjectId)
		{
			return this.worldMovableCharacter.RemoveStateByGroup(stateGroup, casterObjectId);
		}

		
		public override void RemoveStateByType(StateType stateType)
		{
			this.worldMovableCharacter.RemoveAllStateByType(stateType);
		}

		
		public override void ModifyStateValue(int stateGroup, int casterId, float durationChangeAmount, int changeStackCount, bool isResetCreateedTime)
		{
			this.worldMovableCharacter.ModifyStateValue(stateGroup, casterId, durationChangeAmount, changeStackCount, isResetCreateedTime);
		}

		
		public override bool IsMoving()
		{
			return this.worldMovableCharacter.IsMoving();
		}

		
		public override bool IsStopped()
		{
			return this.worldMovableCharacter.IsStopped();
		}

		
		public override void StopMove()
		{
			this.worldMovableCharacter.StopMove();
		}

		
		public override void LockRotation(bool isLock)
		{
			this.worldMovableCharacter.LockRotation(isLock);
		}

		
		public override bool IsLockRotation()
		{
			return this.worldMovableCharacter.IsLockRotation();
		}

		
		public override void LookAt(Vector3 direction, float duration = 0f, bool isServerRotateInstant = false)
		{
			if (duration < 0f)
			{
				duration = 0f;
			}
			this.worldMovableCharacter.LookAt(direction, duration, isServerRotateInstant);
		}

		
		public override float MoveToDestinationAtSpeed(Vector3 destination, float speed, EasingFunction.Ease ease, bool passingWall, out Vector3 finalDestination, out bool canMoveToDestination, out float finalDuration, bool canRotate = false)
		{
			Vector3 normalized = (destination - base.Position).normalized;
			float magnitude = (destination - base.Position).magnitude;
			float num = magnitude / speed;
			this.MoveToDirectionInternal(normalized, magnitude, num, ease, passingWall, canRotate, out finalDestination, out canMoveToDestination, out finalDuration);
			return num;
		}

		
		public override void MoveToDestinationForTime(Vector3 destination, float duration, EasingFunction.Ease ease, bool passingWall, out Vector3 finalDestination, out bool canMoveToDestination, out float finalDuration, bool canRotate = false)
		{
			Vector3 normalized = (destination - base.Position).normalized;
			float magnitude = (destination - base.Position).magnitude;
			this.MoveToDirectionInternal(normalized, magnitude, duration, ease, passingWall, canRotate, out finalDestination, out canMoveToDestination, out finalDuration);
		}

		
		public override void MoveToDirectionForTime(Vector3 direction, float distance, float duration, EasingFunction.Ease ease, bool passingWall, out Vector3 finalDestination, out bool canMoveToDestination, out float finalDuration, bool canRotate = false)
		{
			this.MoveToDirectionInternal(direction, distance, duration, ease, passingWall, canRotate, out finalDestination, out canMoveToDestination, out finalDuration);
		}

		
		public override void MoveStraightWithoutNavSpeed(Vector3 moveStartPos, Vector3 moveEndPos, float moveSpeed)
		{
			this.worldMovableCharacter.MoveStraightWithoutNavSpeed(moveStartPos, moveEndPos, moveSpeed);
		}

		
		public override void MoveStraightWithoutNavDuration(Vector3 moveStartPos, Vector3 moveEndPos, float duration)
		{
			this.worldMovableCharacter.MoveStraightWithoutNavDuration(moveStartPos, moveEndPos, duration);
		}

		
		public override void MoveToTargetWithoutNavSpeed(Vector3 moveStartPos, WorldCharacter target, float moveSpeed, float arriveRadius)
		{
			this.worldMovableCharacter.MoveToTargetWithoutNavSpeed(moveStartPos, target, moveSpeed, arriveRadius);
		}

		
		private void MoveToDirectionInternal(Vector3 direction, float distance, float duration, EasingFunction.Ease ease, bool passingWall, bool canRotate, out Vector3 finalDestination, out bool canMoveToDestination, out float finalDuration)
		{
			finalDestination = base.Position + direction * distance;
			canMoveToDestination = false;
			if (duration == float.NaN)
			{
				finalDuration = 0f;
			}
			else
			{
				finalDuration = duration;
			}
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(finalDestination, out navMeshHit, 2f, 2147483640))
			{
				finalDestination.y = navMeshHit.position.y;
			}
			if (passingWall)
			{
				Vector3 vector;
				canMoveToDestination = MoveAgent.CanStandToPosition(finalDestination, this.WalkableNavMask, out vector);
				if (!canMoveToDestination)
				{
					Vector3 position;
					MoveAgent.CanStraightMoveToDestination(base.Position, finalDestination, this.WalkableNavMask, out position);
					int num = Mathf.FloorToInt(GameUtil.Distance(position, finalDestination) / 0.4f);
					for (int i = 1; i <= num; i++)
					{
						NavMeshHit navMeshHit2;
						if (NavMesh.Raycast(finalDestination - direction * 0.4f * (float)i, finalDestination, out navMeshHit2, this.WalkableNavMask))
						{
							position = navMeshHit2.position;
							break;
						}
					}
					finalDestination = position;
				}
			}
			else
			{
				Vector3 vector2;
				canMoveToDestination = MoveAgent.CanStraightMoveToDestination(base.Position, finalDestination, this.WalkableNavMask, out vector2);
				if (!canMoveToDestination)
				{
					finalDestination = vector2;
				}
			}
			if (duration <= 0f)
			{
				this.worldMovableCharacter.WarpTo(finalDestination, true);
				return;
			}
			if (!canMoveToDestination)
			{
				Vector3 vector = finalDestination - base.Position;
				float magnitude = vector.magnitude;
				if (distance == 0f)
				{
					finalDuration = 0f;
				}
				else
				{
					finalDuration *= magnitude / distance;
				}
			}
			this.worldMovableCharacter.MoveStraight(finalDestination, finalDuration, ease, canRotate);
		}

		
		public override void WarpTo(Vector3 destination, bool needCheckNavMesh)
		{
			this.worldMovableCharacter.WarpTo(destination, needCheckNavMesh);
		}

		
		public override void MoveInDirection(Vector3 direction)
		{
			this.worldMovableCharacter.MoveInDirection(direction);
		}

		
		public override void MoveInCurve(float angularSpeed)
		{
			this.worldMovableCharacter.MoveInCurve(angularSpeed);
		}

		
		public override int GetSkillSequence(SkillSlotSet skillSlotSet)
		{
			return this.worldMovableCharacter.GetSkillSequence(skillSlotSet);
		}

		
		private readonly WorldMovableCharacter worldMovableCharacter;

		
		private const float checkRayDistance = 0.4f;
	}
}
