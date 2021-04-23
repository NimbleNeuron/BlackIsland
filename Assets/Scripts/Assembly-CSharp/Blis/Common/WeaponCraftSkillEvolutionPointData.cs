using System;

namespace Blis.Common
{
	
	public class WeaponCraftSkillEvolutionPointData : SkillEvolutionPointData
	{
		
		public readonly int itemCode;

		
		public readonly ItemGrade itemGrade;

		
		public readonly MasteryType masteryType;

		
		public WeaponCraftSkillEvolutionPointData(SkillEvolutionPointData data) : base(data.code, data.characterCode,
			data.conditionType, data.conditionValue1, data.conditionValue2, data.conditionValue3, data.conditionValue4,
			data.pointType, data.point, data.limitPoint)
		{
			masteryType = (MasteryType) Enum.Parse(typeof(MasteryType), data.conditionValue1);
			itemGrade = (ItemGrade) Enum.Parse(typeof(ItemGrade), data.conditionValue2);
			if (!string.IsNullOrEmpty(data.conditionValue3))
			{
				itemCode = int.Parse(data.conditionValue3);
			}
		}
	}
}