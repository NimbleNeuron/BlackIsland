using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.YukiActive2)]
	public class YukiActive2 : LocalSkillScript
	{
		private const string FX_BI_Yuki_Skill02_Buff = "FX_BI_Yuki_Skill02_Buff";


		private const string FX_BI_Yuki_Skill02 = "FX_BI_Yuki_Skill02";


		private const string FX_BI_Yuki_Passive = "FX_BI_Yuki_Passive";


		private const string Yuki_Passive = "Yuki_Passive";


		private const string Fx_Bottom = "Fx_Bottom";


		private const string Yuki_Skill02_Active = "Yuki_Skill02_Active";

		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
			SetAnimation(Self, BooleanSkill02, true);
			PlayEffectChild(Self, "FX_BI_Yuki_Skill02_Buff", "Fx_Bottom");
			PlaySoundPoint(Self, "Yuki_Skill02_Active");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill02, false);
			if (!cancel)
			{
				PlayEffectChildManual(Self, "Yuki_Passive", "FX_BI_Yuki_Passive");
				PlayEffectChild(Self, "FX_BI_Yuki_Skill02");
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Mathf.Abs(Singleton<YukiSkillActive2Data>.inst.CooldownReduceActive2).ToString();
				case 1:
					return GameDB.characterState.GetData(Singleton<YukiSkillActive2Data>.inst.DefenceBuffState).duration
						.ToString();
				case 2:
				{
					float statValue = GameDB.characterState
						.GetData(Singleton<YukiSkillActive2Data>.inst.DefenceBuffState).statValue1;
					return string.Format("{0}%", statValue);
				}
				case 3:
					return Math.Abs(Singleton<YukiSkillActive2Data>.inst.CooldownReduceActive3[skillData.level])
						.ToString();
				case 4:
					return SelfStat.MaxExtraPoint.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/DecreaseCoolTime";
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
				return Math.Abs(Singleton<YukiSkillActive2Data>.inst.CooldownReduceActive3[level]).ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cooldown.ToString();
		}
	}
}