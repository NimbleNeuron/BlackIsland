using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AyaActive1)]
	public class AyaActive1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerReloadCancel);
			PlayAnimation(Self, TriggerSkill01);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }

		public override void Finish(bool cancel)
		{
			ResetAnimatorTrigger(Self, TriggerReloadCancel);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return ((int) (Singleton<AyaSkillActive1Data>.inst.SkillApCoef_2[skillData.level] *
				               SelfStat.AttackPower)).ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return Singleton<AyaSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/Damage";
				case 1:
					return "ToolTipType/CoolTime";
				case 2:
					return "ToolTipType/Cost";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<AyaSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return skillData.cooldown.ToString();
				case 2:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}