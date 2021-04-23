using System.Collections.Generic;

namespace Blis.Common
{
	
	public class AyaSkillActive2Data : Singleton<AyaSkillActive2Data>
	{
		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> ProjectileCode = new Dictionary<int, int>();

		
		public readonly int ShootCount = 10;

		
		public readonly float SkillApCoef = 1.4f;

		
		public readonly Dictionary<int, float> SkillApCoefByLevel = new Dictionary<int, float>();

		
		public AyaSkillActive2Data()
		{
			DamageByLevel.Add(1, 22);
			DamageByLevel.Add(2, 44);
			DamageByLevel.Add(3, 66);
			DamageByLevel.Add(4, 88);
			DamageByLevel.Add(5, 110);
			SkillApCoefByLevel.Add(1, 0.3f);
			SkillApCoefByLevel.Add(2, 0.35f);
			SkillApCoefByLevel.Add(3, 0.4f);
			SkillApCoefByLevel.Add(4, 0.45f);
			SkillApCoefByLevel.Add(5, 0.5f);
			ProjectileCode.Add(0, 3);
			ProjectileCode.Add(9, 3);
			ProjectileCode.Add(10, 31);
			ProjectileCode.Add(11, 32);
		}
	}
}