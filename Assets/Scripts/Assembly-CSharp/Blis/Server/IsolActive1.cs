using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.IsolActive1)]
	public class IsolActive1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D sectorAttach;

		
		private CollisionCircle3D sectorOnGround;

		
		protected override void Start()
		{
			base.Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (sectorOnGround == null)
			{
				sectorOnGround = new CollisionCircle3D(Vector3.zero, 0f);
				sectorAttach = new CollisionCircle3D(Vector3.zero, 0f);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			IsolActive1 isolActive1 = this;
			isolActive1.Start();
			if (isolActive1.SkillCastingTime1 > 0.0)
			{
				yield return isolActive1.FirstCastingTime();
			}

			Vector3 targetPosition = isolActive1.GetSkillPoint();
			Vector3 zero = Vector3.zero;
			if (MoveAgent.SamplePosition(targetPosition, isolActive1.Caster.WalkableNavMask, out zero))
			{
				targetPosition = zero;
				if (GameUtil.DistanceOnPlane(targetPosition, isolActive1.Caster.Position) >
				    (double) isolActive1.SkillRange)
				{
					MoveAgent.CanStraightMoveToDestination(isolActive1.Caster.Position, targetPosition,
						isolActive1.Caster.WalkableNavMask, out zero);
					targetPosition = zero;
				}
			}
			else
			{
				MoveAgent.CanStraightMoveToDestination(isolActive1.Caster.Position, targetPosition,
					isolActive1.Caster.WalkableNavMask, out zero);
				targetPosition = zero;
			}

			ProjectileProperty projectileProperty = isolActive1.PopProjectileProperty(isolActive1.Caster,
				Singleton<IsolSkillActive1Data>.inst.ProjectileCode);
			projectileProperty.SetTargetDirection(GameUtil.Direction(isolActive1.Caster.Position, targetPosition));
			projectileProperty.SetDistance(GameUtil.Distance(isolActive1.Caster.Position, targetPosition));
			projectileProperty.SetSpeed(projectileProperty.Distance, projectileProperty.ProjectileData.duration);
			WorldSummonBase worldSummonBase = null;
			projectileProperty.SetActionOnArrive(
				(destination, isCollision, worldProjectile) =>
				{
					WorldPlayerCharacter character = Caster.Character as WorldPlayerCharacter;
					if (!(character != null))
					{
						return;
					}

					worldSummonBase = MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(character,
						Singleton<IsolSkillActive1Data>.inst.SummonObjectCode, targetPosition);
					worldSummonBase.StartThrowingCoroutine(SummonObjectAction(worldSummonBase as WorldSummonTrap),
						exception =>
							Log.E(string.Format("[EXCEPTION][SKILL][{0}] Message:{1}, StackTrace:{2}",
								SkillId, exception.Message, exception.StackTrace)));
				});
			isolActive1.LaunchProjectile(projectileProperty);
			if (isolActive1.SkillFinishDelayTime > 0.0)
			{
				yield return isolActive1.FinishDelayTime();
			}

			isolActive1.Finish();

			// co: dotPeek
			// this.Start();
			// if (base.SkillCastingTime1 > 0f)
			// {
			// 	yield return base.FirstCastingTime();
			// }
			// Vector3 targetPosition = base.GetSkillPoint();
			// Vector3 zero = Vector3.zero;
			// if (MoveAgent.SamplePosition(targetPosition, base.Caster.WalkableNavMask, out zero))
			// {
			// 	targetPosition = zero;
			// 	if (GameUtil.DistanceOnPlane(targetPosition, base.Caster.Position) > base.SkillRange)
			// 	{
			// 		MoveAgent.CanStraightMoveToDestination(base.Caster.Position, targetPosition, base.Caster.WalkableNavMask, out zero);
			// 		targetPosition = zero;
			// 	}
			// }
			// else
			// {
			// 	MoveAgent.CanStraightMoveToDestination(base.Caster.Position, targetPosition, base.Caster.WalkableNavMask, out zero);
			// 	targetPosition = zero;
			// }
			// ProjectileProperty projectileProperty = base.PopProjectileProperty(base.Caster, Singleton<IsolSkillActive1Data>.inst.ProjectileCode);
			// projectileProperty.SetTargetDirection(GameUtil.Direction(base.Caster.Position, targetPosition));
			// projectileProperty.SetDistance(GameUtil.Distance(base.Caster.Position, targetPosition));
			// projectileProperty.SetSpeed(projectileProperty.Distance, projectileProperty.ProjectileData.duration);
			// WorldSummonBase worldSummonBase = null;
			// Action<Exception> <>9__1;
			// projectileProperty.SetActionOnArrive(delegate(Vector3 destination, bool isCollision, WorldProjectile worldProjectile)
			// {
			// 	WorldPlayerCharacter worldPlayerCharacter = this.Caster.Character as WorldPlayerCharacter;
			// 	if (worldPlayerCharacter != null)
			// 	{
			// 		MonoBehaviour worldSummonBase = MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(worldPlayerCharacter, Singleton<IsolSkillActive1Data>.inst.SummonObjectCode, targetPosition);
			// 		worldSummonBase = worldSummonBase;
			// 		IEnumerator enumerator = this.SummonObjectAction(worldSummonBase as WorldSummonTrap);
			// 		Action<Exception> done;
			// 		if ((done = <>9__1) == null)
			// 		{
			// 			done = (<>9__1 = delegate(Exception exception)
			// 			{
			// 				Log.E(string.Format("[EXCEPTION][SKILL][{0}] Message:{1}, StackTrace:{2}", this.SkillId, exception.Message, exception.StackTrace));
			// 			});
			// 		}
			// 		worldSummonBase.StartThrowingCoroutine(enumerator, done);
			// 	}
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
			float attachDetectTimer = Singleton<IsolSkillActive1Data>.inst.AttachDetectTimer;
			float timeStack = 0f;
			SkillAgent attachTargetSkillAgent = null;
			WaitForFrameUpdate waitFrame = new WaitForFrameUpdate();
			sectorOnGround.UpdatePosition(worldSummonTrap.GetPosition());
			sectorOnGround.UpdateRadius(SkillInnerRange);
			List<SkillAgent> enemyCharacters;
			for (;;)
			{
				enemyCharacters = GetEnemyCharacters(sectorOnGround);
				if (enemyCharacters.Any<SkillAgent>())
				{
					break;
				}

				timeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (attachDetectTimer <= timeStack)
				{
					goto IL_146;
				}

				yield return waitFrame.Frame(1);
			}

			attachTargetSkillAgent = enemyCharacters.NearestOne(worldSummonTrap.GetPosition());
			IL_146:
			if (attachTargetSkillAgent != null)
			{
				CharacterStateData data =
					GameDB.characterState.GetData(Singleton<IsolSkillActive1Data>.inst.AttachState);
				attachTargetSkillAgent.RemoveStateByGroup(data.group, Caster.ObjectId);
				AddState(attachTargetSkillAgent, data.code);
				MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(worldSummonTrap);
				yield break;
			}

			yield return waitFrame.Seconds(Singleton<IsolSkillActive1Data>.inst.BombTimerOnGround);
			worldSummonTrap.SetActionOnTrapBurst(delegate
			{
				sectorAttach.UpdatePosition(worldSummonTrap.GetPosition());
				sectorAttach.UpdateRadius(summonData.rangeRadius);
				List<SkillAgent> enemyCharacters2 = GetEnemyCharacters(sectorAttach);
				if (enemyCharacters2.Count == 0)
				{
					return;
				}

				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<IsolSkillActive1Data>.inst.BaseDamage[SkillLevel]);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<IsolSkillActive1Data>.inst.BaseSkillApCoef);
				DamageTo(enemyCharacters2, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 0, true, 0,
					1f, false);
				AddState(enemyCharacters2, Singleton<IsolSkillActive1Data>.inst.DebuffState);
				int stateCode =
					Singleton<IsolSkillPassiveData>.inst.InstallTrapAdditionalStateEffect[
						Caster.GetSkillLevel(SkillSlotIndex.Passive)];
				AddState(enemyCharacters2, stateCode);
			});
			worldSummonTrap.Burst(null, true);
		}
	}
}