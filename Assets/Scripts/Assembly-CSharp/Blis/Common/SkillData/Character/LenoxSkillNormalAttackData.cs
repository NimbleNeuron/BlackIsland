using System.Collections.Generic;

namespace Blis.Common
{
	
	public class LenoxSkillNormalAttackData : Singleton<LenoxSkillNormalAttackData>
	{
		
		public readonly Dictionary<MasteryType, List<int>> EffectAndSoundWeaponType =
			new Dictionary<MasteryType, List<int>>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.Whip,
					new List<int>
					{
						1020101,
						1020101
					}
				}
			};

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly Dictionary<MasteryType, float> NormalAttackDelay =
			new Dictionary<MasteryType, float>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.Whip,
					0.26f
				}
			};

		
		public readonly float NormalAttackNextTime = 3f;
	}
}