using System.Collections.Generic;

namespace Blis.Common
{
	
	public class LenoxSkillPassiveData : Singleton<LenoxSkillPassiveData>
	{
		
		public readonly HashSet<int> AnglerRewardExclusionItems = new HashSet<int>
		{
			0
		};

		
		public readonly Dictionary<ItemGrade, float> AnglerRewardItemProbability =
			new Dictionary<ItemGrade, float>(SingletonComparerEnum<ItemGradeComparer, ItemGrade>.Instance)
			{
				{
					ItemGrade.Common,
					85f
				},
				{
					ItemGrade.Uncommon,
					14f
				},
				{
					ItemGrade.Rare,
					1f
				}
			};

		
		public readonly HashSet<ItemType> AnglerRewardItemTypes =
			new HashSet<ItemType>(SingletonComparerEnum<ItemTypeComparer, ItemType>.Instance)
			{
				ItemType.None,
				ItemType.Armor,
				ItemType.Special,
				ItemType.Misc,
				ItemType.Consume
			};

		
		public readonly float PassiveBuffMaxHpRatio = 0.1f;

		
		public readonly Dictionary<int, int> PassiveCaptainBuffCode = new Dictionary<int, int>
		{
			{
				1,
				1020111
			},
			{
				2,
				1020112
			},
			{
				3,
				1020113
			}
		};

		
		public readonly int PassiveCaptainBuffGroup = 1020110;
	}
}