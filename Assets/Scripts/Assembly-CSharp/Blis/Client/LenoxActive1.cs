using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LenoxActive1)]
	public class LenoxActive1 : LocalSkillScript
	{
		private const string Lenox_Skill01_sfx = "Lenox_Skill01";


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill01, true);
			SetAnimation(Self, BooleanMotionWait, true);
			PlaySoundPoint(Self, "Lenox_Skill01", 15);
			PlayEffectChildManual(Self, "LenoxSkil01_range", "FX_BI_Lenox_Skill01_Range");
			StartCoroutine(CoroutineUtil.DelayedAction(0.26f,
				delegate { PlayEffectChildManual(Self, "LenoxSkil01_attack", "FX_BI_Lenox_Skill01_Attack_01"); }));
			PlayAnimation(Self, TriggerSkill01);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				StartCoroutine(CoroutineUtil.DelayedAction(0.09f, delegate
				{
					SetNoParentEffectManual(Self, "LenoxSkil01_range");
					SetNoParentEffectManual(Self, "LenoxSkil01_attack");
				}));
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill01, false);
			SetAnimation(Self, BooleanMotionWait, false);
			StopEffectChildManual(Self, "LenoxSkil01_range", true);
			StopEffectChildManual(Self, "LenoxSkil01_attack", true);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<LenoxSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<LenoxSkillActive1Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
				{
					float num = 100f *
					            Singleton<LenoxSkillActive1Data>.inst.SmageMaxHPPerDamageByLevel[skillData.level];
					return string.Format("{0}%", num);
				}
				case 3:
				{
					float num2 = 1f - 1f * SelfStat.CooldownReduction;
					if (num2 >= 0.9f)
					{
						return 1.ToString();
					}

					return num2.ToString("N1");
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/Damage";
				case 1:
					return "ToolTipType/SkillAddDamageMaxHpRatio";
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
					return Singleton<LenoxSkillActive1Data>.inst.DamageByLevel[level].ToString();
				case 1:
					return (100f * Singleton<LenoxSkillActive1Data>.inst.SmageMaxHPPerDamageByLevel[level])
						.ToString("N1") + "%";
				case 2:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}