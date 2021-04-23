using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaBikeActive2)]
	public class LocalSilviaBikeActive2 : LocalSkillScript
	{
		public override void Start()
		{
			LockSkillSlot(SkillSlotSet.Active4_2);
			PlayAnimation(Self, TriggerSkill02);
			SetAnimation(Self, BooleanSkill02, true);
			PlaySoundPoint(Self, "Silvia_Skill06_Start", 15);
			StopEffectChildManual(Self, "Silvia_Skill04_Spin", true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayEffectPoint(Self, "FX_BI_Silvia_Skill06_Ground",
					Self.transform.forward * Singleton<SilviaSkillBikeData>.inst.A2SkillBikeWheelDistance);
				PlaySoundPoint(Self, "Silvia_Skill06_End", 15);
			}
		}


		public override void Finish(bool cancel)
		{
			UnlockSkillSlot(SkillSlotSet.Active4_2);
			SetAnimation(Self, BooleanSkill02, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return Singleton<SilviaSkillBikeData>.inst.A2BaseDamage[skillData.level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return ((int) (Singleton<SilviaSkillBikeData>.inst.A2ApDamage * SelfStat.AttackPower)).ToString();
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Damage";
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
				return Singleton<SilviaSkillBikeData>.inst.A2BaseDamage[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cooldown.ToString();
		}


		public override void OnDisplaySkillIndicator(Splat indicator)
		{
			base.OnDisplaySkillIndicator(indicator);
			if (Singleton<SilviaSkillBikeData>.inst.A2SkillBikeWheelDistance != 0f)
			{
				indicator.SetLateUpdateAction(IndicatorLateUpdateAction);
			}
		}


		private void IndicatorLateUpdateAction(Splat indicator)
		{
			PointIndicator pointIndicator = indicator as PointIndicator;
			if (pointIndicator == null)
			{
				return;
			}

			if (pointIndicator.Inner == null)
			{
				return;
			}

			Vector3 position = pointIndicator.Inner.transform.position;
			Vector3 a = GameUtil.Direction(Self.GetPosition(), position);
			pointIndicator.PointCompensationValue = a * Singleton<SilviaSkillBikeData>.inst.A2SkillBikeWheelDistance;
		}
	}
}