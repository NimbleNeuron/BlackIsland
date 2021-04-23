using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.TwoHandSwordActive)]
	public class TwoHandSwordActive : LocalSkillScript
	{
		private const string twohandsword_Weapon_Defense = "twohandsword_Weapon_Defense";


		protected static readonly int TriggerSkill = Animator.StringToHash("tTwoHandSword_Skill");


		protected static readonly int TriggerSkill_2 = Animator.StringToHash("tTwoHandSword_Skill_2");


		protected static readonly int BooleanSkill = Animator.StringToHash("bTwoHandSword_Skill");


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill, true);
			PlayAnimation(Self, TriggerSkill);
			PlayEffectChildManual(Self, "WSkill_TwoHandSword_Active", "FX_BI_WSkill_TwoHandSword_01");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayAnimation(Self, TriggerSkill_2);
				PlaySoundPoint(Self, "twohandsword_Weapon_Defense", 15);
				StopEffectChildManual(Self, "WSkill_TwoHandSword_Active", true);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill, false);
			StopEffectChildManual(Self, "WSkill_TwoHandSword_Active", true);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<TwoHandSwordSkillActiveData>.inst.BlockingDurtaion.ToString();
				case 1:
					return ((int) (SelfStat.AttackPower *
					               Singleton<TwoHandSwordSkillActiveData>.inst.DamageApCoef[skillData.level]))
						.ToString();
				case 2:
					return Singleton<TwoHandSwordSkillActiveData>.inst.DashDistance.ToString();
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

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return ((int) (SelfStat.AttackPower * Singleton<TwoHandSwordSkillActiveData>.inst.DamageApCoef[level]))
					.ToString();
			}

			return "";
		}
	}
}