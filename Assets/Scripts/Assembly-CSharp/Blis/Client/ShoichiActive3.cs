using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ShoichiActive3)]
	public class ShoichiActive3 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill03);
			SetAnimation(Self, BooleanSkill03, true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill03, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ShoichiSkillActive3Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<ShoichiSkillActive3Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.characterState
						.GetData(Singleton<ShoichiSkillActive3Data>.inst.DaggerDebuffStateCodes[skillData.level])
						.duration.ToString();
				case 3:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<ShoichiSkillActive3Data>.inst.DaggerDebuffStateCodes[skillData.level])
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
					return "ToolTipType/DecreaseMoveRatio";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ShoichiSkillActive3Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return skillData.cooldown.ToString();
				case 2:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<ShoichiSkillActive3Data>.inst.DaggerDebuffStateCodes[skillData.level])
						.statValue1);
					return string.Format("{0}%", num);
				}
				default:
					return "";
			}
		}
	}
}