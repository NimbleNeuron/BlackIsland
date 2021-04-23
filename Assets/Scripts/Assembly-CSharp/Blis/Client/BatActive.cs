using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.BatActive)]
	public class BatActive : LocalSkillScript
	{
		protected static readonly int TriggerSkill = Animator.StringToHash("tBat_Skill");

		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return ((int) (Singleton<BatSkillActiveData>.inst.SkillApCoef[skillData.level] *
					               SelfStat.AttackPower)).ToString();
				case 1:
					return Singleton<BatSkillActiveData>.inst.StunDuration.ToString();
				case 2:
					return Singleton<BatSkillActiveData>.inst.DamageByLevel[skillData.level].ToString();
				case 3:
					return Singleton<BatSkillActiveData>.inst.KnockBackDistance.ToString();
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

			return "ToolTipType/SkillApCoef";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<BatSkillActiveData>.inst.DamageByLevel[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return Singleton<BatSkillActiveData>.inst.SkillApCoef[level].ToString();
		}
	}
}