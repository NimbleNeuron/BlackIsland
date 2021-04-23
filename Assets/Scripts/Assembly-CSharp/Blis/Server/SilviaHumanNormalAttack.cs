using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaHumanNormalAttack)]
	public class SilviaHumanNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection cacheParameterCollection =
			SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			MasteryType masteryType = GetEquipWeaponMasteryType(Caster);
			bool? isLaunchProjectile = masteryType.IsLaunchProjectile();
			if (isLaunchProjectile == null)
			{
				Finish();
				yield break;
			}

			LookAtTarget(Caster, Target, 0.1f);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<SilviaSkillHumanData>.inst.NormalAttackDelay[masteryType]);
			bool? flag = isLaunchProjectile;
			bool flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				if (IsEnoughBullet())
				{
					ProjectileProperty projectile = PopProjectileProperty(Caster,
						Singleton<SilviaSkillHumanData>.inst.ProjectileCode[masteryType]);
					projectile.SetTargetObject(Target.ObjectId);
					projectile.SetActionOnCollisionCharacter(
						delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
							Vector3 damageDirection)
						{
							cacheParameterCollection.Clear();
							cacheParameterCollection.Add(SkillScriptParameterType.DamageApCoef,
								Singleton<SilviaSkillHumanData>.inst.NormalAttackApCoef);
							DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
								projectile.ProjectileData.damageSubType, 0, cacheParameterCollection,
								projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection,
								Singleton<SilviaSkillHumanData>.inst.NormalAttackEffectAndSoundWeaponType[masteryType]);
						});
					LaunchProjectile(projectile);
				}
			}
			else
			{
				cacheParameterCollection.Clear();
				cacheParameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<SilviaSkillHumanData>.inst.NormalAttackApCoef);
				DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 0, cacheParameterCollection,
					Singleton<SilviaSkillHumanData>.inst.NormalAttackEffectAndSoundWeaponType[masteryType]);
			}

			FinishNormalAttack();
			flag = isLaunchProjectile;
			flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				if (IsEnoughBullet())
				{
					yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
				}
				else
				{
					CheckReload();
				}
			}
			else
			{
				yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			}

			Finish();
		}
	}
}