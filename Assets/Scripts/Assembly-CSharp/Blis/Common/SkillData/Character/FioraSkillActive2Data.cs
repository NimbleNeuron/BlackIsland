using System.Collections.Generic;

namespace Blis.Common
{
	
	public class FioraSkillActive2Data : Singleton<FioraSkillActive2Data>
	{
		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly int EffectAndSound = 1003301;

		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> FioraActive2Attack = new Dictionary<int, int>();

		
		public readonly Dictionary<int, float> NormalAttackApCoef = new Dictionary<int, float>();

		
		public readonly Dictionary<int, float> NormalAttackApCoef2 = new Dictionary<int, float>();

		
		public readonly float NormalAttackDelay = 0.19f;

		
		public readonly float NormalAttackDelay_2 = 0.13f;

		
		public FioraSkillActive2Data()
		{
			BuffState.Add(1, 1003301);
			BuffState.Add(2, 1003302);
			BuffState.Add(3, 1003303);
			BuffState.Add(4, 1003304);
			BuffState.Add(5, 1003305);
			NormalAttackApCoef.Add(1, 0.6f);
			NormalAttackApCoef.Add(2, 0.7f);
			NormalAttackApCoef.Add(3, 0.8f);
			NormalAttackApCoef.Add(4, 0.9f);
			NormalAttackApCoef.Add(5, 1f);
			NormalAttackApCoef2.Add(1, 0.2f);
			NormalAttackApCoef2.Add(2, 0.3f);
			NormalAttackApCoef2.Add(3, 0.4f);
			NormalAttackApCoef2.Add(4, 0.5f);
			NormalAttackApCoef2.Add(5, 0.6f);
			EffectAndSoundWeaponType.Add(0, 1003001);
			EffectAndSoundWeaponType.Add(21, 1003001);
			EffectAndSoundWeaponType.Add(16, 1003002);
			EffectAndSoundWeaponType.Add(19, 1003003);
			FioraActive2Attack.Add(1, 1003311);
			FioraActive2Attack.Add(2, 1003312);
			FioraActive2Attack.Add(3, 1003313);
			FioraActive2Attack.Add(4, 1003314);
			FioraActive2Attack.Add(5, 1003315);
		}
	}
}