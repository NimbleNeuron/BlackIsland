using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LukeActive2)]
	public class LukeActive2 : LocalSkillScript
	{
		private const string Skill02_Mob = "Skill02_Mob";


		private const string Effect_SKill2_Loop_key = "Effect_SKill2_Loop_key";


		private readonly Dictionary<int, string> Active2_Sfx = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill2_Loop = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill2_Start = new Dictionary<int, string>();


		private readonly Dictionary<int, int> tSkill02_Evo = new Dictionary<int, int>();


		public LukeActive2()
		{
			Effect_Skill2_Start.Add(0, "FX_BI_Luke_Skill02_Mop_BuffStart");
			Effect_Skill2_Start.Add(1, "FX_BI_Luke_Skill02_Mop_BuffStart_Evo");
			Effect_Skill2_Loop.Add(0, "FX_BI_Luke_Skill02_Mop_water");
			Effect_Skill2_Loop.Add(1, "FX_BI_Luke_Skill02_Mop_water_Evo");
			Active2_Sfx.Add(0, "Luke_Skill02_Buff");
			Active2_Sfx.Add(1, "Luke_Skill02_Buff");
			tSkill02_Evo.Add(0, BooleanSkill02);
			tSkill02_Evo.Add(1, BooleanSkill04_02);
		}


		public override void Start()
		{
			SetAnimation(Self, tSkill02_Evo[evolutionLevel], true);
			PlayEffectChild(Self, Effect_Skill2_Start[evolutionLevel], "Skill02_Mob");
			PlayEffectChildManual(Self, "Effect_SKill2_Loop_key", Effect_Skill2_Loop[evolutionLevel], "Skill02_Mob");
			PlaySoundPoint(Self, Active2_Sfx[evolutionLevel]);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
				{
					float num = Singleton<LukeSkillActive2Data>.inst.ReductionCCBuffCodeByLevel[SkillData.level];
					return string.Format("{0}%", num * 100f);
				}
				case 1:
					return GameDB.characterState
						.GetData(Singleton<LukeSkillActive2Data>.inst.AddAttackBuffStateCodeByLevel[skillData.level])
						.duration.ToString();
				case 2:
					return Singleton<LukeSkillActive2Data>.inst.BaseDamage[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<LukeSkillActive2Data>.inst.DamageApCoef * SelfStat.AttackPower))
						.ToString();
				case 4:
					return GameDB.characterState.GetData(Singleton<LukeSkillActive2Data>.inst.AddAttackBuffStackCode)
						.duration.ToString();
				case 5:
				{
					float statValue = GameDB.characterState
						.GetData(Singleton<LukeSkillActive2Data>.inst.AddAttackBuffStackCode).statValue1;
					return string.Format("{0}%", statValue);
				}
				case 6:
					return ((float) GameDB.characterState
						.GetData(Singleton<LukeSkillActive2Data>.inst.AddAttackBuffStackCode).maxStack).ToString();
				case 7:
					return Singleton<LukeSkillActive2Data>.inst.EvolutionSkillActive1CoolDown.ToString();
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
					return Singleton<LukeSkillActive2Data>.inst.BaseDamage[skillData.level].ToString();
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