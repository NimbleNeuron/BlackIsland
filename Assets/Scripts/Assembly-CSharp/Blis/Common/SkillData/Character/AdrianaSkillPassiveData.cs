using System.Collections.Generic;

namespace Blis.Common
{
	
	public class AdrianaSkillPassiveData : Singleton<AdrianaSkillPassiveData>
	{
		
		public readonly float BurnsDamageApCoef = 0.15f;

		
		public readonly Dictionary<int, int> BurnsDamageByLevel = new Dictionary<int, int>();

		
		public readonly int BurnsDamageEffectAndSoundCode = 1017102;

		
		public readonly float BurnsDamageStackCoef = 0.2f;

		
		public readonly int BurnsMoveSpeedDownStateCode = 1017121;

		
		public readonly int BurnsMoveSpeedDownStateGroupCode = 1017120;

		
		public readonly int BurnsStateCode = 1017111;

		
		public readonly int BurnsStateGroupCode = 1017110;

		
		public readonly float FireFlameProjectileRefreshPeriod = 0.5f;

		
		public readonly float PassiveUpdateTime = 0.09f;

		
		public readonly float PyromaniacHealSPTerm = 0.5f;

		
		public readonly int PyromaniacStateCode = 1017131;

		
		public readonly int PyromaniacStateGroupCode = 1017130;

		
		public readonly Dictionary<int, float> RecoverySpRatio = new Dictionary<int, float>();

		
		public AdrianaSkillPassiveData()
		{
			BurnsDamageByLevel.Add(1, 4);
			BurnsDamageByLevel.Add(2, 7);
			BurnsDamageByLevel.Add(3, 10);
			RecoverySpRatio.Add(1, 0.5f);
			RecoverySpRatio.Add(2, 1f);
			RecoverySpRatio.Add(3, 1.5f);
		}
	}
}