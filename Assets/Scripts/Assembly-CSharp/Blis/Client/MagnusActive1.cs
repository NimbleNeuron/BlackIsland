using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.MagnusActive1)]
	public class MagnusActive1 : LocalSkillScript
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
					return Singleton<MagnusSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<MagnusSkillActive1Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
				{
					int num = Math.Abs((int) GameDB.characterState
						.GetData(Singleton<MagnusSkillActive1Data>.inst.DebuffState[skillData.level]).statValue1);
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
					return "ToolTipType/DecreaseMoveRatio";
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
					return Singleton<MagnusSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
				{
					int num = Math.Abs((int) GameDB.characterState
						.GetData(Singleton<MagnusSkillActive1Data>.inst.DebuffState[skillData.level]).statValue1);
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