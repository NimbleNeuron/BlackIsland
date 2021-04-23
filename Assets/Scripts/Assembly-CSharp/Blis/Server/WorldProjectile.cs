using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.Projectile)]
	public class WorldProjectile : WorldObject
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.Projectile;
		}

		
		protected override int GetTeamNumber()
		{
			WorldCharacter worldCharacter = this.Owner;
			if (worldCharacter == null)
			{
				return 0;
			}
			return worldCharacter.TeamNumber;
		}

		
		protected override ColliderAgent GetColliderAgent()
		{
			return this.colliderAgent;
		}

		
		protected override IItemBox GetItemBox()
		{
			throw new GameException(ErrorType.InvalidAction);
		}

		
		public override byte[] CreateSnapshot()
		{
			ProjectileSnapshot projectileSnapshot = new ProjectileSnapshot
			{
				code = this.ProjectileData.code,
				ownerId = this.owner.ObjectId,
				createdPosition = new BlisVector(this.createdPosition),
				collisionCount = this.collisionCount
			};
			this.OverwriteSnapShotByType(projectileSnapshot);
			return WorldObject.serializer.Serialize<ProjectileSnapshot>(projectileSnapshot);
		}

		
		protected void OverwriteSnapShotByType(ProjectileSnapshot snapshot)
		{
			switch (this.ProjectileData.type)
			{
			case ProjectileType.Target:
				snapshot.snapshot = WorldObject.serializer.Serialize<TargetProjectileSnapshot>(new TargetProjectileSnapshot
				{
					targetId = this.property.TargetObjectId,
					projectileSpeed = new BlisFixedPoint(this.property.ProjectileSpeed)
				});
				return;
			case ProjectileType.Point:
				if (this.ProjectileData.IsInstantArrival())
				{
					snapshot.snapshot = WorldObject.serializer.Serialize<InstantArrivalProjectileSnapshot>(new InstantArrivalProjectileSnapshot
					{
						projectileSpeed = new BlisFixedPoint(this.property.ProjectileSpeed),
						projectileDirection = new BlisVector(base.transform.forward)
					});
					return;
				}
				snapshot.snapshot = WorldObject.serializer.Serialize<DirectionProjectileSnapshot>(new DirectionProjectileSnapshot
				{
					timeAfterCreated = new BlisFixedPoint(MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.createdTime),
					duration = new BlisFixedPoint(this.property.Duration),
					targetDirectionEndPos = new BlisVector(this.directionEndPosition)
				});
				return;
			case ProjectileType.Direction:
				snapshot.snapshot = WorldObject.serializer.Serialize<DirectionProjectileSnapshot>(new DirectionProjectileSnapshot
				{
					timeAfterCreated = new BlisFixedPoint(MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.createdTime),
					duration = new BlisFixedPoint(this.property.Duration),
					targetDirectionEndPos = new BlisVector(this.directionEndPosition)
				});
				return;
			case ProjectileType.Around:
				snapshot.snapshot = WorldObject.serializer.Serialize<AroundProjectileSnapshot>(new AroundProjectileSnapshot
				{
					timeAfterCreated = new BlisFixedPoint(MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.createdTime),
					createdAngle = new BlisFixedPoint(this.property.StartAngle),
					duration = new BlisFixedPoint(this.property.Duration),
					speed = new BlisFixedPoint(this.property.ProjectileSpeed),
					distance = new BlisFixedPoint(this.property.Distance),
					aroundTargetId = this.property.TargetObjectId
				});
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		
		
		public ProjectileProperty Property
		{
			get
			{
				return this.property;
			}
		}

		
		
		public WorldCharacter Owner
		{
			get
			{
				return this.owner;
			}
		}

		
		
		public AttackerInfo AttackerInfo
		{
			get
			{
				return this.attackerInfo;
			}
		}

		
		
		public WorldCharacter Target
		{
			get
			{
				return this.target;
			}
		}

		
		
		protected ProjectileData ProjectileData
		{
			get
			{
				return this.property.ProjectileData;
			}
		}

		
		
		public int CollisionCount
		{
			get
			{
				return this.collisionCount;
			}
		}

		
		
		protected Vector3 CreatedPosition
		{
			get
			{
				return this.createdPosition;
			}
		}

		
		
		public bool IsAlive
		{
			get
			{
				return this.isAlive;
			}
		}

		
		
		public bool IsArrived
		{
			get
			{
				return this.isArrived;
			}
		}

		
		public override void SetPosition(Vector3 position)
		{
			this.prePosition = base.GetPosition();
			base.SetPosition(position);
		}

		
		public void Init(ProjectileProperty property)
		{
			this.property = property;
			this.aniCurve = (this.ProjectileData.isUseCurve ? ProjectileCurveManager.instance.GetAnimationCurve(this.ProjectileData.code) : null);
			this.collisionCount = (base.IsTypeOf<WorldHookLineProjectile>() ? (this.ProjectileData.penetrationCount - 1) : 0);
			this.createdTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			this.isAlive = true;
			this.isArrived = false;
			GameUtil.BindOrAdd<SphereColliderAgent>(base.gameObject, ref this.colliderAgent);
			this.colliderAgent.Init(this.ProjectileData.collisionObjectRadius);
			this.targetSharedProjectiles.Clear();
			this.collisionTargetIds.Clear();
			this.mySkillAgent = new WorldProjectileSkillAgent(this);
			this.world = MonoBehaviourInstance<GameService>.inst.World;
			this.owner = this.world.Find<WorldCharacter>(this.Property.OwnerObjectId);
			this.attackerInfo.SetAttackerStat(this.owner, this.cachedOwnerStat);
			this.target = null;
			if (this.Property.TargetObjectId != 0)
			{
				ProjectileType type = this.ProjectileData.type;
				if (type != ProjectileType.Target)
				{
					if (type == ProjectileType.Around)
					{
						this.world.TryFind<WorldCharacter>(this.Property.TargetObjectId, ref this.target);
						new SightRangeLink(this.target, this);
					}
				}
				else
				{
					this.world.TryFind<WorldCharacter>(this.Property.TargetObjectId, ref this.target);
				}
			}
			if (this.owner != null)
			{
				this.owner.AddOwnProjectile(this);
			}
			this.collisionObject = this.CreateCollisionProperty().CreateCollisionObject();
			if (this.ProjectileData.type == ProjectileType.Around)
			{
				this.AroundTypeSetInitPos();
			}
			this.createdPosition = base.GetPosition();
			switch (this.ProjectileData.type)
			{
			case ProjectileType.Target:
			case ProjectileType.Around:
				if (this.target != null)
				{
					Vector3 forward = GameUtil.DirectionOnPlane(base.GetPosition(), this.target.GetPosition());
					if (!forward.Equals(Vector3.zero))
					{
						base.transform.forward = forward;
					}
				}
				break;
			case ProjectileType.Point:
			case ProjectileType.Direction:
				this.directionEndPosition = this.createdPosition + this.Property.Direction * this.Property.Distance;
				if (!this.Property.Direction.Equals(Vector3.zero))
				{
					base.transform.forward = this.Property.Direction;
				}
				break;
			default:
				return;
			}
		}

		
		protected override void OnFrameUpdate()
		{
			base.OnFrameUpdate();
			if (!this.isAlive)
			{
				return;
			}
			Action<WorldProjectile> onUpdateAction = this.property.OnUpdateAction;
			if (onUpdateAction != null)
			{
				onUpdateAction(this);
			}
			if (this.isArrived)
			{
				this.CheckObjectCollision();
				return;
			}
			switch (this.ProjectileData.type)
			{
			case ProjectileType.Target:
				this.MoveToTarget();
				return;
			case ProjectileType.Point:
				if (this.ProjectileData.IsInstantArrival())
				{
					this.CheckObjectCollision();
					this.ArrivedDestination(false);
					return;
				}
				this.MoveToDirection();
				return;
			case ProjectileType.Direction:
				this.MoveToDirection();
				return;
			case ProjectileType.Around:
				this.MoveToAround();
				return;
			default:
				return;
			}
		}

		
		private void MoveToTarget()
		{
			if (this.target == null)
			{
				this.CheckObjectCollision();
				this.ArrivedDestination(false);
				return;
			}
			if (this.target.GetCollisionObject().Collision(this.GetCollisionObject()))
			{
				if (!this.target.IsUntargetable())
				{
					this.OnCollisionCharacter(this.target, this.Owner);
				}
				this.ArrivedDestination(true);
				return;
			}
			this.CheckObjectCollision();
			if (0 < this.collisionCount)
			{
				this.ArrivedDestination(true);
				return;
			}
			Vector3 vector = GameUtil.DirectionOnPlane(base.GetPosition(), this.target.GetPosition());
			float num = this.Property.ProjectileSpeed * MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
			Vector3 vector2 = base.GetPosition() + num * vector;
			if (this.CheckWallCollision(vector2))
			{
				this.ArrivedDestination(this.ProjectileData.isExplosionWithoutCollision);
				return;
			}
			int num2 = Physics.RaycastNonAlloc(base.GetPosition(), vector, this.raycastHits, num, GameConstants.LayerMask.WORLD_OBJECT_LAYER);
			for (int i = 0; i < num2; i++)
			{
				WorldCharacter component = this.raycastHits[i].collider.GetComponent<WorldCharacter>();
				if (component != null && this.target.ObjectId.Equals(component.ObjectId))
				{
					vector2 = this.target.GetPosition();
					break;
				}
			}
			if (!vector.Equals(Vector3.zero))
			{
				base.transform.forward = vector;
			}
			this.SetPosition(vector2);
		}

		
		private void MoveToDirection()
		{
			float num = 1f;
			if (this.property.Duration != 0f)
			{
				num = (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.createdTime) / this.property.Duration;
				if (num > 1f)
				{
					num = 1f;
				}
			}
			if (!this.ProjectileData.isUseCurve)
			{
				this.MoveToDirectionInLinear(num);
				return;
			}
			this.MoveToDirectionInCurve(num);
		}

		
		private void MoveToDirectionInCurve(float durationRate)
		{
			if (this.aniCurve == null)
			{
				Log.E("No Animation Curve For Projectile. code : " + this.ProjectileData.code);
				return;
			}
			float t = this.aniCurve.Evaluate(durationRate);
			Vector3 nextPosition = Vector3.Lerp(this.createdPosition, this.directionEndPosition, t);
			this.SettingPositionDirection(nextPosition, durationRate == 1f);
		}

		
		private void MoveToDirectionInLinear(float durationRate)
		{
			Vector3 nextPosition = Vector3.Lerp(this.createdPosition, this.directionEndPosition, durationRate);
			this.SettingPositionDirection(nextPosition, durationRate == 1f);
		}

		
		private void SettingPositionDirection(Vector3 nextPosition, bool isArrive)
		{
			if (isArrive)
			{
				this.SetPosition(nextPosition);
				this.CheckObjectCollision();
				this.ArrivedDestination(false);
				return;
			}
			this.CheckObjectCollision();
			if (this.CheckWallCollision(nextPosition))
			{
				this.ArrivedDestination(this.ProjectileData.isExplosionWithoutCollision);
				return;
			}
			this.SetPosition(nextPosition);
		}

		
		private void AroundTypeSetInitPos()
		{
			if (this.target == null)
			{
				base.StopAllCoroutines();
				this.DestroySelf();
				Log.E("MoveToAround target is null");
				return;
			}
			this.SetPosition(this.GetPositionOnAround(0f));
			if (this.ProjectileData.collisionObjectType != CollisionObjectType.Circle)
			{
				Vector3 direction = GameUtil.DirectionOnPlane(this.target.GetPosition(), base.GetPosition());
				base.SetRotation(GameUtil.LookRotation(direction, Vector3.up));
			}
		}

		
		private void MoveToAround()
		{
			if (this.Property.ProjectileSpeed == 0f)
			{
				return;
			}
			if (this.target == null)
			{
				base.StopAllCoroutines();
				this.DestroySelf();
				Log.E("MoveToAround target is null");
				return;
			}
			float num = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.createdTime;
			if (num >= this.property.Duration)
			{
				this.SetPosition(this.GetPositionOnAround(this.property.Duration));
				if (this.ProjectileData.collisionObjectType != CollisionObjectType.Circle)
				{
					Vector3 direction = GameUtil.DirectionOnPlane(this.target.GetPosition(), base.GetPosition());
					base.SetRotation(GameUtil.LookRotation(direction, Vector3.up));
				}
				this.CheckObjectCollision();
				this.ArrivedDestination(false);
				return;
			}
			Vector3 positionOnAround = this.GetPositionOnAround(num);
			this.CheckObjectCollision();
			if (this.CheckWallCollision(positionOnAround))
			{
				this.ArrivedDestination(this.ProjectileData.isExplosionWithoutCollision);
				return;
			}
			this.SetPosition(positionOnAround);
			if (this.ProjectileData.collisionObjectType != CollisionObjectType.Circle)
			{
				Vector3 direction2 = GameUtil.DirectionOnPlane(this.target.GetPosition(), base.GetPosition());
				base.SetRotation(GameUtil.LookRotation(direction2, Vector3.up));
			}
		}

		
		private Vector3 GetPositionOnAround(float elapsedTime)
		{
			Vector3 position = this.target.GetPosition();
			Vector3 result;
			if (this.Property.Distance <= 0f)
			{
				result = position;
			}
			else
			{
				float f = (this.Property.StartAngle + elapsedTime * this.Property.ProjectileSpeed % 360f) * 0.017453292f;
				float x = this.Property.Distance * Mathf.Cos(f);
				float z = this.Property.Distance * Mathf.Sin(f);
				result = position + new Vector3(x, 0f, z);
			}
			return result;
		}

		
		private bool CheckWallCollision(Vector3 nextPosition)
		{
			if (this.ProjectileData.isPassWall)
			{
				return false;
			}
			Vector3 vector = base.GetPosition();
			vector.y = 100f;
			RaycastHit raycastHit;
			bool flag;
			if (Physics.Raycast(new Ray(vector, Vector3.down), out raycastHit, 200f, GameConstants.LayerMask.GROUND_LAYER, QueryTriggerInteraction.Collide))
			{
				vector = raycastHit.point;
				Vector3 vector2;
				flag = MoveAgent.CanStandToPosition(vector, 2147483640, out vector2);
			}
			else
			{
				flag = false;
			}
			if (flag)
			{
				nextPosition.y = 100f;
				if (Physics.Raycast(new Ray(nextPosition, Vector3.down), out raycastHit, 200f, GameConstants.LayerMask.GROUND_LAYER, QueryTriggerInteraction.Collide))
				{
					nextPosition = raycastHit.point;
					Vector3 vector2;
					flag = MoveAgent.CanStraightMoveToDestination(vector, nextPosition, 2147483640, out vector2);
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				return false;
			}
			this.OnCollisionWall(base.GetPosition());
			return true;
		}

		
		private void CheckObjectCollision()
		{
			if (this.isArrived)
			{
				if (!this.ProjectileData.enableObjectCollsionCheckAfterArrival)
				{
					return;
				}
			}
			else if (!this.ProjectileData.enableObjectCollisionCheck)
			{
				return;
			}
			int num = Physics.OverlapSphereNonAlloc(base.GetPosition(), this.ProjectileData.collisionObjectRadius, this.projectileColliders, GameConstants.LayerMask.WORLD_OBJECT_LAYER);
			while (this.projectileColliders.Length == num)
			{
				this.projectileColliders = new Collider[this.projectileColliders.Length * 2];
				num = Physics.OverlapSphereNonAlloc(base.GetPosition(), this.ProjectileData.collisionObjectRadius, this.projectileColliders, GameConstants.LayerMask.WORLD_OBJECT_LAYER);
			}
			bool flag = false;
			int num2 = 0;
			while (num2 < num && !flag)
			{
				WorldObject component = this.projectileColliders[num2].GetComponent<WorldObject>();
				if (component != null)
				{
					WorldCharacter worldCharacter = component as WorldCharacter;
					if (worldCharacter != null)
					{
						this.CheckOnCollisionCharacter(worldCharacter, out flag);
					}
					else if (this.Property.CollisionProjectileCode != 0)
					{
						WorldProjectile worldProjectile = component as WorldProjectile;
						if (worldProjectile != null)
						{
							this.CheckOnCollisionProjectile(worldProjectile);
						}
					}
				}
				num2++;
			}
			if (flag)
			{
				this.ArrivedDestination(true);
			}
		}

		
		private void CheckOnCollisionProjectile(WorldProjectile targetProjectile)
		{
			if (!targetProjectile.GetCollisionObject().Collision(this.GetCollisionObject()))
			{
				return;
			}
			if (!this.ProjectileData.collisionHostileType.HasFlag(this.owner.GetHostileType(targetProjectile.owner)))
			{
				return;
			}
			if (this.Property.CollisionProjectileCode == 0 || this.Property.CollisionProjectileCode != targetProjectile.ProjectileData.code)
			{
				return;
			}
			this.OnCollisionProjectile(targetProjectile, this.Owner);
			this.collisionCount++;
		}

		
		private void CheckOnCollisionCharacter(WorldCharacter targetCharacter, out bool destroy)
		{
			destroy = false;
			if (!targetCharacter.IsAlive)
			{
				return;
			}
			if (targetCharacter.ObjectType.IsSummonObject())
			{
				return;
			}
			if (targetCharacter.IsUntargetable())
			{
				return;
			}
			if (!targetCharacter.GetCollisionObject().Collision(this.GetCollisionObject()))
			{
				return;
			}
			if (this.ProjectileData.collisionHostileType.HasFlag(this.owner.GetHostileType(targetCharacter)))
			{
				if (this.ProjectileData.isExplosion && 0f < this.ProjectileData.explosionRadius)
				{
					destroy = true;
					return;
				}
				if (!this.IsCollisionTarget(targetCharacter.ObjectId))
				{
					this.OnCollisionCharacter(targetCharacter, this.Owner);
					this.AddCollisionTarget(targetCharacter.ObjectId);
					for (int i = 0; i < this.targetSharedProjectiles.Count; i++)
					{
						this.targetSharedProjectiles[i].AddCollisionTarget(targetCharacter.ObjectId);
					}
					this.collisionCount++;
				}
			}
			if (this.ProjectileData.penetrationCount <= this.collisionCount)
			{
				destroy = true;
			}
		}

		
		protected virtual void ArrivedDestination(bool isCollision)
		{
			if (this.isArrived)
			{
				if (isCollision && this.Property.ProjectileData.enableObjectCollsionCheckAfterArrival)
				{
					base.StopAllCoroutines();
					this.DestroySelf();
				}
				return;
			}
			this.isArrived = true;
			Action<Vector3, bool, WorldProjectile> onArrive = this.property.OnArrive;
			if (onArrive != null)
			{
				onArrive(base.GetPosition(), isCollision, this);
			}
			if (0f < this.ProjectileData.lifeTimeAfterArrival)
			{
				base.StartCoroutine(CoroutineUtil.DelayedAction(this.ProjectileData.lifeTimeAfterArrival, delegate()
				{
					this.colliderAgent.UpdateRadius(this.ProjectileData.explosionRadius);
					this.ExplosionProjectile(isCollision);
					if (0f < this.ProjectileData.lifeTimeAfterExplosion)
					{
						this.StartCoroutine(CoroutineUtil.DelayedAction(this.ProjectileData.lifeTimeAfterExplosion, new Action(this.DestroySelf)));
						return;
					}
					this.DestroySelf();
				}));
				return;
			}
			this.colliderAgent.UpdateRadius(this.ProjectileData.explosionRadius);
			this.ExplosionProjectile(isCollision);
			if (0f < this.ProjectileData.lifeTimeAfterExplosion)
			{
				base.StartCoroutine(CoroutineUtil.DelayedAction(this.ProjectileData.lifeTimeAfterExplosion, new Action(this.DestroySelf)));
				return;
			}
			this.DestroySelf();
		}

		
		private void ExplosionProjectile(bool isCollision)
		{
			if (!this.ProjectileData.isExplosion)
			{
				return;
			}
			if (isCollision)
			{
				this.Explosion();
				return;
			}
			if (this.ProjectileData.isExplosionWithoutCollision)
			{
				this.Explosion();
			}
		}

		
		private void Explosion()
		{
			base.EnqueueCommand(new CmdProjectileExplosion());
			if (this.property.ExplosionSkillId != SkillId.None)
			{
				this.StartExplosionSkill(this.property.ExplosionSkillId);
			}
			if (0f < this.ProjectileData.explosionRadius)
			{
				int num = Physics.OverlapSphereNonAlloc(base.GetPosition(), this.ProjectileData.explosionRadius, this.projectileColliders, GameConstants.LayerMask.WORLD_OBJECT_LAYER);
				for (int i = 0; i < num; i++)
				{
					WorldCharacter component = this.projectileColliders[i].GetComponent<WorldCharacter>();
					if (!(component == null) && component.IsAlive && (this.ProjectileData.type != ProjectileType.Target || !(this.target != null) || this.target.ObjectId != component.ObjectId) && this.ProjectileData.collisionHostileType.HasFlag(this.owner.GetHostileType(component)))
					{
						this.OnExplosion(component, this.Owner);
					}
				}
			}
		}

		
		protected virtual void OnCollisionCharacter(WorldCharacter target, WorldCharacter caster)
		{
			Vector3 vector = GameUtil.DirectionOnPlane(base.GetPosition(), target.GetPosition());
			if (vector == Vector3.zero)
			{
				vector = GameUtil.DirectionOnPlane(this.prePosition, target.GetPosition());
			}
			Action<SkillAgent, AttackerInfo, Vector3, Vector3> onCollisionCharacter = this.Property.OnCollisionCharacter;
			if (onCollisionCharacter != null)
			{
				onCollisionCharacter(target.SkillAgent, this.attackerInfo, base.GetPosition(), vector);
			}
			base.EnqueueCommand(new CmdProjectileCollision
			{
				targetId = target.ObjectId
			});
		}

		
		private void OnExplosion(WorldCharacter target, WorldCharacter caster)
		{
			Vector3 arg = GameUtil.DirectionOnPlane(base.GetPosition(), target.GetPosition());
			Action<SkillAgent, AttackerInfo, Vector3, Vector3> onExplosion = this.Property.OnExplosion;
			if (onExplosion != null)
			{
				onExplosion(target.SkillAgent, this.attackerInfo, base.GetPosition(), arg);
			}
			base.EnqueueCommand(new CmdProjectileCollision
			{
				targetId = target.ObjectId
			});
		}

		
		protected virtual void OnCollisionProjectile(WorldProjectile target, WorldCharacter caster)
		{
			Action<WorldProjectile, Vector3, Vector3> onCollisionProjectile = this.Property.OnCollisionProjectile;
			if (onCollisionProjectile != null)
			{
				onCollisionProjectile(target, base.GetPosition(), (base.GetPosition() - this.owner.GetPosition()).normalized);
			}
			base.EnqueueCommand(new CmdProjectileCollision
			{
				targetId = target.ObjectId
			});
		}

		
		protected virtual void OnCollisionWall(Vector3 damagePoint)
		{
			if (this.Property.OnCollisionWall == null)
			{
				return;
			}
			if (this.target != null)
			{
				Vector3 arg = GameUtil.DirectionOnPlane(base.GetPosition(), this.target.GetPosition());
				Action<Vector3, Vector3> onCollisionWall = this.Property.OnCollisionWall;
				if (onCollisionWall != null)
				{
					onCollisionWall(damagePoint, arg);
				}
			}
			else
			{
				Action<Vector3, Vector3> onCollisionWall2 = this.Property.OnCollisionWall;
				if (onCollisionWall2 != null)
				{
					onCollisionWall2(damagePoint, this.Property.Direction);
				}
			}
			base.EnqueueCommand(new CmdProjectileCollisionWall
			{
				targetPosition = damagePoint
			});
		}

		
		protected override SkillAgent GetSkillAgent()
		{
			return this.mySkillAgent;
		}

		
		public override void DestroySelf()
		{
			if (!this.isAlive)
			{
				return;
			}
			this.isAlive = false;
			Action<WorldProjectile> deadAction = this.Property.DeadAction;
			if (deadAction != null)
			{
				deadAction(this);
			}
			if (this.owner != null)
			{
				this.owner.RemoveOwnProjectile(this);
			}
			if (this.Property.ProjectileData.type == ProjectileType.Around)
			{
				base.RemoveAllSightRangeLink();
			}
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterProjectileDestory(this.ProjectileData.code, this.objectId, (this.owner != null) ? this.owner.ObjectId : 0);
			base.StartCoroutine(CoroutineUtil.DelayedAction(1, new Action(this.DestroyAfterAction)));
		}

		
		protected virtual void DestroyAfterAction()
		{
			SkillScript skillScript = this.playingScripts;
			if (skillScript != null)
			{
				skillScript.Stop(false);
			}
			MonoBehaviourInstance<GameService>.inst.World.PushProjectileProperty(this.property);
			this.property = null;
			MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(this);
		}

		
		public CollisionObject3D GetCollisionObject()
		{
			this.collisionObject.UpdatePosition(base.GetPosition());
			return this.collisionObject;
		}

		
		private CollisionObjectProperty CreateCollisionProperty()
		{
			switch (this.ProjectileData.collisionObjectType)
			{
			case CollisionObjectType.Circle:
				return CollisionObjectProperty.Circle(base.GetPosition(), this.ProjectileData.collisionObjectRadius);
			case CollisionObjectType.Sector:
				return CollisionObjectProperty.Sector(base.GetPosition(), this.ProjectileData.collisionObjectRadius, this.ProjectileData.collisionObjectAngle, this.Property.CollisionObjectDirection);
			case CollisionObjectType.Box:
				return CollisionObjectProperty.Box(base.GetPosition(), this.ProjectileData.collisionObjectWidth, this.ProjectileData.collisionObjectDepth, this.Property.CollisionObjectDirection);
			default:
				return CollisionObjectProperty.Circle(base.GetPosition(), this.ProjectileData.collisionObjectRadius);
			}
		}

		
		public void ShareTarget(List<WorldProjectile> projectiles)
		{
			this.targetSharedProjectiles = projectiles;
		}

		
		private bool IsCollisionTarget(int objectId)
		{
			return this.collisionTargetIds.Contains(objectId);
		}

		
		private void AddCollisionTarget(int objectId)
		{
			this.collisionTargetIds.Add(objectId);
		}

		
		public void StartExplosionSkill(SkillId skillId)
		{
			SkillUseInfo info = SkillUseInfo.Create(this.mySkillAgent, this.property.SkillUseInfo.target, this.property.SkillUseInfo.skillData, this.property.SkillUseInfo.skillSlotSet, this.property.SkillUseInfo.weaponSkillMastery, this.property.SkillUseInfo.skillEvolutionLevel, this.property.SkillUseInfo.cursorPosition, this.property.SkillUseInfo.releasePosition, this.property.SkillUseInfo.stateData, this.property.SkillUseInfo.injected);
			this.playingScripts = this.skillScriptManager.Create(skillId);
			this.playingScripts.StartAction(new Action<SkillScript>(this.StartSkill));
			this.playingScripts.FinishAction(new Action<SkillScript, bool, bool>(this.FinishSkill));
			this.playingScripts.Setup(info, this);
			this.playingSkill = base.StartCoroutine(this.playingScripts.Play(this));
		}

		
		public void PlaySkillAction(SkillId skillId, int actionNo, int targetId, BlisVector targetPosition)
		{
			base.EnqueueCommand(new CmdPlaySkillAction
			{
				skillId = skillId,
				actionNo = actionNo,
				targets = new List<SkillActionTarget>
				{
					new SkillActionTarget
					{
						targetId = targetId,
						targetPos = targetPosition
					}
				}
			});
		}

		
		public void PlaySkillAction(SkillId skillId, int actionNo, List<SkillActionTarget> targets)
		{
			base.EnqueueCommand(new CmdPlaySkillAction
			{
				skillId = skillId,
				actionNo = actionNo,
				targets = targets
			});
		}

		
		private void StartSkill(SkillScript skillScript)
		{
			base.EnqueueCommand(new CmdStartSkill
			{
				skillId = this.property.ExplosionSkillId,
				skillCode = this.property.SkillUseInfo.SkillCode,
				skillEvolutionLevel = this.property.SkillUseInfo.skillEvolutionLevel,
				targetObjectId = ((this.property.SkillUseInfo.target != null) ? this.property.SkillUseInfo.target.ObjectId : 0)
			});
		}

		
		private void FinishSkill(SkillScript skillScript, bool toNextSequence, bool cancel)
		{
			this.playingScripts = null;
			base.EnqueueCommand(new CmdFinishSkill
			{
				skillId = skillScript.SkillId,
				cancel = cancel,
				skillSlotSet = SkillSlotSet.None
			});
		}

		
		private ProjectileProperty property;

		
		private GameWorld world;

		
		private WorldCharacter owner;

		
		private readonly SimpleCharacterStat cachedOwnerStat = new SimpleCharacterStat();

		
		private readonly AttackerInfo attackerInfo = new AttackerInfo();

		
		private WorldCharacter target;

		
		private SkillAgent mySkillAgent;

		
		private CollisionObject3D collisionObject;

		
		private SphereColliderAgent colliderAgent;

		
		private Collider[] projectileColliders = new Collider[50];

		
		private int collisionCount;

		
		private RaycastHit[] raycastHits = new RaycastHit[20];

		
		private Vector3 createdPosition;

		
		private Vector3 directionEndPosition;

		
		private Vector3 prePosition;

		
		private float createdTime;

		
		private bool isAlive;

		
		protected bool isArrived;

		
		private AnimationCurve aniCurve;

		
		private List<WorldProjectile> targetSharedProjectiles = new List<WorldProjectile>();

		
		private List<int> collisionTargetIds = new List<int>();

		
		private readonly SkillScriptManager skillScriptManager = SkillScriptManager.inst;

		
		protected SkillScript playingScripts;

		
		private Coroutine playingSkill;
	}
}
