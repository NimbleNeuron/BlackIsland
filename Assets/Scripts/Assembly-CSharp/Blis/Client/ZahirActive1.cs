using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ZahirActive1)]
	public class ZahirActive1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
			SetAnimation(Self, BooleanSkill01, true);
			StartCoroutine(CoroutineUtil.DelayedAction(1f, delegate { SetAnimation(Self, BooleanSkill01, false); }));
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ZahirSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
				{
					int num = Singleton<ZahirSkillActive1Data>.inst.DebuffStackByLevel_2[skillData.level];
					return string.Format("{0}%", num);
				}
				case 2:
					return (Singleton<ZahirSkillActive1Data>.inst.DamageByLevel_2[skillData.level] -
					        Singleton<ZahirSkillActive1Data>.inst.DamageByLevel[skillData.level]).ToString();
				case 3:
					return ((int) (Singleton<ZahirSkillActive1Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 4:
				{
					int num2 = Mathf.Abs(Singleton<ZahirSkillActive1Data>.inst.ZahirSkillActive1DebuffMinBonus);
					return string.Format("{0}%", num2);
				}
				case 5:
					return GameDB.characterState.GetData(Singleton<ZahirSkillActive1Data>.inst.DebuffState_2).duration
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
					return "ToolTipType/Cost";
				case 2:
					return "ToolTipType/AdditionalDamage";
				case 3:
					return "ToolTipType/DecreaseDefenseRatio";
				case 4:
					return "ToolTipType/CoolTime";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ZahirSkillActive1Data>.inst.DamageByLevel[level].ToString();
				case 1:
					return skillData.cost.ToString();
				case 2:
					return (Singleton<ZahirSkillActive1Data>.inst.DamageByLevel_2[level] -
					        Singleton<ZahirSkillActive1Data>.inst.DamageByLevel[level]).ToString();
				case 3:
				{
					int num = Singleton<ZahirSkillActive1Data>.inst.DebuffStackByLevel_2[level];
					return string.Format("{0}%", num);
				}
				case 4:
					return skillData.cooldown.ToString();
				default:
					return "";
			}
		}
	}
}