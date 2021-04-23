using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ShoichiActive4)]
	public class ShoichiActive4 : ShoichiSkillScript
	{
		
		private readonly HashSet<SkillAgent> collisionTarget = new HashSet<SkillAgent>();

		
		private readonly SkillScriptParameterCollection daggerParameterCollection =
			SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D sector;

		
		protected override void Start()
		{
			base.Start();
			collisionTarget.Clear();
			if (sector == null)
			{
				sector = new CollisionCircle3D(Caster.Position, info.SkillInnerRange);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			int skillLevel = SkillLevel;
			sector.UpdatePosition(Caster.Position);
			sector.UpdateRadius(info.SkillInnerRange);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
			if (enemyCharacters.Count > 0)
			{
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<ShoichiSkillActive4Data>.inst.SkillApCoef);
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<ShoichiSkillActive4Data>.inst.DamageByLevel[skillLevel]);
				DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
					Singleton<ShoichiSkillActive4Data>.inst.EffectAndSoundCode);
				AddState(enemyCharacters, Singleton<ShoichiSkillActive4Data>.inst.DebuffStateCode);
			}

			int num3;
			for (int Index = 0; Index < Singleton<ShoichiSkillActive4Data>.inst.DaggerAngles.Count; Index = num3)
			{
				Vector3 vector = Singleton<ShoichiSkillActive4Data>.inst.DaggerAngles[Index] * Vector3.forward;
				ProjectileProperty projectileProperty =
					PopProjectileProperty(Caster, Singleton<ShoichiSkillActive4Data>.inst.DaggerProjectile);
				projectileProperty.SetTargetDirection(vector);
				projectileProperty.SetActionOnCollisionCharacter(
					delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
						Vector3 damageDirection)
					{
						if (collisionTarget.Contains(targetAgent))
						{
							return;
						}

						daggerParameterCollection.Clear();
						daggerParameterCollection.Add(SkillScriptParameterType.Damage,
							Singleton<ShoichiSkillActive4Data>.inst.DaggerDamageByLevel[SkillLevel]);
						daggerParameterCollection.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<ShoichiSkillActive4Data>.inst.SkillApCoef);
						DamageTo(targetAgent, attackerInfo, DamageType.Skill, DamageSubType.Normal, 0,
							daggerParameterCollection, SkillSlotSet.Active4_1, damagePoint, damageDirection, 0);
						collisionTarget.Add(targetAgent);
					});
				projectileProperty.SetDeadAction(delegate(WorldProjectile obj)
				{
					CreatePassiveDagger(obj.Property.Distance == 0f ? Caster.Position : obj.GetPosition());
				});
				float num = projectileProperty.ProjectileData.distance -
				            projectileProperty.ProjectileData.serverInterpolationPosition;
				Vector3 vector2 = Caster.Position +
				                  vector * projectileProperty.ProjectileData.serverInterpolationPosition;
				Vector3 vector3 = vector2 + vector * num;
				Vector3 vector4;
				Vector3 destination;
				if (!MoveAgent.CanStraightMoveToDestination(vector2, vector3, Caster.Character.WalkableNavMask,
					    out vector4) &&
				    MoveAgent.SamplePosition(vector3, Caster.Character.WalkableNavMask, out destination))
				{
					MoveAgent.CanStraightMoveToDestination(vector2, destination, Caster.Character.WalkableNavMask,
						out vector4);
				}

				num = GameUtil.Distance(vector2, vector4);
				float num2 = num / projectileProperty.ProjectileData.distance;
				if (num <= Singleton<ShoichiSkillActive4Data>.inst.DaggerMinRange)
				{
					CreatePassiveDagger(vector4);
				}
				else
				{
					projectileProperty.SetSpeed(num, projectileProperty.ProjectileData.duration * num2);
					LaunchProjectile(projectileProperty);
				}

				PlaySkillAction(Caster, Index + 1);
				yield return WaitForSeconds(Singleton<ShoichiSkillActive4Data>.inst.LaunchDelay);
				num3 = Index + 1;
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}