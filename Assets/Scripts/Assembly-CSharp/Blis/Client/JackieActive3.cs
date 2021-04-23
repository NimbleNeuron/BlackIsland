using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.JackieActive3)]
	public class JackieActive3 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill03);
			PlayEffectChildManual(Self, "JackieActive3", "FX_BI_Jackie_Skill03", "Fx_Center");
			SetAnimation(Self, BooleanSkill03, true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill03, false);
			StopEffectChildManual(Self, "JackieActive3", true);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<JackieSkillActive3Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<JackieSkillActive3Data>.inst.SkillApCoefByLevel[skillData.level] *
					               SelfStat.AttackPower)).ToString();
				case 2:
					return GameDB.characterState
						.GetData(Singleton<JackieSkillActive3Data>.inst.ReinforcedDebuffState[skillData.level]).duration
						.ToString();
				case 3:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<JackieSkillActive3Data>.inst.ReinforcedDebuffState[skillData.level])
						.statValue1);
					return string.Format("{0}%", num);
				}
				case 4:
					return GameDB.characterState.GetData(Singleton<JackieSkillActive3Data>.inst.DebuffState).duration
						.ToString();
				case 5:
				{
					int num2 = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<JackieSkillActive3Data>.inst.DebuffState).statValue1);
					return string.Format("{0}%", num2);
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
					return "ToolTipType/SkillApCoef";
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
					return Singleton<JackieSkillActive3Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return Singleton<JackieSkillActive3Data>.inst.SkillApCoefByLevel[skillData.level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				case 3:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}


		[SkillScript(SkillId.JackieActive3Debuff)]
		public class JackieActive3Debuff : LocalSkillScript
		{
			public override void Start()
			{
				PlayEffectChildManual(Self, "Jackie_Skill03_Debuff", "FX_BI_Jackie_Skill03_Debuff");
			}


			public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


			public override void Finish(bool cancel)
			{
				StopEffectChildManual(Self, "Jackie_Skill03_Debuff", true);
			}
		}
	}
}