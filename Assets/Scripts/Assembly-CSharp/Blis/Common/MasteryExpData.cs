using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class MasteryExpData
	{
		public readonly int code;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryConditionType conditionType;


		public readonly int conditionValue;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ItemGrade grade;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType masteryType1;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType masteryType2;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType masteryType3;


		public readonly int value1;


		public readonly int value2;


		public readonly int value3;

		[JsonConstructor]
		public MasteryExpData(int code, MasteryConditionType conditionType, ItemGrade grade, int conditionValue,
			MasteryType masteryType1, int value1, MasteryType masteryType2, int value2, MasteryType masteryType3,
			int value3)
		{
			this.code = code;
			this.conditionType = conditionType;
			this.grade = grade;
			this.conditionValue = conditionValue;
			this.masteryType1 = masteryType1;
			this.value1 = value1;
			this.masteryType2 = masteryType2;
			this.value2 = value2;
			this.masteryType3 = masteryType3;
			this.value3 = value3;
		}
	}
}