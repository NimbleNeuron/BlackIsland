using Blis.Common;
using UnityEngine;

namespace Blis.Server.CharacterAction
{
	
	public class UsePointSkill : SkillActionBase
	{
		
		private readonly WorldCharacter target;

		
		public readonly Vector3 targetPoint;

		
		private readonly Vector3 targetRelease;

		
		public UsePointSkill(WorldMovableCharacter self, SkillSlotSet skillSlotSet, MasteryType masteryType,
			Vector3 targetPoint, Vector3 targetRelease, WorldCharacter target) : base(self, false)
		{
			this.skillSlotSet = skillSlotSet;
			this.masteryType = masteryType;
			this.targetPoint = targetPoint;
			this.targetRelease = targetRelease;
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
			Vector3 cursorPosition = targetPoint;
			Vector3 releasePosition = targetRelease;
			if (self.CanUseSkill(skillSlotSet, null, ref cursorPosition, ref releasePosition))
			{
				if (self.GetSkillData(skillSlotSet).StopWhenStartSkill)
				{
					StopMove();
				}

				self.SkillController.Cast(
					SkillUseInfo.Point(self.SkillAgent, skillSlotSet, masteryType,
						self.CharacterSkill.GetSkillEvolutionLevel(skillSlotSet.SlotSet2Index()), cursorPosition,
						releasePosition), self.GetSkillSequence(skillSlotSet));
				AfterCasting();
				isCastSkill = true;
				return;
			}

			SkillData skillData = self.GetSkillData(skillSlotSet);
			if (skillData.CastWaysType == SkillCastWaysType.Directional ||
			    skillData.CastWaysType == SkillCastWaysType.Instant)
			{
				return;
			}

			if (!self.SkillController.CanActionDuringSkillPlaying())
			{
				return;
			}

			if (self.CanMove())
			{
				self.SkillController.CancelNormalAttack();
				self.MoveToDestination(targetPoint);
			}
		}

		
		protected override ActionBase Update()
		{
			if (isCastSkill)
			{
				return NextAction();
			}

			Vector3 vector = targetPoint;
			Vector3 releasePosition = targetRelease;
			if (self.CanUseSkill(skillSlotSet, null, ref vector, ref releasePosition))
			{
				SkillData skillData = self.GetSkillData(skillSlotSet);
				if (skillData.StopWhenStartSkill || skillData.StopWhenCastReserveSkill)
				{
					StopMove();
				}

				self.SkillController.Cast(
					SkillUseInfo.Point(self.SkillAgent, skillSlotSet, masteryType,
						self.CharacterSkill.GetSkillEvolutionLevel(skillSlotSet.SlotSet2Index()), vector,
						releasePosition), self.GetSkillSequence(skillSlotSet));
				AfterCasting();
				return NextAction();
			}

			if (!self.CheckCooldown(skillSlotSet))
			{
				return NextAction();
			}

			if (!self.IsCanUseSkillInScriptCondition(skillSlotSet, null, vector))
			{
				return NextAction();
			}

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

				if (target != null && target.IsAlive)
				{
					return new AttackTarget(self, target);
				}
			}

			return new WaitForSkillFinish(self, skillSlotSet, target, this.nextAction);
		}
	}
}