using System.Collections.Generic;

namespace Blis.Common
{
	
	public class MagnusSkillNormalAttackData : Singleton<MagnusSkillNormalAttackData>
	{
		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>();

		
		public MagnusSkillNormalAttackData()
		{
			NormalAttackDelay.Add(0, 0.23f);
			NormalAttackDelay.Add(13, 0.23f);
			NormalAttackDelay.Add(3, 0.16f);
			EffectAndSoundWeaponType.Add(0, 1004001);
			EffectAndSoundWeaponType.Add(3, 1004001);
			EffectAndSoundWeaponType.Add(13, 1004002);
		}
	}
}