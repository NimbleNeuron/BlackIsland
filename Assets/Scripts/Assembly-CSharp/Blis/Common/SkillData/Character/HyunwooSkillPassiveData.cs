using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HyunwooSkillPassiveData : Singleton<HyunwooSkillPassiveData>
	{
		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly int EffectAndSoundCode = 1007101;

		
		public readonly Dictionary<int, float> HpCoef = new Dictionary<int, float>();

		
		public readonly int IncreasePassiveBuffStackCountOnAttack = 4;

		
		public readonly int IncreasePassiveBuffStackCountOnHit = 1;

		
		public HyunwooSkillPassiveData()
		{
			BuffState.Add(1, 1007101);
			BuffState.Add(2, 1007102);
			BuffState.Add(3, 1007103);
			HpCoef.Add(1, 0.07f);
			HpCoef.Add(2, 0.11f);
			HpCoef.Add(3, 0.15f);
		}
	}
}