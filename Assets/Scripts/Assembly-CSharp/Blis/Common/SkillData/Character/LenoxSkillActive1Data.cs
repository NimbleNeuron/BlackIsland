using System.Collections.Generic;

namespace Blis.Common
{
	
	public class LenoxSkillActive1Data : Singleton<LenoxSkillActive1Data>
	{
		
		public readonly int Active1BuffCode = 1020201;

		
		public readonly int Active1BuffGroup = 1020200;

		
		public readonly float Active1DemageWaitTime = 0.19f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				15
			},
			{
				2,
				50
			},
			{
				3,
				70
			},
			{
				4,
				100
			},
			{
				5,
				130
			}
		};

		
		public readonly int EffectAndSoundCode = 1020201;

		
		public readonly float SkillApCoef = 0.3f;

		
		public readonly Dictionary<int, float> SmageMaxHPPerDamageByLevel = new Dictionary<int, float>
		{
			{
				1,
				0.055f
			},
			{
				2,
				0.06f
			},
			{
				3,
				0.065f
			},
			{
				4,
				0.07f
			},
			{
				5,
				0.075f
			}
		};

		
		public readonly int SmashEffectAndSoundCode = 1020202;
	}
}