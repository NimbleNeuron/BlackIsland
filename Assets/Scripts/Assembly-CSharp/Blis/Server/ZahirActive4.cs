using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ZahirActive4)]
	public class ZahirActive4 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			ProjectileProperty projectileProperty =
				PopProjectileProperty(Caster, Singleton<ZahirSkillActive4Data>.inst.ProjectileCode);
			projectileProperty.SetExplosionSkill(SkillId.ZahirActive4Projectile);
			WorldProjectile target = LaunchProjectile(projectileProperty, GetSkillPoint());
			float duration = projectileProperty.ProjectileData.lifeTimeAfterArrival +
			                 projectileProperty.ProjectileData.lifeTimeAfterExplosion;
			Caster.AttachSight(target, projectileProperty.ProjectileData.explosionRadius, duration, false);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}