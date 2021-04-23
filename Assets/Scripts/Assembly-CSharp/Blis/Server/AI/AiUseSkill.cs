using Blis.Common;
using ParadoxNotion.Design;
using UnityEngine;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("스킬을 사용한다.")]
	public class AiUseSkill : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			if (!base.agent.IsAlive)
			{
				base.EndAction(false);
				return;
			}
			if (!base.agent.IsTypeOf<WorldPlayerCharacter>())
			{
				base.EndAction(false);
				return;
			}
			WorldPlayerCharacter worldPlayerCharacter = base.agent as WorldPlayerCharacter;
			WorldCharacter worldCharacter = base.agent.Controller.GetTarget() as WorldCharacter;
			if (worldCharacter == null)
			{
				base.EndAction(false);
				return;
			}
			SkillSlotSet? skillSlotSet = base.agent.CharacterSkill.GetSkillSlotSet(this.skillSlotIndex);
			if (skillSlotSet == null)
			{
				return;
			}
			switch (worldPlayerCharacter.GetSkillData(skillSlotSet.Value, -1).CastWaysType)
			{
			case SkillCastWaysType.Instant:
			case SkillCastWaysType.PickPoint:
			case SkillCastWaysType.PickPointInArea:
			case SkillCastWaysType.PickPointThenDirection:
				base.agent.UseSkill(skillSlotSet.Value, worldCharacter.GetPosition(), worldCharacter.GetPosition());
				break;
			case SkillCastWaysType.Directional:
			{
				Vector3 b = GameUtil.DirectionOnPlane(worldPlayerCharacter.GetPosition(), worldCharacter.GetPosition());
				base.agent.UseSkill(skillSlotSet.Value, worldCharacter.GetPosition(), worldCharacter.GetPosition() + b);
				break;
			}
			case SkillCastWaysType.PickTargetEdge:
			case SkillCastWaysType.PickTargetCenter:
				base.agent.UseSkill(skillSlotSet.Value, worldCharacter);
				break;
			}
			base.EndAction(true);
		}

		
		public SkillSlotIndex skillSlotIndex;
	}
}
