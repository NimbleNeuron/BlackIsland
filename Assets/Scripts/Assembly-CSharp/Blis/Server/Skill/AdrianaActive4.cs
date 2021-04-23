using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AdrianaActive4)]
	public class AdrianaActive4 : AdrianaSkillScript
	{
		
		private CollisionCircle3D collisionFireFlame;

		
		protected override void Start()
		{
			base.Start();
			LookAtPosition(Caster, info.cursorPosition);
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			if (collisionFireFlame == null)
			{
				collisionFireFlame = new CollisionCircle3D(Vector3.zero, 0f);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (0f < SkillCastingTime1)
			{
				yield return FirstCastingTime();
			}

			Vector3 skillPoint = GetSkillPoint();
			Vector3 targetDirection = GameUtil.Direction(Caster.Position, skillPoint);
			ProjectileProperty projectileProperty =
				PopProjectileProperty(Caster, Singleton<AdrianaSkillActive4Data>.inst.FireBombProjectileCode);
			projectileProperty.SetTargetDirection(targetDirection);
			projectileProperty.SetDistance(GameUtil.Distance(Caster.Position, skillPoint));
			projectileProperty.SetSpeed(projectileProperty.Distance, projectileProperty.ProjectileData.duration);
			projectileProperty.SetActionOnArrive(
				delegate(Vector3 destination, bool isCollision, WorldProjectile worldProjectile)
				{
					ProjectileProperty projectileProperty3 = PopProjectileProperty(Caster,
						Singleton<AdrianaSkillActive4Data>.inst.FireFlame3ProjectileCode);
					WorldProjectile target2 = LaunchProjectile(projectileProperty3, destination);
					Caster.AttachSight(target2, SkillInnerRange,
						projectileProperty3.ProjectileData.lifeTimeAfterArrival, false);
					collisionFireFlame.UpdatePosition(destination);
					collisionFireFlame.UpdateRadius(projectileProperty3.ProjectileData.collisionObjectRadius);
					if (ProcessChangeProjectileFromOilAreaToFireFlame1(Caster.Character as WorldMovableCharacter,
						collisionFireFlame))
					{
						PlaySkillAction(Caster, 1);
					}
				});
			projectileProperty.SetExplosionSkill(SkillId.AdrianaActive4FireBomb);
			LaunchProjectile(projectileProperty);
			ProjectileProperty projectileProperty2 = PopProjectileProperty(Caster,
				Singleton<AdrianaSkillActive4Data>.inst.FireBombPositionProjectileCode);
			projectileProperty2.SetTargetDirection(targetDirection);
			WorldProjectile target = LaunchProjectile(projectileProperty2, skillPoint);
			Caster.AttachSight(target, SkillInnerRange, projectileProperty2.ProjectileData.lifeTimeAfterArrival, false);
			if (0f < SkillFinishDelayTime)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
			base.Finish(cancel);
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
		}
	}
}