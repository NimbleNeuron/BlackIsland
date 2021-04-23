using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WicklinePassiveProjectile)]
	public class WicklinePassiveProjectile : SkillScript
	{
		
		private readonly CollisionCircle3D collision = new CollisionCircle3D(Vector3.zero, 0f);

		
		private float tickTime;

		
		protected override void Start()
		{
			base.Start();
			tickTime = 0f;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			float poisonInterval = Singleton<WicklineSkillPassiveData>.inst.PoisonInterval;
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
						collision.UpdatePosition(worldProjectile.GetPosition());
						collision.UpdateRadius(worldProjectile.Property.ProjectileData.explosionRadius);
						List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
						AddState(enemyCharacters, Singleton<WicklineSkillPassiveData>.inst.PoisoningState);
					}

					yield return WaitForFrame();
				}
			}

			Finish();
		}
	}
}