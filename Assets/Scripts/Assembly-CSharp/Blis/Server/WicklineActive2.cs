using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WicklineActive2)]
	public class WicklineActive2 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly CollisionBox3D sector = new CollisionBox3D(Vector3.zero, 0f, 0f, Vector3.zero);

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 direction = info.cursorPosition;
			LookAtDirection(Caster, direction);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			yield return WaitForSeconds(SkillConcentrationTime);
			FinishConcentration(false);
			sector.UpdatePosition(Caster.Position + SkillRange * 0.5f * direction);
			sector.UpdateWidth(SkillWidth);
			sector.UpdateDepth(SkillRange);
			sector.UpdateNormalized(direction);
			foreach (SkillAgent target in GetEnemyCharacters(sector))
			{
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<WicklineSkillActive2Data>.inst.SkillDamage);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<WicklineSkillActive2Data>.inst.SkillDamageCoef);
				DamageTo(target, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
					Singleton<WicklineSkillActive2Data>.inst.EffectAndSound);
			}

			ProjectileProperty projectileProperty =
				PopProjectileProperty(Caster, Singleton<WicklineSkillActive2Data>.inst.ProjectileCode);
			projectileProperty.SetCollisionObjectDirection(direction);
			projectileProperty.SetExplosionSkill(SkillId.WicklineActive2Projectile);
			LaunchProjectile(projectileProperty);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}