using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	
	public class SkillEvolutionPointData
	{
		
		[JsonConstructor]
		public SkillEvolutionPointData(int code, int characterCode, SkillEvolutionConditionType conditionType, string conditionValue1, string conditionValue2, string conditionValue3, string conditionValue4, SkillEvolutionPointType pointType, int point, int limitPoint)
		{
			this.code = code;
			this.characterCode = characterCode;
			this.conditionType = conditionType;
			this.conditionValue1 = conditionValue1;
			this.conditionValue2 = conditionValue2;
			this.conditionValue3 = conditionValue3;
			this.conditionValue4 = conditionValue4;
			this.pointType = pointType;
			this.point = point;
			this.limitPoint = limitPoint;
		}

		
		public readonly int code;

		
		public readonly int characterCode;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillEvolutionConditionType conditionType;

		
		public readonly string conditionValue1;

		
		public readonly string conditionValue2;

		
		public readonly string conditionValue3;

		
		public readonly string conditionValue4;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillEvolutionPointType pointType;

		
		public readonly int point;

		
		public readonly int limitPoint;
	}
}
