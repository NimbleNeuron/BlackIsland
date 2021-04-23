using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class HartSkillScript : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
			Finish();
		}

		
		private bool IsEvolution(int evolve)
		{
			return false;
		}

		
		protected bool IsEnchanted()
		{
			return Caster.AnyHaveStateByGroup(Singleton<HartSkillActive2Data>.inst.Active2BuffGroup);
		}

		
		protected void LaunchProjectile(int targetId, int projectileCode, int baseDamage, float apCoef)
		{
			ProjectileProperty projectile = PopProjectileProperty(Caster, projectileCode);
			projectile.SetTargetObject(targetId);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage, baseDamage);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef, apCoef);
					DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
				});
			base.LaunchProjectile(projectile);
		}
	}
}