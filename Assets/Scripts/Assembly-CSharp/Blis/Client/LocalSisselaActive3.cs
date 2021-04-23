using System;
using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SisselaActive3)]
	public class LocalSisselaActive3 : LocalSisselaSkill
	{
		private const string ServantTrigerSkill03_T = "tSkill03_T_Wilson";


		private const string ServantTrigerSkill03_S = "tSkill03_S_Wilson";


		private const string Skill03_Start = "FX_BI_Sissela_Skill03_Fire";


		private const float wilsonTongueSizeRate = 1.1f;


		public override void Start()
		{
			SetAnimation(Self, BooleanMotionWait, true);
			PlayAnimation(Self, TriggerSkill03);
			SetAnimation(Self, BooleanSkill03, true);
			if (wilson != null)
			{
				PlayEffectChild(wilson, "FX_BI_Sissela_Skill03_Fire", "Weapon_Special_Sissela_01");
			}
		}


		private IEnumerator WilsonTongueShot(LocalProjectile projectile)
		{
			while (wilson == null)
			{
				yield return null;
			}

			if (projectile == null)
			{
				Log.E("WilsonTongueShot projectile is null");
				yield break;
			}

			wilson.transform.LookAt(projectile.GetPosition());
			for (;;)
			{
				float num = GameUtil.Distance(wilson.GetPosition(), projectile.GetPosition()) * 1.1f;
				SetWilsonTongueLength(num / Singleton<SisselaSkillData>.inst.WilsonTongueBaseLength);
				yield return null;
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			base.Play(action, target, targetPosition);
			StopCoroutines();
			switch (action)
			{
				case 1:
					SetWilsonTongueLength(0f);
					return;
				case 2:
					if (wilson != null)
					{
						wilson.SetCharacterAnimatorTrigger("tSkill03_T_Wilson");
					}

					PlaySoundPoint(Self, "Sissela_Skill03_Take", 15);
					return;
				case 3:
					StartCoroutine(WilsonTongueShot(target as LocalProjectile));
					if (wilson != null)
					{
						wilson.SetCharacterAnimatorTrigger("tSkill03_S_Wilson");
					}

					return;
				default:
					return;
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill03, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<SisselaSkillData>.inst.A3BaseShield[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<SisselaSkillData>.inst.A3ApShield * SelfStat.AttackPower)).ToString();
				case 2:
					return GameDB.characterState.GetData(Singleton<SisselaSkillData>.inst.A3ShieldState).duration
						.ToString();
				case 3:
					return Singleton<SisselaSkillData>.inst.A3BaseDamage[skillData.level].ToString();
				case 4:
					return ((int) (Singleton<SisselaSkillData>.inst.A3ApDamage * SelfStat.AttackPower)).ToString();
				case 5:
					return GameDB.characterState.GetData(Singleton<SisselaSkillData>.inst.A3StunState).duration
						.ToString();
				case 6:
					return Math.Abs(Singleton<SisselaSkillData>.inst.A3ShieldCooldownReduce).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/Shield";
				case 1:
					return "ToolTipType/Damage";
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
					return Singleton<SisselaSkillData>.inst.A3BaseShield[skillData.level].ToString();
				case 1:
					return Singleton<SisselaSkillData>.inst.A3BaseDamage[skillData.level].ToString();
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
			if (lineIndicatorCustomTarget != null)
			{
				if (LocalPlayerCharacter.GetCharacterStateValueByGroup(
					Singleton<SisselaSkillData>.inst.WilsonUnionStateGroup, Self.ObjectId) != null)
				{
					lineIndicatorCustomTarget.Length =
						SkillRange + Singleton<SisselaSkillData>.inst.A3SeparateStartDistance;
				}

				LocalSummonServant wilson = GetWilson(LocalPlayerCharacter);
				if (wilson != null)
				{
					lineIndicatorCustomTarget.SetCustomLineTarget(wilson.transform);
				}
			}
		}


		public override Vector3 GetSkillMyCharacterPos()
		{
			return GetWilson(LocalPlayerCharacter).GetPosition();
		}
	}
}