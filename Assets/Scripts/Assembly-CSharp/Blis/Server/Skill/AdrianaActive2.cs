using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AdrianaActive2)]
	public class AdrianaActive2 : AdrianaSkillScript
	{
		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (0f < SkillCastingTime1)
			{
				yield return FirstCastingTime();
			}

			Vector3 skillPoint = GetSkillPoint();
			if (!GameUtil.ApproximatelyToPlane(Caster.Position, skillPoint, 0.01f))
			{
				LookAtPosition(Caster, skillPoint);
			}

			ProjectileProperty projectileProperty =
				PopProjectileProperty(Caster, Singleton<AdrianaSkillActive2Data>.inst.OilThrowProjectileCode);
			projectileProperty.SetTargetDirection(GameUtil.Direction(Caster.Position, skillPoint));
			projectileProperty.SetDistance(GameUtil.Distance(Caster.Position, skillPoint));
			projectileProperty.SetSpeed(projectileProperty.Distance, projectileProperty.ProjectileData.duration);
			projectileProperty.SetActionOnArrive(
				delegate(Vector3 destination, bool isCollision, WorldProjectile worldProjectile)
				{
					worldProjectile.StartCoroutine(
						SetOilAreaProjectile(skillPoint, GetSkillPoint(info.releasePosition)));
				});
			LaunchProjectile(projectileProperty);
			if (0f < SkillFinishDelayTime)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		private IEnumerator SetOilAreaProjectile(Vector3 cursorPosition, Vector3 releasePosition)
		{
			Vector3 skillDirection = !GameUtil.Approximately(cursorPosition, releasePosition, 0.01f)
				?
				GameUtil.DirectionOnPlane(cursorPosition, releasePosition)
				: !GameUtil.Approximately(Caster.Position, cursorPosition, 0.01f)
					? GameUtil.DirectionOnPlane(Caster.Position, cursorPosition)
					: Caster.Forward;
			int num;
			for (int i = 0; i < Singleton<AdrianaSkillActive2Data>.inst.MaxCountCreateOilAreaProjectile; i = num + 1)
			{
				ProjectileProperty projectileProperty = PopProjectileProperty(Caster,
					Singleton<AdrianaSkillActive2Data>.inst.OilAreaProjectileCode);
				Vector3 vector = cursorPosition + skillDirection *
					(i * Singleton<AdrianaSkillActive2Data>.inst.OilAreaProjectileDistance);
				CollisionCircle3D collisionCircle =
					new CollisionCircle3D(vector, projectileProperty.ProjectileData.explosionRadius);
				float timeStack = Singleton<AdrianaSkillActive2Data>.inst.OilAreaProjectileRefreshPeriod;
				projectileProperty.SetUpdateAction(delegate
				{
					if (Singleton<AdrianaSkillActive2Data>.inst.OilAreaProjectileRefreshPeriod <= timeStack)
					{
						timeStack -= Singleton<AdrianaSkillActive2Data>.inst.OilAreaProjectileRefreshPeriod;
						using (List<SkillAgent>.Enumerator enumerator2 =
							GetEnemyCharacters(collisionCircle).GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								SkillAgent skillAgent = enumerator2.Current;
								CharacterState characterState = skillAgent.FindStateByGroup(
									GameDB.characterState
										.GetData(Singleton<AdrianaSkillActive2Data>.inst.OilAreaInfluenceStateCode)
										.group, Caster.ObjectId);
								if (characterState == null || characterState.Duration - characterState.RemainTime() >=
									Singleton<AdrianaSkillActive2Data>.inst.OilAreaProjectileStateCanAddTime)
								{
									CharacterState state = CreateState(skillAgent,
										Singleton<AdrianaSkillActive2Data>.inst.OilAreaInfluenceStateCode);
									AddState(skillAgent, state);
								}
							}

							return;
						}
					}

					timeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				});
				WorldProjectile worldProjectile3 = LaunchProjectile(projectileProperty, vector);
				Caster.AttachSight(worldProjectile3, SkillInnerRange,
					projectileProperty.ProjectileData.lifeTimeAfterArrival, false);
				WorldMovableCharacter worldMovableCharacter = Caster.Character as WorldMovableCharacter;
				List<WorldProjectile> ownProjectiles =
					worldMovableCharacter.GetOwnProjectiles(IsAdrianaFireFlameProjectile);
				if (ownProjectiles != null && 0 < ownProjectiles.Count)
				{
					foreach (WorldProjectile worldProjectile2 in ownProjectiles)
					{
						if (worldProjectile3.SkillAgent.CollisionObject.Collision(worldProjectile2.SkillAgent
							.CollisionObject))
						{
							PlaySkillAction(Caster, 1);
							ChangeProjectileFromOilAreaToFireFlame1(worldProjectile3, worldMovableCharacter);
							break;
						}
					}
				}

				yield return WaitForSeconds(Singleton<AdrianaSkillActive2Data>.inst.OilAreaProjectileTerm);
				num = i;
			}
		}
	}
}