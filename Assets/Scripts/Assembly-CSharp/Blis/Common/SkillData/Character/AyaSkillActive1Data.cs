using System.Collections.Generic;

namespace Blis.Common
{
	
	public class AyaSkillActive1Data : Singleton<AyaSkillActive1Data>
	{
		
		public readonly int BuffState = 1002201;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly int ProjectileCode = 2;

		
		public readonly int ProjectileCode_2 = 22;

		
		public readonly Dictionary<int, float> SkillApCoef = new Dictionary<int, float>();

		
		public readonly Dictionary<int, float> SkillApCoef_2 = new Dictionary<int, float>();

		
		public readonly float SkillAttackDelay = 0.33f;

		
		public AyaSkillActive1Data()
		{
			SkillApCoef.Add(1, 1f);
			SkillApCoef.Add(2, 1f);
			SkillApCoef.Add(3, 1f);
			SkillApCoef.Add(4, 1f);
			SkillApCoef.Add(5, 1f);
			SkillApCoef_2.Add(1, 0.2f);
			SkillApCoef_2.Add(2, 0.2f);
			SkillApCoef_2.Add(3, 0.2f);
			SkillApCoef_2.Add(4, 0.2f);
			SkillApCoef_2.Add(5, 0.2f);
			DamageByLevel.Add(1, 20);
			DamageByLevel.Add(2, 70);
			DamageByLevel.Add(3, 120);
			DamageByLevel.Add(4, 170);
			DamageByLevel.Add(5, 220);
		}
	}
}