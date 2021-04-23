using System;
using System.Collections.Generic;
using BIFog;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[RequireComponent(typeof(LocalProjectile))]
	[ObjectAttr(ObjectType.Projectile)]
	public class LocalProjectile : LocalObject
	{
		private const string HitPoint = "HitPoint";


		private const string HitBoneSpine = "Bip001 Spine";


		private readonly List<ParticleSystem> hideProjectileParticleSystems = new List<ParticleSystem>();


		private readonly Collider[] projectileColliders = new Collider[50];


		private readonly RaycastHit[] raycastHits = new RaycastHit[20];


		[NonSerialized] public ActiveOnHostileType activeOnHostileType;


		private AnimationCurve aniCurve;


		private float arriveRate;


		private SphereColliderAgent colliderAgent;


		private int collisionCount;


		private CollisionObject3D collisionObject;


		protected Vector3 createdPosition;


		private float createdTime;


		private Vector3 destination;


		private float distance;


		private Vector3 finalPosition;


		protected FogHiderOnCenter fogHiderOnCenter;


		private bool isAlive = true;


		private bool isArrived;


		private bool isHide;


		private Vector3 lastProjectileObjPosition;


		private Vector3 lastProjectilePosition;


		protected Vector3 launchPosition;


		private Vector3 middlePosition;


		private LocalCharacter owner;


		private ProjectileData projectileData;


		private Vector3 projectileDirection;


		private float projectileDuration;


		private GameObject projectileObj;


		private float projectileSpeed;


		private float projectileStartAngle;


		protected Transform projectileTrans;


		private LocalCharacter target;


		private Transform targetHitPoint;


		protected virtual bool needRebuildFogHider => true;


		public bool IsAlive => isAlive;


		public LocalCharacter Owner => owner;


		public ProjectileData ProjectileData => projectileData;


		protected virtual void Update()
		{
			if (!isAlive)
			{
				return;
			}

			UpdateProjectile();
			UpdateLocalProjectile();
		}

		protected override ObjectType GetObjectType()
		{
			return ObjectType.Projectile;
		}


		protected override int GetTeamNumber()
		{
			LocalCharacter localCharacter = owner;
			if (localCharacter == null)
			{
				return 0;
			}

			return localCharacter.TeamNumber;
		}


		protected override ColliderAgent GetColliderAgent()
		{
			return colliderAgent;
		}


		protected override bool GetIsOutSight()
		{
			return false;
		}


		public override void Init(byte[] snapshotData)
		{
			LocalSkillPlayer localSkillPlayer = this.localSkillPlayer;
			if (localSkillPlayer != null)
			{
				localSkillPlayer.FinishAll();
			}

			this.localSkillPlayer = new LocalSkillPlayer(this);
			this.localSkillPlayer.SetOnBeforeStartSkillAction(null);
			ProjectileSnapshot baseSnapshot = serializer.Deserialize<ProjectileSnapshot>(snapshotData);
			Init(baseSnapshot);
		}


		public void Init(ProjectileSnapshot baseSnapshot)
		{
			isAlive = true;
			isArrived = false;
			isHide = false;
			arriveRate = 0f;
			projectileData = GameDB.projectile.GetData(baseSnapshot.code);
			aniCurve = projectileData.isUseCurve
				? ProjectileCurveManager.instance.GetAnimationCurve(projectileData.code)
				: null;
			owner = MonoBehaviourInstance<ClientService>.inst.World.Find<LocalCharacter>(baseSnapshot.ownerId);
			collisionCount = baseSnapshot.collisionCount;
			createdPosition = baseSnapshot.createdPosition.ToVector3();
			createdPosition += new Vector3(0f, projectileData.localBaseHeight, 0f);
			destination += new Vector3(0f, projectileData.localBaseHeight, 0f);
			GameUtil.BindOrAdd<SphereColliderAgent>(gameObject, ref colliderAgent);
			colliderAgent.Init(projectileData.collisionObjectRadius);
			switch (projectileData.type)
			{
				case ProjectileType.Target:
					InitTargetProjectile(baseSnapshot);
					if (target == null)
					{
						ArrivedDestination(false);
					}

					break;
				case ProjectileType.Point:
					if (projectileData.IsInstantArrival())
					{
						InitInstantArrivalProjectile(baseSnapshot);
					}
					else
					{
						InitDirectionProjectile(baseSnapshot);
					}

					break;
				case ProjectileType.Direction:
					InitDirectionProjectile(baseSnapshot);
					break;
				case ProjectileType.Around:
					InitAroundProjectile(baseSnapshot);
					break;
			}

			collisionObject = CreateCollisionProperty().CreateCollisionObject();
			Transform transform = null;
			launchPosition = createdPosition;
			if (!projectileData.IsInstantArrival() && !string.IsNullOrEmpty(projectileData.shotPoint) &&
			    projectileData.type != ProjectileType.Around && !owner.SightAgent.IsOutSight)
			{
				transform = owner.transform.FindRecursively(projectileData.shotPoint);
				if (transform != null)
				{
					launchPosition = transform.position;
					launchPosition += new Vector3(0f, projectileData.localBaseHeight, 0f);
				}
			}

			if (projectileData.type != ProjectileType.Around)
			{
				finalPosition = destination;
				RaycastHit raycastHit;
				Vector3 vector;
				if (Physics.Raycast(
					    new Ray(new Vector3(destination.x, destination.y + 10f, destination.z), Vector3.down),
					    out raycastHit, 100f, GameConstants.LayerMask.GROUND_LAYER, QueryTriggerInteraction.Collide) &&
				    MoveAgent.CanStandToPosition(raycastHit.point, 2147483640, out vector))
				{
					finalPosition = vector;
				}

				if (projectileData.localMoveType == ProjectileLocalMoveType.Direct && transform != null)
				{
					finalPosition.y = finalPosition.y + Mathf.Abs(launchPosition.y - createdPosition.y);
				}
			}

			middlePosition = (createdPosition + finalPosition) * 0.5f;
			middlePosition.y = finalPosition.y + projectileData.localHighAngleHeight;
			if (projectileData.type == ProjectileType.Around)
			{
				SetRotation(GameUtil.LookRotation(GameUtil.DirectionOnPlane(target.GetPosition(), GetPosition())));
			}
			else
			{
				SetRotation(GameUtil.LookRotation(projectileDirection));
			}

			lastProjectilePosition = createdPosition;
			GameObject prefab = GetPrefab();
			if (prefab != null)
			{
				projectileObj = Instantiate<GameObject>(prefab, launchPosition, this.transform.rotation);
				projectileTrans = projectileObj.transform;
				projectileTrans.SetParent(this.transform, true);
				lastProjectileObjPosition = projectileTrans.position;
			}
			else
			{
				Log.V("[LocalProjectile] Init() : Not Found Projectile Prefab (" + projectileData.prefabName + ")");
			}

			GameUtil.BindOrAdd<FogHiderOnCenter>(gameObject, ref fogHiderOnCenter);
			fogHiderOnCenter.Init(GetObjectType());
			fogHiderOnCenter.SetHideRenderer(true);
			if (needRebuildFogHider)
			{
				fogHiderOnCenter.RebuildRendererList();
			}

			FogProjectileHider fogProjectileHider = fogHiderOnCenter as FogProjectileHider;
			if (fogProjectileHider != null)
			{
				fogProjectileHider.SetProjectileType(projectileData.type);
			}

			LocalCharacter component = owner.GetComponent<LocalCharacter>();
			if (component != null)
			{
				component.PlayLocalEffect(projectileData.shotEffectAndSoundCode);
			}

			PlayLocalEffect(projectileData.attachEffectAndSoundCode);
			owner.LaunchProjectile(this);
			if (projectileObj != null)
			{
				activeOnHostileType = projectileObj.GetComponent<ActiveOnHostileType>();
				if (activeOnHostileType != null)
				{
					activeOnHostileType.Init(this);
				}
			}
		}


		private void InitTargetProjectile(ProjectileSnapshot baseSnapshot)
		{
			TargetProjectileSnapshot targetProjectileSnapshot =
				serializer.Deserialize<TargetProjectileSnapshot>(baseSnapshot.snapshot);
			if (!MonoBehaviourInstance<ClientService>.inst.World.TryFind<LocalCharacter>(
				targetProjectileSnapshot.targetId, ref target))
			{
				return;
			}

			projectileDirection = GameUtil.DirectionOnPlane(GetPosition(), target.GetPosition());
			projectileSpeed = targetProjectileSnapshot.projectileSpeed.GetValue();
			targetHitPoint = target.transform.FindRecursively(!string.IsNullOrEmpty(projectileData.hitPoint)
				? projectileData.hitPoint
				: "HitPoint");
			if (targetHitPoint == null)
			{
				targetHitPoint = target.transform.FindRecursively("Bip001 Spine");
			}
		}


		private void InitInstantArrivalProjectile(ProjectileSnapshot baseSnapshot)
		{
			InstantArrivalProjectileSnapshot instantArrivalProjectileSnapshot =
				serializer.Deserialize<InstantArrivalProjectileSnapshot>(baseSnapshot.snapshot);
			projectileDirection = instantArrivalProjectileSnapshot.projectileDirection.ToVector3();
			destination = GetPosition();
			distance = 0f;
			projectileSpeed = instantArrivalProjectileSnapshot.projectileSpeed.GetValue();
		}


		private void InitDirectionProjectile(ProjectileSnapshot baseSnapshot)
		{
			DirectionProjectileSnapshot directionProjectileSnapshot =
				serializer.Deserialize<DirectionProjectileSnapshot>(baseSnapshot.snapshot);
			createdTime = Time.time - directionProjectileSnapshot.timeAfterCreated.Value;
			destination = directionProjectileSnapshot.targetDirectionEndPos.ToVector3();
			Vector3 vector = destination - createdPosition;
			projectileDirection = vector.normalized;
			projectileDuration = directionProjectileSnapshot.duration.Value;
			distance = vector.magnitude;
		}


		private void InitAroundProjectile(ProjectileSnapshot baseSnapshot)
		{
			AroundProjectileSnapshot aroundProjectileSnapshot =
				serializer.Deserialize<AroundProjectileSnapshot>(baseSnapshot.snapshot);
			createdTime = Time.time - aroundProjectileSnapshot.timeAfterCreated.Value;
			projectileStartAngle = aroundProjectileSnapshot.createdAngle.Value;
			projectileDuration = aroundProjectileSnapshot.duration.Value;
			distance = aroundProjectileSnapshot.distance.Value;
			projectileSpeed = aroundProjectileSnapshot.speed.Value;
			target = MonoBehaviourInstance<ClientService>.inst.World.Find<LocalCharacter>(aroundProjectileSnapshot
				.aroundTargetId);
			destination = Vector3.zero;
			projectileDirection = Vector3.zero;
		}


		private GameObject GetPrefab()
		{
			string text = projectileData.prefabName;
			if (string.IsNullOrEmpty(text))
			{
				return new GameObject();
			}

			if (projectileData.suffixItemCode && owner.IsTypeOf<LocalPlayerCharacter>())
			{
				Item weapon = ((LocalPlayerCharacter) owner).GetWeapon();
				if (weapon == null)
				{
					return null;
				}

				text = string.Format("{0}_{1}", projectileData.prefabName, weapon.itemCode);
			}

			return LoadProjectile(text);
		}


		private void UpdateProjectile()
		{
			if (isArrived)
			{
				return;
			}

			switch (projectileData.type)
			{
				case ProjectileType.Target:
					MoveToTarget();
					break;
				case ProjectileType.Point:
					if (projectileData.IsInstantArrival())
					{
						ArrivedDestination(false);
					}
					else
					{
						MoveToDirection();
					}

					break;
				case ProjectileType.Direction:
					MoveToDirection();
					break;
				case ProjectileType.Around:
					MoveToAround();
					break;
			}

			if (isAlive && !isArrived)
			{
				ProjectileType type = projectileData.type;
				if (type == ProjectileType.Target || type == ProjectileType.Direction)
				{
					Vector3 forward = GameUtil.DirectionOnPlane(lastProjectilePosition, GetPosition());
					if (!forward.Equals(Vector3.zero))
					{
						transform.forward = forward;
					}
				}
			}

			lastProjectilePosition = GetPosition();
		}


		private void MoveToTarget()
		{
			if (isArrived)
			{
				return;
			}

			if (target == null)
			{
				CheckObjectCollision();
				ArrivedDestination(false);
				return;
			}

			float num = GameUtil.DistanceOnPlane(createdPosition, target.GetPosition());
			float num2 = Mathf.Min(num, GameUtil.DistanceOnPlane(createdPosition, GetPosition()));
			if (num == num2)
			{
				arriveRate = 1f;
			}
			else
			{
				arriveRate = num2 / num;
			}

			if (target.GetCollisionObject().Collision(GetCollisionObject()))
			{
				ArrivedDestination(true);
				return;
			}

			CheckObjectCollision();
			if (0 < collisionCount)
			{
				ArrivedDestination(true);
				return;
			}

			Vector3 vector = GameUtil.DirectionOnPlane(GetPosition(), target.GetPosition());
			float num3 = projectileSpeed * Time.deltaTime;
			Vector3 position = GetPosition() + num3 * vector;
			if (CheckWallCollision())
			{
				ArrivedDestination(projectileData.isExplosionWithoutCollision);
				return;
			}

			int num4 = Physics.RaycastNonAlloc(GetPosition(), vector, raycastHits, num3,
				GameConstants.LayerMask.WORLD_OBJECT_LAYER);
			for (int i = 0; i < num4; i++)
			{
				LocalCharacter component = raycastHits[i].collider.GetComponent<LocalCharacter>();
				if (component != null && target.ObjectId.Equals(component.ObjectId))
				{
					position = target.GetPosition();
					break;
				}
			}

			SetPosition(position);
		}


		private void MoveToDirection()
		{
			if (projectileDuration == 0f)
			{
				arriveRate = 1f;
			}
			else
			{
				arriveRate = (Time.time - createdTime) / projectileDuration;
				if (arriveRate > 1f)
				{
					arriveRate = 1f;
				}
			}

			if (!projectileData.isUseCurve)
			{
				MoveToDirectionInLinear();
				return;
			}

			MoveToDirectionInCurve();
		}


		private void MoveToDirectionInCurve()
		{
			if (aniCurve == null)
			{
				Log.E("Client No Animation Curve For Projectile. code : " + projectileData.code);
				return;
			}

			float t = aniCurve.Evaluate(arriveRate);
			Vector3 nextPosition = Vector3.Lerp(createdPosition, finalPosition, t);
			SettingPositionDirection(nextPosition, arriveRate == 1f);
		}


		private void MoveToDirectionInLinear()
		{
			Vector3 nextPosition = Vector3.Lerp(createdPosition, finalPosition, arriveRate);
			SettingPositionDirection(nextPosition, arriveRate == 1f);
		}


		private void SettingPositionDirection(Vector3 nextPosition, bool isArrive)
		{
			if (isArrive)
			{
				SetPosition(nextPosition);
				ArrivedDestination(false);
				return;
			}

			if (CheckWallCollision())
			{
				ArrivedDestination(projectileData.isExplosionWithoutCollision);
				return;
			}

			SetPosition(nextPosition);
		}


		private void MoveToAround()
		{
			if (projectileSpeed == 0f)
			{
				return;
			}

			if (target == null)
			{
				StopAllCoroutines();
				HideProjectile();
				Log.E("MoveToAround target is null");
				return;
			}

			float num = Time.time - createdTime;
			if (num >= projectileDuration)
			{
				SetPosition(GetPositionOnAround(projectileDuration));
				SetRotation(GameUtil.LookRotation(GameUtil.DirectionOnPlane(target.GetPosition(), GetPosition())));
				ArrivedDestination(false);
				return;
			}

			if (CheckWallCollision())
			{
				ArrivedDestination(projectileData.isExplosionWithoutCollision);
				return;
			}

			SetPosition(GetPositionOnAround(num));
			SetRotation(GameUtil.LookRotation(GameUtil.DirectionOnPlane(target.GetPosition(), GetPosition())));
		}


		private Vector3 GetPositionOnAround(float elapsedTime)
		{
			Vector3 position = target.GetPosition();
			Vector3 result;
			if (distance <= 0f)
			{
				result = position;
			}
			else
			{
				float f = (projectileStartAngle + elapsedTime * projectileSpeed % 360f) * 0.017453292f;
				float x = distance * Mathf.Cos(f);
				float z = distance * Mathf.Sin(f);
				result = position + new Vector3(x, 0f, z);
			}

			return result;
		}


		private bool CheckWallCollision()
		{
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;
			Vector3 vector4;
			return !projectileData.isPassWall &&
			       (!MoveAgent.CanStandToPosition(GetPosition(), 2147483640, 2f, out vector) ||
			        !MoveAgent.CanStandToPosition(new Vector3(GetPosition().x, vector.y, GetPosition().z), 2147483640,
				        1f, out vector2) ||
			        !MoveAgent.CanStandToPosition(new Vector3(GetPosition().x, vector2.y, GetPosition().z), 2147483640,
				        0.5f, out vector3) || !MoveAgent.CanStandToPosition(
				        new Vector3(GetPosition().x, vector3.y, GetPosition().z), 2147483640, 0.25f, out vector4));
		}


		private void UpdateLocalProjectile()
		{
			if (projectileObj == null)
			{
				return;
			}

			if (projectileData.type == ProjectileType.Around)
			{
				return;
			}

			if (projectileSpeed <= 0f && projectileDuration <= 0f)
			{
				return;
			}

			Vector3 vector = GetPosition();
			float num = arriveRate;
			if (projectileData.isUseCurve)
			{
				num = aniCurve.Evaluate(arriveRate);
			}

			switch (projectileData.localMoveType)
			{
				case ProjectileLocalMoveType.Direct:
					if (projectileData.type == ProjectileType.Target)
					{
						if (target != null)
						{
							finalPosition = target.GetPosition();
						}

						if (targetHitPoint != null)
						{
							finalPosition.y = targetHitPoint.position.y;
						}
					}

					vector = Vector3.Lerp(launchPosition, finalPosition, num);
					if (!isArrived && 0f < num && num < 1f)
					{
						projectileTrans.forward = GameUtil.Direction(lastProjectileObjPosition, vector);
					}

					break;
				case ProjectileLocalMoveType.HighAngle:
					if (projectileData.type == ProjectileType.Target)
					{
						if (target != null)
						{
							finalPosition = target.GetPosition();
						}

						if (targetHitPoint != null)
						{
							finalPosition.y = targetHitPoint.position.y;
						}

						middlePosition = (createdPosition + finalPosition) * 0.5f;
						middlePosition.y = finalPosition.y + projectileData.localHighAngleHeight;
						vector.y = GameUtil.CalculateQuadraticBezierPoint(num, launchPosition, middlePosition,
							finalPosition);
					}
					else if (projectileData.type == ProjectileType.Point)
					{
						vector.y = GameUtil.CalculateQuadraticBezierPoint(num, launchPosition, middlePosition,
							finalPosition);
					}

					break;
				case ProjectileLocalMoveType.Ground:
					vector.y = GameUtil.CalculateLinearBezierPoint(num, launchPosition, finalPosition);
					break;
			}

			projectileTrans.position = vector;
			lastProjectileObjPosition = vector;
		}


		private void CheckObjectCollision()
		{
			if (isArrived)
			{
				if (!projectileData.enableObjectCollsionCheckAfterArrival)
				{
					return;
				}
			}
			else if (!projectileData.enableObjectCollisionCheck)
			{
				return;
			}

			int num = Physics.OverlapSphereNonAlloc(GetPosition(), projectileData.collisionObjectRadius * 0.9f,
				projectileColliders, GameConstants.LayerMask.WORLD_OBJECT_LAYER);
			bool flag = false;
			for (int i = 0; i < num; i++)
			{
				LocalCharacter component = projectileColliders[i].GetComponent<LocalCharacter>();
				if (!(component == null) && component.IsAlive && !component.ObjectType.IsSummonObject() &&
				    !component.IsUntargetable() && component.GetCollisionObject().Collision(GetCollisionObject()))
				{
					if (ProjectileData.collisionHostileType.HasFlag(
						owner.HostileAgent.GetHostileType(component.HostileAgent)))
					{
						if (projectileData.isExplosion && 0f < projectileData.explosionRadius)
						{
							flag = true;
							break;
						}

						collisionCount++;
					}

					if (projectileData.penetrationCount <= collisionCount)
					{
						flag = true;
						break;
					}
				}
			}

			if (flag)
			{
				ArrivedDestination(true);
			}
		}


		protected void ArrivedDestination(bool isCollision)
		{
			if (isArrived)
			{
				if (isCollision && projectileData.enableObjectCollsionCheckAfterArrival)
				{
					StopAllCoroutines();
					HideProjectile();
				}

				return;
			}

			isArrived = true;
			if (!isCollision)
			{
				PlayLocalEffect(projectileData.arrivedEffectAndSoundCode, GameUtil.LookRotation(projectileDirection));
			}

			if (0f < projectileData.lifeTimeAfterArrival)
			{
				this.StartThrowingCoroutine(CoroutineUtil.DelayedAction(projectileData.lifeTimeAfterArrival, delegate
					{
						if (colliderAgent == null)
						{
							HideProjectile();
							return;
						}

						colliderAgent.Init(projectileData.collisionObjectRadius);
						if (0f < projectileData.lifeTimeAfterExplosion)
						{
							this.StartThrowingCoroutine(
								CoroutineUtil.DelayedAction(projectileData.lifeTimeAfterExplosion, HideProjectile),
								delegate(Exception exception)
								{
									Log.E("[EXCEPTION][lifeTimeAfterExplosion] Message:" + exception.Message +
									      ", StackTrace:" + exception.StackTrace);
								});
							return;
						}

						HideProjectile();
					}),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][lifeTimeAfterArrival] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
				return;
			}

			if (colliderAgent == null)
			{
				HideProjectile();
				return;
			}

			colliderAgent.Init(projectileData.collisionObjectRadius);
			if (0f < projectileData.lifeTimeAfterExplosion)
			{
				this.StartThrowingCoroutine(
					CoroutineUtil.DelayedAction(projectileData.lifeTimeAfterExplosion, HideProjectile),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][lifeTimeAfterExplosion2] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
				return;
			}

			HideProjectile();
		}


		protected void HideProjectile()
		{
			if (isHide)
			{
				return;
			}

			isHide = true;
			if (projectileObj == null)
			{
				return;
			}

			if (projectileData.doHideWithParticleSystem)
			{
				projectileObj.GetComponentsInChildren<ParticleSystem>(hideProjectileParticleSystems);
				foreach (ParticleSystem particleSystem in hideProjectileParticleSystems)
				{
					ParticleSystem.MainModule main = particleSystem.main;
					if (main.loop)
					{
						main.loop = false;
						main.stopAction = ParticleSystemStopAction.Destroy;
					}
					else
					{
						Renderer component = particleSystem.GetComponent<Renderer>();
						if (component != null)
						{
							component.enabled = false;
						}
					}
				}
			}

			Destroy(projectileObj, projectileData.localDestroyDelay);
			if (projectileData.localDestroyDelay > 0f)
			{
				projectileTrans.SetParent(null);
			}
		}


		public override void DestroySelf()
		{
			if (!isAlive)
			{
				return;
			}

			isAlive = false;
			HideProjectile();
			if (owner != null)
			{
				owner.RemoveOwnProjectile(this);
			}
		}


		public virtual void OnCollision(LocalCharacter character)
		{
			if (owner == null)
			{
				return;
			}

			character.PlayLocalDamagedEffect(owner, projectileData.collisionTargetEffectAndSoundCode,
				GameUtil.LookRotation(character.GetPosition() - owner.GetPosition()));
		}


		public virtual void OnCollisionWall(Vector3 position) { }


		public void OnExplosion()
		{
			if (!projectileData.isExplosion)
			{
				return;
			}

			if (owner == null)
			{
				return;
			}

			ProjectileType type = projectileData.type;
			if (type == ProjectileType.Point)
			{
				PlayLocalDamagedEffectWorldPoint(owner, projectileData.collisionSelfEffectAndSoundCode, destination,
					GameUtil.LookRotation(projectileDirection));
				return;
			}

			PlayLocalDamagedEffect(owner, projectileData.collisionSelfEffectAndSoundCode,
				GameUtil.LookRotation(projectileDirection));
		}


		private CollisionObject3D GetCollisionObject()
		{
			collisionObject.UpdatePosition(GetPosition());
			return collisionObject;
		}


		private CollisionObjectProperty CreateCollisionProperty()
		{
			switch (projectileData.collisionObjectType)
			{
				case CollisionObjectType.Circle:
					return CollisionObjectProperty.Circle(GetPosition(), projectileData.collisionObjectRadius);
				case CollisionObjectType.Sector:
					return CollisionObjectProperty.Sector(GetPosition(), projectileData.collisionObjectRadius,
						projectileData.collisionObjectAngle, projectileDirection);
				case CollisionObjectType.Box:
					return CollisionObjectProperty.Box(GetPosition(), projectileData.collisionObjectWidth,
						projectileData.collisionObjectDepth, projectileDirection);
				default:
					return CollisionObjectProperty.Circle(GetPosition(), projectileData.collisionObjectRadius);
			}
		}


		public override GameObject LoadProjectile(string projectileName)
		{
			if (Owner != null)
			{
				return owner.LoadProjectile(projectileName);
			}

			return LoadCommonProjectile(projectileName);
		}


		public override GameObject LoadObject(string objectName)
		{
			if (Owner != null)
			{
				return owner.LoadObject(objectName);
			}

			return LoadCommonObject(objectName);
		}


		public override GameObject LoadEffect(string effectName)
		{
			if (Owner != null)
			{
				return owner.LoadEffect(effectName);
			}

			return LoadCommonEffect(effectName);
		}


		public override AudioClip LoadFXSound(string soundName)
		{
			if (Owner != null)
			{
				return owner.LoadFXSound(soundName);
			}

			return LoadCommonFXSound(soundName);
		}


		public override AudioClip LoadVoice(string characterResource, string voiceName, int randomCount = 0)
		{
			if (Owner != null)
			{
				return owner.LoadVoice(characterResource, voiceName, randomCount);
			}

			return SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(voiceName);
		}
	}
}