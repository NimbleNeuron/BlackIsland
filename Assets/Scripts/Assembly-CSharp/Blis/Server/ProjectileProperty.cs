using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class ProjectileProperty
	{
		
		
		public int OwnerObjectId
		{
			get
			{
				return this.ownerObjectId;
			}
		}

		
		
		public int TargetObjectId
		{
			get
			{
				return this.targetObjectId;
			}
		}

		
		
		public Vector3 Direction
		{
			get
			{
				return this.direction;
			}
		}

		
		
		public float ProjectileSpeed
		{
			get
			{
				return this.projectileSpeed;
			}
		}

		
		
		public float Distance
		{
			get
			{
				return this.distance;
			}
		}

		
		
		public float Duration
		{
			get
			{
				return this.duration;
			}
		}

		
		
		public float StartAngle
		{
			get
			{
				return this.startAngle;
			}
		}

		
		
		public int CollisionProjectileCode
		{
			get
			{
				return this.collisionProjectileCode;
			}
		}

		
		
		public Vector3 CollisionObjectDirection
		{
			get
			{
				return this.collisionObjectDirection;
			}
		}

		
		
		public SkillId ExplosionSkillId
		{
			get
			{
				return this.explosionSkillId;
			}
		}

		
		
		public SkillUseInfo SkillUseInfo
		{
			get
			{
				return this.skillUseInfo;
			}
		}

		
		
		public ProjectileData ProjectileData
		{
			get
			{
				return this.projectileData;
			}
		}

		
		
		public Action<Vector3, bool, WorldProjectile> OnArrive
		{
			get
			{
				return this.onArrive;
			}
		}

		
		
		public Action<SkillAgent, AttackerInfo, Vector3, Vector3> OnCollisionCharacter
		{
			get
			{
				return this.onCollisionCharacter;
			}
		}

		
		
		public Action<SkillAgent, AttackerInfo, Vector3, Vector3> OnExplosion
		{
			get
			{
				return this.onExplosion;
			}
		}

		
		
		public Action<WorldProjectile> DeadAction
		{
			get
			{
				return this.onDeadAction;
			}
		}

		
		
		public Action<WorldProjectile> OnUpdateAction
		{
			get
			{
				return this.onUpdateAction;
			}
		}

		
		
		public Action<WorldProjectile, Vector3, Vector3> OnCollisionProjectile
		{
			get
			{
				return this.onCollisionProjectile;
			}
		}

		
		
		public Action<Vector3, Vector3> OnCollisionWall
		{
			get
			{
				return this.onCollisionWall;
			}
		}

		
		public ProjectileProperty(SkillAgent owner, int code, SkillUseInfo skillUseInfo)
		{
			this.Set(owner, code, skillUseInfo);
		}

		
		public ProjectileProperty()
		{
		}

		
		public void Set(SkillAgent owner, int code, SkillUseInfo skillUseInfo)
		{
			this.ownerObjectId = owner.ObjectId;
			this.projectileData = GameDB.projectile.GetData(code);
			this.distance = this.ProjectileData.distance;
			float num = skillUseInfo.skillSlotSet.IsNormalAttack() ? owner.Stat.AttackSpeed : 1f;
			ProjectileType type = this.projectileData.type;
			if (type == ProjectileType.Around)
			{
				this.duration = this.ProjectileData.duration;
				this.projectileSpeed = this.ProjectileData.speed;
			}
			else if (this.ProjectileData.duration > 0f)
			{
				this.duration = this.ProjectileData.duration;
				this.projectileSpeed = this.distance / this.duration;
			}
			else
			{
				float num2 = this.ProjectileData.speed * num;
				this.projectileSpeed = ((this.ProjectileData.speed >= num2) ? this.ProjectileData.speed : num2);
				this.duration = this.distance / this.projectileSpeed;
			}
			this.skillUseInfo = skillUseInfo;
		}

		
		public void SetTargetObject(int targetObjectId)
		{
			this.targetObjectId = targetObjectId;
		}

		
		public void SetTargetDirection(Vector3 direction)
		{
			this.direction = direction;
		}

		
		public void SetSpeed(float speed)
		{
			this.projectileSpeed = speed;
			this.duration = this.distance / this.projectileSpeed;
		}

		
		public void SetSpeed(float distance, float duration)
		{
			if (duration <= 0f)
			{
				return;
			}
			this.distance = distance;
			this.projectileSpeed = distance / duration;
			this.duration = duration;
		}

		
		public void SetDistance(float distance)
		{
			this.distance = distance;
			this.duration = distance / this.projectileSpeed;
		}

		
		public void SetStartAngle(float angle)
		{
			this.startAngle = angle;
		}

		
		public void SetCollisionObjectDirection(Vector3 direction)
		{
			this.collisionObjectDirection = direction;
		}

		
		public void SetExplosionSkill(SkillId explosionSkillId)
		{
			this.explosionSkillId = explosionSkillId;
		}

		
		public void SetActionOnArrive(Action<Vector3, bool, WorldProjectile> action)
		{
			this.onArrive = action;
		}

		
		public void SetActionOnCollisionCharacter(Action<SkillAgent, AttackerInfo, Vector3, Vector3> action)
		{
			this.onCollisionCharacter = action;
		}

		
		public void SetActionOnExplosion(Action<SkillAgent, AttackerInfo, Vector3, Vector3> action)
		{
			this.onExplosion = action;
		}

		
		public void SetDeadAction(Action<WorldProjectile> action)
		{
			this.onDeadAction = action;
		}

		
		public void SetUpdateAction(Action<WorldProjectile> action)
		{
			this.onUpdateAction = action;
		}

		
		public void SetActionOnCollisionProjectile(Action<WorldProjectile, Vector3, Vector3> action)
		{
			this.onCollisionProjectile = action;
		}

		
		public void SetCollisionObjectCode(int collisionObjectCode)
		{
			this.collisionProjectileCode = collisionObjectCode;
		}

		
		public void SetActionCollisionWall(Action<Vector3, Vector3> action)
		{
			this.onCollisionWall = action;
		}

		
		public void Clear()
		{
			this.ownerObjectId = 0;
			this.targetObjectId = 0;
			this.direction = Vector3.zero;
			this.projectileSpeed = 0f;
			this.distance = 0f;
			this.duration = 0f;
			this.startAngle = 0f;
			this.collisionProjectileCode = 0;
			this.collisionObjectDirection = Vector3.zero;
			this.explosionSkillId = SkillId.None;
			this.skillUseInfo = null;
			this.projectileData = null;
			this.onArrive = null;
			this.onCollisionCharacter = null;
			this.onExplosion = null;
			this.onDeadAction = null;
			this.onUpdateAction = null;
			this.onCollisionProjectile = null;
			this.onCollisionWall = null;
		}

		
		private int ownerObjectId;

		
		private int targetObjectId;

		
		private Vector3 direction;

		
		private float projectileSpeed;

		
		private float distance;

		
		private float duration;

		
		private float startAngle;

		
		private int collisionProjectileCode;

		
		private Vector3 collisionObjectDirection;

		
		private SkillId explosionSkillId;

		
		private SkillUseInfo skillUseInfo;

		
		private ProjectileData projectileData;

		
		private Action<Vector3, bool, WorldProjectile> onArrive;

		
		private Action<SkillAgent, AttackerInfo, Vector3, Vector3> onCollisionCharacter;

		
		private Action<SkillAgent, AttackerInfo, Vector3, Vector3> onExplosion;

		
		private Action<WorldProjectile> onDeadAction;

		
		private Action<WorldProjectile> onUpdateAction;

		
		private Action<WorldProjectile, Vector3, Vector3> onCollisionProjectile;

		
		private Action<Vector3, Vector3> onCollisionWall;
	}
}
