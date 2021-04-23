using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.OneHandSwordActive)]
	public class OneHandSwordActive : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectPoint(Self, "FX_BI_WSkill_OneHandSword_02");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<OneHandSwordSkillActiveData>.inst.BuffState[skillData.level]).duration
						.ToString();
				case 1:
				{
					float statValue = GameDB.characterState
						.GetData(Singleton<OneHandSwordSkillActiveData>.inst.BuffState[skillData.level]).statValue1;
					return string.Format("{0}%", statValue);
				}
				case 2:
					return GameDB.characterState
						.GetData(Singleton<OneHandSwordSkillActiveData>.inst.BuffState_2[skillData.level]).duration
						.ToString();
				case 3:
					return ((int) ((SelfStat.AttackPower * (2f + SelfStat.CriticalStrikeDamage) +
					                SelfStat.IncreaseBasicAttackDamage) *
					               (1f + SelfStat.IncreaseBasicAttackDamageRatio))).ToString();
				case 4:
				{
					int daggerAttackDamageRatioPerHp =
						Singleton<OneHandSwordSkillActiveData>.inst.DaggerAttackDamageRatioPerHp;
					return string.Format("{0}%", daggerAttackDamageRatioPerHp);
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "StatType/MoveSpeedRatio";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/Time";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				float statValue = GameDB.characterState
					.GetData(Singleton<OneHandSwordSkillActiveData>.inst.BuffState[level]).statValue1;
				return string.Format("{0}%", statValue);
			}

			if (index != 1)
			{
				return "";
			}

			return GameDB.characterState.GetData(Singleton<OneHandSwordSkillActiveData>.inst.BuffState[level]).duration
				.ToString();
		}
	}
}