using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.EmmaActive4Explosion)]
	public class EmmaActive4Explosion : EmmaSkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParameter = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D collisionCircle;

		
		protected override void Start()
		{
			base.Start();
			if (collisionCircle == null)
			{
				collisionCircle = new CollisionCircle3D(Vector3.zero, 0f);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			WorldProjectile worldProjectile = extraData as WorldProjectile;
			if (worldProjectile != null)
			{
				Vector3 position = worldProjectile.GetPosition();
				collisionCircle.UpdatePosition(position);
				collisionCircle.UpdateRadius(worldProjectile.Property.ProjectileData.explosionRadius);
				foreach (SkillAgent target in GetEnemyCharacters(collisionCircle))
				{
					damageParameter.Clear();
					damageParameter.Add(SkillScriptParameterType.Damage,
						Singleton<EmmaSkillActive4Data>.inst.FireworkHatExplosionDamageBySkillLevel[SkillLevel]);
					damageParameter.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<EmmaSkillActive4Data>.inst.FireworkHatExplosionDamageApCoef);
					DamageTo(target, worldProjectile.Property.ProjectileData.damageType,
						worldProjectile.Property.ProjectileData.damageSubType, 0, damageParameter,
						Singleton<EmmaSkillActive2Data>.inst.FireworkHatExplosionDamageEffectAndSoundCode);
				}

				PlaySkillAction(Caster, SkillId.EmmaActive4Explosion, 2, 0, new BlisVector(position));
			}

			yield return WaitForFrame();
			Finish();
		}

		
		protected override void Finish(bool cancel = false) { }
	}
}