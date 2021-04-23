using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.NadineActive3_1)]
	public class NadineActive3_1 : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<NadineSkillActive3Data>.inst.BuffState2[SkillLevel]);
			if (Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
			{
				Caster.OverwriteState(data.code, Caster.ObjectId);
				return;
			}

			AddState(Caster, data.code);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			NadineActive3_1 nadineActive31 = this;
			nadineActive31.Start();
			nadineActive31.LookAtPosition(nadineActive31.Caster, nadineActive31.info.cursorPosition);
			if (nadineActive31.SkillCastingTime1 > 0.0)
			{
				yield return nadineActive31.FirstCastingTime();
			}

			Vector3 targetPosition = nadineActive31.GetSkillPoint();
			if (!MoveAgent.CanStandToPosition(targetPosition, nadineActive31.Caster.WalkableNavMask, out Vector3 _))
			{
				Vector3 nearestDestination;
				MoveAgent.CanStraightMoveToDestination(nadineActive31.Caster.Position, targetPosition,
					nadineActive31.Caster.WalkableNavMask, out nearestDestination);
				targetPosition = nearestDestination;
			}

			int projectileCode = Singleton<NadineSkillActive3Data>.inst.ProjectileCode;
			int summonCode = Singleton<NadineSkillActive3Data>.inst.SummonCode;
			int buffStateCode = Singleton<NadineSkillActive3Data>.inst.BuffState1[nadineActive31.SkillLevel];
			WorldSummonBase worldSummonBase = null;
			ProjectileProperty projectileProperty =
				nadineActive31.PopProjectileProperty(nadineActive31.Caster, projectileCode);
			projectileProperty.SetTargetDirection((targetPosition - nadineActive31.Caster.Position).normalized);
			projectileProperty.SetDistance(Mathf.Min(
				GameUtil.DistanceOnPlane(nadineActive31.Caster.Position, targetPosition), nadineActive31.SkillRange));
			projectileProperty.SetActionOnArrive(
				(destination, isCollision, worldProjectile) =>
				{
					WorldPlayerCharacter player = Caster.Character as WorldPlayerCharacter;
					if (!(player != null))
					{
						return;
					}

					worldSummonBase =
						MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(player, summonCode, targetPosition);
					worldSummonBase.SetCustomAction(wsb =>
						{
							if (!player.CharacterSkill.IsFirstSequence(SkillSlotSet))
							{
								player.SequenceTimeOver(SkillSlotSet, player.GetEquipWeaponMasteryType());
							}

							worldSummonBase.Dead(DamageType.Normal);
							worldSummonBase.SetCustomAction(null,
								null);
						},
						wsb =>
							(double) Singleton<NadineSkillActive3Data>.inst.MaxLineConnectionRange <
							(double) GameUtil.DistanceOnPlane(wsb.GetPosition(), Caster.Position));
					worldSummonBase.DeadAction(pWorldSummon =>
					{
						PlaySkillAction(Caster, info.skillData.PassiveSkillId, 2);
						CharacterStateData data = GameDB.characterState.GetData(buffStateCode);
						if (player.SkillAgent.IsHaveStateByGroup(data.group, Caster.ObjectId))
						{
							player.SkillAgent.OverwriteState(data.code, Caster.ObjectId);
						}
						else
						{
							AddState(player.SkillAgent, data.code);
						}
					});
					PlaySkillAction(Caster, info.skillData.PassiveSkillId, 1, worldSummonBase.ObjectId);
				});
			WorldProjectile worldProjectile1 = nadineActive31.LaunchProjectile(projectileProperty);
			nadineActive31.PlaySkillAction(nadineActive31.Caster, nadineActive31.info.skillData.PassiveSkillId, 1,
				worldProjectile1.ObjectId);
			while (worldSummonBase == null)
			{
				yield return nadineActive31.WaitForFrame();
			}

			if (nadineActive31.SkillFinishDelayTime > 0.0)
			{
				yield return nadineActive31.FinishDelayTime();
			}

			nadineActive31.Finish();

			// co: dotPeek
			// this.Start();
			// base.LookAtPosition(base.Caster, this.info.cursorPosition, 0f, false);
			// if (base.SkillCastingTime1 > 0f)
			// {
			// 	yield return base.FirstCastingTime();
			// }
			// Vector3 targetPosition = base.GetSkillPoint();
			// Vector3 vector;
			// if (!MoveAgent.CanStandToPosition(targetPosition, base.Caster.WalkableNavMask, out vector))
			// {
			// 	Vector3 targetPosition2;
			// 	MoveAgent.CanStraightMoveToDestination(base.Caster.Position, targetPosition, base.Caster.WalkableNavMask, out targetPosition2);
			// 	targetPosition = targetPosition2;
			// }
			// int projectileCode = Singleton<NadineSkillActive3Data>.inst.ProjectileCode;
			// int summonCode = Singleton<NadineSkillActive3Data>.inst.SummonCode;
			// int buffStateCode = Singleton<NadineSkillActive3Data>.inst.BuffState1[base.SkillLevel];
			// WorldSummonBase worldSummonBase = null;
			// ProjectileProperty projectileProperty = base.PopProjectileProperty(base.Caster, projectileCode);
			// projectileProperty.SetTargetDirection((targetPosition - base.Caster.Position).normalized);
			// projectileProperty.SetDistance(Mathf.Min(GameUtil.DistanceOnPlane(base.Caster.Position, targetPosition), base.SkillRange));
			// Func<WorldSummonBase, bool> <>9__2;
			// projectileProperty.SetActionOnArrive(delegate(Vector3 destination, bool isCollision, WorldProjectile worldProjectile)
			// {
			// 	WorldPlayerCharacter player = this.Caster.Character as WorldPlayerCharacter;
			// 	if (player != null)
			// 	{
			// 		WorldSummonBase worldSummonBase = MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(player, summonCode, targetPosition);
			// 		worldSummonBase = worldSummonBase;
			// 		Action<WorldSummonBase> customAction = delegate(WorldSummonBase wsb)
			// 		{
			// 			if (!player.CharacterSkill.IsFirstSequence(this.SkillSlotSet))
			// 			{
			// 				player.SequenceTimeOver(this.SkillSlotSet, player.GetEquipWeaponMasteryType());
			// 			}
			// 			worldSummonBase.Dead(DamageType.Normal);
			// 			worldSummonBase.SetCustomAction(null, null);
			// 		};
			// 		Func<WorldSummonBase, bool> customActionCondition;
			// 		if ((customActionCondition = <>9__2) == null)
			// 		{
			// 			customActionCondition = (<>9__2 = ((WorldSummonBase wsb) => Singleton<NadineSkillActive3Data>.inst.MaxLineConnectionRange < GameUtil.DistanceOnPlane(wsb.GetPosition(), this.Caster.Position)));
			// 		}
			// 		worldSummonBase.SetCustomAction(customAction, customActionCondition);
			// 		worldSummonBase.DeadAction(delegate(WorldSummonBase pWorldSummon)
			// 		{
			// 			this.PlaySkillAction(this.Caster, this.info.skillData.PassiveSkillId, 2, 0, null);
			// 			CharacterStateData data = GameDB.characterState.GetData(buffStateCode);
			// 			if (player.SkillAgent.IsHaveStateByGroup(data.group, this.Caster.ObjectId))
			// 			{
			// 				player.SkillAgent.OverwriteState(data.code, this.Caster.ObjectId);
			// 				return;
			// 			}
			// 			this.AddState(player.SkillAgent, data.code, null);
			// 		});
			// 		this.PlaySkillAction(this.Caster, this.info.skillData.PassiveSkillId, 1, worldSummonBase.ObjectId, null);
			// 	}
			// });
			// WorldProjectile worldProjectile2 = base.LaunchProjectile(projectileProperty);
			// base.PlaySkillAction(base.Caster, this.info.skillData.PassiveSkillId, 1, worldProjectile2.ObjectId, null);
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