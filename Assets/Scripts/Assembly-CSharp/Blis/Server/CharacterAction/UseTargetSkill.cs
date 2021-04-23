using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server.CharacterAction
{
	
	public class UseTargetSkill : SkillActionBase
	{
		
		protected const float MoveToTargetTick = 0.1f;

		
		protected readonly WorldCharacter target;

		
		protected float lastMoveToTargetTick;

		
		private Vector3? pastTargetPos;

		
		public UseTargetSkill(WorldMovableCharacter self, SkillSlotSet skillSlotSet, MasteryType masteryType,
			WorldCharacter target) : base(self, false)
		{
			this.skillSlotSet = skillSlotSet;
			this.masteryType = masteryType;
			this.target = target;
			isCastSkill = false;
		}

		
		public override WorldObject GetTarget()
		{
			return target;
		}

		
		public override WorldCharacter GetTargetCharacter()
		{
			return target;
		}

		
		public override void SetNextAction(ActionBase action)
		{
			base.SetNextAction(action);
			if (action != null && action is UseInjectSkill)
			{
				nextAction = action;
			}
		}

		
		public override void Start()
		{
			if (target == null)
			{
				return;
			}

			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			if (self.CanUseSkill(skillSlotSet, target, ref zero, ref zero2))
			{
				if (self.GetSkillData(skillSlotSet).StopWhenStartSkill)
				{
					StopMove();
				}

				self.SkillController.Cast(
					SkillUseInfo.Target(self.SkillAgent, skillSlotSet, masteryType,
						self.CharacterSkill.GetSkillEvolutionLevel(skillSlotSet.SlotSet2Index()), target.SkillAgent),
					self.GetSkillSequence(skillSlotSet));
				AfterCasting();
				isCastSkill = true;
			}
			else
			{
				if (self.IsInAttackableDistance(target))
				{
					return;
				}

				if (!self.CanMove())
				{
					return;
				}

				self.SkillController.CancelNormalAttack();
				SkillData skillData = self.GetSkillData(skillSlotSet);
				float skillRange = ServerUtil.GetSkillRange(skillData, self);
				self.MoveToTarget(target, skillRange, skillData.UseWeaponRange);
			}

			pastTargetPos = target.GetPosition();
		}

		
		protected override ActionBase Update()
		{
			if (target == null || !target.IsAlive || isCastSkill)
			{
				return NextAction();
			}

			if (!self.IsAttackable(target))
			{
				if (GameUtil.DistanceOnPlane(self.GetPosition(), target.GetPosition()) > self.Stat.SightRange * 3f)
				{
					return new MoveToTargetPosition(self, target, pastTargetPos);
				}

				return new MoveToTargetPosition(self, target);
			}

			pastTargetPos = target.GetPosition();
			SkillData skillData = self.GetSkillData(skillSlotSet);
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			if (self.CanUseSkill(skillSlotSet, target, ref zero, ref zero2))
			{
				if (skillData.StopWhenStartSkill || skillData.StopWhenCastReserveSkill)
				{
					StopMove();
				}

				self.SkillController.Cast(
					SkillUseInfo.Target(self.SkillAgent, skillSlotSet, masteryType,
						self.CharacterSkill.GetSkillEvolutionLevel(skillSlotSet.SlotSet2Index()), target.SkillAgent),
					self.GetSkillSequence(skillSlotSet));
				AfterCasting();
				return NextAction();
			}

			if (!self.CheckCooldown(skillSlotSet))
			{
				return NextAction();
			}

			if (!self.IsCanUseSkillInScriptCondition(skillSlotSet, target, zero))
			{
				return NextAction();
			}

			if (!self.CanMove())
			{
				return null;
			}

			if (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - lastMoveToTargetTick < 0.1f)
			{
				return null;
			}

			lastMoveToTargetTick = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			self.SkillController.CancelNormalAttack();
			float skillRange = ServerUtil.GetSkillRange(skillData, self);
			self.MoveToTarget(target, skillRange, skillData.UseWeaponRange);
			return null;
		}

		
		protected ActionBase NextAction()
		{
			if (!self.SkillController.IsPlaying(skillSlotSet))
			{
				if (this.nextAction != null)
				{
					ActionBase nextAction = this.nextAction;
					this.nextAction = null;
					return nextAction;
				}

				SkillData skillData = self.GetSkillData(skillSlotSet);
				if (skillData.StopAttackWhenStartSkill && !self.IsAI)
				{
					return new Idle(self, false);
				}

				if (skillData.StopWhenStartSkill && !self.IsAI)
				{
					return new Hold(self, target);
				}

				if (target != null)
				{
					return new AttackTarget(self, target);
				}
			}

			return new WaitForSkillFinish(self, skillSlotSet, target, this.nextAction);
		}
	}
}