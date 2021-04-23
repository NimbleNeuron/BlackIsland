using System.Collections.Generic;

namespace Blis.Common
{
	
	public class FioraSkillNormalAttackData : Singleton<FioraSkillNormalAttackData>
	{
		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType2 = new Dictionary<int, int>();

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>();

		
		public FioraSkillNormalAttackData()
		{
			NormalAttackDelay.Add(0, 0.19f);
			NormalAttackDelay.Add(21, 0.19f);
			NormalAttackDelay.Add(16, 0.26f);
			NormalAttackDelay.Add(19, 0.23f);
			EffectAndSoundWeaponType.Add(0, 1003001);
			EffectAndSoundWeaponType.Add(21, 1003001);
			EffectAndSoundWeaponType.Add(16, 1003002);
			EffectAndSoundWeaponType.Add(19, 1003003);
			EffectAndSoundWeaponType2.Add(0, 1003021);
			EffectAndSoundWeaponType2.Add(21, 1003021);
			EffectAndSoundWeaponType2.Add(16, 1003022);
			EffectAndSoundWeaponType2.Add(19, 1003023);
		}
	}
}