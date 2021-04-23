using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.MagnusActive3)]
	public class MagnusActive3 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill03);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<MagnusSkillActive3Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<MagnusSkillActive3Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return Singleton<MagnusSkillActive3Data>.inst.TargetMoveDistance.ToString();
				case 3:
					return GameDB.characterState.GetData(Singleton<MagnusSkillActive3Data>.inst.StunCode).duration
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
					return Singleton<MagnusSkillActive3Data>.inst.DamageByLevel[skillData.level].ToString();
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