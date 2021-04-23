using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HartActive1_2)]
	public class HartActive1_2 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01_2);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return ((int) (1f / Singleton<HartSkillActive1Data>.inst.ChargeDuration)).ToString();
				case 1:
				{
					int num = (int) (1f / Singleton<HartSkillActive1Data>.inst.ChargeDuration);
					return ((int) (GameDB.characterState.GetData(Singleton<HartSkillActive1Data>.inst.BuffState)
						.duration - num)).ToString();
				}
				case 2:
					return Singleton<HartSkillActive1Data>.inst.MinSkillDamage[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<HartSkillActive1Data>.inst.MinSkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 4:
					return Singleton<HartSkillActive1Data>.inst.MaxSkillDamage[skillData.level].ToString();
				case 5:
					return ((int) (Singleton<HartSkillActive1Data>.inst.MaxSkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 6:
				{
					int num2 = evolutionLevel;
					if (num2 < 1)
					{
						num2 = 1;
					}

					float num3 = Mathf.Abs(GameDB.characterState
						.GetData(Singleton<HartSkillActive1Data>.inst.DebuffState[num2]).statValue1);
					return string.Format("{0}%", num3);
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/MinDamage";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/MaxDamage";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<HartSkillActive1Data>.inst.MinSkillDamage[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return Singleton<HartSkillActive1Data>.inst.MaxSkillDamage[level].ToString();
		}
	}
}