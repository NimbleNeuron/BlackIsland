using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyunwooActive1)]
	public class HyunwooActive1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<HyunwooSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return Mathf.RoundToInt(Singleton<HyunwooSkillActive1Data>.inst.SkillApCoef * SelfStat.AttackPower)
						.ToString();
				case 2:
					return GameDB.characterState.GetData(Singleton<HyunwooSkillActive1Data>.inst.DebuffState).duration
						.ToString();
				case 3:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<HyunwooSkillActive1Data>.inst.DebuffState).statValue1);
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
					return Singleton<HyunwooSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
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