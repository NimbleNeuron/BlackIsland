using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AssaultRifleOverHeatAttack)]
	public class AssaultRifleOverHeatAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollection_3 = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		private ProjectileProperty CreateProjectile_1_2()
		{
			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<AssaultRifleSkillActiveData>.inst.ProjectileCode);
			projectile.SetTargetObject(Target.ObjectId);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAttackApCoef_1_2);
					DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
				});
			return projectile;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			AssaultRifleOverHeatAttack rifleOverHeatAttack1 = this;
			rifleOverHeatAttack1.Start();
			rifleOverHeatAttack1.LookAtTarget(rifleOverHeatAttack1.Caster, rifleOverHeatAttack1.Target);
			if (rifleOverHeatAttack1.IsEnoughBullet())
			{
				yield return rifleOverHeatAttack1.WaitForSecondsByAttackSpeed(rifleOverHeatAttack1.Caster,
					Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAttackDelay);
				rifleOverHeatAttack1.LaunchProjectile(rifleOverHeatAttack1.CreateProjectile_1_2());
				rifleOverHeatAttack1.FinishNormalAttack();
			}

			if (rifleOverHeatAttack1.IsEnoughBullet())
			{
				yield return rifleOverHeatAttack1.WaitForSecondsByAttackSpeed(rifleOverHeatAttack1.Caster,
					Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAttackDelay);
				rifleOverHeatAttack1.LaunchProjectile(rifleOverHeatAttack1.CreateProjectile_1_2());
			}

			if (rifleOverHeatAttack1.IsEnoughBullet())
			{
				AssaultRifleOverHeatAttack rifleOverHeatAttack = rifleOverHeatAttack1;
				yield return rifleOverHeatAttack1.WaitForSecondsByAttackSpeed(rifleOverHeatAttack1.Caster,
					Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAttackDelay);
				ProjectileProperty projectile_3 = rifleOverHeatAttack1.PopProjectileProperty(
					rifleOverHeatAttack1.Caster, Singleton<AssaultRifleSkillActiveData>.inst.ProjectileCode);
				projectile_3.SetTargetObject(rifleOverHeatAttack1.Target.ObjectId);
				projectile_3.SetActionOnCollisionCharacter(
					(targetAgent, attackerInfo, damagePoint,
						damageDirection) =>
					{
						rifleOverHeatAttack.parameterCollection_3.Clear();
						rifleOverHeatAttack.parameterCollection_3.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAttackApCoef_3);
						rifleOverHeatAttack.DamageTo(targetAgent, attackerInfo, projectile_3.ProjectileData.damageType,
							projectile_3.ProjectileData.damageSubType, 0, rifleOverHeatAttack.parameterCollection_3,
							projectile_3.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
						KnockbackState state = rifleOverHeatAttack.CreateState<KnockbackState>(targetAgent, 2000010);
						state.Init(damageDirection, 0.1f,
							Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAttackDelay,
							EasingFunction.Ease.EaseOutQuad, false);
						targetAgent.AddState(state, rifleOverHeatAttack.Caster.ObjectId);
					});
				rifleOverHeatAttack1.LaunchProjectile(projectile_3);
			}

			rifleOverHeatAttack1.CheckReload();
			rifleOverHeatAttack1.Finish();

			// co: dotPeek
			// this.Start();
			// base.LookAtTarget(base.Caster, base.Target, 0f, false);
			// if (base.IsEnoughBullet())
			// {
			// 	yield return base.WaitForSecondsByAttackSpeed(base.Caster, Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAttackDelay);
			// 	base.LaunchProjectile(this.CreateProjectile_1_2());
			// 	base.FinishNormalAttack();
			// }
			// if (base.IsEnoughBullet())
			// {
			// 	yield return base.WaitForSecondsByAttackSpeed(base.Caster, Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAttackDelay);
			// 	base.LaunchProjectile(this.CreateProjectile_1_2());
			// }
			// if (base.IsEnoughBullet())
			// {
			// 	AssaultRifleOverHeatAttack.<>c__DisplayClass4_0 CS$<>8__locals1 = new AssaultRifleOverHeatAttack.<>c__DisplayClass4_0();
			// 	CS$<>8__locals1.<>4__this = this;
			// 	yield return base.WaitForSecondsByAttackSpeed(base.Caster, Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAttackDelay);
			// 	CS$<>8__locals1.projectile_3 = base.PopProjectileProperty(base.Caster, Singleton<AssaultRifleSkillActiveData>.inst.ProjectileCode);
			// 	CS$<>8__locals1.projectile_3.SetTargetObject(base.Target.ObjectId);
			// 	CS$<>8__locals1.projectile_3.SetActionOnCollisionCharacter(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			// 	{
			// 		CS$<>8__locals1.<>4__this.parameterCollection_3.Clear();
			// 		CS$<>8__locals1.<>4__this.parameterCollection_3.Add(SkillScriptParameterType.DamageApCoef, Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAttackApCoef_3);
			// 		CS$<>8__locals1.<>4__this.DamageTo(targetAgent, attackerInfo, CS$<>8__locals1.projectile_3.ProjectileData.damageType, CS$<>8__locals1.projectile_3.ProjectileData.damageSubType, 0, CS$<>8__locals1.<>4__this.parameterCollection_3, CS$<>8__locals1.projectile_3.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0, true, 0, 1f, true);
			// 		KnockbackState knockbackState = CS$<>8__locals1.<>4__this.CreateState<KnockbackState>(targetAgent, 2000010, 0, null);
			// 		knockbackState.Init(damageDirection, 0.1f, Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAttackDelay, EasingFunction.Ease.EaseOutQuad, false);
			// 		targetAgent.AddState(knockbackState, CS$<>8__locals1.<>4__this.Caster.ObjectId);
			// 	});
			// 	base.LaunchProjectile(CS$<>8__locals1.projectile_3);
			// 	CS$<>8__locals1 = null;
			// }
			// base.CheckReload();
			// this.Finish(false);
			// yield break;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}