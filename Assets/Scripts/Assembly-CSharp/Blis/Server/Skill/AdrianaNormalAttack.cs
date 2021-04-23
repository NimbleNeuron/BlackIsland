using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AdrianaNormalAttack)]
	public class AdrianaNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			MasteryType masteryType = GetEquipWeaponMasteryType(Caster);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<AdrianaSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
			ProjectileProperty projectileProperty = PopProjectileProperty(Caster,
				Singleton<AdrianaSkillNormalAttackData>.inst.ProjectileCode[masteryType]);
			projectileProperty.SetTargetObject(Target.ObjectId);
			projectileProperty.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<AdrianaSkillNormalAttackData>.inst.NormalAttackApCoef);
					DamageTo(targetAgent, attackerInfo, projectileProperty.ProjectileData.damageType,
						projectileProperty.ProjectileData.damageSubType, 0, parameterCollection,
						projectileProperty.SkillUseInfo.skillSlotSet, damagePoint, damageDirection,
						Singleton<AdrianaSkillNormalAttackData>.inst.HighAngleFireDamageEffectAndSoundCode);
				});
			LaunchProjectile(projectileProperty);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}