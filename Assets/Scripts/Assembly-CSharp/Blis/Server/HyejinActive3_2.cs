using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyejinActive3_2)]
	public class HyejinActive3_2 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParam = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D collision;

		
		protected override void Start()
		{
			base.Start();
			if (collision == null)
			{
				collision = new CollisionCircle3D(Caster.Position, Singleton<HyejinSkillData>.inst.A3_2ApRange);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			HyejinActive3_2 hyejinActive32 = this;
			hyejinActive32.Start();
			if (hyejinActive32.SkillCastingTime1 > 0.0)
			{
				yield return hyejinActive32.FirstCastingTime();
			}

			WorldMovableCharacter character = hyejinActive32.Caster.Character as WorldMovableCharacter;
			if (character == null)
			{
				Log.E("HyeJin E Skill Seq1 can not cast WorldMovableCharacter");
				hyejinActive32.Finish();
			}
			else
			{
				WorldProjectile ownProjectile = character.GetOwnProjectile(Condition);
				if (ownProjectile == null)
				{
					Log.V("HyeJin E Skill Seq1 can not find Projectile");
					hyejinActive32.Finish();
				}
				else
				{
					hyejinActive32.PlaySkillAction(hyejinActive32.Caster, 1);
					NavMeshHit hit;
					if (NavMesh.SamplePosition(ownProjectile.GetPosition(), out hit, hyejinActive32.SkillRange,
						2147483640))
					{
						Vector3 position = hit.position;
						hyejinActive32.Caster.WarpTo(position);
					}

					ownProjectile.DestroySelf();
					hyejinActive32.collision.UpdatePosition(hyejinActive32.Caster.Position);
					hyejinActive32.collision.UpdateRadius(Singleton<HyejinSkillData>.inst.A3_2ApRange);
					List<SkillAgent> enemyCharacters =
						hyejinActive32.GetEnemyCharacters(hyejinActive32.collision);
					if (enemyCharacters.Count > 0)
					{
						hyejinActive32.damageParam.Clear();
						hyejinActive32.damageParam.Add(SkillScriptParameterType.Damage,
							Singleton<HyejinSkillData>.inst.A3_2BaseDamage[hyejinActive32.SkillLevel]);
						hyejinActive32.damageParam.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<HyejinSkillData>.inst.A3_2ApDamage);
						hyejinActive32.DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 1,
							hyejinActive32.damageParam, 0);
					}

					hyejinActive32.PlaySkillAction(hyejinActive32.Caster, 2);
					if (hyejinActive32.SkillFinishDelayTime > 0.0)
					{
						yield return hyejinActive32.FinishDelayTime();
					}

					hyejinActive32.Finish();
				}
			}

			bool Condition(WorldProjectile projectile)
			{
				return projectile.Property.ProjectileData.code ==
				       Singleton<HyejinSkillData>.inst.A3ProjectileCode;
			}

			// co: dotPeek
			// this.Start();
			// if (base.SkillCastingTime1 > 0f)
			// {
			// 	yield return base.FirstCastingTime();
			// }
			// WorldMovableCharacter worldMovableCharacter = base.Caster.Character as WorldMovableCharacter;
			// if (worldMovableCharacter == null)
			// {
			// 	Log.E("HyeJin E Skill Seq1 can not cast WorldMovableCharacter");
			// 	this.Finish(false);
			// 	yield break;
			// }
			// WorldProjectile ownProjectile = worldMovableCharacter.GetOwnProjectile(new Func<WorldProjectile, bool>(HyejinActive3_2.<>c.<>9.<Play>g__Condition|3_0));
			// if (ownProjectile == null)
			// {
			// 	Log.V("HyeJin E Skill Seq1 can not find Projectile");
			// 	this.Finish(false);
			// 	yield break;
			// }
			// base.PlaySkillAction(base.Caster, 1, null, null);
			// Vector3 position = ownProjectile.GetPosition();
			// NavMeshHit navMeshHit;
			// if (NavMesh.SamplePosition(position, out navMeshHit, base.SkillRange, 2147483640))
			// {
			// 	position = navMeshHit.position;
			// 	base.Caster.WarpTo(position, true);
			// }
			// ownProjectile.DestroySelf();
			// this.collision.UpdatePosition(base.Caster.Position);
			// this.collision.UpdateRadius(Singleton<HyejinSkillData>.inst.A3_2ApRange);
			// List<SkillAgent> enemyCharacters = base.GetEnemyCharacters(this.collision);
			// if (enemyCharacters.Count > 0)
			// {
			// 	this.damageParam.Clear();
			// 	this.damageParam.Add(SkillScriptParameterType.Damage, (float)Singleton<HyejinSkillData>.inst.A3_2BaseDamage[base.SkillLevel]);
			// 	this.damageParam.Add(SkillScriptParameterType.DamageApCoef, Singleton<HyejinSkillData>.inst.A3_2ApDamage);
			// 	base.DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 1, this.damageParam, 0, true, 0, 1f, true);
			// }
			// base.PlaySkillAction(base.Caster, 2, null, null);
			// if (base.SkillFinishDelayTime > 0f)
			// {
			// 	yield return base.FinishDelayTime();
			// }
			// this.Finish(false);
			// yield break;
		}
	}
}