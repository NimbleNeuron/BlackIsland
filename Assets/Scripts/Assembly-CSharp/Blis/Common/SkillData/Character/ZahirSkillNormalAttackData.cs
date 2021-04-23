using System.Collections.Generic;

namespace Blis.Common
{
	
	public class ZahirSkillNormalAttackData : Singleton<ZahirSkillNormalAttackData>
	{
		
		public readonly Dictionary<int, float> NormalAttackApCoef = new Dictionary<int, float>();

		
		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>();

		
		public readonly Dictionary<int, int> ProjectileCode = new Dictionary<int, int>();

		
		public ZahirSkillNormalAttackData()
		{
			NormalAttackDelay.Add(5, 0.29f);
			NormalAttackDelay.Add(6, 0.29f);
			NormalAttackApCoef.Add(5, 1f);
			NormalAttackApCoef.Add(6, 1f);
			ProjectileCode.Add(5, 71);
			ProjectileCode.Add(6, 72);
		}
	}
}