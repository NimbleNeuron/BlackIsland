using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Blis.Common
{
	
	public class CharacterStateGroupData
	{
		
		public readonly bool activateStatCalculatorOnCreate;

		
		public readonly bool alwaysShowStack;

		
		public readonly bool debuffCombatTarget;

		
		public readonly int defaultStack;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly EffectType effectType;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ForcedMoveSpeedType forcedMoveSpeedType;

		
		public readonly int group;

		
		public readonly string iconName;

		
		public readonly string name;

		
		public readonly bool notCheckCasterId;

		
		public readonly bool onlyShowMine;

		
		public readonly string polymorphPrefabPath;

		
		public readonly bool removeOnDead;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillId skillId;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly StateStatCalculationType statCalculationType;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly StateBehaviourType stateBehaviourType;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly StateType stateType;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly StateUIBehaviourType uiBehaviourType;

		
		public readonly bool uiVisible;

		
		[JsonConstructor]
		public CharacterStateGroupData(int group, string skillId, string name, EffectType effectType,
			StateType stateType, StateBehaviourType stateBehaviourType, StateStatCalculationType statCalculationType,
			bool activateStatCalculatorOnCreate, StateUIBehaviourType uiBehaviourType, bool uiVisible,
			bool onlyShowMine, int defaultStack, bool alwaysShowStack, bool notCheckCasterId, bool removeOnDead,
			ForcedMoveSpeedType forcedMoveSpeedType, bool debuffCombatTarget, string iconName,
			string polymorphPrefabPath)
		{
			this.group = group;
			if (!Enum.TryParse<SkillId>(skillId, true, out this.skillId))
			{
				this.skillId = SkillId.None;
			}

			this.name = name;
			this.effectType = effectType;
			this.stateType = stateType;
			this.stateBehaviourType = stateBehaviourType;
			this.statCalculationType = statCalculationType;
			this.activateStatCalculatorOnCreate = activateStatCalculatorOnCreate;
			this.uiBehaviourType = uiBehaviourType;
			this.uiVisible = uiVisible;
			this.onlyShowMine = onlyShowMine;
			this.defaultStack = defaultStack;
			this.alwaysShowStack = alwaysShowStack;
			this.notCheckCasterId = notCheckCasterId;
			this.removeOnDead = removeOnDead;
			this.forcedMoveSpeedType = forcedMoveSpeedType;
			this.debuffCombatTarget = debuffCombatTarget;
			this.iconName = iconName;
			this.polymorphPrefabPath = Path.GetFileName(polymorphPrefabPath);
		}

		
		public Sprite GetSprite()
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.GetSkillsSprite(iconName);
		}
	}
}