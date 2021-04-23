using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaHumanNormalAttackMultifyMount)]
	public class SilviaHumanNormalAttackMultifyMount : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection cacheParameterCollection =
			SkillScriptParameterCollection.Create();

		
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
				Singleton<SilviaSkillHumanData>.inst.NormalAttackMountDelay[masteryType]);
			bool? flag = isLaunchProjectile;
			bool flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				if (IsEnoughBullet())
				{
					ProjectileProperty projectile = PopProjectileProperty(Caster,
						Singleton<SilviaSkillHumanData>.inst.ProjectileCode[masteryType]);
					projectile.SetActionOnCollisionCharacter(
						delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
							Vector3 damageDirection)
						{
							cacheParameterCollection.Clear();
							cacheParameterCollection.Add(SkillScriptParameterType.DamageApCoef,
								Singleton<SilviaSkillHumanData>.inst.NormalAttackReinforceApCoef[info.SkillLevel]);
							DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
								projectile.ProjectileData.damageSubType, 0, cacheParameterCollection,
								projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection,
								Singleton<SilviaSkillHumanData>.inst.NormalAttackEffectAndSoundWeaponType[masteryType]);
						});
					projectile.SetTargetObject(Target.ObjectId);
					LaunchProjectile(projectile);
				}
			}
			else
			{
				cacheParameterCollection.Clear();
				cacheParameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<SilviaSkillHumanData>.inst.NormalAttackReinforceApCoef[info.SkillLevel]);
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

			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<SilviaSkillHumanData>.inst.NormalAttackReinforceState).group,
				Caster.ObjectId);
			Finish();
		}
	}
}