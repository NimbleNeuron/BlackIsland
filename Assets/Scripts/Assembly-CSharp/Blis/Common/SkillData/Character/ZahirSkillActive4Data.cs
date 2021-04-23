using System.Collections.Generic;

namespace Blis.Common
{
	
	public class ZahirSkillActive4Data : Singleton<ZahirSkillActive4Data>
	{
		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DamageByLevel_2 = new Dictionary<int, int>();

		
		public readonly int ProjectileCode = 76;

		
		public readonly float ProjectileSkillDelay = 0.29f;

		
		public readonly float SkillApCoef = 0.5f;

		
		public readonly float SkillApCoef_2 = 0.5f;

		
		public ZahirSkillActive4Data()
		{
			DamageByLevel.Add(1, 60);
			DamageByLevel.Add(2, 150);
			DamageByLevel.Add(3, 240);
			DamageByLevel_2.Add(1, 30);
			DamageByLevel_2.Add(2, 75);
			DamageByLevel_2.Add(3, 120);
		}
	}
}