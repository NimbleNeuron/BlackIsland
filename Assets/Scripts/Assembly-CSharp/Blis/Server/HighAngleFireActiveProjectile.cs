using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HighAngleFireActiveProjectile)]
	public class HighAngleFireActiveProjectile : SkillScript
	{
		
		private const float Tick = 0.1f;

		
		private CollisionCircle3D collision;

		
		private readonly List<SkillAgent> hitEnemies = new List<SkillAgent>();

		
		protected override void Start()
		{
			base.Start();
			hitEnemies.Clear();
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
				float lifeTime = worldProjectile.Property.ProjectileData.lifeTimeAfterExplosion;
				CharacterStateData debuffState =
					GameDB.characterState.GetData(Singleton<HighAngleFireSkillActiveData>.inst.DebuffState[SkillLevel]);
				collision.UpdatePosition(worldProjectile.GetPosition());
				collision.UpdateRadius(worldProjectile.Property.ProjectileData.explosionRadius);
				while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - startTime <= lifeTime)
				{
					for (int i = hitEnemies.Count - 1; i >= 0; i--)
					{
						SkillAgent skillAgent = hitEnemies[i];
						if (!collision.Collision(skillAgent.CollisionObject))
						{
							skillAgent.RemoveStateByGroup(debuffState.group, Caster.Owner.ObjectId);
							hitEnemies.RemoveAt(i);
						}
					}

					foreach (SkillAgent skillAgent2 in GetEnemyCharacters(collision))
					{
						if (!skillAgent2.AnyHaveStateByGroup(debuffState.group))
						{
							CharacterState state = CreateState(skillAgent2, debuffState.code);
							AddState(skillAgent2, state);
							hitEnemies.Add(skillAgent2);
						}
					}

					yield return WaitForSeconds(0.1f);
				}

				debuffState = null;
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<HighAngleFireSkillActiveData>.inst.DebuffState[SkillLevel]);
			for (int i = 0; i < hitEnemies.Count; i++)
			{
				SkillAgent skillAgent = hitEnemies[i];
				if (skillAgent.AnyHaveStateByGroup(data.group))
				{
					skillAgent.RemoveStateByGroup(data.group, Caster.Owner.ObjectId);
				}
			}

			hitEnemies.Clear();
		}
	}
}