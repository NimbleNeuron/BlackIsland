using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LukeActive1_1)]
	public class LukeActive1_1 : LocalSkillScript
	{
		private readonly Dictionary<int, string> Active1_Sfx = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill01 = new Dictionary<int, string>();


		private readonly Dictionary<int, int> Skill01_Trigger = new Dictionary<int, int>();


		public LukeActive1_1()
		{
			Skill01_Trigger.Add(0, TriggerSkill01);
			Skill01_Trigger.Add(1, TriggerSkill01_4);
		}


		public override void Start()
		{
			PlayAnimation(Self, Skill01_Trigger[evolutionLevel]);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<LukeSkillActive1_1Data>.inst.BaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<LukeSkillActive1_1Data>.inst.DamageApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.characterState.GetData(Singleton<LukeSkillActive1_1Data>.inst.BuffCode).duration
						.ToString();
				case 3:
					return Singleton<LukeSkillActive1_2Data>.inst.BaseDamage[skillData.level].ToString();
				case 4:
					return ((int) (Singleton<LukeSkillActive1_2Data>.inst.DamageApCoef * SelfStat.AttackPower))
						.ToString();
				case 5:
				{
					float evolutionBuffRatio = Singleton<LukeSkillActive1_2Data>.inst.EvolutionBuffRatio;
					return string.Format("{0}%", evolutionBuffRatio * 100f);
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
					return "ToolTipType/BulletDamage";
				case 1:
					return "ToolTipType/DashDamage";
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
					return Singleton<LukeSkillActive1_1Data>.inst.BaseDamage[skillData.level].ToString();
				case 1:
					return Singleton<LukeSkillActive1_2Data>.inst.BaseDamage[skillData.level].ToString();
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