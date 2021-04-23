using System.Collections.Generic;

namespace Blis.Common
{
	
	public class AssaultRifleSkillActiveData : Singleton<AssaultRifleSkillActiveData>
	{
		
		public readonly int AssaultPassiveBuff = 3010101;

		
		public readonly int AssaultPassiveBuffGroup = 3010100;

		
		public readonly Dictionary<int, int> AssaultRifleOverHeatAttack = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly int OverHeatAddStack = 5;

		
		public readonly float OverHeatAttackApCoef_1_2 = 0.35f;

		
		public readonly float OverHeatAttackApCoef_3 = 0.5f;

		
		public readonly float OverHeatAttackDelay = 0.1f;

		
		public readonly float OverHeatCheckTime = 6f;

		
		public readonly int OverHeatRemoveStack = 10;

		
		public readonly float OverHeatRemoveTime = 1f;

		
		public readonly int ProjectileCode = 100000;

		
		public AssaultRifleSkillActiveData()
		{
			BuffState.Add(1, 3010001);
			BuffState.Add(2, 3010002);
			AssaultRifleOverHeatAttack.Add(1, 3010011);
			AssaultRifleOverHeatAttack.Add(2, 3010012);
		}
	}
}