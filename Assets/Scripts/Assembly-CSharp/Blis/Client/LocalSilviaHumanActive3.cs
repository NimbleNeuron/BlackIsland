using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaHumanActive3)]
	public class LocalSilviaHumanActive3 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill03);
			PlayEffectPoint(Self, "FX_BI_Silvia_Skill03_Fire");
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				PlayEffectPoint(Self, "FX_BI_Silvia_Skill03_Indicator");
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<SilviaSkillHumanData>.inst.A3BaseDamage[skillData.level].ToString();
				case 1:
					return Mathf.RoundToInt(Singleton<SilviaSkillHumanData>.inst.A3ApDamage * SelfStat.AttackPower)
						.ToString();
				case 2:
					return Mathf.RoundToInt(Singleton<SilviaSkillHumanData>.inst.A3BaseDamage[skillData.level] *
					                        (1f + (skillData.range -
					                               Singleton<SilviaSkillHumanData>.inst.A3KnockbackDistance) *
						                        Singleton<SilviaSkillHumanData>.inst.A3MeterPerDamageRate)).ToString();
				case 3:
					return Mathf.RoundToInt(Singleton<SilviaSkillHumanData>.inst.A3ApDamage * SelfStat.AttackPower *
					                        (1f + (skillData.range -
					                               Singleton<SilviaSkillHumanData>.inst.A3KnockbackDistance) *
						                        Singleton<SilviaSkillHumanData>.inst.A3MeterPerDamageRate)).ToString();
				case 4:
					return Singleton<SilviaSkillHumanData>.inst.A3KnockbackDistance.ToString();
				case 5:
					return Singleton<SilviaSkillHumanData>.inst.A3EpGainPerHit.ToString();
				case 6:
					return Mathf.RoundToInt(Singleton<SilviaSkillHumanData>.inst.A3EpGainPerHit *
					                        (skillData.range -
					                         Singleton<SilviaSkillHumanData>.inst.A3KnockbackDistance)).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/MinDamage";
				case 1:
					return "ToolTipType/MaxDamage";
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
					return Singleton<SilviaSkillHumanData>.inst.A3BaseDamage[level].ToString();
				case 1:
					return Mathf.RoundToInt(Singleton<SilviaSkillHumanData>.inst.A3BaseDamage[level] *
					                        (1f + (skillData.range -
					                               Singleton<SilviaSkillHumanData>.inst.A3KnockbackDistance) *
						                        Singleton<SilviaSkillHumanData>.inst.A3MeterPerDamageRate)).ToString();
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