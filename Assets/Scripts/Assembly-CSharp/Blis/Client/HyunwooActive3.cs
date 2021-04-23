using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyunwooActive3)]
	public class HyunwooActive3 : LocalSkillScript
	{
		protected static readonly int BooleanSkill03_2 = Animator.StringToHash("bSkill03_2");


		protected static readonly int BooleanSkill03_3 = Animator.StringToHash("bSkill03_3");


		private GameObject Skill03Attack;


		private GameObject Skill03Move;


		private GameObject Skill03Slide;

		public override void Start()
		{
			SetAnimation(Self, BooleanSkill03, true);
			Skill03Move = PlayEffectChild(Self, "FX_BI_Hyunwoo_Skill03_Move_02");
			Skill03Attack = PlayEffectChild(Self, "FX_BI_Hyunwoo_Skill03_Attack");
			Skill03Slide = PlayEffectChild(Self, "FX_BI_Character_Skill", "Fx_Center");
			PlaySoundPoint(Self, "hyunwoo_Skill03_Slide");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				SetAnimation(Self, BooleanSkill03, false);
				SetAnimation(Self, BooleanSkill03_2, true);
				return;
			}

			if (action == 2)
			{
				SetAnimation(Self, BooleanSkill03_2, false);
				SetAnimation(Self, BooleanSkill03_3, true);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill03, false);
			SetAnimation(Self, BooleanSkill03_2, false);
			SetAnimation(Self, BooleanSkill03_3, false);
			if (Skill03Move != null)
			{
				Skill03Move.transform.parent = null;
			}

			if (Skill03Attack != null)
			{
				Skill03Attack.transform.parent = null;
			}

			if (Skill03Slide != null)
			{
				Skill03Slide.transform.parent = null;
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
				{
					int num = Mathf.RoundToInt(Singleton<HyunwooSkillActive3Data>.inst.RateByLevel[skillData.level] *
					                           100f);
					return string.Format("{0}%", num);
				}
				case 1:
					return Singleton<HyunwooSkillActive3Data>.inst.StunDuration.ToString();
				case 2:
					return Singleton<HyunwooSkillActive3Data>.inst.DamageByLevel[skillData.level].ToString();
				case 3:
					return Mathf.RoundToInt(SelfStat.Defense * Singleton<HyunwooSkillActive3Data>.inst.SkillDefCoef)
						.ToString();
				case 4:
				{
					int num2 = Math.Abs((int) GameDB.characterState
						.GetData(Singleton<HyunwooSkillActive3Data>.inst.DebuffState[skillData.level]).statValue1);
					return string.Format("{0}%", num2);
				}
				case 5:
					return ((int) GameDB.characterState
							.GetData(Singleton<HyunwooSkillActive3Data>.inst.DebuffState[skillData.level]).duration)
						.ToString();
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
					return "StatType/DefenseRatio";
				case 2:
					return "ToolTipType/CoolTime";
				case 3:
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
					return Singleton<HyunwooSkillActive3Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
				{
					int num = Math.Abs((int) GameDB.characterState
						.GetData(Singleton<HyunwooSkillActive3Data>.inst.DebuffState[skillData.level]).statValue1);
					return string.Format("{0}%", num);
				}
				case 2:
					return skillData.cooldown.ToString();
				case 3:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}