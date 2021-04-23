using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.DualSwordActive_1)]
	public class DualSwordActive_1 : LocalSkillScript
	{
		protected readonly int BooleanWeaponSkill = Animator.StringToHash("bDualSword_Skill");


		protected readonly int TriggerWeaponSkill = Animator.StringToHash("tDualSword_Skill");

		public override void Start()
		{
			SetAnimation(Self, BooleanWeaponSkill, true);
			PlayAnimation(Self, TriggerWeaponSkill);
			PlayEffectChildManual(Self, "WSkill_DualSword_Move", "FX_BI_WSkill_DualSword_01", "Fx_Center");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayEffectChildManual(Self, "WSkill_DualSword_Attack_L", "FX_BI_WSkill_DualSword_02", "TrailPoint_L");
				PlayEffectChildManual(Self, "WSkill_DualSword_Attack_R", "FX_BI_WSkill_DualSword_02", "TrailPoint_R");
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanWeaponSkill, false);
			ResetAnimatorTrigger(Self, TriggerWeaponSkill);
			StopEffectChildManual(Self, "WSkill_DualSword_Move", true);
			StopEffectChildManual(Self, "WSkill_DualSword_Attack_L", true);
			StopEffectChildManual(Self, "WSkill_DualSword_Attack_R", true);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return ((int) (Singleton<DualSwordSkillActiveData>.inst.DamageApCoef[skillData.level] *
				               SelfStat.AttackPower)).ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return Singleton<DualSwordSkillActiveData>.inst.WaitingForCast.ToString();
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Damage";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return ((int) (Singleton<DualSwordSkillActiveData>.inst.DamageApCoef[level] * SelfStat.AttackPower))
					.ToString();
			}

			return "";
		}
	}
}