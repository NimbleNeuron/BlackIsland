using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class ItemWeaponData : ItemData
	{
		public readonly int attackPower;


		public readonly float attackRange;


		public readonly float attackSpeedRatio;


		public readonly bool consumable;


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


		public readonly float moveSpeed;


		public readonly float sightRange;


		public readonly float spRegen;


		public readonly float spRegenRatio;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly WeaponType weaponType;

		[JsonConstructor]
		public ItemWeaponData(int code, string name, ItemType itemType, ItemGrade itemGrade, int stackable,
			int initialCount, int makeMaterial1, int makeMaterial2, string craftAnimTrigger, WeaponType weaponType,
			bool consumable, int attackPower, int defense, int maxHp, float hpRegenRatio, float hpRegen,
			float spRegenRatio, float spRegen, float attackSpeedRatio, float moveSpeed, float sightRange,
			float criticalStrikeChance, float criticalStrikeDamage, float cooldownReduction, float lifeSteal,
			float attackRange, float increaseBasicAttackDamage, float increaseSkillDamage,
			float increaseSkillDamageRatio, int decreaseRecoveryToBasicAttack, int decreaseRecoveryToSkill) : base(code,
			name, itemType, itemGrade, stackable, initialCount, makeMaterial1, makeMaterial2, craftAnimTrigger)
		{
			this.weaponType = weaponType;
			this.consumable = consumable;
			this.attackPower = attackPower;
			this.defense = defense;
			this.maxHp = maxHp;
			this.hpRegenRatio = hpRegenRatio;
			this.hpRegen = hpRegen;
			this.spRegenRatio = spRegenRatio;
			this.spRegen = spRegen;
			this.attackSpeedRatio = attackSpeedRatio;
			this.moveSpeed = moveSpeed;
			this.sightRange = sightRange;
			this.criticalStrikeChance = criticalStrikeChance;
			this.criticalStrikeDamage = criticalStrikeDamage;
			this.cooldownReduction = cooldownReduction;
			this.lifeSteal = lifeSteal;
			this.attackRange = attackRange;
			this.increaseBasicAttackDamage = increaseBasicAttackDamage;
			this.increaseSkillDamage = increaseSkillDamage;
			this.increaseSkillDamageRatio = increaseSkillDamageRatio;
			this.decreaseRecoveryToBasicAttack = decreaseRecoveryToBasicAttack;
			this.decreaseRecoveryToSkill = decreaseRecoveryToSkill;
		}


		public override int GetSubType()
		{
			return (int) weaponType;
		}


		public override MasteryConditionType GetMasteryConditionType()
		{
			if (weaponType == WeaponType.None)
			{
				return MasteryConditionType.None;
			}

			return MasteryConditionType.CraftWeapon;
		}


		public override MasteryType GetMasteryType()
		{
			return weaponType.GetWeaponMasteryType();
		}


		public override bool IsGunType()
		{
			return weaponType.IsGunType();
		}


		public override bool IsThrowType()
		{
			return weaponType.IsThrowType();
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