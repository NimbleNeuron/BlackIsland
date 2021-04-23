using System.Collections.Generic;

namespace Blis.Common
{
	
	public class EmmaSkillActive1Data : Singleton<EmmaSkillActive1Data>
	{
		
		public readonly float CooldownReduce = -1f;

		
		public readonly float DamageApCoef = 0.45f;

		
		public readonly Dictionary<int, int> DamageBySkillLevel = new Dictionary<int, int>();

		
		public readonly int PigeonDealerAttackStateCode = 1019201;

		
		public readonly int PigeonDealerDamagedByPigeonStateCode = 1019221;

		
		public readonly int PigeonDealerDamagedByPigeonStateGroupCode = 1019220;

		
		public readonly int PigeonDealerDamagedByPlayerStateCode = 1019211;

		
		public readonly int PigeonDealerDamagedByPlayerStateGroupCode = 1019210;

		
		public readonly int PigeonDealerDamageEffectAndSoundCode = 1019201;

		
		public readonly int PigeonDealerMoveSpeedDownStateCode = 1019231;

		
		public readonly int PigeonDealerProjectileCode = 101921;

		
		public readonly int PigeonSummonCode = 1051;

		
		public EmmaSkillActive1Data()
		{
			DamageBySkillLevel.Add(1, 40);
			DamageBySkillLevel.Add(2, 80);
			DamageBySkillLevel.Add(3, 120);
			DamageBySkillLevel.Add(4, 160);
			DamageBySkillLevel.Add(5, 200);
		}
	}
}