using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LiDailinPassive)]
	public class LocalLiDailinPassive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<LiDailinSkillData>.inst.SkillReinforceExtraPoint.ToString();
				case 1:
					return Singleton<LiDailinSkillData>.inst.SkillOverExtraPoint.ToString();
				case 2:
				{
					int num = Mathf.Abs(Singleton<LiDailinSkillData>.inst.PassiveDrunkennessDecompositionAmount);
					return ((int) Mathf.Ceil(100 / num)).ToString();
				}
				case 3:
					return ((int) (Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackAp[skillData.level] *
					               SelfStat.AttackPower)).ToString();
				case 4:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(
							Singleton<LiDailinSkillData>.inst.PassiveAlcoholItemConsumeBuff[skillData.level]);
					return string.Format("{0}", (int) data.duration);
				}
				case 5:
				{
					CharacterStateData data2 =
						GameDB.characterState.GetData(
							Singleton<LiDailinSkillData>.inst.PassiveAlcoholItemConsumeBuff[skillData.level]);
					return string.Format("{0}%", (int) data2.statValue1);
				}
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

			return "StatType/AttackSpeed";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return ((int) (Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackAp[level] * SelfStat.AttackPower))
					.ToString();
			}

			if (index != 1)
			{
				return "";
			}

			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<LiDailinSkillData>.inst.PassiveAlcoholItemConsumeBuff[level]);
			return string.Format("{0}%", (int) data.statValue1);
		}
	}
}