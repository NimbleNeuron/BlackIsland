using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LenoxPassive)]
	public class LenoxPassive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
				{
					float num = 100f * Singleton<LenoxSkillPassiveData>.inst.PassiveBuffMaxHpRatio;
					return string.Format("{0}%", (int) num);
				}
				case 1:
					return ((int) GameDB.characterState
						.GetData(Singleton<LenoxSkillPassiveData>.inst.PassiveCaptainBuffCode[skillData.level])
						.duration).ToString();
				case 2:
				{
					float num2 = Singleton<LenoxSkillPassiveData>.inst.AnglerRewardItemProbability[ItemGrade.Common];
					return string.Format("{0}%", (int) num2);
				}
				case 3:
				{
					float num3 = Singleton<LenoxSkillPassiveData>.inst.AnglerRewardItemProbability[ItemGrade.Uncommon];
					return string.Format("{0}%", (int) num3);
				}
				case 4:
				{
					float num4 = Singleton<LenoxSkillPassiveData>.inst.AnglerRewardItemProbability[ItemGrade.Rare];
					return string.Format("{0}%", (int) num4);
				}
				case 5:
					return skillData.cooldown.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/CoolTime";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return skillData.cooldown.ToString();
			}

			return "";
		}
	}
}