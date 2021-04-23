using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HyunwooSkillActive4Data : Singleton<HyunwooSkillActive4Data>
	{
		
		public readonly int BuffState = 1007501;

		
		public readonly Dictionary<int, int> MaxDamageByLevel = new Dictionary<int, int>();

		
		public readonly float MaxSkillApCoef = 2.1f;

		
		public readonly Dictionary<int, int> MinDamageByLevel = new Dictionary<int, int>();

		
		public readonly float MinSkillApCoef = 0.7f;

		
		public readonly float MinSkillRange = 3f;

		
		public HyunwooSkillActive4Data()
		{
			MinDamageByLevel.Add(1, 200);
			MinDamageByLevel.Add(2, 300);
			MinDamageByLevel.Add(3, 400);
			MaxDamageByLevel.Add(1, 600);
			MaxDamageByLevel.Add(2, 900);
			MaxDamageByLevel.Add(3, 1200);
		}
	}
}