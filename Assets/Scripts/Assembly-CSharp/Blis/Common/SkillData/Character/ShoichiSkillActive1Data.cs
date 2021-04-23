using System.Collections.Generic;

namespace Blis.Common
{
	
	public class ShoichiSkillActive1Data : Singleton<ShoichiSkillActive1Data>
	{
		
		public readonly int BuffStateCode = 1018201;

		
		public readonly int BuffStateGroup = 1018200;

		
		public readonly float CooldownReduce = -2f;

		
		public readonly float DaggerInstallTime = 0.13f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				10
			},
			{
				2,
				60
			},
			{
				3,
				110
			},
			{
				4,
				160
			},
			{
				5,
				210
			}
		};

		
		public readonly int DeBuffStateCode = 1018211;

		
		public readonly int DeBuffStateGroup = 1018210;

		
		public readonly int EffectAndSoundCode = 1018101;

		
		public readonly int EffectAndSoundCode2 = 1018102;

		
		public readonly float PassiveDaggerRange = 3.3f;

		
		public readonly float SkillApCoef = 0.45f;
	}
}