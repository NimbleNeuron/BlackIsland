using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.XiukaiActive2)]
	public class XiukaiActive2 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
			SetAnimation(Self, BooleanMotionWait, true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<XiukaiSkillActive2Data>.inst.BaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<XiukaiSkillActive2Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.characterState
						.GetData(Singleton<XiukaiSkillActive2Data>.inst.DebuffState[skillData.level]).duration
						.ToString();
				case 3:
				{
					int num = Math.Abs((int) GameDB.characterState
						.GetData(Singleton<XiukaiSkillActive2Data>.inst.DebuffState[skillData.level]).statValue1);
					return string.Format("{0}%", num);
				}
				case 4:
					return GameDB.characterState
						.GetData(Singleton<XiukaiSkillActive2Data>.inst.DebuffState2[skillData.level]).duration
						.ToString();
				case 5:
				{
					float f = Singleton<XiukaiSkillActive2Data>.inst.SkillAddDamageMaxHpRatio[skillData.level];
					return string.Format("{0}%", Mathf.Abs(f));
				}
				case 6:
					return ((int) (Singleton<XiukaiSkillActive2Data>.inst.SkillAddDamageMaxHpRatio[skillData.level] /
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
					return "ToolTipType/SkillAddDamageMaxHpRatio";
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
					return Singleton<XiukaiSkillActive2Data>.inst.BaseDamage[level].ToString();
				case 1:
				{
					float num = Singleton<XiukaiSkillActive2Data>.inst.SkillAddDamageMaxHpRatio[level];
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