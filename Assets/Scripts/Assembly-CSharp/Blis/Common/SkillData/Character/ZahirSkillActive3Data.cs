using System.Collections.Generic;

namespace Blis.Common
{
	
	public class ZahirSkillActive3Data : Singleton<ZahirSkillActive3Data>
	{
		
		public readonly float AirborneDuration = 0.5f;

		
		public readonly float BigAirborneDuration = 1f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly int DebuffState = 1005401;

		
		public readonly int ProjectileCode = 75;

		
		public readonly float SkillApCoef = 0.5f;

		
		public ZahirSkillActive3Data()
		{
			DamageByLevel.Add(1, 80);
			DamageByLevel.Add(2, 110);
			DamageByLevel.Add(3, 140);
			DamageByLevel.Add(4, 170);
			DamageByLevel.Add(5, 200);
		}
	}
}