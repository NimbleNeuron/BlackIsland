using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.DirectFireActive)]
	public class DirectFireActive : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 skillPoint = GetSkillPoint();
			LookAtPosition(Caster, skillPoint);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			ProjectileProperty projectileProperty =
				PopProjectileProperty(Caster, Singleton<DirectFireSkillActiveData>.inst.ProjectileCode);
			projectileProperty.SetTargetDirection(GameUtil.DirectionOnPlane(Caster.Position, skillPoint));
			projectileProperty.SetDistance(GameUtil.DistanceOnPlane(Caster.Position, skillPoint));
			projectileProperty.SetSpeed(projectileProperty.Distance, projectileProperty.ProjectileData.duration);
			projectileProperty.SetExplosionSkill(SkillId.DirectFireActiveProjectile);
			LaunchProjectile(projectileProperty);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}