using Newtonsoft.Json;

namespace Blis.Common
{
	
	public class CharacterLevelUpStatData
	{
		
		[JsonConstructor]
		public CharacterLevelUpStatData(int code, string name, int maxHp, int maxSp, float attackPower, float defense, float hpRegen, float spRegen, float attackSpeed, int moveSpeed)
		{
			this.code = code;
			this.name = name;
			this.maxHp = maxHp;
			this.maxSp = maxSp;
			this.attackPower = attackPower;
			this.defense = defense;
			this.hpRegen = hpRegen;
			this.spRegen = spRegen;
			this.attackSpeed = attackSpeed;
			this.moveSpeed = moveSpeed;
		}

		
		public readonly int code;

		
		public readonly string name;

		
		public readonly int maxHp;

		
		public readonly int maxSp;

		
		public readonly float attackPower;

		
		public readonly float defense;

		
		public readonly float hpRegen;

		
		public readonly float spRegen;

		
		public readonly float attackSpeed;

		
		public readonly int moveSpeed;
	}
}
