using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LukeActive3)]
	public class LukeActive3 : LocalSkillScript
	{
		private readonly Dictionary<int, string> Active3_Sfx = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill3 = new Dictionary<int, string>();


		public LukeActive3()
		{
			Effect_Skill3.Add(0, "FX_BI_Luke_Skill03_Attack_Fire");
			Effect_Skill3.Add(1, "FX_BI_Luke_Skill03_Attack_Fire_Evo");
		}


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill03, true);
			PlayAnimation(Self, TriggerSkill03);
			PlaySoundPoint(Self, "Luke_Skill03_Move_01");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1 && targetPosition != null)
			{
				Self.PlayLocalEffectWorldPoint(1022303, targetPosition.Value);
			}

			if (actionNo == 2)
			{
				PlayEffectPoint(Self, Effect_Skill3[evolutionLevel], "Skill03_Shot");
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill03, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<LukeSkillActive3Data>.inst.DamageBySkillLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<LukeSkillActive3Data>.inst.DamageApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.characterState
						.GetData(Singleton<LukeSkillActive3Data>.inst.SilentVacuumCleanerMoveSpeedDownStateCode)
						.duration.ToString();
				case 3:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<LukeSkillActive3Data>.inst.SilentVacuumCleanerMoveSpeedDownStateCode)
						.statValue1);
					return string.Format("{0}%", num);
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
					return Singleton<LukeSkillActive3Data>.inst.DamageBySkillLevel[skillData.level].ToString();
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