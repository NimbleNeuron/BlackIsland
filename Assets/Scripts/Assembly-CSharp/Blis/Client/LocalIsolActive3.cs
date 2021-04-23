using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.IsolActive3)]
	public class LocalIsolActive3 : LocalSkillScript
	{
		private const string Skill03 = "FX_BI_Isol_Skill03_Dash";


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill03);
			SetAnimation(Self, BooleanSkill03, true);
			PlayEffectChild(Self, "FX_BI_Isol_Skill03_Dash");
			PlaySoundPoint(Self, "Isol_Skill03_Active", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill03, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return ((int) GameDB.characterState
					.GetData(Singleton<IsolSkillActive3Data>.inst.BuffState[skillData.level]).duration).ToString();
			}

			if (index != 1)
			{
				return "";
			}

			int num = (int) Math.Abs(GameDB.characterState
				.GetData(Singleton<IsolSkillActive3Data>.inst.BuffState[skillData.level]).statValue1);
			return string.Format("{0}%", num);
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/StealthTime";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/CoolTime";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return ((int) GameDB.characterState.GetData(Singleton<IsolSkillActive3Data>.inst.BuffState[level])
					.duration).ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cooldown.ToString();
		}
	}
}