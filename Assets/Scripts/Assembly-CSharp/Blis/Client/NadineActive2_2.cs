using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.NadineActive2_2)]
	public class NadineActive2_2 : LocalSkillScript
	{
		public override void Start()
		{
			SetAnimation(Self, BooleanSkill02, true);
			PlayAnimation(Self, TriggerSkill02);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill02, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<NadineSkillActive2Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return Singleton<NadineSkillActive2Data>.inst.ExclusiveViewSkillReuseTime.ToString();
				case 2:
					return Singleton<NadineSkillActive2Data>.inst.MaxLinkRange.ToString();
				case 3:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<NadineSkillActive2Data>.inst.DebuffState).statValue1);
					return string.Format("{0}%", num);
				}
				case 4:
					return Singleton<NadineSkillActive2Data>.inst.GainSightDuration.ToString();
				case 5:
					return Singleton<NadineSkillActive2Data>.inst.ExclusiveViewTrapConnectTime.ToString();
				case 6:
					return ((int) (Singleton<NadineSkillActive2Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 7:
					return Singleton<NadineSkillActive2Data>.inst.DamageByLevel_2[skillData.level].ToString();
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
					return "ToolTipType/TrapDamage";
				case 2:
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
					return Singleton<NadineSkillActive2Data>.inst.DamageByLevel[level].ToString();
				case 1:
					return Singleton<NadineSkillActive2Data>.inst.DamageByLevel_2[level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				default:
					return "";
			}
		}
	}
}