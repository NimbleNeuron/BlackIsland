using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.BowActive)]
	public class BowActive : LocalSkillScript
	{
		protected static readonly int TriggerSkill = Animator.StringToHash("tBow_Skill");


		protected static readonly int BooleanSkill = Animator.StringToHash("bBow_Skill");


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill, true);
			PlayAnimation(Self, TriggerSkill);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return (GameDB.projectile.GetData(Singleton<BowSkillActiveData>.inst.ProjectileCode)
						.lifeTimeAfterArrival + Singleton<BowSkillActiveData>.inst.SkillDamageDelay).ToString();
				case 1:
					return Singleton<BowSkillActiveData>.inst.DamageByLevel_OUT[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<BowSkillActiveData>.inst.SkillApCoef_OUT[skillData.level] *
					               SelfStat.AttackPower)).ToString();
				case 3:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<BowSkillActiveData>.inst.DebuffState[skillData.level]).statValue1);
					return string.Format("{0}%", num);
				}
				case 4:
					return Singleton<BowSkillActiveData>.inst.DamageByLevel_IN[skillData.level].ToString();
				case 5:
					return ((int) (Singleton<BowSkillActiveData>.inst.SkillApCoef_IN[skillData.level] *
					               SelfStat.AttackPower)).ToString();
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

			return "ToolTipType/InnerDamage";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<BowSkillActiveData>.inst.DamageByLevel_OUT[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return Singleton<BowSkillActiveData>.inst.DamageByLevel_IN[level].ToString();
		}
	}
}