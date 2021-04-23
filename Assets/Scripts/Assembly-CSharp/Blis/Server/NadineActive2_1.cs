using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.NadineActive2_1)]
	public class NadineActive2_1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			NadineActive2_1 nadineActive21 = this;
			nadineActive21.Start();
			nadineActive21.LookAtPosition(nadineActive21.Caster, nadineActive21.info.cursorPosition);
			if (nadineActive21.SkillCastingTime1 > 0.0)
			{
				yield return nadineActive21.FirstCastingTime();
			}

			Vector3 vector3 = nadineActive21.GetSkillPoint();
			Vector3 sampledPosition;
			if (!MoveAgent.CanStandToPosition(vector3, nadineActive21.Caster.WalkableNavMask, out sampledPosition))
			{
				Vector3 nearestDestination;
				MoveAgent.CanStraightMoveToDestination(nadineActive21.Caster.Position, vector3,
					nadineActive21.Caster.WalkableNavMask, out nearestDestination);
				vector3 = nearestDestination;
			}

			ProjectileProperty projectile = nadineActive21.PopProjectileProperty(nadineActive21.Caster,
				Singleton<NadineSkillActive2Data>.inst.ProjectileCode1);
			ProjectileProperty projectileProperty1 = projectile;
			sampledPosition = vector3 - nadineActive21.Caster.Position;
			Vector3 normalized = sampledPosition.normalized;
			projectileProperty1.SetTargetDirection(normalized);
			ProjectileProperty projectileProperty2 = projectile;
			sampledPosition = vector3 - nadineActive21.Caster.Position;
			double magnitude = sampledPosition.magnitude;
			projectileProperty2.SetDistance((float) magnitude);
			projectile.SetSpeed(projectile.Distance, projectile.ProjectileData.duration);
			projectile.SetActionOnExplosion(
				(targetAgent, attackerInfo, damagePoint,
					damageDirection) =>
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<NadineSkillActive2Data>.inst.DamageByLevel[SkillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<NadineSkillActive2Data>.inst.SkillApCoef);
					DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
				});
			WorldSummonBase worldSummonBase = null;
			projectile.SetActionOnArrive(
				(destination, isCollision, worldProjectile) =>
				{
					WorldPlayerCharacter character = Caster.Character as WorldPlayerCharacter;
					if (!(character != null))
					{
						return;
					}

					worldSummonBase = MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(character,
						Singleton<NadineSkillActive2Data>.inst.SummonCode1, destination);
					worldSummonBase.DeadAction(pWorldSummon =>
						PlaySkillAction(Caster, info.skillData.PassiveSkillId, 2,
							worldSummonBase.ObjectId));
					Caster.PushLifeLinkPool(worldSummonBase as WorldSummonTrap);
					PlaySkillAction(Caster, info.skillData.PassiveSkillId, 1, worldSummonBase.ObjectId);
				});
			nadineActive21.LaunchProjectile(projectile);
			while (worldSummonBase == null)
			{
				yield return nadineActive21.WaitForFrame();
			}

			if (nadineActive21.SkillFinishDelayTime > 0.0)
			{
				yield return nadineActive21.FinishDelayTime();
			}

			nadineActive21.Finish();

			// co: dotPeek  
			// this.Start();
			// base.LookAtPosition(base.Caster, this.info.cursorPosition, 0f, false);
			// if (base.SkillCastingTime1 > 0f)
			// {
			// 	yield return base.FirstCastingTime();
			// }
			// Vector3 vector = base.GetSkillPoint();
			// Vector3 vector2;
			// if (!MoveAgent.CanStandToPosition(vector, base.Caster.WalkableNavMask, out vector2))
			// {
			// 	Vector3 vector3;
			// 	MoveAgent.CanStraightMoveToDestination(base.Caster.Position, vector, base.Caster.WalkableNavMask, out vector3);
			// 	vector = vector3;
			// }
			// ProjectileProperty projectile = base.PopProjectileProperty(base.Caster, Singleton<NadineSkillActive2Data>.inst.ProjectileCode1);
			// ProjectileProperty projectile3 = projectile;
			// vector2 = vector - base.Caster.Position;
			// projectile3.SetTargetDirection(vector2.normalized);
			// ProjectileProperty projectile2 = projectile;
			// vector2 = vector - base.Caster.Position;
			// projectile2.SetDistance(vector2.magnitude);
			// projectile.SetSpeed(projectile.Distance, projectile.ProjectileData.duration);
			// projectile.SetActionOnExplosion(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			// {
			// 	this.parameterCollection.Clear();
			// 	this.parameterCollection.Add(SkillScriptParameterType.Damage, (float)Singleton<NadineSkillActive2Data>.inst.DamageByLevel[this.SkillLevel]);
			// 	this.parameterCollection.Add(SkillScriptParameterType.DamageApCoef, Singleton<NadineSkillActive2Data>.inst.SkillApCoef);
			// 	this.DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType, projectile.ProjectileData.damageSubType, 0, this.parameterCollection, projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0, true, 0, 1f, true);
			// });
			// WorldSummonBase worldSummonBase = null;
			// Action<WorldSummonBase> <>9__2;
			// projectile.SetActionOnArrive(delegate(Vector3 destination, bool isCollision, WorldProjectile worldProjectile)
			// {
			// 	WorldPlayerCharacter worldPlayerCharacter = this.Caster.Character as WorldPlayerCharacter;
			// 	if (worldPlayerCharacter != null)
			// 	{
			// 		WorldSummonBase worldSummonBase = MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(worldPlayerCharacter, Singleton<NadineSkillActive2Data>.inst.SummonCode1, destination);
			// 		worldSummonBase = worldSummonBase;
			// 		Action<WorldSummonBase> action;
			// 		if ((action = <>9__2) == null)
			// 		{
			// 			action = (<>9__2 = delegate(WorldSummonBase pWorldSummon)
			// 			{
			// 				this.PlaySkillAction(this.Caster, this.info.skillData.PassiveSkillId, 2, worldSummonBase.ObjectId, null);
			// 			});
			// 		}
			// 		worldSummonBase.DeadAction(action);
			// 		this.Caster.PushLifeLinkPool(worldSummonBase as WorldSummonTrap);
			// 		this.PlaySkillAction(this.Caster, this.info.skillData.PassiveSkillId, 1, worldSummonBase.ObjectId, null);
			// 	}
			// });
			// base.LaunchProjectile(projectile);
			// while (worldSummonBase == null)
			// {
			// 	yield return base.WaitForFrame();
			// }
			// if (base.SkillFinishDelayTime > 0f)
			// {
			// 	yield return base.FinishDelayTime();
			// }
			// this.Finish(false);
			// yield break;
		}
	}
}