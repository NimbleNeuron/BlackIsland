using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.FioraActive1)]
	public class FioraActive1 : LocalSkillScript
	{
		private const string fiora_Skill01_Active = "fiora_Skill01_Active";


		private const string FioraSkill01Finish = "FioraSkill01Finish";


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
			PlaySoundPoint(Self, "fiora_Skill01_Active");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayEffectChild(target, "FX_BI_Fiora_Passive_Hit");
			}
		}


		public override void Finish(bool cancel)
		{
			StopEffectByTag(Self, "FioraSkill01Finish");
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<FioraSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return (Singleton<FioraSkillActive1Data>.inst.DamageByLevel[skillData.level] *
					        (1.2f + SelfStat.CriticalStrikeDamage)).ToString();
				case 2:
					return Mathf.RoundToInt(Singleton<FioraSkillActive1Data>.inst.SkillApCoef * SelfStat.AttackPower)
						.ToString();
				case 3:
					return Mathf.RoundToInt(Singleton<FioraSkillActive1Data>.inst.SkillApCoef *
					                        (1.2f + SelfStat.CriticalStrikeDamage) * SelfStat.AttackPower).ToString();
				case 4:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<FioraSkillActive1Data>.inst.DebuffState[skillData.level]).statValue1);
					return string.Format("{0}%", num);
				}
				case 5:
				{
					int num2 = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<FioraSkillActive1Data>.inst.MarkingSlowState[skillData.level]).statValue1);
					return string.Format("{0}%", num2);
				}
				case 6:
					return GameDB.characterState
						.GetData(Singleton<FioraSkillActive1Data>.inst.DebuffState[skillData.level]).duration
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
					return Singleton<FioraSkillActive1Data>.inst.DamageByLevel[level].ToString();
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