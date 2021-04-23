using Newtonsoft.Json;

namespace Blis.Common
{
	public class SkillData
	{
		public readonly int activeLevel;
		public readonly float angle;
		public readonly int code;
		public readonly float concentrationCancelCooldown;
		public readonly float cooldown;
		public readonly int cost;
		public readonly int exCost;
		public readonly int group;
		public readonly float innerRange;
		public readonly float length;
		public readonly int level;
		public readonly int maxStack;
		public readonly float range;
		public readonly float stackUseIntervalTime;
		public readonly float width;

		[JsonIgnore] private SkillGroupData skillGroupDataCache;

		[JsonConstructor]
		public SkillData(int code, int group, int level, int activeLevel, int cost, int exCost, float cooldown,
			float stackUseIntervalTime, int maxStack, float concentrationCancelCooldown, float range, float innerRange,
			float length, float width, float angle)
		{
			this.code = code;
			this.group = group;
			this.level = level;
			this.activeLevel = activeLevel;
			this.cost = cost;
			this.exCost = exCost;
			this.cooldown = cooldown;
			this.stackUseIntervalTime = stackUseIntervalTime;
			this.maxStack = maxStack;
			this.concentrationCancelCooldown = concentrationCancelCooldown;
			this.range = range;
			this.innerRange = innerRange;
			this.length = length;
			this.width = width;
			this.angle = angle;
		}


		[JsonIgnore]
		private SkillGroupData SkillGroupData {
			get
			{
				if (skillGroupDataCache == null)
				{
					skillGroupDataCache = GameDB.skill.GetSkillGroupData(group);
					if (skillGroupDataCache == null)
					{
						Log.W("No SkillGroupData | code: {0}, group: {1}", code, group);
					}
				}

				return skillGroupDataCache;
			}
		}


		[JsonIgnore]
		public int RepresentGroup {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return 0;
				}

