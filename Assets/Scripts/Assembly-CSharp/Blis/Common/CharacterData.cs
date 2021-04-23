using Newtonsoft.Json;

namespace Blis.Common
{
	public class CharacterData
	{
		public readonly float attackPower;
		public readonly float attackSpeed;
		public readonly int code;
		public readonly float defense;
		public readonly float hpRegen;
		public readonly int initExtraPoint;
		public readonly int maxExtraPoint;
		public readonly int maxHp;
		public readonly int maxSp;
		public readonly float moveSpeed;
		public readonly string name;
		public readonly float radius;
		public readonly string resource;
		public readonly float sightRange;
		public readonly float spRegen;
		public readonly float uiHeight;

		[JsonConstructor]
		public CharacterData(int code, string name, int maxHp, int maxSp, int initExtraPoint, int maxExtraPoint,
			float attackPower, float defense, float hpRegen, float spRegen, float attackSpeed, float moveSpeed,
			float sightRange, float radius, float uiHeight, string resource)
		{
			this.code = code;
			this.name = name;
			this.maxHp = maxHp;
			this.maxSp = maxSp;
			this.initExtraPoint = initExtraPoint;
			this.maxExtraPoint = maxExtraPoint;
			this.attackPower = attackPower;
			this.defense = defense;
			this.hpRegen = hpRegen;
			this.spRegen = spRegen;
			this.attackSpeed = attackSpeed;
			this.moveSpeed = moveSpeed;
			this.sightRange = sightRange;
			this.radius = radius;
			this.uiHeight = uiHeight;
			this.resource = resource;
		}
	}
}