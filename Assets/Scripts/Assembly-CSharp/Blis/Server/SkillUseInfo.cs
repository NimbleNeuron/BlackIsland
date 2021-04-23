using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class SkillUseInfo
	{
		
		
		public int SkillCode
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0;
				}
				return skillData.code;
			}
		}

		
		
		public int SkillGroup
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0;
				}
				return skillData.group;
			}
		}

		
		
		public int SkillLevel
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0;
				}
				return skillData.level;
			}
		}

		
		
		public float SkillRange
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0f;
				}
				return skillData.range;
			}
		}

		
		
		public float SkillInnerRange
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0f;
				}
				return skillData.innerRange;
			}
		}

		
		
		public float SkillLength
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0f;
				}
				return skillData.length;
			}
		}

		
		
		public float SkillAngle
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0f;
				}
				return skillData.angle;
			}
		}

		
		
		public float SkillWidth
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0f;
				}
				return skillData.width;
			}
		}

		
		
		public SkillCostType SkillCostType
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return SkillCostType.NoCost;
				}
				return skillData.CostType;
			}
		}

		
		
		public int SkillCostKey
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0;
				}
				return skillData.CostKey;
			}
		}

		
		
		public int SkillCost
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0;
				}
				return skillData.cost;
			}
		}

		
		
		public SkillCostType ExSkillCostType
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return SkillCostType.NoCost;
				}
				return skillData.ExCostType;
			}
		}

		
		
		public int ExSkillCostKey
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0;
				}
				return skillData.ExCostKey;
			}
		}

		
		
		public int ExSkillCost
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0;
				}
				return skillData.exCost;
			}
		}

		
		
		public float SkillCooldown
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0f;
				}
				return skillData.cooldown;
			}
		}

		
		
		public float SkillCastingTime1
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0f;
				}
				return skillData.CastingTime1;
			}
		}

		
		
		public float SkillConcentrationTime
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0f;
				}
				return skillData.ConcentrationTime;
			}
		}

		
		
		public float SkillCastingTime2
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0f;
				}
				return skillData.CastingTime2;
			}
		}

		
		
		public float FinishDelayTime
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return 0f;
				}
				return skillData.FinishDelayTime;
			}
		}

		
		
		public SkillCastWaysType SkillCastWaysType
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return SkillCastWaysType.Instant;
				}
				return skillData.CastWaysType;
			}
		}

		
		
		public SkillCastType SkillCastType
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return SkillCastType.Casting;
				}
				return skillData.CastType;
			}
		}

		
		
		public SkillTargetType SkillTargetType
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return SkillTargetType.Self;
				}
				return skillData.TargetType;
			}
		}

		
		
		public SkillPlayType SkillPlayType
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return SkillPlayType.Alone;
				}
				return skillData.PlayType;
			}
		}

		
		
		public bool CanAdditionalAction
		{
			get
			{
				SkillData skillData = this.skillData;
				return skillData != null && skillData.CanAdditionalAction;
			}
		}

		
		
		public bool CanMoveDuringSkillPlaying
		{
			get
			{
				SkillData skillData = this.skillData;
				return skillData != null && skillData.CanMoveDuringSkillPlaying;
			}
		}

		
		
		public SequenceIncreaseType SequenceIncreaseType
		{
			get
			{
				SkillData skillData = this.skillData;
				if (skillData == null)
				{
					return SequenceIncreaseType.None;
				}
				return skillData.SequenceIncreaseType;
			}
		}

		
		
		public int StateCode
		{
			get
			{
				CharacterStateData characterStateData = this.stateData;
				if (characterStateData == null)
				{
					return 0;
				}
				return characterStateData.code;
			}
		}

		
		
		public int StateGroup
		{
			get
			{
				CharacterStateData characterStateData = this.stateData;
				if (characterStateData == null)
				{
					return 0;
				}
				return characterStateData.group;
			}
		}

		
		
		public float StateDuration
		{
			get
			{
				CharacterStateData characterStateData = this.stateData;
				if (characterStateData == null)
				{
					return 0f;
				}
				return characterStateData.duration;
			}
		}

		
		
		public StateType StateType
		{
			get
			{
				if (this.stateData == null)
				{
					return StateType.Common;
				}
				return GameDB.characterState.GetGroupData(this.stateData.group).stateType;
			}
		}

		
		private SkillUseInfo()
		{
		}

		
		private SkillUseInfo(SkillUseInfo info)
		{
			this.caster = info.caster;
			this.target = info.target;
			this.skillData = info.skillData;
			this.skillSlotSet = info.skillSlotSet;
			this.weaponSkillMastery = info.weaponSkillMastery;
			this.skillEvolutionLevel = info.skillEvolutionLevel;
			this.cursorPosition = info.cursorPosition;
			this.releasePosition = info.releasePosition;
			this.stateData = info.stateData;
		}

		
		public static SkillUseInfo Create(SkillUseInfo info)
		{
			return new SkillUseInfo(info);
		}

		
		public static SkillUseInfo Create(SkillAgent caster, SkillAgent target, SkillData skillData, SkillSlotSet skillSlotSet, MasteryType weaponSkillMastery, int skillEvolutionLevel, Vector3 cursorPosition, Vector3 releasePosition, CharacterStateData stateData, bool injected)
		{
			return new SkillUseInfo
			{
				caster = caster,
				target = target,
				skillData = skillData,
				skillSlotSet = skillSlotSet,
				weaponSkillMastery = weaponSkillMastery,
				skillEvolutionLevel = skillEvolutionLevel,
				cursorPosition = cursorPosition,
				releasePosition = releasePosition,
				stateData = stateData,
				injected = injected
			};
		}

		
		public static SkillUseInfo Target(SkillAgent caster, SkillSlotSet skillSlotSet, MasteryType weaponSkillMastery, int skillEvolutionLevel, SkillAgent target)
		{
			return new SkillUseInfo
			{
				caster = caster,
				target = target,
				skillData = caster.GetSkillData(skillSlotSet),
				skillSlotSet = skillSlotSet,
				weaponSkillMastery = weaponSkillMastery,
				skillEvolutionLevel = skillEvolutionLevel,
				cursorPosition = Vector3.zero,
				releasePosition = Vector3.zero,
				stateData = null,
				injected = false
			};
		}

		
		public static SkillUseInfo Point(SkillAgent caster, SkillSlotSet skillSlotSet, MasteryType weaponSkillMastery, int skillEvolutionLevel, Vector3 cursorPosition, Vector3 releasePosition)
		{
			return new SkillUseInfo
			{
				caster = caster,
				target = null,
				skillData = caster.GetSkillData(skillSlotSet),
				skillSlotSet = skillSlotSet,
				weaponSkillMastery = weaponSkillMastery,
				skillEvolutionLevel = skillEvolutionLevel,
				cursorPosition = cursorPosition,
				releasePosition = releasePosition,
				stateData = null,
				injected = false
			};
		}

		
		public SkillAgent caster;

		
		public SkillAgent target;

		
		public SkillData skillData;

		
		public SkillSlotSet skillSlotSet;

		
		public MasteryType weaponSkillMastery;

		
		public int skillEvolutionLevel;

		
		public Vector3 cursorPosition;

		
		public Vector3 releasePosition;

		
		public CharacterStateData stateData;

		
		public bool injected;
	}
}
