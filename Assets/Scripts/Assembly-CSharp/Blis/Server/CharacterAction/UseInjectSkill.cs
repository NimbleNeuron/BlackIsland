using UnityEngine;

namespace Blis.Server.CharacterAction
{
	
	public class UseInjectSkill : SkillActionBase
	{
		
		private readonly SkillUseInfo skillUseInfo;

		
		private readonly WorldCharacter target;

		
		public UseInjectSkill(WorldMovableCharacter self, SkillUseInfo skillUseInfo) : base(self, false)
		{
			this.skillUseInfo = skillUseInfo;
			skillSlotSet = skillUseInfo.skillSlotSet;
			target = skillUseInfo.target.Character;
		}

		
		public override WorldObject GetTarget()
		{
			return skillUseInfo.target.Character;
		}

		
		public override WorldCharacter GetTargetCharacter()
		{
			return skillUseInfo.target.Character;
		}

		
		public override void SetNextAction(ActionBase action)
		{
			base.SetNextAction(action);
			if (action != null && (action is UsePointSkill || action is UseTargetSkill))
			{
				nextAction = action;
			}
		}

		
		public override void Start()
		{
			Vector3 cursorPosition = skillUseInfo.cursorPosition;
			Vector3 releasePosition = skillUseInfo.releasePosition;
			if (self.CanUseInjectSkill(skillUseInfo.SkillCode, null, ref cursorPosition, ref releasePosition))
			{
				if (skillUseInfo.skillData.StopWhenStartSkill)
				{
					StopMove();
				}

				self.SkillController.Cast(skillUseInfo, 0);
				AfterCasting();
				isCastSkill = true;
				return;
			}

			if (!self.SkillController.CanActionDuringSkillPlaying())
			{
				return;
			}

			if (self.CanMove())
			{
				self.SkillController.CancelNormalAttack();
				self.MoveToDestination(skillUseInfo.cursorPosition);
			}
		}

		
		protected override ActionBase Update()
		{
			if (isCastSkill)
			{
				return NextAction();
			}

			Vector3 cursorPosition = skillUseInfo.cursorPosition;
			Vector3 releasePosition = skillUseInfo.releasePosition;
			if (self.CanUseInjectSkill(skillUseInfo.SkillCode, target, ref cursorPosition, ref releasePosition))
			{
				if (skillUseInfo.skillData.StopWhenStartSkill || skillUseInfo.skillData.StopWhenCastReserveSkill)
				{
					StopMove();
				}

				self.SkillController.Cast(skillUseInfo, 0);
				AfterCasting();
				return NextAction();
			}

			if (!self.IsCanUseSkillInScriptCondition(skillSlotSet, target, cursorPosition))
			{
				return NextAction();
			}

			return null;
		}

		
		private ActionBase NextAction()
		{
			return new WaitForSkillFinish(self, skillUseInfo.skillSlotSet, target, nextAction);
		}
	}
}