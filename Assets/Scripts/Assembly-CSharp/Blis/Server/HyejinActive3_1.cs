using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyejinActive3_1)]
	public class HyejinActive3_1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParam = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
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
			ProjectileProperty projectileProperty =
				PopProjectileProperty(Caster, Singleton<HyejinSkillData>.inst.A3ProjectileCode);
			projectileProperty.SetTargetDirection(targetDirection);
			projectileProperty.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					damageParam.Clear();
					damageParam.Add(SkillScriptParameterType.Damage,
						Singleton<HyejinSkillData>.inst.A3_1BaseDamage[SkillLevel]);
					damageParam.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<HyejinSkillData>.inst.A3_1ApDamage);
					DamageTo(targetAgent, attackerInfo, projectileProperty.ProjectileData.damageType,
						projectileProperty.ProjectileData.damageSubType, 1, damageParam,
						projectileProperty.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
				});
			ProjectileProperty projectileProperty2 = PopProjectileProperty(Caster,
				Singleton<HyejinSkillData>.inst.A3EndPositionDisplayProjectileCode);
			projectileProperty2.SetTargetDirection(targetDirection);
			WorldProjectile target = LaunchProjectile(projectileProperty);
			Caster.AttachSight(target, 1f, projectileProperty.ProjectileData.lifeTimeAfterArrival, false);
			LaunchProjectile(projectileProperty2);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}