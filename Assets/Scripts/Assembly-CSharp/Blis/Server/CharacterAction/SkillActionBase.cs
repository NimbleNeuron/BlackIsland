using Blis.Common;
using UnityEngine;

namespace Blis.Server.CharacterAction
{
	
	public abstract class SkillActionBase : ActionBase
	{
		
		private bool isAddPlayAgain;

		
		protected bool isCastSkill;

		
		protected MasteryType masteryType;

		
		protected ActionBase nextAction;

		
		private Vector3 playAgainCursorPosition;

		
		private SkillData playAgainSkillData;

		
		protected SkillSlotSet skillSlotSet;

		
		protected SkillActionBase(WorldMovableCharacter self, bool findAttackTarget) : base(self, findAttackTarget)
		{
			nextAction = null;
		}

		
		
		public SkillSlotSet SkillSlotSet => skillSlotSet;

		
		
		public MasteryType MasteryType => masteryType;

		
		
		public bool IsCastSkill => isCastSkill;

		
		public abstract WorldCharacter GetTargetCharacter();

		
		public void AddPlayAgain(Vector3 cursorPosition, SkillData skillData)
		{
			isAddPlayAgain = true;
			playAgainCursorPosition = cursorPosition;
			playAgainSkillData = skillData;
		}

		
		protected void AfterCasting()
		{
			if (!isAddPlayAgain)
			{
				return;
			}

			self.PlayAgain(skillSlotSet, playAgainCursorPosition, playAgainSkillData);
		}

		
		public virtual void SetNextAction(ActionBase action)
		{
			SkillData skillData = self.GetSkillData(skillSlotSet);
			if (skillData == null)
			{
				return;
			}

			if (action != null)
			{
				MoveTo moveTo;
				if ((moveTo = action as MoveTo) == null)
				{
					AttackTarget attackTarget;
					if ((attackTarget = action as AttackTarget) != null)
					{
						AttackTarget attackTarget2 = attackTarget;
						if (skillData.StopAttackWhenStartSkill)
						{
							return;
						}

						WorldCharacter targetCharacter = attackTarget2.GetTargetCharacter();
						if (targetCharacter == null)
						{
							return;
						}

						if (skillData.CastWaysType.IsTargeting())
						{
							WorldCharacter targetCharacter2 = GetTargetCharacter();
							if (targetCharacter2 != null && self.GetHostileType(targetCharacter2) == HostileType.Enemy)
							{
								return;
							}
						}

						nextAction = new AttackTarget(self, targetCharacter);
					}
				}
				else
				{
					MoveTo moveTo2 = moveTo;
					if (skillData.StartPrevMoveWhenFinishSkill)
					{
						nextAction = new MoveTo(self, moveTo2.Destination, moveTo2.FindAttackTarget);
					}
				}
			}
		}
	}
}