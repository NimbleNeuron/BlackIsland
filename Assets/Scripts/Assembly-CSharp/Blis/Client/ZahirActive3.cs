using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ZahirActive3)]
	public class ZahirActive3 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill03);
			SetAnimation(Self, BooleanSkill03, true);
			PlayEffectChildManual(Self, "FX_BI_Zahir_Skill03", "FX_BI_Zahir_Skill03_Start");
			StartCoroutine(CoroutineUtil.DelayedAction(1f, delegate
			{
				SetAnimation(Self, BooleanSkill03, false);
				StopEffectChildManual(Self, "FX_BI_Zahir_Skill03", false);
			}));
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ZahirSkillActive3Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<ZahirSkillActive3Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return Singleton<ZahirSkillActive3Data>.inst.AirborneDuration.ToString();
				case 3:
				{
					float num = Math.Abs(GameDB.characterState
						.GetData(Singleton<ZahirSkillActive3Data>.inst.DebuffState).statValue1);
					return string.Format("{0}%", num);
				}
				case 4:
					return Singleton<ZahirSkillActive3Data>.inst.BigAirborneDuration.ToString();
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
					return Singleton<ZahirSkillActive3Data>.inst.DamageByLevel[level].ToString();
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