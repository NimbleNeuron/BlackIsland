using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class ItemArmorData : ItemData
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ArmorType armorType;


		public readonly int attackPower;


		public readonly float attackRange;


		public readonly float attackSpeedRatio;


		public readonly float cooldownReduction;


		public readonly float criticalStrikeChance;


		public readonly float criticalStrikeDamage;


		public readonly int decreaseRecoveryToBasicAttack;


		public readonly int decreaseRecoveryToSkill;


		public readonly int defense;


		public readonly float hpRegen;


		public readonly float hpRegenRatio;


		public readonly float increaseBasicAttackDamage;


		public readonly float increaseSkillDamage;


		public readonly float increaseSkillDamageRatio;


		public readonly float lifeSteal;


		public readonly int maxHp;


		public readonly int maxSp;


		public readonly float moveSpeed;


		public readonly float outOfCombatMoveSpeed;


		public readonly float preventBasicAttackDamaged;


		public readonly float preventCriticalStrikeDamaged;


		public readonly float preventSkillDamagedRatio;


		public readonly float sightRange;


		public readonly float spRegen;


		public readonly float spRegenRatio;

		[JsonConstructor]
		public ItemArmorData(int code, string name, ItemType itemType, ItemGrade itemGrade, int stackable,
			int initialCount, int makeMaterial1, int makeMaterial2, string craftAnimTrigger, ArmorType armorType,
			int attackPower, int defense, int maxHp, int maxSp, float hpRegen, float spRegen, float hpRegenRatio,
			float spRegenRatio, float attackSpeedRatio, float moveSpeed, float sightRange, float criticalStrikeChance,
			float criticalStrikeDamage, float preventCriticalStrikeDamaged, float cooldownReduction, float lifeSteal,
			float outOfCombatMoveSpeed, float increaseBasicAttackDamage, float preventBasicAttackDamaged,
			float increaseSkillDamage, float increaseSkillDamageRatio, float preventSkillDamagedRatio,
			int decreaseRecoveryToBasicAttack, int decreaseRecoveryToSkill, float attackRange) : base(code, name,
			itemType, itemGrade, stackable, initialCount, makeMaterial1, makeMaterial2, craftAnimTrigger)
		{
			this.armorType = armorType;
			this.attackPower = attackPower;
			this.defense = defense;
			this.maxHp = maxHp;
			this.maxSp = maxSp;
			this.hpRegen = hpRegen;
			this.spRegen = spRegen;
			this.hpRegenRatio = hpRegenRatio;
			this.spRegenRatio = spRegenRatio;
			this.attackSpeedRatio = attackSpeedRatio;
			this.moveSpeed = moveSpeed;
			this.sightRange = sightRange;
			this.criticalStrikeChance = criticalStrikeChance;
			this.criticalStrikeDamage = criticalStrikeDamage;
			this.preventCriticalStrikeDamaged = preventCriticalStrikeDamaged;
			this.cooldownReduction = cooldownReduction;
			this.lifeSteal = lifeSteal;
			this.outOfCombatMoveSpeed = outOfCombatMoveSpeed;
			this.increaseBasicAttackDamage = increaseBasicAttackDamage;
			this.preventBasicAttackDamaged = preventBasicAttackDamaged;
			this.increaseSkillDamage = increaseSkillDamage;
			this.increaseSkillDamageRatio = increaseSkillDamageRatio;
			this.preventSkillDamagedRatio = preventSkillDamagedRatio;
			this.decreaseRecoveryToBasicAttack = decreaseRecoveryToBasicAttack;
			this.decreaseRecoveryToSkill = decreaseRecoveryToSkill;
			this.attackRange = attackRange;
		}


		public override int GetSubType()
		{
			return (int) armorType;
		}


		public override MasteryConditionType GetMasteryConditionType()
		{
			switch (armorType)
			{
				case ArmorType.None:
					return MasteryConditionType.None;
				case ArmorType.Head:
					return MasteryConditionType.CraftHead;
				case ArmorType.Chest:
					return MasteryConditionType.CraftChest;
				case ArmorType.Arm:
					return MasteryConditionType.CraftArm;
				case ArmorType.Leg:
					return MasteryConditionType.CraftLeg;
				case ArmorType.Trinket:
					return MasteryConditionType.CraftTrinket;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		public override float GetStatValue(StatType statType)
		{
			if (statType == StatType.SightRange)
			{
				return sightRange;
			}

			return 0f;
		}
	}
}