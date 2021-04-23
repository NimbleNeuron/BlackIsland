using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AyaNormalAttack)]
	public class AyaNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		private ProjectileProperty CreateProjectileProperty(int projectileCode, float AttackCoef)
		{
			ProjectileProperty projectile = PopProjectileProperty(Caster, projectileCode);
			projectile.SetTargetObject(Target.ObjectId);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef, AttackCoef);
					DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
				});
			return projectile;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			int masteryType = (int) GetEquipWeaponMasteryType(Caster);
			if (IsEnoughBullet())
			{
				yield return WaitForSecondsByAttackSpeed(Caster,
					Singleton<AyaSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
				LaunchProjectile(CreateProjectileProperty(
					Singleton<AyaSkillNormalAttackData>.inst.ProjectileCode[masteryType],
					Singleton<AyaSkillNormalAttackData>.inst.NormalAttackApCoef[masteryType]));
				MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(Caster.WorldObject, null, NoiseType.Gunshot);
				FinishNormalAttack();
			}

			if (masteryType == 10)
			{
				if (IsEnoughBullet())
				{
					yield return WaitForSecondsByAttackSpeed(Caster,
						Singleton<AyaSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
					LaunchProjectile(CreateProjectileProperty(
						Singleton<AyaSkillNormalAttackData>.inst.ProjectileCode[masteryType],
						Singleton<AyaSkillNormalAttackData>.inst.NormalAttackApCoef[masteryType]));
				}

				if (IsEnoughBullet())
				{
					yield return WaitForSecondsByAttackSpeed(Caster,
						Singleton<AyaSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
					LaunchProjectile(CreateProjectileProperty(
						Singleton<AyaSkillNormalAttackData>.inst.ProjectileCode[masteryType],
						Singleton<AyaSkillNormalAttackData>.inst.AssaultRifleApCoef));
				}
			}

			if (IsEnoughBullet())
			{
				yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			}

			CheckReload();
			Finish();
		}
	}
}