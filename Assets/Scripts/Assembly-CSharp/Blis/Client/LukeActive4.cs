using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LukeActive4)]
	public class LukeActive4 : LocalSkillScript
	{
		private readonly Dictionary<int, int> tSkill04_Evo = new Dictionary<int, int>();

		public LukeActive4()
		{
			tSkill04_Evo.Add(0, TriggerSkill04);
			tSkill04_Evo.Add(1, TriggerSkill04_2);
		}


		public override void Start()
		{
			PlayAnimation(Self, tSkill04_Evo[evolutionLevel]);
			SetAnimation(Self, BooleanSkill04, true);
			SwitchMaterialChildManualFromDefault(Self, "Wp_Special_Luke_01_LOD1", 0, "Luke_01_LOD1_Skill04");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill04, false);
			SwitchMaterialChildManualFromDefault(Self, "Wp_Special_Luke_01_LOD1", 0, "Luke_01_LOD1");
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState.GetData(Singleton<LukeSkillActive4Data>.inst.AfterServiceStateCode)
						.duration.ToString();
				case 1:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<LukeSkillActive4Data>.inst.AfterServiceStateCode).statValue1);
					return string.Format("{0}%", num);
				}
				case 2:
					return Singleton<LukeSkillActive4Data>.inst.DamageBySkillLevel[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<LukeSkillActive4Data>.inst.DamageApCoef * SelfStat.AttackPower))
						.ToString();
				case 4:
					return (2 * Singleton<LukeSkillActive4Data>.inst.DamageBySkillLevel[skillData.level]).ToString();
				case 5:
					return ((int) (2f * Singleton<LukeSkillActive4Data>.inst.DamageApCoef * SelfStat.AttackPower))
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
					return "ToolTipType/Damage";
				case 1:
					return "ToolTipType/CoolTime";
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
					return Singleton<LukeSkillActive4Data>.inst.DamageBySkillLevel[skillData.level].ToString();
				case 1:
					return skillData.cooldown.ToString();
				case 2:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}