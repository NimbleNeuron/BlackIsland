using System.Collections.Generic;

namespace Blis.Common
{
	
	public class XiukaiSkillNormalAttackData : Singleton<XiukaiSkillNormalAttackData>
	{
		
		public Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public Dictionary<int, int> EffectAndSoundWeaponType2 = new Dictionary<int, int>();

		
		public float NormalAttackApCoef = 1f;

		
		public Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>();

		
		public XiukaiSkillNormalAttackData()
		{
			NormalAttackDelay.Add(0, 0.19f);
			NormalAttackDelay.Add(15, 0.19f);
			NormalAttackDelay.Add(19, 0.23f);
			EffectAndSoundWeaponType.Add(0, 1013002);
			EffectAndSoundWeaponType.Add(15, 1013001);
			EffectAndSoundWeaponType.Add(19, 1013003);
			EffectAndSoundWeaponType2.Add(0, 1013002);
			EffectAndSoundWeaponType2.Add(15, 1013001);
			EffectAndSoundWeaponType2.Add(19, 1013004);
		}
	}
}