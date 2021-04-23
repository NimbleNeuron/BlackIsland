using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ChiaraActive1)]
	public class ChiaraActive1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damage_1 = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection damage_2 = SkillScriptParameterCollection.Create();

		
		private bool canMoveDuringSkillPlaying = true;

		
		private CollisionSector3D collisionSector;

		
		private float concentrationStartTime;

		
		private CollisionBox3D groundCollision;

		
		private Vector3 hitPoint;

		
		private bool isPlayAgain;

		
		private SkillType skillType;

		
		
		public override bool CanMoveDuringSkillPlaying => canMoveDuringSkillPlaying;

		
		protected override void Start()
		{
			base.Start();
			isPlayAgain = false;
			skillType = SkillType.Invalid;
			if (collisionSector == null)
			{
				collisionSector = new CollisionSector3D(Vector3.zero, SkillRange, SkillAngle, Vector3.zero);
				groundCollision = new CollisionBox3D(Caster.Position, SkillRange, SkillAngle, Vector3.zero);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (skillType == SkillType.SectorAttack)
			{
				ModifySkillCooldown(Caster, SkillSlotSet.Active1_1,
					Caster.GetSkillCooldown(SkillSlotSet.Active1_1) *
					Singleton<ChiaraSkillData>.inst.A1SkillActiveCooldownModify_1);
			}
			else if (skillType == SkillType.ConcentrationTimeOver)
			{
				ModifySkillCooldown(Caster, SkillSlotSet.Active1_1,
					Caster.GetSkillCooldown(SkillSlotSet.Active1_1) *
					Singleton<ChiaraSkillData>.inst.A1TimeOverCooldownModify);
				SpHealTo(Caster, 0, 0f, (int) (SkillCost * Singleton<ChiaraSkillData>.inst.A1TimeOverRecoverySpRatio),
					true, 0);
			}

			canMoveDuringSkillPlaying = true;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			concentrationStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			float concentrationEndTime =
				MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime + SkillConcentrationTime;
			while (!isPlayAgain &&
			       concentrationEndTime > MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
			{
				yield return WaitForFrame();
			}

			FinishConcentration(false);
			canMoveDuringSkillPlaying = false;
			Caster.StopMove();
			if (isPlayAgain)
			{
				Vector3 direction = GameUtil.Direction(Caster.Position, hitPoint);
				LookAtDirection(Caster, direction);
				if (concentrationStartTime + Singleton<ChiaraSkillData>.inst.A1SkillChangeTime >
				    MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
				{
					skillType = SkillType.SectorAttack;
					PlaySkillAction(Caster, 11);
					if (Singleton<ChiaraSkillData>.inst.A1PlayAgainCastTime_1 > 0f)
					{
						yield return WaitForSeconds(Singleton<ChiaraSkillData>.inst.A1PlayAgainCastTime_1);
					}

					SkillAction_1(direction);
				}
				else
				{
					skillType = SkillType.ProjectileFire;
					PlaySkillAction(Caster, 12);
					if (Singleton<ChiaraSkillData>.inst.A1PlayAgainCastTime_2 > 0f)
					{
						yield return WaitForSeconds(Singleton<ChiaraSkillData>.inst.A1PlayAgainCastTime_2);
						direction = GameUtil.Direction(Caster.Position, hitPoint);
						LookAtDirection(Caster, direction);
					}

					SkillAction_2(direction);
				}

				direction = default;
			}
			else
			{
				skillType = SkillType.ConcentrationTimeOver;
				PlaySkillAction(Caster, 13);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			this.hitPoint = hitPoint;
			isPlayAgain = true;
		}

		
		private void SkillAction_1(Vector3 direction)
		{
			collisionSector.UpdatePosition(Caster.Position);
			collisionSector.UpdateNormalized(direction);
			collisionSector.UpdateAngle(SkillAngle);
			collisionSector.UpdateRadius(SkillRange);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collisionSector);
			damage_1.Clear();
			damage_1.Add(SkillScriptParameterType.Damage, Singleton<ChiaraSkillData>.inst.A1BaseDamage_1[SkillLevel]);
			damage_1.Add(SkillScriptParameterType.DamageApCoef, Singleton<ChiaraSkillData>.inst.A1ApDamage_1);
			DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, damage_1,
				Singleton<ChiaraSkillData>.inst.A1EffectSoundCode_1);
			float timeStack = Singleton<ChiaraSkillData>.inst.A1DebuffGroundProjectileRefreshPeriod;
			ProjectileProperty projectileProperty =
				PopProjectileProperty(Caster, Singleton<ChiaraSkillData>.inst.A1GroundProjectileCode_1);
			projectileProperty.SetUpdateAction(delegate
			{
				timeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (timeStack >= Singleton<ChiaraSkillData>.inst.A1DebuffGroundProjectileRefreshPeriod)
				{
					timeStack -= Singleton<ChiaraSkillData>.inst.A1DebuffGroundProjectileRefreshPeriod;
					List<SkillAgent> enemyCharacters2 = GetEnemyCharacters(collisionSector);
					AddState(enemyCharacters2, Singleton<ChiaraSkillData>.inst.A1GroundDebuffStateCode);
				}
			});
			projectileProperty.SetDeadAction(delegate
			{
				List<SkillAgent> enemyCharacters2 = GetEnemyCharacters(collisionSector);
				AddState(enemyCharacters2, Singleton<ChiaraSkillData>.inst.A1GroundDebuffStateCode);
			});
			projectileProperty.SetTargetDirection(direction);
			LaunchProjectile(projectileProperty);
		}

		
		private void SkillAction_2(Vector3 direction)
		{
			Vector3 projectileSpawnPos = Caster.Position;
			ProjectileProperty projectileProperty =
				PopProjectileProperty(Caster, Singleton<ChiaraSkillData>.inst.A1ProjectileCode_2);
			projectileProperty.SetTargetDirection(direction);
			projectileProperty.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					damage_2.Clear();
					damage_2.Add(SkillScriptParameterType.Damage,
						Singleton<ChiaraSkillData>.inst.A1BaseDamage_2[SkillLevel]);
					damage_2.Add(SkillScriptParameterType.DamageApCoef, Singleton<ChiaraSkillData>.inst.A1ApDamage_2);
					DamageTo(targetAgent, attackerInfo, DamageType.Skill, DamageSubType.Normal, 0, damage_2,
						SkillSlotSet.Active1_1, damagePoint, damageDirection,
						Singleton<ChiaraSkillData>.inst.A1EffectSoundCode_2);
				});
			WorldProjectile bulletProjectile = LaunchProjectile(projectileProperty, projectileSpawnPos);
			ProjectileProperty projectileProperty2 =
				PopProjectileProperty(Caster, Singleton<ChiaraSkillData>.inst.A1GroundProjectileCode_2);
			projectileProperty2.SetTargetDirection(direction);
			float collisionAreaReduceStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime +
			                                     Singleton<ChiaraSkillData>.inst.A1GroundProjectileDuration_2;
			float groundProjectileDestroyTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime +
			                                    projectileProperty.Duration + Singleton<ChiaraSkillData>.inst
				                                    .A1GroundProjectileDuration_2;
			Vector3 startPivot = projectileSpawnPos;
			Vector3 endPivot = projectileSpawnPos;
			groundCollision.UpdatePosition(projectileSpawnPos);
			groundCollision.UpdateWidth(Singleton<ChiaraSkillData>.inst.A1GroundProjectileWidth_2);
			groundCollision.UpdateDepth(0f);
			groundCollision.UpdateNormalized(direction);
			float speed = projectileProperty.ProjectileSpeed;
			if (speed == 0f)
			{
				speed = projectileProperty.Distance / projectileProperty.Duration;
			}

			float timeStack = Singleton<ChiaraSkillData>.inst.A1DebuffGroundProjectileRefreshPeriod;
			projectileProperty2.SetUpdateAction(delegate(WorldProjectile worldProjectile)
			{
				if (groundProjectileDestroyTime <= MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
				{
					worldProjectile.DestroySelf();
					return;
				}

				timeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				float num = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime -
				            collisionAreaReduceStartTime;
				if (0f < num)
				{
					startPivot = projectileSpawnPos + direction * num * speed;
				}

				if (bulletProjectile != null && bulletProjectile.IsAlive)
				{
					endPivot = bulletProjectile.GetPosition();
				}

				if (timeStack >= Singleton<ChiaraSkillData>.inst.A1DebuffGroundProjectileRefreshPeriod)
				{
					timeStack -= Singleton<ChiaraSkillData>.inst.A1DebuffGroundProjectileRefreshPeriod;
					groundCollision.UpdatePosition((startPivot + endPivot) * 0.5f);
					groundCollision.UpdateDepth(GameUtil.DistanceOnPlane(startPivot, endPivot));
					List<SkillAgent> enemyCharacters = GetEnemyCharacters(groundCollision);
					AddState(enemyCharacters, Singleton<ChiaraSkillData>.inst.A1GroundDebuffStateCode);
				}
			});
			LaunchProjectile(projectileProperty2, projectileSpawnPos);
		}

		
		private enum SkillType
		{
			
			Invalid,

			
			ConcentrationTimeOver,

			
			ProjectileFire,

			
			SectorAttack
		}
	}
}