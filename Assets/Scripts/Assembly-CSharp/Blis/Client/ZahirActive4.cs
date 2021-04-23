using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ZahirActive4)]
	public class ZahirActive4 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill04);
			SetAnimation(Self, BooleanSkill04, true);
			PlaySoundPoint(Self, "zahir_Skill04_Ready", 15);
			StartCoroutine(CoroutineUtil.DelayedAction(1f, delegate { SetAnimation(Self, BooleanSkill04, false); }));
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ZahirSkillActive4Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return Singleton<ZahirSkillActive4Data>.inst.DamageByLevel_2[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<ZahirSkillActive4Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 3:
					return ((int) (Singleton<ZahirSkillActive4Data>.inst.SkillApCoef_2 * SelfStat.AttackPower))
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
					return Singleton<ZahirSkillActive4Data>.inst.DamageByLevel[level].ToString();
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