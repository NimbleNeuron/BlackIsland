using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HartActive4)]
	public class HartActive4 : LocalSkillScript
	{
		private const string Hart_Skill04 = "Hart_Skill04";


		private const string FX_BI_Hart_Special_01 = "FX_BI_Hart_Special_01";


		private const string FX_BI_Hart_Special_02 = "FX_BI_Hart_Special_02";


		private const string Hart_Skill04_Effect_1 = "Hart_Skill04_Effect_1";


		private const string Hart_Skill04_Effect_2 = "Hart_Skill04_Effect_2";


		private readonly Dictionary<int, string> Effect_Skill = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Skill04Active_SFX = new Dictionary<int, string>();


		public HartActive4()
		{
			Effect_Skill.Add(1, "FX_BI_Hart_Skill04_Range");
			Effect_Skill.Add(2, "FX_BI_Hart_Skill04_Range_2");
			Effect_Skill.Add(3, "FX_BI_Hart_Skill04_Range_3");
			Skill04Active_SFX.Add(0, "hart_Skill04_Active");
			Skill04Active_SFX.Add(1, "hart_Skill04_Evo_Active");
			Skill04Active_SFX.Add(2, "hart_Skill04_Evo_Active");
		}


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill04);
			SetAnimation(Self, BooleanSkill04, true);
			PlayEffectChildManual(Self, "Hart_Skill04", Effect_Skill[data.level]);
			PlayEffectChildManual(Self, "Hart_Skill04_Effect_1", "FX_BI_Hart_Special_01");
			PlayEffectChildManual(Self, "Hart_Skill04_Effect_2", "FX_BI_Hart_Special_02");
			PlaySoundChildManual(Self, Skill04Active_SFX[evolutionLevel], 15);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill04, false);
			StopEffectChildManual(Self, "Hart_Skill04", true);
			StopEffectChildManual(Self, "Hart_Skill04_Effect_1", true);
			StopEffectChildManual(Self, "Hart_Skill04_Effect_2", true);
			StopSoundChildManual(Self, Skill04Active_SFX[evolutionLevel]);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return skillData.ConcentrationTime.ToString();
				case 1:
					return skillData.range.ToString();
				case 2:
					return Singleton<HartSkillActive4Data>.inst.HpAmount[skillData.level].ToString();
				case 3:
					return ((int) (SelfStat.MaxHp * Singleton<HartSkillActive4Data>.inst.HpCoef[skillData.level]))
						.ToString();
				case 4:
					return GameDB.characterState.GetData(Singleton<HartSkillActive4Data>.inst.BuffState).forcedMoveSpeed
						.ToString();
				case 5:
				{
					int num = evolutionLevel;
					if (num < 1)
					{
						num = 1;
					}

					return Singleton<HartSkillActive4Data>.inst.DecSpAmount[num].ToString();
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/SkillRange";
				case 1:
					return "ToolTipType/Heal";
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
					return skillData.range.ToString();
				case 1:
					return Singleton<HartSkillActive4Data>.inst.HpAmount[level].ToString();
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