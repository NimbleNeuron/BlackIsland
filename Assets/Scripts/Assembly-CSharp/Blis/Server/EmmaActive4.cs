using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Server
{
	
	[SkillScript(SkillId.EmmaActive4)]
	public class EmmaActive4 : EmmaSkillScript
	{
		
		private CollisionBox3D collisionBox;

		
		private readonly SkillScriptParameterCollection pigeonLineDamageParameter =
			SkillScriptParameterCollection.Create();

		
		private Vector3 selectPosition;

		
		private SelectType selectType;

		
		private Vector3 warpPrevCasterPosition;

		
		protected override void Start()
		{
			base.Start();
			selectType = SelectType.None;
			warpPrevCasterPosition = Caster.Position;
			selectPosition = Caster.Position;
			if (collisionBox == null)
			{
				collisionBox = new CollisionBox3D(Vector3.zero, 0f, 0f, Vector3.zero);
			}

			if (Target != null)
			{
				if (Target.IsHaveStateByGroup(Singleton<EmmaSkillActive3Data>.inst.MagicRabbitStateGroupCode,
					Caster.ObjectId))
				{
					selectType = SelectType.Rabbit;
				}
				else
				{
					WorldSummonBase worldSummonBase = Target.Character as WorldSummonBase;
					if (worldSummonBase != null && worldSummonBase.Owner == Caster.Owner)
					{
						if (Target.Character.ObjectType == ObjectType.SummonServant && IsPigeon(worldSummonBase))
						{
							selectType = SelectType.Pigeon;
						}
						else if (Target.Character.ObjectType == ObjectType.SummonTrap && IsFireworkHat(worldSummonBase))
						{
							selectType = SelectType.FireworkHat;
						}
					}
				}

				if (selectType != SelectType.None)
				{
					NavMeshHit navMeshHit;
					selectPosition = NavMesh.SamplePosition(Target.Position, out navMeshHit, SkillRange, 2147483640)
						? navMeshHit.position
						: Caster.Position;
					Caster.WarpTo(selectPosition);
				}
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (selectType == SelectType.None)
			{
				Finish();
				yield break;
			}

			PlaySkillAction(Caster, 4, null, warpPrevCasterPosition);
			PlaySkillAction(Caster, 5, null, warpPrevCasterPosition);
			if (0f < SkillCastingTime1)
			{
				yield return FirstCastingTime();
			}

			switch (selectType)
			{
				case SelectType.Pigeon:
				{
					WorldSummonBase worldSummonBase = MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(
						Caster.Character as WorldPlayerCharacter, Singleton<EmmaSkillActive1Data>.inst.PigeonSummonCode,
						warpPrevCasterPosition);
					float num = GameUtil.DistanceOnPlane(warpPrevCasterPosition, selectPosition);
					Vector3 vector = GameUtil.Direction(warpPrevCasterPosition, selectPosition);
					Vector3 position = warpPrevCasterPosition + num * 0.5f * vector;
					ProjectileProperty pigeonLineProperty = PopProjectileProperty(worldSummonBase.SkillAgent,
						Singleton<EmmaSkillActive4Data>.inst.PigeonLineProjectileCode);
					pigeonLineProperty.SetDistance(num);
					pigeonLineProperty.SetTargetObject(Caster.ObjectId);
					collisionBox.UpdatePosition(position);
					collisionBox.UpdateWidth(pigeonLineProperty.ProjectileData.collisionObjectWidth);
					collisionBox.UpdateDepth(num);
					collisionBox.UpdateNormalized(vector);
					pigeonLineProperty.SetActionOnCollisionCharacter(
						delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
							Vector3 damageDirection)
						{
							List<SkillAgent> enemyCharacters2 = GetEnemyCharacters(collisionBox);
							if (enemyCharacters2 != null && 0 < enemyCharacters2.Count)
							{
								pigeonLineDamageParameter.Clear();
								pigeonLineDamageParameter.Add(SkillScriptParameterType.Damage,
									Singleton<EmmaSkillActive4Data>.inst.PigeonAttackDamageBySkillLevel[SkillLevel]);
								pigeonLineDamageParameter.Add(SkillScriptParameterType.DamageApCoef,
									Singleton<EmmaSkillActive4Data>.inst.PigeonAttackDamageApCoef);
								foreach (SkillAgent target2 in enemyCharacters2)
								{
									DamageTo(target2, attackerInfo, pigeonLineProperty.ProjectileData.damageType,
										pigeonLineProperty.ProjectileData.damageSubType, 0, pigeonLineDamageParameter,
										SkillSlotSet, damagePoint, damageDirection,
										Singleton<EmmaSkillActive1Data>.inst.PigeonDealerDamageEffectAndSoundCode);
									AddState(target2, Singleton<EmmaSkillActive4Data>.inst.PigeonDealerFetterStateCode);
								}
							}
						});
					LaunchProjectile(pigeonLineProperty, worldSummonBase.GetPosition());
					PlaySkillAction(Caster, 6, worldSummonBase.SkillAgent);
					if (Target != null)
					{
						MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(Target.WorldObject);
					}

					break;
				}
				case SelectType.FireworkHat:
				{
					ProjectileProperty projectileProperty = PopProjectileProperty(Caster,
						Singleton<EmmaSkillActive4Data>.inst.FireworkHatExplosionAreaProjectileCode);
					WorldProjectile target = LaunchProjectile(projectileProperty, warpPrevCasterPosition);
					projectileProperty.SetExplosionSkill(SkillId.EmmaActive4Explosion);
					projectileProperty.SetActionOnExplosion(delegate
					{
						ProjectileProperty projectileProperty2 = PopProjectileProperty(Caster,
							Singleton<EmmaSkillActive4Data>.inst.FireworkHatKnockbackProjectileCode);
						LaunchProjectile(projectileProperty2, warpPrevCasterPosition);
						foreach (SkillAgent skillAgent2 in GetEnemyCharacters(
							new CollisionCircle3D(warpPrevCasterPosition,
								projectileProperty2.ProjectileData.collisionObjectRadius)))
						{
							Vector3 vector2 = warpPrevCasterPosition +
							                  GameUtil.Direction(warpPrevCasterPosition, skillAgent2.Position) *
							                  Singleton<EmmaSkillActive4Data>.inst.KnockbackInnerRange;
							float distance = Mathf.Min(Singleton<EmmaSkillActive4Data>.inst.KnockbackDistance,
								GameUtil.Distance(vector2, skillAgent2.Position));
							Vector3 vector3 = GameUtil.Direction(skillAgent2.Position, vector2);
							if (vector3 == Vector3.zero)
							{
								vector3 = Vector3.forward;
							}

							KnockbackState knockbackState = CreateState<KnockbackState>(skillAgent2, 2000010);
							knockbackState.Init(vector3, distance,
								Singleton<EmmaSkillActive4Data>.inst.KnockbackDuration, EasingFunction.Ease.EaseOutQuad,
								false);
							skillAgent2.AddState(knockbackState, Caster.ObjectId);
						}
					});
					Caster.AttachSight(target, projectileProperty.ProjectileData.explosionRadius,
						projectileProperty.ProjectileData.lifeTimeAfterArrival, false);
					PlaySkillAction(Caster, 1, null, warpPrevCasterPosition);
					if (Target != null)
					{
						MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(Target.WorldObject);
					}

					break;
				}
				case SelectType.Rabbit:
				{
					List<SkillAgent> enemyCharacters =
						GetEnemyCharacters(new CollisionCircle3D(Target.Position, SkillInnerRange));
					if (enemyCharacters.Contains(Target))
					{
						enemyCharacters.Remove(Target);
					}

					if (enemyCharacters != null && 0 < enemyCharacters.Count)
					{
						foreach (SkillAgent skillAgent in enemyCharacters)
						{
							LaunchMagicRabbitBeamProjectile(SkillSlotIndex.Active4, skillAgent.ObjectId);
						}
					}

					break;
				}
			}

			if (0f < SkillFinishDelayTime)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish();
		}

		
		public override UseSkillErrorCode IsCanUseSkill(WorldCharacter hitTarget, Vector3? cursorPosition,
			WorldMovableCharacter caster)
		{
			if (hitTarget == null)
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			if (hitTarget.SkillAgent.AnyHaveStateByGroup(Singleton<EmmaSkillActive3Data>.inst.MagicRabbitStateGroupCode)
			)
			{
				return UseSkillErrorCode.None;
			}

			if (hitTarget.ObjectType != ObjectType.SummonServant && hitTarget.ObjectType != ObjectType.SummonTrap)
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			WorldSummonBase worldSummonBase = hitTarget as WorldSummonBase;
			if (worldSummonBase == null)
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			if (worldSummonBase.Owner.ObjectId != caster.ObjectId)
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			if (!IsPigeon(worldSummonBase) && !IsFireworkHat(worldSummonBase))
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			return UseSkillErrorCode.None;
		}

		
		private enum SelectType
		{
			
			None,

			
			Pigeon,

			
			FireworkHat,

			
			Rabbit
		}
	}
}