using System.Collections.Generic;

namespace Blis.Common
{
	
	public class NadineSkillNormalAttackData : Singleton<NadineSkillNormalAttackData>
	{
		
		public readonly Dictionary<int, float> NormalAttackApCoef = new Dictionary<int, float>();

		
		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>();

		
		public readonly Dictionary<int, int> ProjectileCode = new Dictionary<int, int>();

		
		public NadineSkillNormalAttackData()
		{
			NormalAttackDelay.Add(7, 0.23f);
			NormalAttackDelay.Add(8, 0.13f);
			NormalAttackApCoef.Add(7, 1f);
			NormalAttackApCoef.Add(8, 1f);
			ProjectileCode.Add(7, 41);
			ProjectileCode.Add(8, 51);
		}
	}
}