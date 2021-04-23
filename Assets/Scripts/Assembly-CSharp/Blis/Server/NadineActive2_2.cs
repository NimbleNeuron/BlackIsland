using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.NadineActive2_2)]
	public class NadineActive2_2 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			Vector3 targetPosition = GetSkillPoint();
			Vector3 spwanPosition = targetPosition;
			int summonCode1 = Singleton<NadineSkillActive2Data>.inst.SummonCode1;
			SummonData summonData2 = GameDB.character.GetSummonData(Singleton<NadineSkillActive2Data>.inst.SummonCode2);
			WorldSummonTrap worldSummonTrap = (Caster.Character as WorldPlayerCharacter).GetOwnSummon(
				delegate(WorldSummonBase worldSummonBase)
				{
					if (worldSummonBase.Owner == null)
					{
						return false;
					}

					if (worldSummonBase.Owner.ObjectId != Caster.ObjectId)
					{
						return false;
					}

					if (worldSummonBase.SummonData.code != summonCode1)
					{
						return false;
					}

					if (summonData2.radius < GameUtil.DistanceOnPlane(worldSummonBase.GetPosition(), targetPosition))
					{
						return false;
					}

					WorldSummonTrap worldSummonTrap2 = worldSummonBase as WorldSummonTrap;
					return !(worldSummonTrap2 == null) && !worldSummonTrap2.HasLifeLink();
				}) as WorldSummonTrap;
			if (worldSummonTrap != null)
			{
				Vector3 vector = GameUtil.DirectionOnPlane(worldSummonTrap.GetPosition(), targetPosition);
				if (vector == Vector3.zero)
				{
					vector = Vector3.forward;
				}

				spwanPosition = worldSummonTrap.ColliderAgent.ClosestPointOnBounds(worldSummonTrap.GetPosition() +
					vector * (worldSummonTrap.Stat.Radius * 2f));
			}

			Vector3 vector2;
			if (!MoveAgent.CanStandToPosition(targetPosition, Caster.WalkableNavMask, out vector2))
			{
				Vector3 targetPosition2;
				MoveAgent.CanStraightMoveToDestination(Caster.Position, targetPosition, Caster.WalkableNavMask,
					out targetPosition2);
				targetPosition = targetPosition2;
			}

			if (!MoveAgent.CanStandToPosition(spwanPosition, Caster.WalkableNavMask, out vector2))
			{
				Vector3 spwanPosition2;
				MoveAgent.CanStraightMoveToDestination(Caster.Position, spwanPosition, Caster.WalkableNavMask,
					out spwanPosition2);
				spwanPosition = spwanPosition2;
			}

			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<NadineSkillActive2Data>.inst.ProjectileCode2);
			ProjectileProperty projectile3 = projectile;
			vector2 = targetPosition - Caster.Position;
			projectile3.SetTargetDirection(vector2.normalized);
			ProjectileProperty projectile2 = projectile;
			vector2 = targetPosition - Caster.Position;
			projectile2.SetDistance(vector2.magnitude);
			projectile.SetSpeed(projectile.Distance, projectile.ProjectileData.duration);
			projectile.SetActionOnExplosion(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo,
				Vector3 damagePoint, Vector3 damageDirection)
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
			projectile.SetActionOnArrive(delegate
			{
				WorldPlayerCharacter worldPlayerCharacter = Caster.Character as WorldPlayerCharacter;
				if (worldPlayerCharacter != null)
				{
					WorldSummonTrap worldSummon =
						MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(worldPlayerCharacter,
							summonData2.code, spwanPosition) as WorldSummonTrap;
					WorldSummonTrap worldSummonTrap2 = Caster.FindLifeLinkPairSummonObject(worldSummon, summonCode1,
						summonData2.radius, Singleton<NadineSkillActive2Data>.inst.MaxLinkRange);
					if (worldSummonTrap2 != null)
					{
						worldSummon.LifeLink(worldSummonTrap2);
						worldSummonTrap2.LifeLink(worldSummon);
						worldSummon.ResetDuration(worldSummon.SummonData.duration);
						worldSummonTrap2.ResetDuration(worldSummon.SummonData.duration);
						worldSummon.InstallRopeTrap(worldSummonTrap2, SkillId);
						worldSummon.SetActionOnRopeBurst(
							delegate(List<WorldCharacter> pTargets, WorldSummonBase pWorldSummon)
							{
								foreach (WorldCharacter worldCharacter in pTargets)
								{
									parameterCollection.Clear();
									parameterCollection.Add(SkillScriptParameterType.Damage,
										Singleton<NadineSkillActive2Data>.inst.DamageByLevel_2[SkillLevel]);
									parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
										Singleton<NadineSkillActive2Data>.inst.SkillApCoef);
									DamageTo(worldCharacter.SkillAgent, worldSummon.AttackerInfo, DamageType.Skill,
										DamageSubType.Trap, 0, parameterCollection, info.skillSlotSet,
										worldCharacter.GetPosition(),
										GameUtil.Direction(pWorldSummon.GetPosition(), worldCharacter.GetPosition()),
										0);
									Caster.AttachSight(worldCharacter,
										worldCharacter.Stat.GetValue(StatType.SightRange),
										Singleton<NadineSkillActive2Data>.inst.GainSightDuration, false);
									AddState(worldCharacter.SkillAgent,
										Singleton<NadineSkillActive2Data>.inst.DebuffState);
								}
							});
						PlaySkillAction(Caster, info.skillData.PassiveSkillId, 2, worldSummonTrap2.ObjectId);
						return;
					}

					worldSummon.Dead(DamageType.Normal);
				}
			});
			LaunchProjectile(projectile);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}