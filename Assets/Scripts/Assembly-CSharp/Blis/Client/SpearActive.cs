using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SpearActive)]
	public class SpearActive : LocalSkillScript
	{
		protected static readonly int TriggerSkill = Animator.StringToHash("tSpear_Skill");

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
					return Singleton<SpearSkillActiveData>.inst.FirstRange.ToString();
				case 1:
					return ((int) (Singleton<SpearSkillActiveData>.inst.SkillApCoef[skillData.level] *
					               SelfStat.AttackPower)).ToString();
				case 2:
					return Singleton<SpearSkillActiveData>.inst.SecondRange.ToString();
				case 3:
					return GameDB.characterState
						.GetData(Singleton<SpearSkillActiveData>.inst.DebuffState[skillData.level]).duration.ToString();
				case 4:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<SpearSkillActiveData>.inst.DebuffState[skillData.level]).statValue1);
					return string.Format("{0}%", num);
				}
				case 5:
					return SelfStat.AttackRange.ToString();
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

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return ((int) (Singleton<SpearSkillActiveData>.inst.SkillApCoef[level] * SelfStat.AttackPower))
					.ToString();
			}

			return "";
		}
	}
}