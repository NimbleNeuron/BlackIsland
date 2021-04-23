using System.Collections.Generic;

namespace Blis.Common
{
	
	public class LenoxSkillActive2Data : Singleton<LenoxSkillActive2Data>
	{
		
		public readonly int Active2BuffCode = 1020321;

		
		public readonly int Active2DeBuffCodeFirst = 1020301;

		
		public readonly int Active2DeBuffCodeSecond = 1020311;

		
		public readonly int EffectAndSoundCodeFirst = 1020301;

		
		public readonly int EffectAndSoundCodeSecond = 1020302;

		
		public readonly Dictionary<int, int> FirstDamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				20
			},
			{
				2,
				30
			},
			{
				3,
				40
			},
			{
				4,
				50
			},
			{
				5,
				60
			}
		};

		
		public readonly float FirstSkillApCoef = 0.3f;

		
		public readonly int SecondAttackLengthRiseCount = 1;

		
		public readonly float SecondAttackPartitionTime = 0.03f;

		
		public readonly Dictionary<int, int> SecondDamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				50
			},
			{
				2,
				85
			},
			{
				3,
				120
			},
			{
				4,
				155
			},
			{
				5,
				190
			}
		};

		
		public readonly float SecondSkillApCoef = 0.6f;
	}
}