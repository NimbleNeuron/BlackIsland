using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.JackieActive2)]
	public class JackieActive2 : LocalSkillScript
	{
		public override void Start()
		{
			Vector3 offset = new Vector3(0f, 1f, 0f);
			PlayEffectPoint(Self, "FX_BI_Character_Skill", offset);
			PlaySoundPoint(Self, "jackie_Skill02_Activation");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<JackieSkillActive2Data>.inst.BuffState[skillData.level]).duration.ToString();
				case 1:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(
							Singleton<JackieSkillActive2Data>.inst.BuffState[skillData.level]);
					return string.Format("{0}%", data.statValue1);
				}
				case 2:
					return ((int) (Singleton<JackieSkillActive2Data>.inst.DamageApCoef[skillData.level] *
					               SelfStat.AttackPower)).ToString();
				case 3:
					return Singleton<JackieSkillActive2Data>.inst.HealByLevel[skillData.level].ToString();
				case 4:
					return ((int) (Singleton<JackieSkillActive2Data>.inst.HealApCoef * SelfStat.AttackPower))
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
					return "StatType/MoveSpeedRatio";
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
				{
					CharacterStateData data =
						GameDB.characterState.GetData(
							Singleton<JackieSkillActive2Data>.inst.BuffState[skillData.level]);
					return string.Format("{0}%", data.statValue1);
				}
				case 1:
					return Singleton<JackieSkillActive2Data>.inst.HealByLevel[skillData.level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				case 3:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}


		[SkillScript(SkillId.JackieActive2Buff)]
		public class JackieActive2Buff : LocalSkillScript
		{
			public override void Start()
			{
				SetAnimation(Self, BooleanSkill02, true);
				PlayEffectChildManual(Self, "JackieActive2Buff", "FX_BI_Jackie_Skill02_Buff", "Fx_Center");
				PlayEffectChildManual(Self, "JackieActive2Buff2", "FX_BI_Jackie_Skill02_Buff2");
			}


			public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


			public override void Finish(bool cancel)
			{
				SetAnimation(Self, BooleanSkill02, false);
				StopEffectChildManual(Self, "JackieActive2Buff", true);
				StopEffectChildManual(Self, "JackieActive2Buff2", true);
			}
		}
	}
}