using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AyaActive3)]
	public class AyaActive3 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerReloadCancel);
			SetAnimation(Self, BooleanSkill03, true);
			PlayAnimation(Self, TriggerSkill03);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			ResetAnimatorTrigger(Self, TriggerReloadCancel);
			SetAnimation(Self, BooleanSkill03, false);
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/CoolTime";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/Cost";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return skillData.cooldown.ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cost.ToString();
		}
	}
}