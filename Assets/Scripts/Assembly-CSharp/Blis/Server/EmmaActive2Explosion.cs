using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.EmmaActive2Explosion)]
	public class EmmaActive2Explosion : EmmaSkillScript
	{
		
		private CollisionCircle3D collisionCircle;

		
		private readonly SkillScriptParameterCollection damageParameter = SkillScriptParameterCollection.Create();

		
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
				collisionCircle.UpdatePosition(worldProjectile.GetPosition());
				collisionCircle.UpdateRadius(worldProjectile.Property.ProjectileData.explosionRadius);
				List<SkillAgent> enemyCharacters = GetEnemyCharacters(collisionCircle);
				if (enemyCharacters != null && 0 < enemyCharacters.Count)
				{
					ModifySkillCooldown(Caster, SkillSlotSet.Active2_1,
						Singleton<EmmaSkillActive2Data>.inst.CooldownReduce);
				}

				foreach (SkillAgent target in enemyCharacters)
				{
					damageParameter.Clear();
					damageParameter.Add(SkillScriptParameterType.Damage,
						Singleton<EmmaSkillActive2Data>.inst.DamageBySkillLevel[SkillLevel]);
					damageParameter.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<EmmaSkillActive2Data>.inst.DamageApCoef);
					DamageTo(target, worldProjectile.Property.ProjectileData.damageType,
						worldProjectile.Property.ProjectileData.damageSubType, 0, damageParameter,
						Singleton<EmmaSkillActive2Data>.inst.FireworkHatExplosionDamageEffectAndSoundCode);
				}

				PlaySkillAction(Caster, SkillId.EmmaActive2Explosion, 2, 0, new BlisVector(info.cursorPosition));
			}

			yield return WaitForFrame();
			Finish();
		}

		
		protected override void Finish(bool cancel = false) { }
	}
}