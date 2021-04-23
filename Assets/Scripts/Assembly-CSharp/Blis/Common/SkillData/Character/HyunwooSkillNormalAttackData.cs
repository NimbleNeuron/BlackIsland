using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HyunwooSkillNormalAttackData : Singleton<HyunwooSkillNormalAttackData>
	{
		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>();

		
		public HyunwooSkillNormalAttackData()
		{
			NormalAttackDelay.Add(0, 0.16f);
			NormalAttackDelay.Add(1, 0.16f);
			NormalAttackDelay.Add(2, 0.23f);
			EffectAndSoundWeaponType.Add(0, 1007001);
			EffectAndSoundWeaponType.Add(1, 1007001);
			EffectAndSoundWeaponType.Add(2, 1007002);
		}
	}
}