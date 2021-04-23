using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ChiaraTransformNormalAttack)]
	public class ChiaraTransformNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target, 0.1f);
			yield return WaitForSecondsByAttackSpeed(Caster, Singleton<ChiaraSkillData>.inst.A4NormalAttackDelay);
			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<ChiaraSkillData>.inst.A4NormalAttackProjectileCode);
			projectile.SetTargetObject(Target.ObjectId);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<ChiaraSkillData>.inst.A4NormalAttackApCoef);
					DamageTo(targetAgent, attackerInfo, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
				});
			LaunchProjectile(projectile);
			FinishNormalAttack();
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}