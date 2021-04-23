using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.FioraActive4_1)]
	public class FioraActive4_1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill04);
			SetAnimation(Self, BooleanSkill04, true);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<FioraSkillActive4Data>.inst.ConsumeSp[skillData.level].ToString();
				case 1:
					return Singleton<FioraSkillActive4Data>.inst.SkillAttack[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<FioraSkillActive4Data>.inst.SkillAttackApCoef[skillData.level] *
					               SelfStat.AttackPower)).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ItemOptionCategory/NormalAttackIncrease";
				case 1:
					return "ToolTipType/SkillApCoef";
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
					return Singleton<FioraSkillActive4Data>.inst.SkillAttack[level].ToString();
				case 1:
				{
					float num = Singleton<FioraSkillActive4Data>.inst.SkillAttackApCoef[skillData.level];
					return string.Format("{0}", num);
				}
				case 2:
					return Singleton<FioraSkillActive4Data>.inst.ConsumeSp[level].ToString();
				default:
					return "";
			}
		}
	}
}