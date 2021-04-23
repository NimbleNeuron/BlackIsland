using System.Collections.Generic;

namespace Blis.Common
{
	
	public class NadineSkillActive4Data : Singleton<NadineSkillActive4Data>
	{
		
		public readonly Dictionary<int, int> BuffState1 = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> BuffState2 = new Dictionary<int, int>();

		
		public readonly float DamageAp = 0.5f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly int DamagePerPassiveStackCount = 1;

		
		public readonly int DebuffState = 1006521;

		
		public readonly int PassiveBuffState = 1006101;

		
		public readonly int ProjectileCode = 64;

		
		public NadineSkillActive4Data()
		{
			BuffState1.Add(1, 1006501);
			BuffState1.Add(2, 1006502);
			BuffState1.Add(3, 1006503);
			BuffState2.Add(1, 1006511);
			BuffState2.Add(2, 1006512);
			BuffState2.Add(3, 1006513);
			DamageByLevel.Add(1, 50);
			DamageByLevel.Add(2, 100);
			DamageByLevel.Add(3, 150);
		}
	}
}