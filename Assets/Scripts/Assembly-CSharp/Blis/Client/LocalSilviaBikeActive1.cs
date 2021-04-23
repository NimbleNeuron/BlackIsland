using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaBikeActive1)]
	public class LocalSilviaBikeActive1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
			SetAnimation(Self, BooleanSkill01, true);
			PlaySoundPoint(Self, "Silvia_Skill05_Fire", 15);
			PlayEffectPoint(Self, "FX_BI_Silvia_Skill05_Dust", "Root");
			StopEffectChildManual(Self, "Silvia_Skill04_Spin", true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill01, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return Singleton<SilviaSkillBikeData>.inst.A1BaseDamage[skillData.level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return ((int) (Singleton<SilviaSkillBikeData>.inst.A1ApDamage * SelfStat.AttackPower)).ToString();
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Damage";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<SilviaSkillBikeData>.inst.A1BaseDamage[level].ToString();
			}

			return "";
		}
	}
}