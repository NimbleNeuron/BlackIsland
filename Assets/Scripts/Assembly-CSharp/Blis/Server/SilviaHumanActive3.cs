using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaHumanActive3)]
	public class SilviaHumanActive3 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damage = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			SilviaHumanActive3 silviaHumanActive3 = this;
			silviaHumanActive3.LookAtPosition(silviaHumanActive3.Caster, silviaHumanActive3.info.cursorPosition);
			silviaHumanActive3.Start();
			if (silviaHumanActive3.SkillCastingTime1 > 0.0)
			{
				yield return silviaHumanActive3.FirstCastingTime();
				silviaHumanActive3.LookAtPosition(silviaHumanActive3.Caster, silviaHumanActive3.info.cursorPosition);
			}

			silviaHumanActive3.CasterLockRotation(true);
			Vector3 direction =
				GameUtil.Direction(silviaHumanActive3.Caster.Position, silviaHumanActive3.GetSkillPoint());
			if (direction == Vector3.zero)
			{
				direction = silviaHumanActive3.Caster.Forward;
			}

			Vector3 direcionOnPlane = direction;
			direcionOnPlane.y = 0.0f;
			direcionOnPlane = direcionOnPlane.normalized;
			Vector3 startPosition = Vector3.zero;
			ProjectileProperty projectileProperty = silviaHumanActive3.PopProjectileProperty(silviaHumanActive3.Caster,
				Singleton<SilviaSkillHumanData>.inst.A3FProjectileCode);
			projectileProperty.SetActionOnCollisionCharacter(
				SetActionOnCollisionCharacter);
			projectileProperty.SetTargetDirection(direction);
			startPosition = silviaHumanActive3.LaunchProjectile(projectileProperty).GetPosition();
			if (silviaHumanActive3.SkillFinishDelayTime > 0.0)
			{
				yield return silviaHumanActive3.FinishDelayTime();
			}

			silviaHumanActive3.Finish();

			void SetActionOnCollisionCharacter(
				SkillAgent targetAgent,
				AttackerInfo attackerInfo,
				Vector3 damagePoint,
				Vector3 damageDirection)
			{
				float num1 = Vector3.Distance(damagePoint, startPosition);
				damage.Clear();
				damage.Add(SkillScriptParameterType.Damage,
					Singleton<SilviaSkillHumanData>.inst.A3BaseDamage[SkillLevel]);
				damage.Add(SkillScriptParameterType.DamageApCoef, Singleton<SilviaSkillHumanData>.inst.A3ApDamage);
				if (num1 <= (double) Singleton<SilviaSkillHumanData>.inst.A3ProjectileIntersection)
				{
					KnockbackState state = CreateState<KnockbackState>(targetAgent, 2000010);
					state.Init(direcionOnPlane, Singleton<SilviaSkillHumanData>.inst.A3KnockbackDistance,
						Singleton<SilviaSkillHumanData>.inst.A3KnockbackDuration, EasingFunction.Ease.EaseOutQuad,
						false);
					targetAgent.AddState(state, Caster.ObjectId);
				}
				else
				{
					int num2;
					damage.Add(SkillScriptParameterType.FinalMoreDamage,
						(num2 = (int) (num1 -
						               (double) Singleton<SilviaSkillHumanData>.inst.A3ProjectileIntersection) +
						        1) * Singleton<SilviaSkillHumanData>.inst.A3MeterPerDamageRate);
					int a3EpGainPerHit = Singleton<SilviaSkillHumanData>.inst.A3EpGainPerHit;
					int deltaAmount = num2 * a3EpGainPerHit;
					if (deltaAmount > 0)
					{
						Caster.ExtraPointModifyTo(Caster, deltaAmount);
					}
				}

				DamageTo(targetAgent, attackerInfo, DamageType.Skill, DamageSubType.Normal, 0, damage,
					SkillSlotSet.Active3_1, damagePoint, damageDirection, 0);
			}

			// co: dotPeek
			// SilviaHumanActive3.<>c__DisplayClass3_0 CS$<>8__locals1 = new SilviaHumanActive3.<>c__DisplayClass3_0();
			// CS$<>8__locals1.<>4__this = this;
			// base.LookAtPosition(base.Caster, this.info.cursorPosition, 0f, false);
			// this.Start();
			// if (base.SkillCastingTime1 > 0f)
			// {
			// 	yield return base.FirstCastingTime();
			// 	base.LookAtPosition(base.Caster, this.info.cursorPosition, 0f, false);
			// }
			// base.CasterLockRotation(true);
			// Vector3 vector = GameUtil.Direction(base.Caster.Position, base.GetSkillPoint());
			// if (vector == Vector3.zero)
			// {
			// 	vector = base.Caster.Forward;
			// }
			// CS$<>8__locals1.direcionOnPlane = vector;
			// CS$<>8__locals1.direcionOnPlane.y = 0f;
			// CS$<>8__locals1.direcionOnPlane = CS$<>8__locals1.direcionOnPlane.normalized;
			// CS$<>8__locals1.startPosition = Vector3.zero;
			// ProjectileProperty projectileProperty = base.PopProjectileProperty(base.Caster, Singleton<SilviaSkillHumanData>.inst.A3FProjectileCode);
			// projectileProperty.SetActionOnCollisionCharacter(new Action<SkillAgent, AttackerInfo, Vector3, Vector3>(CS$<>8__locals1.<Play>g__SetActionOnCollisionCharacter|0));
			// projectileProperty.SetTargetDirection(vector);
			// WorldProjectile worldProjectile = base.LaunchProjectile(projectileProperty);
			// CS$<>8__locals1.startPosition = worldProjectile.GetPosition();
			// if (base.SkillFinishDelayTime > 0f)
			// {
			// 	yield return base.FinishDelayTime();
			// }
			// this.Finish(false);
			// yield break;
		}
	}
}