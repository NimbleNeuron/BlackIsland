using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ZahirActive4Projectile)]
	public class ZahirActive4Projectile : SkillScript
	{
		
		private const int MAX_TICK = 4;

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D collision;

		
		private int tick;

		
		private float tickTime;

		
		protected override void Start()
		{
			base.Start();
			tick = 0;
			tickTime = 0f;
			if (collision == null)
			{
				collision = new CollisionCircle3D(Vector3.zero, 0f);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			WorldProjectile worldProjectile = extraData as WorldProjectile;
			if (worldProjectile != null)
			{
				collision.UpdatePosition(worldProjectile.GetPosition());
				collision.UpdateRadius(worldProjectile.Property.ProjectileData.explosionRadius);
				List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<ZahirSkillActive4Data>.inst.SkillApCoef);
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<ZahirSkillActive4Data>.inst.DamageByLevel[SkillLevel]);
				DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 1005006);
				while (tick < 4)
				{
					if (tickTime < 1f)
					{
						tickTime += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
					}
					else
					{
						tick++;
						tickTime -= 1f;
						List<SkillAgent> enemyCharacters2 = GetEnemyCharacters(collision);
						parameterCollection.Clear();
						parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<ZahirSkillActive4Data>.inst.SkillApCoef_2);
						parameterCollection.Add(SkillScriptParameterType.Damage,
							Singleton<ZahirSkillActive4Data>.inst.DamageByLevel_2[SkillLevel]);
						DamageTo(enemyCharacters2, DamageType.Skill, DamageSubType.Dot, 0, parameterCollection,
							1005006);
					}

					yield return WaitForFrame();
				}
			}

			Finish();
		}
	}
}