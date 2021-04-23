namespace Blis.Common
{
	
	public class StateStackSkillEvolutionPointData : SkillEvolutionPointData
	{
		
		public readonly int stackCountCondition;

		
		public readonly int stateGroupCode;

		
		public StateStackSkillEvolutionPointData(SkillEvolutionPointData data) : base(data.code, data.characterCode,
			data.conditionType, data.conditionValue1, data.conditionValue2, data.conditionValue3, data.conditionValue4,
			data.pointType, data.point, data.limitPoint)
		{
			if (!int.TryParse(data.conditionValue1, out stateGroupCode))
			{
				Log.E("int.Parse Fail!! data.conditionValue1: " + data.conditionValue1);
			}

			if (!int.TryParse(data.conditionValue2, out stackCountCondition))
			{
				Log.E("int.Parse Fail!! data.conditionValue2: " + data.conditionValue2);
			}
		}
	}
}