using Blis.Common;

namespace Blis.Client.UIModel
{
	public class UITargetInfoHudStat
	{
		public float attack;


		public float attackSpeed;


		public float cooldown;


		public float criticalStrikeChance;


		public float defense;


		public int hp;


		public int maxHp;


		public int maxSp;


		public float moveSpeed;


		public int sp;

		public UITargetInfoHudStat(CharacterStatBase stat, CharacterStatus status)
		{
			hp = status.Hp;
			maxHp = stat.MaxHp;
			sp = status.Sp;
			maxSp = stat.MaxSp;
			attack = stat.AttackPower;
			defense = stat.Defense;
			attackSpeed = stat.AttackSpeed;
			criticalStrikeChance = stat.CriticalStrikeChance;
			moveSpeed = stat.MoveSpeed;
			cooldown = stat.CooldownReduction;
		}
	}
}