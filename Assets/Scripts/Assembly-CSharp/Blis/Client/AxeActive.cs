using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AxeActive)]
	public class AxeActive : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "WSkill_Axe_Buff", "FX_BI_WSkill_Axe_01");
			PlaySoundPoint(Self, "Common_Axe_Active");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "WSkill_Axe_Buff", true);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState.GetData(Singleton<AxeSkillActiveData>.inst.BuffState).maxStack
						.ToString();
				case 1:
					return GameDB.characterState.GetData(Singleton<AxeSkillActiveData>.inst.BuffState)
						.nonCalculateStatValue + "%";
				case 2:
					return Singleton<AxeSkillActiveData>.inst.Duration[skillData.level].ToString();
				case 3:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<AxeSkillActiveData>.inst.BuffState);
					float num = Singleton<AxeSkillActiveData>.inst.BuffIncreaseValue + data.nonCalculateStatValue;
					return string.Format("{0}%", num);
				}
				case 4:
				{
					CharacterStateData data2 =
						GameDB.characterState.GetData(Singleton<AxeSkillActiveData>.inst.BuffState);
					float num2 = Singleton<AxeSkillActiveData>.inst.BuffIncreaseValue + data2.nonCalculateStatValue +
					             data2.statValue1;
					return string.Format("{0}%", num2);
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Time";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<AxeSkillActiveData>.inst.Duration[skillData.level].ToString();
			}

			return "";
		}
	}
}