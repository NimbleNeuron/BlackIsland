using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AyaActive1)]
	public class AyaActive1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParam_1 = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection damageParam_2 = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			Target.AttachSight(Caster.WorldObject, 3f, 4f, true);
			ProjectileProperty projectile_1 =
				PopProjectileProperty(Caster, Singleton<AyaSkillActive1Data>.inst.ProjectileCode);
			
			projectile_1.SetTargetObject(Target.ObjectId);
			projectile_1.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					damageParam_1.Clear();
					damageParam_1.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<AyaSkillActive1Data>.inst.SkillApCoef[SkillLevel]);
					DamageTo(targetAgent, attackerInfo, projectile_1.ProjectileData.damageType,
						projectile_1.ProjectileData.damageSubType, 0, damageParam_1,
						projectile_1.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
				});
			LaunchProjectile(projectile_1);
			MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(Caster.WorldObject, null, NoiseType.Gunshot);
			yield return WaitForSeconds(Singleton<AyaSkillActive1Data>.inst.SkillAttackDelay);
			ProjectileProperty projectile_2 =
				PopProjectileProperty(Caster, Singleton<AyaSkillActive1Data>.inst.ProjectileCode_2);
			projectile_2.SetTargetObject(Target.ObjectId);
			projectile_2.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					damageParam_2.Clear();
					damageParam_2.Add(SkillScriptParameterType.Damage,
						Singleton<AyaSkillActive1Data>.inst.DamageByLevel[SkillLevel]);
					damageParam_2.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<AyaSkillActive1Data>.inst.SkillApCoef_2[SkillLevel]);
					DamageTo(targetAgent, attackerInfo, projectile_2.ProjectileData.damageType,
						projectile_2.ProjectileData.damageSubType, 0, damageParam_2,
						projectile_2.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
				});
			LaunchProjectile(projectile_2);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}