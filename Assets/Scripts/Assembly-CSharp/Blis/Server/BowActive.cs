using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.BowActive)]
	public class BowActive : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			ProjectileProperty projectileProperty =
				PopProjectileProperty(Caster, Singleton<BowSkillActiveData>.inst.ProjectileCode);
			projectileProperty.SetExplosionSkill(SkillId.BowActiveProjectile);
			LaunchProjectile(projectileProperty, info.cursorPosition);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}