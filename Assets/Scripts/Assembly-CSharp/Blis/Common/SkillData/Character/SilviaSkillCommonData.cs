using System.Collections.Generic;

namespace Blis.Common
{
	
	public class SilviaSkillCommonData : Singleton<SilviaSkillCommonData>
	{
		
		public readonly int bikeStateCode = 1016041;

		
		public readonly int bikeStateGroup = 1016040;

		
		public readonly int humanInitStateCode = 1016011;

		
		public readonly int humanInitStateGroup = 1016010;

		
		public readonly int humanNonSkillStateCode = 1016031;

		
		public readonly int humanNonSkillStateGroup = 1016030;

		
		public readonly int humanSkillStateCode = 1016021;

		
		public readonly int humanSkillStateGroup = 1016020;

		
		public readonly Dictionary<int, int> PassiveAttackSpeedBuffStateCode = new Dictionary<int, int>
		{
			{
				1,
				1016101
			},
			{
				2,
				1016102
			},
			{
				3,
				1016103
			}
		};

		
		public readonly Dictionary<int, int> PassiveEpAmount = new Dictionary<int, int>
		{
			{
				1,
				9
			},
			{
				2,
				11
			},
			{
				3,
				13
			}
		};

		
		public readonly int PassiveSkillCoolTime = 30;

		
		public readonly int PassiveSkillDamageRatioStateCode = 1016111;

		
		public readonly int PassiveSkillNotApplyArea = 33554432;
	}
}