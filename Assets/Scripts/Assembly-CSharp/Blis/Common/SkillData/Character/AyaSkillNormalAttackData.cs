using System.Collections.Generic;

namespace Blis.Common
{
	
	public class AyaSkillNormalAttackData : Singleton<AyaSkillNormalAttackData>
	{
		
		public readonly float AssaultRifleApCoef = 0.5f;

		
		public readonly Dictionary<int, float> NormalAttackApCoef = new Dictionary<int, float>();

		
		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>();

		
		public readonly Dictionary<int, int> ProjectileCode = new Dictionary<int, int>();

		
		public AyaSkillNormalAttackData()
		{
			NormalAttackDelay.Add(0, 0.16f);
			NormalAttackDelay.Add(9, 0.16f);
			NormalAttackDelay.Add(10, 0.09f);
			NormalAttackDelay.Add(11, 0.23f);
			NormalAttackApCoef.Add(0, 1f);
			NormalAttackApCoef.Add(9, 1f);
			NormalAttackApCoef.Add(10, 0.35f);
			NormalAttackApCoef.Add(11, 1f);
			ProjectileCode.Add(0, 1);
			ProjectileCode.Add(9, 1);
			ProjectileCode.Add(10, 11);
			ProjectileCode.Add(11, 12);
		}
	}
}