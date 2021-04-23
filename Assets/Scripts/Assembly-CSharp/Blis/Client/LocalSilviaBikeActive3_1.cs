using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaBikeActive3_1)]
	public class LocalSilviaBikeActive3_1 : LocalSkillScript
	{
		public override void Start()
		{
			LockSkillSlot(SkillSlotSet.Active4_2);
			PlayAnimation(Self, TriggerSkill03_2);
			SetAnimation(Self, BooleanSkill03, true);
			PlayEffectChildManual(Self, "Silvia_Skill03_Dash", "FX_BI_Silvia_Skill03_Dash");
			PlaySoundPoint(Self, "Silvia_Skill07_Jump", 15);
			StopEffectChildManual(Self, "Silvia_Skill04_Spin", true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			UnlockSkillSlot(SkillSlotSet.Active4_2);
			SetAnimation(Self, BooleanSkill03, false);
			StopEffectChildManual(Self, "Silvia_Skill03_Dash", false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<SilviaSkillBikeData>.inst.A3BaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<SilviaSkillBikeData>.inst.A3ApDamage * SelfStat.AttackPower)).ToString();
				case 3:
				{
					int num = Singleton<SilviaSkillBikeData>.inst.A3SpeedCoefDamage[skillData.level];
					return 0.ToString();
				}
				case 4:
					return (Singleton<SilviaSkillBikeData>.inst.A3SpeedCoefDamage[skillData.level] * 7).ToString();
			}

			return "";
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
					return "ToolTipType/DefaultMoveDamageRatio";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<SilviaSkillBikeData>.inst.A3BaseDamage[level].ToString();
				case 1:
					return skillData.cooldown.ToString();
				case 2:
					return (Singleton<SilviaSkillBikeData>.inst.A3SpeedCoefDamage[level] * 7).ToString();
				default:
					return "";
			}
		}
	}
}