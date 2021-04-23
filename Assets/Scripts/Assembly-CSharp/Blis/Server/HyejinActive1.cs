using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyejinActive1)]
	public class HyejinActive1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParam = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
				LookAtPosition(Caster, info.cursorPosition);
			}

			Vector3 targetDirection = GameUtil.Direction(Caster.Position, GetSkillPoint());

			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<HyejinSkillData>.inst.A1ProjectileCode);
			
			projectile.SetActionOnCollisionCharacter(onCollideCharacter);
			projectile.SetTargetDirection(targetDirection);
			
			LaunchProjectile(projectile);
			
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
			
			void onCollideCharacter(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			{
				damageParam.Clear();
				
				damageParam.Add(SkillScriptParameterType.Damage, Singleton<HyejinSkillData>.inst.A1BaseDamage[SkillLevel]);
				damageParam.Add(SkillScriptParameterType.DamageApCoef, Singleton<HyejinSkillData>.inst.A1ApDamage);
				DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType, projectile.ProjectileData.damageSubType, 1, damageParam, projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
				AddState(targetAgent, Singleton<HyejinSkillData>.inst.A1HitDebuff);
			}
		}
	}
}