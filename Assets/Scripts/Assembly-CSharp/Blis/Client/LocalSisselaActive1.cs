using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SisselaActive1)]
	public class LocalSisselaActive1 : LocalSisselaSkill
	{
		public override void Start()
		{
			SetAnimation(Self, BooleanMotionWait, true);
			PlayAnimation(Self, TriggerSkill01);
			SetAnimation(Self, BooleanSkill01, true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			base.Play(action, target, targetPosition);
			if (action != 1)
			{
				if (action == 2)
				{
					if (wilson == null)
					{
						return;
					}

					if (!wilson.MoveAgent.IsLockRotation())
					{
						wilson.LockRotation(true, wilson.transform.rotation);
						StopEffectChildManual(wilson, "trail", true);
						StopEffectChildManual(wilson, "move", true);
						wilson.SetCharacterAnimatorTrigger("tSkill01_2_Wilson");
						PlayEffectPoint(wilson, "FX_BI_Sissela_Skill01_Attack");
						PlayEffectPoint(wilson, "FX_BI_Sissela_Skill01_Explosion");
						PlaySoundPoint(wilson, "Sissela_Skill01_Fire", 15);
					}
				}

				return;
			}

			if (localWilsonData == null)
			{
				return;
			}

			if (!localWilsonData.IsDoingGrab)
			{
				wilson.LockRotation(false, wilson.transform.rotation);
			}

			wilson.SetCharacterAnimatorTrigger("tSkill01_Wilson");
			wilson.SetCharacterAnimatorBool("bSkill01_Wilson", true);
			PlayEffectChildManual(wilson, "trail", "FX_BI_Sissela_Trail_01", "Trail");
			PlaySoundChildManual(wilson, "Sissela_Skill01_Move", 15);
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill01, false);
			if (wilson != null)
			{
				wilson.SetCharacterAnimatorBool("bSeperate", true);
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 1:
					return Singleton<SisselaSkillData>.inst.A1PassBaseDamage[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<SisselaSkillData>.inst.A1PassApDamage * SelfStat.AttackPower)).ToString();
				case 3:
					return Singleton<SisselaSkillData>.inst.A1StopBaseDamage[skillData.level].ToString();
				case 4:
					return ((int) (Singleton<SisselaSkillData>.inst.A1StopApDamage * SelfStat.AttackPower)).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/MoveDamage2";
				case 1:
					return "ToolTipType/ArriveDamage";
				case 2:
					return "ToolTipType/CoolTime";
				case 3:
					return "SkillCostType/HpCost";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<SisselaSkillData>.inst.A1PassBaseDamage[skillData.level].ToString();
				case 1:
					return Singleton<SisselaSkillData>.inst.A1StopBaseDamage[skillData.level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				case 3:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}


		public override void OnDisplaySkillIndicator(Splat indicator)
		{
			base.OnDisplaySkillIndicator(indicator);
			LineIndicatorCustomTarget lineIndicatorCustomTarget = indicator as LineIndicatorCustomTarget;
			lineIndicatorCustomTarget.SetLineRangeOnMouse(true);
			lineIndicatorCustomTarget.Length = Singleton<SisselaSkillData>.inst.WilsonMaxDistance + SkillRange;
			lineIndicatorCustomTarget.SetCustomLineTarget(GetWilson(LocalPlayerCharacter).transform);
		}
	}
}