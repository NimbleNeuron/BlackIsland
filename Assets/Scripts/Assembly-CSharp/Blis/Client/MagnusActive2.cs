using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.MagnusActive2)]
	public class MagnusActive2 : LocalSkillScript
	{
		public override void Start()
		{
			SetAnimation(Self, BooleanSkill02, true);
			PlayEffectChildManual(Self, "Magnus_Skill02_Effect", "FX_BI_Magnus_Skill02");
			PlaySoundChildManual(Self, "magnus_Skill02_Attack", 15, true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill02, false);
			StopEffectChildManual(Self, "Magnus_Skill02_Effect", true);
			StopSoundChildManual(Self, "magnus_Skill02_Attack");
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return skillData.ConcentrationTime.ToString();
				case 1:
					return Singleton<MagnusSkillActive2Data>.inst.DamageByLevel[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<MagnusSkillActive2Data>.inst.SkillApCoef[skillData.level] *
					               SelfStat.AttackPower)).ToString();
				case 3:
					return ((int) (Singleton<MagnusSkillActive2Data>.inst.SkillDefCoef[skillData.level] *
					               SelfStat.Defense)).ToString();
				case 4:
					return Singleton<MagnusSkillActive2Data>.inst.AttackCountByLevel[skillData.level].ToString();
				case 5:
				{
					int num = Math.Abs((int) GameDB.characterState
						.GetData(Singleton<MagnusSkillActive2Data>.inst.MoveSpeedDown).statValue1);
					return string.Format("{0}%", num);
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
					return "ToolTipType/DamageCount";
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
					return Singleton<MagnusSkillActive2Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return Singleton<MagnusSkillActive2Data>.inst.AttackCountByLevel[skillData.level].ToString();
				case 2:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}