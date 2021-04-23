using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.IsolNormalAttack)]
	public class IsolNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection1 = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollection2 = SkillScriptParameterCollection.Create();

		
		private ProjectileProperty GetProjectileProperty(SkillScriptParameterCollection parameterCollection,
			int masteryType)
		{
			ProjectileProperty projectile = PopProjectileProperty(Caster,
				Singleton<IsolSkillNormalAttackData>.inst.ProjectileCode[masteryType]);
			projectile.SetTargetObject(Target.ObjectId);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
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
					Singleton<IsolSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
				parameterCollection1.Clear();
				parameterCollection1.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<IsolSkillNormalAttackData>.inst.NormalAttackApCoef[masteryType]);
				LaunchProjectile(GetProjectileProperty(parameterCollection1, masteryType));
				MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(Caster.WorldObject, null, NoiseType.Gunshot);
				FinishNormalAttack();
			}

			if (masteryType == 10)
			{
				if (IsEnoughBullet())
				{
					yield return WaitForSecondsByAttackSpeed(Caster,
						Singleton<IsolSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
					parameterCollection1.Clear();
					parameterCollection1.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<IsolSkillNormalAttackData>.inst.NormalAttackApCoef[masteryType]);
					LaunchProjectile(GetProjectileProperty(parameterCollection1, masteryType));
				}

				if (IsEnoughBullet())
				{
					yield return WaitForSecondsByAttackSpeed(Caster,
						Singleton<IsolSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
					parameterCollection2.Clear();
					parameterCollection2.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<IsolSkillNormalAttackData>.inst.AssaultRifleApCoef);
					LaunchProjectile(GetProjectileProperty(parameterCollection2, masteryType));
				}

				FinishNormalAttack();
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