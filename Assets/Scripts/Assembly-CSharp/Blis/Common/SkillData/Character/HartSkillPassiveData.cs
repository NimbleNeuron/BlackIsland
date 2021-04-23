using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HartSkillPassiveData : Singleton<HartSkillPassiveData>
	{
		
		public readonly int EnchantedEvolutionProjectileCode_1 = 100805;

		
		public readonly int EnchantedEvolutionProjectileCode_2 = 100806;

		
		public readonly int EvolutionProjectileCode_1 = 100803;

		
		public readonly int EvolutionProjectileCode_2 = 100804;

		
		public readonly float PassiveBonusAttackApCoef = 0.15f;

		
		public readonly Dictionary<int, float> RecoverySpRatio = new Dictionary<int, float>();

		
		public HartSkillPassiveData()
		{
			RecoverySpRatio.Add(1, 1f);
			RecoverySpRatio.Add(2, 1.5f);
			RecoverySpRatio.Add(3, 2f);
		}
	}
}