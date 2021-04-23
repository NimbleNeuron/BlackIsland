using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.XiukaiActive3)]
	public class XiukaiActive3 : LocalSkillScript
	{
		private const string Skill03_Front = "FX_BI_Xiukai_Skill03_Front";


		private const string Skill03_Front_key = "Skill03_Frontkey";


		private const string Skill03_Back = "FX_BI_Xiukai_Skill03_Back";


		private const string Skill03_Back_key = "Skill03_Backkey";


		private const string Skill03_Hit = "FX_BI_Xiukai_Skill03_Hit";


		private const string Skill03_Hit_key = "Skill03_Hit";


		private const string Hitpoint = "Bip001";

		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill03);
			SetAnimation(Self, BooleanSkill03, true);
			SetAnimation(Self, BooleanMotionWait, true);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 2)
			{
				PlayAnimation(Self, TriggerSkill03_2);
				PlayEffectChildManual(Self, "Skill03_Frontkey", "FX_BI_Xiukai_Skill03_Front");
				PlayEffectChildManual(Self, "Skill03_Backkey", "FX_BI_Xiukai_Skill03_Back");
				return;
			}

			if (actionNo == 3)
			{
				StopEffectChildManual(Self, "Skill03_Frontkey", true);
				StopEffectChildManual(Self, "Skill03_Backkey", true);
				PlayAnimation(Self, TriggerSkill03_3);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill03, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<XiukaiSkillActive3Data>.inst.BaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<XiukaiSkillActive3Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
				{
					float f = Singleton<XiukaiSkillActive3Data>.inst.SkillAddDamageMaxHpRatio[skillData.level];
					return string.Format("{0}%", Mathf.Abs(f));
				}
				case 3:
				{
					float airborneDuration = Singleton<XiukaiSkillActive3Data>.inst.AirborneDuration;
					return airborneDuration.ToString();
				}
				case 4:
				{
					float f2 = Singleton<XiukaiSkillActive3Data>.inst.SkillAddDamageMaxHpRatio2[skillData.level];
					return string.Format("{0}%", Mathf.Abs(f2));
				}
				case 5:
					return ((int) (Singleton<XiukaiSkillActive3Data>.inst.SkillAddDamageMaxHpRatio[skillData.level] /
						100f * SelfStat.MaxHp)).ToString();
				case 6:
					return ((int) (Singleton<XiukaiSkillActive3Data>.inst.SkillAddDamageMaxHpRatio2[skillData.level] /
						100f * SelfStat.MaxHp)).ToString();
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
					return Singleton<XiukaiSkillActive3Data>.inst.BaseDamage[level].ToString();
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