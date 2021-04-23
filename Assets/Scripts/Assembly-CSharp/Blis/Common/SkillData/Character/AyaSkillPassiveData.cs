using System.Collections.Generic;

namespace Blis.Common
{
	
	public class AyaSkillPassiveData : Singleton<AyaSkillPassiveData>
	{
		
		public readonly int BuffState = 1002101;

		
		public readonly float CooldownReduce = -2f;

		
		public readonly Dictionary<int, int> ShieldByLevel = new Dictionary<int, int>();

		
		public readonly float SkillApCoef = 0.3f;

		
		public AyaSkillPassiveData()
		{
			ShieldByLevel.Add(1, 100);
			ShieldByLevel.Add(2, 150);
			ShieldByLevel.Add(3, 200);
		}
	}
}