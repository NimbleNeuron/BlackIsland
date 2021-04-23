using Newtonsoft.Json;

namespace Blis.Common
{
	
	public class MonsterLevelUpStatData
	{
		
		[JsonConstructor]
		public MonsterLevelUpStatData(int code, string monster, int maxHp, float attackPower, float defense, float moveSpeed, int gainExp)
		{
			this.code = code;
			this.monster = monster;
			this.maxHp = maxHp;
			this.attackPower = attackPower;
			this.defense = defense;
			this.moveSpeed = moveSpeed;
			this.gainExp = gainExp;
		}

		
		public readonly int code;

		
		public readonly string monster;

		
		public readonly int maxHp;

		
		public readonly float attackPower;

		
		public readonly float defense;

		
		public readonly float moveSpeed;

		
		public readonly int gainExp;
	}
}
