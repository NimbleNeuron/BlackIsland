using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.FioraActive3_2)]
	public class FioraActive3_2 : LocalSkillScript
	{
		public override void Start()
		{
			SetAnimation(Self, BooleanSkill03, true);
			PlayAnimation(Self, TriggerSkill03_3);
			PlayEffectChildManual(Self, "Fiora_Skill03_Back", "FX_BI_Fiora_Skill03_Back");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill03, false);
			StopEffectChildManual(Self, "Fiora_Skill03_Back", false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<FioraSkillActive3Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<FioraSkillActive3Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 4:
					return Singleton<FioraSkillActive3Data>.inst.NextActionWaitTime.ToString();
				case 6:
					return ((int) Math.Abs(Singleton<FioraSkillActive3Data>.inst.CooldownReduce2)).ToString();
				case 7:
					return Mathf.RoundToInt(Singleton<FioraSkillActive3Data>.inst.DamageByLevel[skillData.level] *
					                        (1.2f + SelfStat.CriticalStrikeDamage)).ToString();
				case 8:
					return Mathf.RoundToInt(Singleton<FioraSkillActive3Data>.inst.SkillApCoef *
					                        (1.2f + SelfStat.CriticalStrikeDamage) * SelfStat.AttackPower).ToString();
			}

			return "";
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

			return "ToolTipType/CoolTime";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<FioraSkillActive3Data>.inst.DamageByLevel[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cooldown.ToString();
		}
	}
}