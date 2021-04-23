using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.YukiActive1)]
	public class YukiActive1 : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<YukiSkillActive1Data>.inst.AttackDamage[skillData.level].ToString();
				case 1:
					return Mathf
						.RoundToInt((GetEquipWeaponMasteryType(Self) == MasteryType.DualSword
							? Singleton<YukiSkillActive1Data>.inst.DualSworldNormalAttackApCoef
							: Singleton<YukiSkillActive1Data>.inst.NormalAttackApCoef) * SelfStat.AttackPower)
						.ToString();
				case 2:
					return Singleton<YukiSkillActive1Data>.inst.SlowDuration.ToString();
				case 3:
				{
					float num = Math.Abs(Singleton<YukiSkillActive1Data>.inst.SlowMoveSpeedRatio);
					return string.Format("{0}%", num);
				}
				case 4:
					return Singleton<YukiSkillActive1Data>.inst.StunDuration.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Damage";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/CoolTime";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<YukiSkillActive1Data>.inst.AttackDamage[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cooldown.ToString();
		}
	}
}