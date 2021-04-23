using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RapierActive)]
	public class RapierActive : LocalSkillScript
	{
		protected static readonly int TriggerSkill = Animator.StringToHash("tRapier_Skill");


		protected static readonly int BooleanSkill = Animator.StringToHash("bRapier_Skill");


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill, true);
			PlayAnimation(Self, TriggerSkill);
			StartCoroutine(CoroutineUtil.FrameDelayedAction(4,
				delegate
				{
					PlayEffectChildManual(Self, "Rapier_WeaponSkill", "FX_BI_WSkill_Rapier_01", "Top_Trail");
				}));
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopCoroutines();
			SetAnimation(Self, BooleanSkill, false);
			StopEffectChildManual(Self, "Rapier_WeaponSkill", false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return ((int) (Singleton<RapierSkillActiveData>.inst.SkillApCoef[skillData.level] *
				               (2f + SelfStat.CriticalStrikeDamage) * SelfStat.AttackPower)).ToString();
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
				return ((int) (Singleton<RapierSkillActiveData>.inst.SkillApCoef[level] *
				               (2f + SelfStat.CriticalStrikeDamage) * SelfStat.AttackPower)).ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cooldown.ToString();
		}
	}
}