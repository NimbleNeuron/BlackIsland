using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AdrianaActive4FireBomb)]
	public class AdrianaActive4FireBomb : AdrianaSkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
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
				collisionCircle.UpdatePosition(worldProjectile.GetPosition());
				collisionCircle.UpdateRadius(worldProjectile.Property.ProjectileData.explosionRadius);
				foreach (SkillAgent skillAgent in GetEnemyCharacters(collisionCircle))
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<AdrianaSkillActive4Data>.inst.DamageBySkillLevel[SkillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<AdrianaSkillActive4Data>.inst.DamageApCoef);
					DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
						Singleton<AdrianaSkillActive4Data>.inst.ExplosionEffectSound);
					KnockbackState knockbackState = CreateState<KnockbackState>(skillAgent,
						Singleton<AdrianaSkillActive4Data>.inst.FireBombStateCode);
					Vector3 direction;
					if (worldProjectile.GetPosition() == skillAgent.Position && worldProjectile.Owner != null)
					{
						direction = GameUtil.Direction(skillAgent.Position, worldProjectile.Owner.GetPosition());
					}
					else
					{
						direction = GameUtil.Direction(worldProjectile.GetPosition(), skillAgent.Position);
					}

					knockbackState.Init(direction, Singleton<AdrianaSkillActive4Data>.inst.KnockBackDistance,
						Singleton<AdrianaSkillActive4Data>.inst.KnockBackDuration, EasingFunction.Ease.EaseOutQuad,
						false);
					skillAgent.AddState(knockbackState, Caster.ObjectId);
				}
			}

			yield return WaitForFrame();
			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}