using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class MonsterData
	{
		public const int WICKLINE_CODE = 7;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly DayNight aggressive;


		public readonly float attackPower;


		public readonly float attackRange;


		public readonly float attackSpeed;


		public readonly int code;


		public readonly int createTime;


		public readonly float defense;


		public readonly int dropGroup;


		public readonly float firstAttackRange;


		public readonly int gainExp;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MonsterGrade grade;


		public readonly int maxHp;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MonsterType monster;


		public readonly float moveSpeed;


		public readonly float radius;


		public readonly int randomDropCount;


		public readonly int regenTime;


		public readonly string resource;


		public readonly float sightRange;


		public readonly float uiHeight;

		[JsonConstructor]
		public MonsterData(int code, MonsterType monster, MonsterGrade grade, int maxHp, float attackPower,
			float defense, float attackSpeed, float moveSpeed, float sightRange, float attackRange,
			float firstAttackRange, int gainExp, int dropGroup, int randomDropCount, float radius, float uiHeight,
			int regenTime, string resource, DayNight aggressive, int createTime)
		{
			this.code = code;
			this.monster = monster;
			this.grade = grade;
			this.maxHp = maxHp;
			this.attackPower = attackPower;
			this.defense = defense;
			this.attackSpeed = attackSpeed;
			this.moveSpeed = moveSpeed;
			this.sightRange = sightRange;
			this.attackRange = attackRange;
			this.firstAttackRange = firstAttackRange;
			this.gainExp = gainExp;
			this.dropGroup = dropGroup;
			this.randomDropCount = randomDropCount;
			this.radius = radius;
			this.uiHeight = uiHeight;
			this.regenTime = regenTime;
			this.resource = resource;
			this.aggressive = aggressive;
			this.createTime = createTime;
		}
	}
}