using Blis.Common;

namespace Blis.Server
{
	
	public abstract class AdrianaSkillScript : SkillScript
	{
		
		protected bool IsOilAreaProjectile(WorldProjectile projectile)
		{
			return projectile.Property.ProjectileData.code ==
			       Singleton<AdrianaSkillActive2Data>.inst.OilAreaProjectileCode;
		}

		
		protected bool IsAdrianaFireFlameProjectile(WorldProjectile projectile)
		{
			return projectile.Property.ProjectileData.code ==
			       Singleton<AdrianaSkillActive2Data>.inst.FireFlame1ProjectileCode ||
			       projectile.Property.ProjectileData.code ==
			       Singleton<AdrianaSkillActive3Data>.inst.FireFlame2ProjectileCode ||
			       projectile.Property.ProjectileData.code ==
			       Singleton<AdrianaSkillActive4Data>.inst.FireFlame3ProjectileCode;
		}

		
		protected bool ProcessChangeProjectileFromOilAreaToFireFlame1(WorldMovableCharacter casterCharacter,
			CollisionObject3D fireFlameCollisionObject)
		{
			bool change = false;
			if (casterCharacter == null)
			{
				return false;
			}

			casterCharacter.DoOwnProjectileAction(IsOilAreaProjectile, delegate(WorldProjectile ownOilAreaProjectile)
			{
				if (fireFlameCollisionObject.Collision(ownOilAreaProjectile.SkillAgent.CollisionObject))
				{
					ChangeProjectileFromOilAreaToFireFlame1(ownOilAreaProjectile, casterCharacter);
					change = true;
				}
			});
			return change;
		}

		
		protected void ChangeProjectileFromOilAreaToFireFlame1(WorldProjectile originOilAreaProjectile,
			WorldMovableCharacter casterCharacter)
		{
			ProjectileProperty projectileProperty = PopProjectileProperty(Caster,
				Singleton<AdrianaSkillActive2Data>.inst.FireFlame1ProjectileCode);
			WorldProjectile target = LaunchProjectile(projectileProperty, originOilAreaProjectile.GetPosition());
			Caster.AttachSight(target, SkillInnerRange, projectileProperty.ProjectileData.lifeTimeAfterArrival, false);
			originOilAreaProjectile.DestroySelf();
			casterCharacter.DoOwnProjectileAction(IsOilAreaProjectile, delegate(WorldProjectile ownOilAreaProjectile)
			{
				if (originOilAreaProjectile.SkillAgent.CollisionObject.Collision(ownOilAreaProjectile.SkillAgent
					.CollisionObject))
				{
					ChangeProjectileFromOilAreaToFireFlame1(ownOilAreaProjectile, casterCharacter);
				}
			});
		}
	}
}