namespace Blis.Common
{
	public class ItemStat
	{
		public int attackPower;


		public float attackRange;


		public float attackSpeed;


		public float attackSpeedRatio;


		public float cooldownReduction;


		public float criticalStrikeChance;


		public float criticalStrikeDamage;


		public int decreaseRecoveryToBasicAttack;


		public int decreaseRecoveryToSkill;


		public int defense;


		public float hpRegen;


		public float hpRegenRatio;


		public float increaseBasicAttackDamage;


		public float increaseSkillDamage;


		public float increaseSkillDamageRatio;


		public float lifeSteal;


		public int maxHp;


		public int maxSp;


		public float moveSpeed;


		public float outOfCombatMoveSpeed;


		public float preventBasicAttackDamaged;


		public float preventCriticalStrikeDamaged;


		public float preventSkillDamaged;


		public float preventSkillDamagedRatio;


		public float sightRange;


		public float spRegen;


		public float spRegenRatio;


		public float weaponAttackRange;


		public float weaponAttackSpeed;

		public ItemStat()
		{
			Clear();
		}


		public void Clear()
		{
			attackPower = 0;
			defense = 0;
			maxHp = 0;
			maxSp = 0;
			hpRegen = 0f;
			spRegen = 0f;
			hpRegenRatio = 0f;
			spRegenRatio = 0f;
			attackSpeed = 0f;
			attackRange = 0f;
			attackSpeedRatio = 0f;
			moveSpeed = 0f;
			sightRange = 0f;
			criticalStrikeChance = 0f;
			criticalStrikeDamage = 0f;
			preventCriticalStrikeDamaged = 0f;
			cooldownReduction = 0f;
			lifeSteal = 0f;
			outOfCombatMoveSpeed = 0f;
			increaseBasicAttackDamage = 0f;
			preventBasicAttackDamaged = 0f;
			increaseSkillDamage = 0f;
			increaseSkillDamageRatio = 0f;
			preventSkillDamaged = 0f;
			preventSkillDamagedRatio = 0f;
			decreaseRecoveryToBasicAttack = 0;
			decreaseRecoveryToSkill = 0;
			weaponAttackRange = 0f;
			weaponAttackSpeed = 0f;
		}
	}
}