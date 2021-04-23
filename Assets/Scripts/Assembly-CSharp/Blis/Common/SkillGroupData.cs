using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class SkillGroupData
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly AdditionalAction additionalAction;

		public readonly bool aggressiveSkill;
		public readonly bool canCastingWhileCCState;
		public readonly bool canMoveDuringSkillPlaying;

		[JsonProperty] private readonly string canNotSpecialSkillId;
		[JsonIgnore] public readonly List<SpecialSkillId> canNotSpecialSkillIds;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly CastingBarType castingBarType1;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly CastingBarType castingBarType2;

		public readonly float castingTime1;
		public readonly float castingTime2;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillCastType castType;

		public readonly float castWaitTime;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillCastWaysType castWaysType;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly CastingBarType concentrationBarType;

		public readonly float concentrationTime;
		public readonly float cooldownForAdditionalAction;
		public readonly int cooldownStateFinish;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillCooldownType cooldownType;

		public readonly int costKey;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillCostType costType;

		public readonly bool evolutionable;
		public readonly int exCostKey;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillCostType exCostType;

		public readonly float finishDelayTime;

		public readonly int group;
		public readonly string guideline;
		public readonly string icon;

		public readonly bool impossibleDyingConditionTarget;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillInterruptHandlingType interruptHandlingType;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillInterruptType interruptType;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SequenceItemCooldownApply itemCooldownApply;

		public readonly bool movementSkill;
		public readonly string name;
		public readonly bool needInputForCast;
		public readonly bool onlyMoveInputWhileSkillPlaying;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillId passiveSkillId;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillPlayType playType;

		public readonly int representGroup;
		public readonly float sequenceCooldown;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SequenceIncreaseType sequenceIncreaseType;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillId skillId;

		public readonly bool stackAble;
		public readonly bool startPrevMoveWhenFinishSkill;
		public readonly bool stopAttackWhenStartSkill;
		public readonly bool stopWhenCastReserveSkill;
		public readonly bool stopWhenStartSkill;
		public readonly string subGuideline;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillTargetType targetType;

		public readonly bool useWeaponRange;


		[JsonConstructor]
		public SkillGroupData(int group, int representGroup, string skillId, string passiveSkillId, string name,
			SkillCastWaysType castWaysType, SkillTargetType targetType, SkillCastType castType, float castingTime1,
			CastingBarType castingBarType1, float concentrationTime, CastingBarType concentrationBarType,
			float castingTime2, float finishDelayTime, CastingBarType castingBarType2, SkillPlayType playType,
			SkillInterruptType interruptType, SkillInterruptHandlingType interruptHandlingType,
			bool canCastingWhileCCState, string canNotSpecialSkillId, bool aggressiveSkill, bool movementSkill,
			SkillCostType costType, int costKey, SkillCostType exCostType, int exCostKey,
			SkillCooldownType cooldownType, int cooldownStateFinish, bool useWeaponRange, bool stopWhenStartSkill,
			bool stopAttackWhenStartSkill, bool stopWhenCastReserveSkill, bool startPrevMoveWhenFinishSkill,
			bool onlyMoveInputWhileSkillPlaying, bool canMoveDuringSkillPlaying, AdditionalAction additionalAction,
			float cooldownForAdditionalAction, bool needInputForCast, SequenceIncreaseType sequenceIncreaseType,
			SequenceItemCooldownApply itemCooldownApply, float sequenceCooldown, float castWaitTime, bool evolutionable,
			bool stackAble, bool impossibleDyingConditionTarget, string guideline, string subGuideline, string icon)
		{
			this.group = group;
			this.representGroup = representGroup;
			this.name = name;
			this.castWaysType = castWaysType;
			this.targetType = targetType;
			this.castType = castType;
			this.castingTime1 = castingTime1;
			this.castingBarType1 = castingBarType1;
			this.concentrationTime = concentrationTime;
			this.concentrationBarType = concentrationBarType;
			this.castingTime2 = castingTime2;
			this.finishDelayTime = finishDelayTime;
			this.castingBarType2 = castingBarType2;
			this.playType = playType;
			this.interruptType = interruptType;
			this.interruptHandlingType = interruptHandlingType;
			this.canCastingWhileCCState = canCastingWhileCCState;
			this.canNotSpecialSkillId = canNotSpecialSkillId;
			this.aggressiveSkill = aggressiveSkill;
			this.movementSkill = movementSkill;
			this.costType = costType;
			this.costKey = costKey;
			this.exCostType = exCostType;
			this.exCostKey = exCostKey;
			this.cooldownType = cooldownType;
			this.cooldownStateFinish = cooldownStateFinish;
			this.useWeaponRange = useWeaponRange;
			this.stopWhenStartSkill = stopWhenStartSkill;
			this.stopAttackWhenStartSkill = stopAttackWhenStartSkill;
			this.stopWhenCastReserveSkill = stopWhenCastReserveSkill;
			this.startPrevMoveWhenFinishSkill = startPrevMoveWhenFinishSkill;
			this.onlyMoveInputWhileSkillPlaying = onlyMoveInputWhileSkillPlaying;
			this.canMoveDuringSkillPlaying = canMoveDuringSkillPlaying;
			this.additionalAction = additionalAction;
			this.cooldownForAdditionalAction = cooldownForAdditionalAction;
			this.needInputForCast = needInputForCast;
			this.sequenceIncreaseType = sequenceIncreaseType;
			this.itemCooldownApply = itemCooldownApply;
			this.sequenceCooldown = sequenceCooldown;
			this.castWaitTime = castWaitTime;
			this.stackAble = stackAble;
			this.evolutionable = evolutionable;
			this.impossibleDyingConditionTarget = impossibleDyingConditionTarget;
			this.guideline = guideline;
			this.subGuideline = subGuideline;
			this.icon = icon;
			canNotSpecialSkillIds = new List<SpecialSkillId>();
			if (canNotSpecialSkillId != "None")
			{
				string[] array = canNotSpecialSkillId.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					SpecialSkillId item;
					if (Enum.TryParse<SpecialSkillId>(array[i], true, out item))
					{
						canNotSpecialSkillIds.Add(item);
					}
				}
			}

			if (!Enum.TryParse<SkillId>(skillId, true, out this.skillId))
			{
				this.skillId = SkillId.None;
			}

			if (!Enum.TryParse<SkillId>(passiveSkillId, true, out this.passiveSkillId))
			{
				this.passiveSkillId = SkillId.None;
			}
		}


		public bool canAdditionalAction => additionalAction > AdditionalAction.None;
	}
}