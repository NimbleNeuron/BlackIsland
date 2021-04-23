using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyejinNormalAttack)]
	public class HyejinNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			int masteryType = (int) GetEquipWeaponMasteryType(Caster);
			LookAtTarget(Caster, Target, 0.1f);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<HyejinSkillData>.inst.NormalAttackDelay[masteryType]);
			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<HyejinSkillData>.inst.ProjectileCode[masteryType]);
			projectile.SetTargetObject(Target.ObjectId);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<HyejinSkillData>.inst.NormalAttackApCoef);
					DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
				});
			LaunchProjectile(projectile);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}