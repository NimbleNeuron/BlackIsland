using System.Collections.Generic;

namespace Blis.Common
{
	
	public class JackieSkillNormalAttackData : Singleton<JackieSkillNormalAttackData>
	{
		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>();

		
		public readonly float NormalAttackDelay_2 = 0.16f;

		
		public readonly float SpecialNormalAttackDelay = 0.19f;

		
		public JackieSkillNormalAttackData()
		{
			NormalAttackDelay.Add(0, 0.19f);
			NormalAttackDelay.Add(15, 0.19f);
			NormalAttackDelay.Add(16, 0.26f);
			NormalAttackDelay.Add(14, 0.29f);
			NormalAttackDelay.Add(18, 0.23f);
			EffectAndSoundWeaponType.Add(0, 1001001);
			EffectAndSoundWeaponType.Add(15, 1001001);
			EffectAndSoundWeaponType.Add(16, 1001002);
			EffectAndSoundWeaponType.Add(14, 1001003);
			EffectAndSoundWeaponType.Add(18, 1001004);
		}
	}
}