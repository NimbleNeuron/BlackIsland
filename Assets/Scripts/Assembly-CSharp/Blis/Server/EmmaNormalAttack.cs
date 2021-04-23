using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.EmmaNormalAttack)]
	public class EmmaNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection damageParameter = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, Target.Position, 0.1f);
			MasteryType masteryType = GetEquipWeaponMasteryType(Caster);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<EmmaSkillNormalAttackData>.inst.NormalAttackDelay[masteryType]);
			damageParameter.Clear();
			damageParameter.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<EmmaSkillNormalAttackData>.inst.NormalAttackApCoef);
			ProjectileProperty projectileProperty = PopProjectileProperty(Caster,
				Singleton<EmmaSkillNormalAttackData>.inst.ProjectileCode[masteryType]);
			projectileProperty.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					DamageTo(targetAgent, attackerInfo, projectileProperty.ProjectileData.damageType,
						projectileProperty.ProjectileData.damageSubType, 0, damageParameter,
						projectileProperty.SkillUseInfo.skillSlotSet, damagePoint, damageDirection,
						Singleton<EmmaSkillNormalAttackData>.inst.DamageEffectAndSoundCode[masteryType]);
					if (Caster.AnyHaveStateByGroup(Singleton<EmmaSkillPassiveData>.inst
						.CheerUpNormalAttackBuffStateGroupCode))
					{
						damageParameter.Clear();
						damageParameter.Add(SkillScriptParameterType.DamageCasterMaxSpCoef,
							Singleton<EmmaSkillPassiveData>.inst.CheerUpDamageMaxSpRatioByLevel[SkillLevel]);
						DamageTo(targetAgent, attackerInfo, DamageType.Skill, DamageSubType.Normal, 0, damageParameter,
							projectileProperty.SkillUseInfo.skillSlotSet, damagePoint, damageDirection,
							Singleton<EmmaSkillNormalAttackData>.inst.DamageBuffEffectAndSoundCode);
					}
				});
			projectileProperty.SetTargetObject(Target.ObjectId);
			LaunchProjectile(projectileProperty);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}