using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ShoichiActive3)]
	public class ShoichiActive3 : ShoichiSkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			ShoichiActive3 shoichiActive3 = this;
			shoichiActive3.Start();
			if (shoichiActive3.SkillCastingTime1 > 0.0)
			{
				yield return shoichiActive3.FirstCastingTime();
			}

			Vector3 direction = GameUtil.Direction(shoichiActive3.Caster.Position, shoichiActive3.GetSkillPoint());
			ProjectileProperty daggerProjectile = shoichiActive3.PopProjectileProperty(shoichiActive3.Caster,
				Singleton<ShoichiSkillActive3Data>.inst.DaggerProjectile);
			daggerProjectile.SetTargetDirection(direction);
			daggerProjectile.SetActionOnCollisionCharacter(
				(targetAgent, attackerInfo, damagePoint,
					damageDirection) =>
				{
					int skillLevel = SkillLevel;
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<ShoichiSkillActive3Data>.inst.DamageByLevel[skillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<ShoichiSkillActive3Data>.inst.SkillApCoef);
					DamageTo(targetAgent, attackerInfo, daggerProjectile.ProjectileData.damageType,
						daggerProjectile.ProjectileData.damageSubType, 0, parameterCollection,
						daggerProjectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
					AddState(targetAgent,
						Singleton<ShoichiSkillActive3Data>.inst.DaggerDebuffStateCodes[skillLevel]);
					ProjectileProperty projectileProperty = PopProjectileProperty(Caster,
						Singleton<ShoichiSkillActive3Data>.inst.DaggerCreateEffectProjectileCode);
					projectileProperty.SetTargetDirection(daggerProjectile.Direction);
					projectileProperty.SetDeadAction(obj =>
						CreatePassiveDagger(obj.GetPosition()));
					Vector3 position = targetAgent.Position;
					Vector3 destination =
						position + projectileProperty.Direction * projectileProperty.ProjectileData.distance;
					Vector3 nearestDestination = Vector3.zero;
					if (!MoveAgent.CanStraightMoveToDestination(position, destination,
						Caster.Character.WalkableNavMask, out nearestDestination) && projectileProperty != null)
					{
						projectileProperty.SetSpeed(Vector3.Distance(position, nearestDestination),
							projectileProperty.ProjectileData.duration);
					}
					else
					{
						projectileProperty.SetSpeed(projectileProperty.ProjectileData.distance,
							projectileProperty.ProjectileData.duration);
					}

					LaunchProjectile(projectileProperty, position);
				});
			WorldProjectile worldProjectile = shoichiActive3.LaunchProjectile(daggerProjectile);
			shoichiActive3.Caster.AttachSight(worldProjectile,
				Singleton<ShoichiSkillActive3Data>.inst.ProjectileSightRange, daggerProjectile.ProjectileData.duration,
				false);
			Vector3 destination1 = shoichiActive3.Caster.Position -
			                       direction * Singleton<ShoichiSkillActive3Data>.inst.MoveDistance;
			shoichiActive3.LookAtDirection(shoichiActive3.Caster, direction);
			shoichiActive3.CasterLockRotation(true);
			shoichiActive3.Caster.MoveToDestinationForTime(destination1,
				Singleton<ShoichiSkillActive3Data>.inst.MoveDuration, EasingFunction.Ease.EaseOutQuad, false,
				out Vector3 _, out bool _, out float _);
			yield return shoichiActive3.WaitForSeconds(Singleton<ShoichiSkillActive3Data>.inst.MoveDuration);
			if (shoichiActive3.SkillFinishDelayTime > 0.0)
			{
				yield return shoichiActive3.FinishDelayTime();
			}

			shoichiActive3.Finish();

			// co: dotPeek   
			// this.Start();
			// if (base.SkillCastingTime1 > 0f)
			// {
			// 	yield return base.FirstCastingTime();
			// }
			// Vector3 vector = GameUtil.Direction(base.Caster.Position, base.GetSkillPoint());
			// ProjectileProperty daggerProjectile = base.PopProjectileProperty(base.Caster, Singleton<ShoichiSkillActive3Data>.inst.DaggerProjectile);
			// daggerProjectile.SetTargetDirection(vector);
			// Action<WorldProjectile> <>9__1;
			// daggerProjectile.SetActionOnCollisionCharacter(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			// {
			// 	int skillLevel = this.SkillLevel;
			// 	this.parameterCollection.Clear();
			// 	this.parameterCollection.Add(SkillScriptParameterType.Damage, (float)Singleton<ShoichiSkillActive3Data>.inst.DamageByLevel[skillLevel]);
			// 	this.parameterCollection.Add(SkillScriptParameterType.DamageApCoef, Singleton<ShoichiSkillActive3Data>.inst.SkillApCoef);
			// 	this.DamageTo(targetAgent, attackerInfo, daggerProjectile.ProjectileData.damageType, daggerProjectile.ProjectileData.damageSubType, 0, this.parameterCollection, daggerProjectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0, true, 0, 1f, true);
			// 	this.AddState(targetAgent, Singleton<ShoichiSkillActive3Data>.inst.DaggerDebuffStateCodes[skillLevel], null);
			// 	ProjectileProperty projectileProperty = this.PopProjectileProperty(this.Caster, Singleton<ShoichiSkillActive3Data>.inst.DaggerCreateEffectProjectileCode);
			// 	projectileProperty.SetTargetDirection(daggerProjectile.Direction);
			// 	ProjectileProperty projectileProperty2 = projectileProperty;
			// 	Action<WorldProjectile> deadAction;
			// 	if ((deadAction = <>9__1) == null)
			// 	{
			// 		deadAction = (<>9__1 = delegate(WorldProjectile obj)
			// 		{
			// 			this.CreatePassiveDagger(obj.GetPosition());
			// 		});
			// 	}
			// 	projectileProperty2.SetDeadAction(deadAction);
			// 	Vector3 position = targetAgent.Position;
			// 	Vector3 destination2 = position + projectileProperty.Direction * projectileProperty.ProjectileData.distance;
			// 	Vector3 zero = Vector3.zero;
			// 	if (!MoveAgent.CanStraightMoveToDestination(position, destination2, this.Caster.Character.WalkableNavMask, out zero) && projectileProperty != null)
			// 	{
			// 		projectileProperty.SetSpeed(Vector3.Distance(position, zero), projectileProperty.ProjectileData.duration);
			// 	}
			// 	else
			// 	{
			// 		projectileProperty.SetSpeed(projectileProperty.ProjectileData.distance, projectileProperty.ProjectileData.duration);
			// 	}
			// 	this.LaunchProjectile(projectileProperty, position);
			// });
			// WorldProjectile target = base.LaunchProjectile(daggerProjectile);
			// base.Caster.AttachSight(target, Singleton<ShoichiSkillActive3Data>.inst.ProjectileSightRange, daggerProjectile.ProjectileData.duration, false);
			// Vector3 destination = base.Caster.Position - vector * Singleton<ShoichiSkillActive3Data>.inst.MoveDistance;
			// base.LookAtDirection(base.Caster, vector, 0f, false);
			// base.CasterLockRotation(true);
			// Vector3 vector2;
			// bool flag;
			// float num;
			// base.Caster.MoveToDestinationForTime(destination, Singleton<ShoichiSkillActive3Data>.inst.MoveDuration, EasingFunction.Ease.EaseOutQuad, false, out vector2, out flag, out num, false);
			// yield return base.WaitForSeconds(Singleton<ShoichiSkillActive3Data>.inst.MoveDuration);
			// if (base.SkillFinishDelayTime > 0f)
			// {
			// 	yield return base.FinishDelayTime();
			// }
			// this.Finish(false);
			// yield break;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}