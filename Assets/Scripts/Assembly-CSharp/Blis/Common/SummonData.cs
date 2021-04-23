using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class SummonData
	{
		public readonly float attackDelay;


		public readonly int attackPower;


		public readonly float attackRange;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly CastingActionType castingActionType;


		public readonly int code;


		public readonly float createRange;


		public readonly string createSound;


		public readonly int destroyEffectAndSoundCode;


		public readonly bool detectInvisible;


		public readonly bool detectShare;


		public readonly float duration;


		public readonly bool isFetter;


		public readonly bool isInvincibility;


		public readonly bool isPickingTarget;


		public readonly float localDestroyDelay;


		public readonly int maxHp;


		public readonly string name;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ObjectType objectType;


		public readonly float pileRange;


		public readonly string prefabPath;


		public readonly float radius;


		public readonly float rangeRadius;


		public readonly float sightRange;


		public readonly bool sightShare;


		public readonly int spawnEffectAndSoundCode;


		public readonly int stateEffect;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SummonAttackType summonAttackType;


		public readonly string targetHitEffect;


		public readonly float uiHeight;


		public float attackSpeed;


		public float createVisibleTime;

		[JsonConstructor]
		public SummonData(int code, string name, ObjectType objectType, bool isInvincibility, bool isPickingTarget,
			float duration, float createRange, float createVisibleTime, int maxHp, int attackPower, float attackRange,
			float attackSpeed, SummonAttackType summonAttackType, float rangeRadius, float attackDelay, int stateEffect,
			string targetHitEffect, float radius, float uiHeight, float sightRange, bool sightShare, bool detectShare,
			bool detectInvisible, float localDestroyDelay, string createSound, string prefabPath,
			int spawnEffectAndSoundCode, int destroyEffectAndSoundCode, CastingActionType castingActionType,
			float pileRange)
		{
			this.code = code;
			this.name = name;
			this.objectType = objectType;
			this.isInvincibility = isInvincibility;
			this.isPickingTarget = isPickingTarget;
			this.duration = duration;
			this.createRange = createRange;
			this.createVisibleTime = createVisibleTime;
			this.maxHp = maxHp;
			this.attackPower = attackPower;
			this.attackRange = attackRange;
			this.attackSpeed = attackSpeed;
			this.summonAttackType = summonAttackType;
			this.rangeRadius = rangeRadius;
			this.attackDelay = attackDelay;
			this.stateEffect = stateEffect;
			this.targetHitEffect = targetHitEffect;
			this.radius = radius;
			this.uiHeight = uiHeight;
			this.sightRange = sightRange;
			this.sightShare = sightShare;
			this.detectShare = detectShare;
			this.detectInvisible = detectInvisible;
			this.localDestroyDelay = localDestroyDelay;
			this.createSound = createSound;
			this.prefabPath = Path.GetFileName(prefabPath);
			this.spawnEffectAndSoundCode = spawnEffectAndSoundCode;
			this.destroyEffectAndSoundCode = destroyEffectAndSoundCode;
			this.castingActionType = castingActionType;
			this.pileRange = pileRange;
		}


		public SummonData Copy()
		{
			return (SummonData) MemberwiseClone();
		}
	}
}