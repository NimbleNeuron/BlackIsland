using System.Collections.Generic;

namespace Blis.Common
{
	
	public class ShoichiSkillActive2Data : Singleton<ShoichiSkillActive2Data>
	{
		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				10
			},
			{
				2,
				40
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

		
		public readonly int DebuffState = 1018301;

		
		public readonly int EffectAndSoundCode = 1018201;

		
		public readonly float SkillApCoef = 0.3f;
	}
}