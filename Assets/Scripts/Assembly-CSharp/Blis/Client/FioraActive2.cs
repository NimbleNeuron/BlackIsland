using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.FioraActive2)]
	public class FioraActive2 : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Mathf
						.RoundToInt(
							(Singleton<FioraSkillActive2Data>.inst.NormalAttackApCoef[skillData.level] *
								SelfStat.AttackPower + SelfStat.IncreaseBasicAttackDamage) *
							(1f + SelfStat.IncreaseBasicAttackDamageRatio)).ToString();
				case 1:
					return Mathf
						.RoundToInt(
							(Singleton<FioraSkillActive2Data>.inst.NormalAttackApCoef2[skillData.level] *
								SelfStat.AttackPower + SelfStat.IncreaseBasicAttackDamage) *
							(1f + SelfStat.IncreaseBasicAttackDamageRatio)).ToString();
				case 2:
					return Math.Abs(Singleton<FioraSkillActive1Data>.inst.CooldownReduce).ToString();
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
					return Mathf
						.RoundToInt(
							(Singleton<FioraSkillActive2Data>.inst.NormalAttackApCoef[level] * SelfStat.AttackPower +
							 SelfStat.IncreaseBasicAttackDamage) * (1f + SelfStat.IncreaseBasicAttackDamageRatio))
						.ToString();
				case 1:
					return Mathf
						.RoundToInt(
							(Singleton<FioraSkillActive2Data>.inst.NormalAttackApCoef2[level] * SelfStat.AttackPower +
							 SelfStat.IncreaseBasicAttackDamage) * (1f + SelfStat.IncreaseBasicAttackDamageRatio))
						.ToString();
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