using System.Collections.Generic;

namespace Blis.Common
{
	
	public class YukiSkillNormalAttackData : Singleton<YukiSkillNormalAttackData>
	{
		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>();

		
		public readonly float NormalAttackDelay_2 = 0.16f;

		
		public readonly Dictionary<int, int> PassiveEffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public YukiSkillNormalAttackData()
		{
			NormalAttackDelay.Add(0, 0.26f);
			NormalAttackDelay.Add(16, 0.26f);
			NormalAttackDelay.Add(18, 0.23f);
			NormalAttackDelay.Add(19, 0.23f);
			EffectAndSoundWeaponType.Add(0, 1000000);
			EffectAndSoundWeaponType.Add(16, 1011001);
			EffectAndSoundWeaponType.Add(18, 1011001);
			EffectAndSoundWeaponType.Add(19, 1011001);
			PassiveEffectAndSoundWeaponType.Add(0, 1000000);
			PassiveEffectAndSoundWeaponType.Add(16, 1011002);
			PassiveEffectAndSoundWeaponType.Add(18, 1011002);
			PassiveEffectAndSoundWeaponType.Add(19, 1011002);
		}
	}
}