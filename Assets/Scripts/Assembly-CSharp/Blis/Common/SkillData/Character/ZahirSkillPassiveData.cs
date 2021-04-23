using System.Collections.Generic;

namespace Blis.Common
{
	
	public class ZahirSkillPassiveData : Singleton<ZahirSkillPassiveData>
	{
		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly int PassiveDebuffStateCode = 1005111;

		
		public readonly int PassiveDebuffStateGroup = 1005110;

		
		public readonly float PassiveDuration = 20f;

		
		public readonly float SkillApCoef = 0.3f;

		
		public ZahirSkillPassiveData()
		{
			BuffState.Add(1, 1005101);
			BuffState.Add(2, 1005102);
			BuffState.Add(3, 1005103);
			DamageByLevel.Add(1, 10);
			DamageByLevel.Add(2, 35);
			DamageByLevel.Add(3, 60);
		}
	}
}