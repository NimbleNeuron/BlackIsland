using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class MasteryLevelData
	{
		public readonly int code;


		public readonly int giveLevelExp;


		public readonly int masteryLevel;


		public readonly int nextMasteryExp;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly StatType option1;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly StatType option2;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly StatType option3;


		public readonly float optionValue1;


		public readonly float optionValue2;


		public readonly float optionValue3;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType type;


		public readonly int weaponSkillPoint;

		[JsonConstructor]
		public MasteryLevelData(int code, MasteryType type, int masteryLevel, int nextMasteryExp, int giveLevelExp,
			StatType option1, float optionValue1, StatType option2, float optionValue2, StatType option3,
			float optionValue3, int weaponSkillPoint)
		{
			this.code = code;
			this.type = type;
			this.masteryLevel = masteryLevel;
			this.nextMasteryExp = nextMasteryExp;
			this.giveLevelExp = giveLevelExp;
			this.option1 = option1;
			this.optionValue1 = optionValue1;
			this.option2 = option2;
			this.optionValue2 = optionValue2;
			this.option3 = option3;
			this.optionValue3 = optionValue3;
			this.weaponSkillPoint = weaponSkillPoint;
		}
	}
}