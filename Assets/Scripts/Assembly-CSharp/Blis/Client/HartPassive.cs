using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HartPassive)]
	public class HartPassive : LocalSkillScript
	{
		private int useSkillCount;


		public override void Start() { }


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return ((int) (SelfStat.MaxSp *
				               (Singleton<HartSkillPassiveData>.inst.RecoverySpRatio[skillData.level] * 0.01f)))
					.ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return ((int) (SelfStat.AttackPower * Singleton<HartSkillPassiveData>.inst.PassiveBonusAttackApCoef))
				.ToString();
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/SpHeal";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return ((int) (SelfStat.MaxSp * Singleton<HartSkillPassiveData>.inst.RecoverySpRatio[level] * 0.01f))
					.ToString();
			}

			return "";
		}
	}
}