using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WicklineActive2Projectile)]
	public class WicklineActive2Projectile : SkillScript
	{
		
		private readonly CollisionBox3D collision = new CollisionBox3D(Vector3.zero, 0f, 0f, Vector3.zero);

		
		private float tickTime;

		
		protected override void Start()
		{
			base.Start();
			tickTime = 0f;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			float poisonInterval = Singleton<WicklineSkillActive2Data>.inst.PoisonInterval;
			LookAtDirection(Caster, Caster.Owner.transform.forward);
			WorldProjectile worldProjectile = extraData as WorldProjectile;
			if (worldProjectile != null)
			{
				float lifeTime = worldProjectile.Property.ProjectileData.lifeTimeAfterExplosion;
				while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - startTime <= lifeTime)
				{
					if (tickTime < poisonInterval)
					{
						tickTime += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
					}
					else
					{
						tickTime -= poisonInterval;
						Vector3 forward = Caster.Forward;
						collision.UpdatePosition(Caster.Position + SkillRange * 0.5f * forward);
						collision.UpdateWidth(SkillWidth);
						collision.UpdateRadius(SkillRange);
						collision.UpdateNormalized(forward);
						List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
						AddState(enemyCharacters, Singleton<WicklineSkillActive2Data>.inst.DebuffState);
					}

					yield return WaitForFrame();
				}
			}

			Finish();
		}
	}
}