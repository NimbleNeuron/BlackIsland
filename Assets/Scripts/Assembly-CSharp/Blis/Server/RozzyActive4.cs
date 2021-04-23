using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziActive4)]
	public class RozzyActive4 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private Vector3 targetDirection = Vector3.zero;

		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			RozzyActive4 rozzyActive4 = this;
			rozzyActive4.Start();
			Vector3 targetPosition = rozzyActive4.GetSkillPoint();
			rozzyActive4.LookAtPosition(rozzyActive4.Caster, targetPosition);
			if (rozzyActive4.SkillCastingTime1 > 0.0)
			{
				yield return rozzyActive4.FirstCastingTime();
				targetPosition = rozzyActive4.GetSkillPoint();
				rozzyActive4.LookAtPosition(rozzyActive4.Caster, targetPosition);
			}

			rozzyActive4.targetDirection = GameUtil.DirectionOnPlane(rozzyActive4.Caster.Position, targetPosition);
			ProjectileProperty projectileProperty = rozzyActive4.PopProjectileProperty(rozzyActive4.Caster,
				Singleton<RozziSkillActive4Data>.inst.ProjectileCode);
			projectileProperty.SetTargetDirection(rozzyActive4.targetDirection);
			float distance = GameUtil.Distance(rozzyActive4.Caster.Position, targetPosition);
			projectileProperty.SetDistance(distance);
			projectileProperty.SetSpeed(projectileProperty.Distance, projectileProperty.ProjectileData.duration);
			projectileProperty.SetActionOnArrive(
				(destination, isCollision, worldProjectile) =>
				{
					if (worldProjectile.Property.ProjectileData.penetrationCount <= worldProjectile.CollisionCount)
					{
						return;
					}

					WorldPlayerCharacter character = Caster.Character as WorldPlayerCharacter;
					if (!(character != null))
					{
						return;
					}

					WorldSummonBase monoBehaviour = MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(character,
						Singleton<RozziSkillActive4Data>.inst.SummonObjectCode, targetPosition);
					monoBehaviour.StartThrowingCoroutine(SummonObjectAction(monoBehaviour as WorldSummonTrap),
						exception =>
							Log.E(string.Format("[EXCEPTION][SKILL][{0}] Message:{1}, StackTrace:{2}",
								SkillId, exception.Message, exception.StackTrace)));
				});
			projectileProperty.SetActionOnCollisionCharacter(
				(targetAgent, attackerInfo, damagePoint,
					damageDirection) =>
				{
					AddState(targetAgent,
						Singleton<RozziSkillActive4Data>.inst.AttachDebuffStateCodeByLevel[SkillLevel]);
					AddState(targetAgent, Singleton<RozziSkillActive4Data>.inst.AttachDebuffStackStateCode);
					PlaySkillAction(Caster, 2, targetAgent, targetDirection);
				});
			rozzyActive4.LaunchProjectile(projectileProperty);
			if (rozzyActive4.SkillFinishDelayTime > 0.0)
			{
				yield return rozzyActive4.FinishDelayTime();
			}

			rozzyActive4.Finish();

			// co: dotPeek
			// this.Start();
			// Vector3 targetPosition = base.GetSkillPoint();
			// base.LookAtPosition(base.Caster, targetPosition, 0f, false);
			// if (base.SkillCastingTime1 > 0f)
			// {
			// 	yield return base.FirstCastingTime();
			// 	targetPosition = base.GetSkillPoint();
			// 	base.LookAtPosition(base.Caster, targetPosition, 0f, false);
			// }
			// this.targetDirection = GameUtil.DirectionOnPlane(base.Caster.Position, targetPosition);
			// ProjectileProperty projectileProperty = base.PopProjectileProperty(base.Caster, Singleton<RozziSkillActive4Data>.inst.ProjectileCode);
			// projectileProperty.SetTargetDirection(this.targetDirection);
			// float distance = GameUtil.Distance(base.Caster.Position, targetPosition);
			// projectileProperty.SetDistance(distance);
			// projectileProperty.SetSpeed(projectileProperty.Distance, projectileProperty.ProjectileData.duration);
			// Action<Exception> <>9__2;
			// projectileProperty.SetActionOnArrive(delegate(Vector3 destination, bool isCollision, WorldProjectile worldProjectile)
			// {
			// 	if (worldProjectile.Property.ProjectileData.penetrationCount <= worldProjectile.CollisionCount)
			// 	{
			// 		return;
			// 	}
			// 	WorldPlayerCharacter worldPlayerCharacter = this.Caster.Character as WorldPlayerCharacter;
			// 	if (worldPlayerCharacter != null)
			// 	{
			// 		WorldSummonBase worldSummonBase = MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(worldPlayerCharacter, Singleton<RozziSkillActive4Data>.inst.SummonObjectCode, targetPosition);
			// 		MonoBehaviour monoBehaviour = worldSummonBase;
			// 		IEnumerator enumerator = this.SummonObjectAction(worldSummonBase as WorldSummonTrap);
			// 		Action<Exception> done;
			// 		if ((done = <>9__2) == null)
			// 		{
			// 			done = (<>9__2 = delegate(Exception exception)
			// 			{
			// 				Log.E(string.Format("[EXCEPTION][SKILL][{0}] Message:{1}, StackTrace:{2}", this.SkillId, exception.Message, exception.StackTrace));
			// 			});
			// 		}
			// 		monoBehaviour.StartThrowingCoroutine(enumerator, done);
			// 	}
			// });
			// projectileProperty.SetActionOnCollisionCharacter(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			// {
			// 	this.AddState(targetAgent, Singleton<RozziSkillActive4Data>.inst.AttachDebuffStateCodeByLevel[this.SkillLevel], null);
			// 	this.AddState(targetAgent, Singleton<RozziSkillActive4Data>.inst.AttachDebuffStackStateCode, null);
			// 	this.PlaySkillAction(this.Caster, 2, targetAgent, new Vector3?(this.targetDirection));
			// });
			// base.LaunchProjectile(projectileProperty);
			// if (base.SkillFinishDelayTime > 0f)
			// {
			// 	yield return base.FinishDelayTime();
			// }
			// this.Finish(false);
			// yield break;
		}

		
		private IEnumerator SummonObjectAction(WorldSummonTrap worldSummonTrap)
		{
			SummonData summonData = worldSummonTrap.SummonData;
			WaitForFrameUpdate waitForFrameUpdate = new WaitForFrameUpdate();
			PlaySkillAction(Caster, SkillId.RozziActive4, 1, worldSummonTrap.ObjectId, new BlisVector(targetDirection));
			if (Singleton<RozziSkillActive4Data>.inst.BombTimerOnGround > 0f)
			{
				yield return waitForFrameUpdate.Seconds(Singleton<RozziSkillActive4Data>.inst.BombTimerOnGround);
			}

			worldSummonTrap.SetActionOnTrapBurst(delegate
			{
				CollisionCircle3D collisionObject =
					new CollisionCircle3D(worldSummonTrap.GetPosition(), summonData.rangeRadius);
				List<SkillAgent> enemyCharacters = GetEnemyCharacters(collisionObject);
				if (enemyCharacters.Count == 0)
				{
					return;
				}

				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<RozziSkillActive4Data>.inst.DamageActive4ByLevel[SkillLevel]);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<RozziSkillActive4Data>.inst.DamageActive4ApCoef);
				DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 0);
			});
			worldSummonTrap.Burst(null, true);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}