using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.JackieActive1)]
	public class JackieActive1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectByTag(Self, "JackieSkill01Cancel");
			StopSoundByTag(Self, "JackieSkill01Cancel");
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<JackieSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<JackieSkillActive1Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return Singleton<JackieSkillActive1Data>.inst.DamageByLevel_2[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<JackieSkillActive1Data>.inst.SkillApCoef_2 * SelfStat.AttackPower))
						.ToString();
				case 4:
					return (Singleton<JackieSkillActive1Data>.inst.DFS_IntervalCount *
					        Singleton<JackieSkillActive1Data>.inst.DFS_IntervalTime).ToString();
				case 5:
					return Singleton<JackieSkillActive1Data>.inst.DFS_DamageByLevel[skillData.level].ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/FirstDamage";
				case 1:
					return "ToolTipType/SecondDamage";
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
					return Singleton<JackieSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return Singleton<JackieSkillActive1Data>.inst.DamageByLevel_2[skillData.level].ToString();
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