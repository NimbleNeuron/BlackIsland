using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RozziActive3_2)]
	public class LocalRozziActive3_2 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill03_2);
			PlaySoundPoint(Self, "Rozzi_Skill03_Fire", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<RozziSkillActive3Data>.inst.DamageActive3_1ByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<RozziSkillActive3Data>.inst.DamageActive3_1ApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.skill.GetSkillGroupData(Singleton<RozziSkillActive3Data>.inst.Active3_2Group)
						.castWaitTime.ToString();
				case 3:
					return Singleton<RozziSkillActive3Data>.inst.DamageActive3_2ByLevel[skillData.level].ToString();
				case 4:
					return ((int) (Singleton<RozziSkillActive3Data>.inst.DamageActive3_2ApCoef * SelfStat.AttackPower))
						.ToString();
				case 5:
					return Singleton<RozziSkillActive3Data>.inst.Active3KnockBackStunDuration.ToString();
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
					return Singleton<RozziSkillActive3Data>.inst.DamageActive3_1ByLevel[level].ToString();
				case 1:
					return Singleton<RozziSkillActive3Data>.inst.DamageActive3_2ByLevel[level].ToString();
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