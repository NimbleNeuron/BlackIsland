using Blis.Common;

namespace Blis.Client.UIModel
{
	public class UICharacterStat
	{
		public readonly float attack;


		public readonly float attackRange;


		public readonly float attackSpeed;


		public readonly float blood;


		public readonly float cooldown;


		public readonly float criticalStrikeChance;


		public readonly float defense;


		public readonly float defensePenetrate;


		public readonly int ep;


		public readonly int hp;


		public readonly float hpGen;


		public readonly float increaseAttack;


		public readonly float increaseAttackRatio;


		public readonly float increaseSkill;


		public readonly float increaseSkillRatio;


		public readonly int maxEp;


		public readonly int maxHp;


		public readonly int maxSp;


		public readonly float moveSpeed;


		public readonly float moveSpeedOutOfCombat;


		public readonly float preventAttack;


		public readonly float preventAttackRatio;


		public readonly float preventSkillRatio;


		public readonly float resistance;


		public readonly float sightRange;


		public readonly int sp;


		public readonly float spGen;

		public UICharacterStat(CharacterStatBase stat, CharacterStatus status)
		{
			hp = status.Hp;
			maxHp = stat.MaxHp;
			sp = status.Sp;
			maxSp = stat.MaxSp;
			ep = status.ExtraPoint;
			maxEp = stat.MaxExtraPoint;
			attack = stat.AttackPower;
			attackSpeed = stat.AttackSpeed;
			criticalStrikeChance = stat.CriticalStrikeChance;
			attackRange = stat.AttackRange;
			defensePenetrate = 0f;
			blood = stat.LifeSteal;
			defense = stat.Defense;
			cooldown = stat.CooldownReduction;
			moveSpeed = stat.MoveSpeed;
			resistance = 0f;
			hpGen = stat.HpRegen;
			spGen = stat.SpRegen;
			sightRange = stat.SightRange;
			moveSpeedOutOfCombat = stat.MoveSpeedOutOfCombat;
			increaseAttack = stat.IncreaseBasicAttackDamage;
			preventAttack = stat.PreventBasicAttackDamaged;
			increaseAttackRatio = stat.IncreaseBasicAttackDamageRatio;
			preventAttackRatio = stat.PreventBasicAttackDamagedRatio;
			increaseSkill = stat.IncreaseSkillDamage;
			increaseSkillRatio = stat.IncreaseSkillDamageRatio;
			preventSkillRatio = stat.PreventSkillDamagedRatio;
		}
	}
}