				return skillGroupData.representGroup;
			}
		}


		[JsonIgnore]
		public string Name {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return (skillGroupData != null ? skillGroupData.name : null) ?? null;
			}
		}


		[JsonIgnore]
		public SkillId SkillId {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SkillId.None;
				}

				return skillGroupData.skillId;
			}
		}


		[JsonIgnore]
		public SkillId PassiveSkillId {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SkillId.None;
				}

				return skillGroupData.passiveSkillId;
			}
		}


		[JsonIgnore]
		public SkillCastWaysType CastWaysType {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SkillCastWaysType.Instant;
				}

				return skillGroupData.castWaysType;
			}
		}


		[JsonIgnore]
		public SkillTargetType TargetType {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SkillTargetType.Self;
				}

				return skillGroupData.targetType;
			}
		}


		[JsonIgnore]
		public SkillCastType CastType {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SkillCastType.Casting;
				}

				return skillGroupData.castType;
			}
		}


		[JsonIgnore]
		public float CastingTime1 {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return 0f;
				}

				return skillGroupData.castingTime1;
			}
		}


		[JsonIgnore]
		public CastingBarType CastingBarType1 {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return CastingBarType.LeftToRight;
				}

				return skillGroupData.castingBarType1;
			}
		}


		[JsonIgnore]
		public float ConcentrationTime {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return 0f;
				}

				return skillGroupData.concentrationTime;
			}
		}


		[JsonIgnore]
		public CastingBarType ConcentrationBarType {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return CastingBarType.LeftToRight;
				}

				return skillGroupData.concentrationBarType;
			}
		}


		[JsonIgnore]
		public float CastingTime2 {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return 0f;
				}

				return skillGroupData.castingTime2;
			}
		}


		[JsonIgnore]
		public float FinishDelayTime {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return 0f;
				}

				return skillGroupData.finishDelayTime;
			}
		}


		[JsonIgnore]
		public CastingBarType CastingBarType2 {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return CastingBarType.LeftToRight;
				}

				return skillGroupData.castingBarType2;
			}
		}


		[JsonIgnore]
		public SkillPlayType PlayType {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SkillPlayType.Alone;
				}

				return skillGroupData.playType;
			}
		}


		[JsonIgnore]
		public SkillInterruptType InterruptType {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SkillInterruptType.None;
				}

				return skillGroupData.interruptType;
			}
		}


		[JsonIgnore]
		public SkillInterruptHandlingType InterruptHandlingType {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SkillInterruptHandlingType.Ignore;
				}

				return skillGroupData.interruptHandlingType;
			}
		}


		[JsonIgnore]
		public bool CanCastingWhileCCState {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.canCastingWhileCCState;
			}
		}


		[JsonIgnore]
		public bool AggressiveSkill {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.aggressiveSkill;
			}
		}


		[JsonIgnore]
		public bool MovementSkill {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.movementSkill;
			}
		}


		[JsonIgnore]
		public SkillCostType CostType {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SkillCostType.NoCost;
				}

				return skillGroupData.costType;
			}
		}


		[JsonIgnore]
		public int CostKey {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return 0;
				}

				return skillGroupData.costKey;
			}
		}


		[JsonIgnore]
		public SkillCostType ExCostType {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SkillCostType.NoCost;
				}

				return skillGroupData.exCostType;
			}
		}


		[JsonIgnore]
		public int ExCostKey {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return 0;
				}

				return skillGroupData.exCostKey;
			}
		}


		[JsonIgnore]
		public SkillCooldownType CooldownType {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SkillCooldownType.None;
				}

				return skillGroupData.cooldownType;
			}
		}


		[JsonIgnore]
		public int CooldownStateFinish {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return 0;
				}

				return skillGroupData.cooldownStateFinish;
			}
		}


		[JsonIgnore]
		public bool UseWeaponRange {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.useWeaponRange;
			}
		}


		[JsonIgnore]
		public bool StopWhenStartSkill {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.stopWhenStartSkill;
			}
		}


		[JsonIgnore]
		public bool StopAttackWhenStartSkill {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.stopAttackWhenStartSkill;
			}
		}


		[JsonIgnore]
		public bool StopWhenCastReserveSkill {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.stopWhenCastReserveSkill;
			}
		}


		[JsonIgnore]
		public bool StartPrevMoveWhenFinishSkill {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.startPrevMoveWhenFinishSkill;
			}
		}


		[JsonIgnore]
		public bool CanAdditionalAction {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.canAdditionalAction;
			}
		}


		[JsonIgnore]
		public AdditionalAction AdditionalAction {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return AdditionalAction.None;
				}

				return skillGroupData.additionalAction;
			}
		}


		[JsonIgnore]
		public float cooldownForAdditionalAction {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return 0f;
				}

				return skillGroupData.cooldownForAdditionalAction;
			}
		}


		[JsonIgnore]
		public bool NeedInputForCast {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.needInputForCast;
			}
		}


		[JsonIgnore]
		public bool OnlyMoveInputWhileSkillPlaying {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.onlyMoveInputWhileSkillPlaying;
			}
		}


		[JsonIgnore]
		public bool CanMoveDuringSkillPlaying {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.canMoveDuringSkillPlaying;
			}
		}


		[JsonIgnore]
		public SequenceIncreaseType SequenceIncreaseType {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SequenceIncreaseType.None;
				}

				return skillGroupData.sequenceIncreaseType;
			}
		}


		[JsonIgnore]
		public SequenceItemCooldownApply ItemCooldownApply {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return SequenceItemCooldownApply.None;
				}

				return skillGroupData.itemCooldownApply;
			}
		}


		[JsonIgnore]
		public float SequenceCooldown {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return 0f;
				}

				return skillGroupData.sequenceCooldown;
			}
		}


		[JsonIgnore]
		public float CastWaitTime {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				if (skillGroupData == null)
				{
					return 0f;
				}

				return skillGroupData.castWaitTime;
			}
		}


		[JsonIgnore]
		public bool Evolutionable {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return skillGroupData != null && skillGroupData.evolutionable;
			}
		}


		[JsonIgnore]
		public string Guideline {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return (skillGroupData != null ? skillGroupData.guideline : null) ?? null;
			}
		}


		[JsonIgnore]
		public string SubGuideline {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return (skillGroupData != null ? skillGroupData.subGuideline : null) ?? null;
			}
		}


		[JsonIgnore]
		public string Icon {
			get
			{
				SkillGroupData skillGroupData = SkillGroupData;
				return (skillGroupData != null ? skillGroupData.icon : null) ?? null;
			}
		}


		public bool InstantCast()
		{
			return CastWaysType == SkillCastWaysType.Instant;
		}
	}
}