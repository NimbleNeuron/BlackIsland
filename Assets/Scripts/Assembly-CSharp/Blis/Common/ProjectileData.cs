using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Blis.Common
{
	public class ProjectileData
	{
		public readonly int arrivedEffectAndSoundCode;


		public readonly int attachEffectAndSoundCode;


		public readonly int code;


		public readonly HostileType collisionHostileType;


		public readonly float collisionObjectAngle;


		public readonly float collisionObjectDepth;


		public readonly float collisionObjectRadius;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly CollisionObjectType collisionObjectType;


		public readonly float collisionObjectWidth;


		public readonly int collisionSelfEffectAndSoundCode;


		public readonly int collisionTargetEffectAndSoundCode;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly DamageSubType damageSubType;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly DamageType damageType;


		public readonly float distance;


		public readonly bool doHideWithParticleSystem;


		public readonly float duration;


		public readonly bool enableObjectCollisionCheck;


		public readonly bool enableObjectCollsionCheckAfterArrival;


		public readonly float explosionRadius;


		public readonly string hitPoint;


		public readonly bool isBullet;


		public readonly bool isExplosion;


		public readonly bool isExplosionWithoutCollision;


		public readonly bool isPassWall;


		public readonly bool isUseCurve;


		public readonly float lifeTimeAfterArrival;


		public readonly float lifeTimeAfterExplosion;


		public readonly float localBaseHeight;


		public readonly float localDestroyDelay;


		public readonly float localHighAngleHeight;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ProjectileLocalMoveType localMoveType;


		public readonly int penetrationCount;


		public readonly string prefabName;


		public readonly float serverInterpolationPosition;


		public readonly int shotEffectAndSoundCode;


		public readonly string shotPoint;


		public readonly float speed;


		public readonly bool suffixItemCode;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ProjectileType type;

		[JsonConstructor]
		public ProjectileData(int code, string prefabName, ProjectileType type, bool suffixItemCode,
			bool enableObjectCollisionCheck, bool enableObjectCollsionCheckAfterArrival, bool isPassWall,
			bool isUseCurve, float speed, float duration, float distance, float lifeTimeAfterArrival,
			float lifeTimeAfterExplosion, bool doHideWithParticleSystem, DamageType damageType,
			DamageSubType damageSubType, int penetrationCount, bool isExplosion, bool isExplosionWithoutCollision,
			float explosionRadius, int attachEffectAndSoundCode, int shotEffectAndSoundCode, string shotPoint,
			float serverInterpolationPosition, string hitPoint, int collisionSelfEffectAndSoundCode,
			int arrivedEffectAndSoundCode, int collisionTargetEffectAndSoundCode,
			CollisionObjectType collisionObjectType, float collisionObjectRadius, float collisionObjectAngle,
			float collisionObjectWidth, float collisionObjectDepth, int collisionHostileType, bool isBullet,
			ProjectileLocalMoveType localMoveType, float localHighAngleHeight, float localBaseHeight,
			float localDestroyDelay)
		{
			this.code = code;
			this.prefabName = prefabName;
			this.type = type;
			this.suffixItemCode = suffixItemCode;
			this.enableObjectCollisionCheck = enableObjectCollisionCheck;
			this.enableObjectCollsionCheckAfterArrival = enableObjectCollsionCheckAfterArrival;
			this.isPassWall = isPassWall;
			this.isUseCurve = type == ProjectileType.Direction && isUseCurve;
			this.speed = speed;
			this.duration = duration;
			this.distance = distance;
			this.lifeTimeAfterArrival = lifeTimeAfterArrival;
			this.lifeTimeAfterExplosion = lifeTimeAfterExplosion;
			this.doHideWithParticleSystem = doHideWithParticleSystem;
			this.damageType = damageType;
			this.damageSubType = damageSubType;
			this.penetrationCount = penetrationCount;
			this.isExplosion = isExplosion;
			this.isExplosionWithoutCollision = isExplosionWithoutCollision;
			this.explosionRadius = explosionRadius;
			this.attachEffectAndSoundCode = attachEffectAndSoundCode;
			this.shotEffectAndSoundCode = shotEffectAndSoundCode;
			this.shotPoint = shotPoint;
			this.serverInterpolationPosition = serverInterpolationPosition;
			this.hitPoint = hitPoint;
			this.collisionSelfEffectAndSoundCode = collisionSelfEffectAndSoundCode;
			this.arrivedEffectAndSoundCode = arrivedEffectAndSoundCode;
			this.collisionTargetEffectAndSoundCode = collisionTargetEffectAndSoundCode;
			this.collisionObjectType = collisionObjectType;
			this.collisionObjectAngle = collisionObjectAngle;
			this.collisionObjectWidth = collisionObjectWidth;
			this.collisionObjectDepth = collisionObjectDepth;
			this.collisionHostileType = (HostileType) collisionHostileType;
			this.isBullet = isBullet;
			this.localMoveType = localMoveType;
			this.localHighAngleHeight = localHighAngleHeight;
			this.localBaseHeight = localBaseHeight;
			this.localDestroyDelay = localDestroyDelay;
			if (collisionObjectType == CollisionObjectType.Box)
			{
				this.collisionObjectRadius = Mathf.Sqrt(this.collisionObjectWidth * this.collisionObjectWidth +
				                                        (this.collisionObjectDepth + this.collisionObjectDepth));
				return;
			}

			this.collisionObjectRadius = collisionObjectRadius;
		}


		public bool IsInstantArrival()
		{
			return duration <= 0f && speed <= 0f;
		}
	}
}