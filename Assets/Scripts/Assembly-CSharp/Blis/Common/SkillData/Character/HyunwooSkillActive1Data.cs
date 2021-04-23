using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HyunwooSkillActive1Data : Singleton<HyunwooSkillActive1Data>
	{
		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly int DebuffState = 1007201;

		
		public readonly float SkillApCoef = 0.4f;

		
		public HyunwooSkillActive1Data()
		{
			DamageByLevel.Add(1, 100);
			DamageByLevel.Add(2, 150);
			DamageByLevel.Add(3, 200);
			DamageByLevel.Add(4, 250);
			DamageByLevel.Add(5, 300);
		}
	}
}