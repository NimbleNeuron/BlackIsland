using System.Collections.Generic;

namespace Blis.Common
{
	
	public class ZahirSkillActive2Data : Singleton<ZahirSkillActive2Data>
	{
		
		public readonly int AddStackCount = 2;

		
		public readonly int BuffState = 1005301;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly int DebuffState = 1005311;

		
		public readonly int ProjectileCode = 74;

		
		public readonly float SkillApCoef = 0.3f;

		
		public readonly float SkillCooldownReduce = -1.5f;

		
		public ZahirSkillActive2Data()
		{
			DamageByLevel.Add(1, 25);
			DamageByLevel.Add(2, 50);
			DamageByLevel.Add(3, 75);
			DamageByLevel.Add(4, 100);
			DamageByLevel.Add(5, 125);
		}
	}
}