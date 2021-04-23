using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LenoxActive2)]
	public class LenoxActive2 : LocalSkillScript
	{
		private const string Lenox_Skill02_01_sfx = "Lenox_Skill02_01";


		private const string Lenox_Skill02_02_sfx = "Lenox_Skill02_02";

		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
			PlaySoundPoint(Self, "Lenox_Skill02_01", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 3)
			{
				PlayEffectChildManual(Self, "FX_BI_Lenox_Skill02_Attack_02_Range_key",
					"FX_BI_Lenox_Skill02_Attack_02_Range");
			}

			if (action != 1)
			{
				if (action == 2)
				{
					StopEffectChildManual(Self, "FX_BI_Lenox_Skill02_Attack_02_Range_key", true);
					return;
				}

				if (action == 3)
				{
					bool flag = targetPosition != null;
				}
			}
		}


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<LenoxSkillActive2Data>.inst.FirstDamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<LenoxSkillActive2Data>.inst.FirstSkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.characterState.GetData(Singleton<LenoxSkillActive2Data>.inst.Active2DeBuffCodeFirst)
						.duration.ToString();
				case 3:
					return Singleton<LenoxSkillActive2Data>.inst.SecondDamageByLevel[skillData.level].ToString();
				case 4:
					return ((int) (Singleton<LenoxSkillActive2Data>.inst.SecondSkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 5:
					return GameDB.characterState.GetData(Singleton<LenoxSkillActive2Data>.inst.Active2DeBuffCodeSecond)
						.duration.ToString();
				case 6:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<LenoxSkillActive2Data>.inst.Active2DeBuffCodeSecond);
					return string.Format("{0}%", Math.Abs((int) data.statValue1));
				}
				case 7:
				{
					CharacterStateData data2 =
						GameDB.characterState.GetData(Singleton<LenoxSkillActive2Data>.inst.Active2BuffCode);
					return string.Format("{0}%", Math.Abs((int) data2.statValue1));
				}
				case 8:
					return GameDB.characterState.GetData(Singleton<LenoxSkillActive2Data>.inst.Active2BuffCode).duration
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
					return "ToolTipType/FirstDamage";
				case 1:
					return "ToolTipType/SecondDamage";
				case 2:
					return "ToolTipType/CoolTime";
				case 3:
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
					return Singleton<LenoxSkillActive2Data>.inst.FirstDamageByLevel[level].ToString();
				case 1:
					return Singleton<LenoxSkillActive2Data>.inst.SecondDamageByLevel[level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				case 3:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}