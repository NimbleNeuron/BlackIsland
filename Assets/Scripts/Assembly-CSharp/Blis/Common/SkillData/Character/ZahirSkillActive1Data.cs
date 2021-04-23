using System.Collections.Generic;

namespace Blis.Common
{
	
	public class ZahirSkillActive1Data : Singleton<ZahirSkillActive1Data>
	{
		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DamageByLevel_2 = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DebuffStackByLevel_2 = new Dictionary<int, int>();

		
		public readonly int DebuffState_2 = 1005121;

		
		public readonly int ProjectileCode = 73;

		
		public readonly float SkillApCoef = 0.5f;

		
		public readonly int ZahirSkillActive1DebuffMinBonus = -70;

		
		public ZahirSkillActive1Data()
		{
			DamageByLevel.Add(1, 40);
			DamageByLevel.Add(2, 100);
			DamageByLevel.Add(3, 160);
			DamageByLevel.Add(4, 220);
			DamageByLevel.Add(5, 280);
			DamageByLevel_2.Add(1, 75);
			DamageByLevel_2.Add(2, 150);
			DamageByLevel_2.Add(3, 225);
			DamageByLevel_2.Add(4, 300);
			DamageByLevel_2.Add(5, 375);
			DebuffStackByLevel_2.Add(1, 10);
			DebuffStackByLevel_2.Add(2, 15);
			DebuffStackByLevel_2.Add(3, 20);
			DebuffStackByLevel_2.Add(4, 25);
			DebuffStackByLevel_2.Add(5, 30);
		}
	}
}