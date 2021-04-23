using System.Collections.Generic;

namespace Blis.Common
{
	
	public class LukeSkillNormalAttackData : Singleton<LukeSkillNormalAttackData>
	{
		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>();

		
		public LukeSkillNormalAttackData()
		{
			NormalAttackDelay.Add(0, 0.23f);
			NormalAttackDelay.Add(3, 0.16f);
			EffectAndSoundWeaponType.Add(0, 0);
			EffectAndSoundWeaponType.Add(3, 1022101);
		}
	}
}