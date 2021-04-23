using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ShoichiActive1_1)]
	public class ShoichiActive1_1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetNoParentEffectByTag(Self, "FX_Shoichi_Skill_01_Range");
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ShoichiSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<ShoichiSkillActive1Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return Math.Abs(Singleton<ShoichiSkillActive1Data>.inst.CooldownReduce).ToString();
				case 3:
					return skillData.cooldown.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Damage";
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
				return Singleton<ShoichiSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cost.ToString();
		}
	}
}