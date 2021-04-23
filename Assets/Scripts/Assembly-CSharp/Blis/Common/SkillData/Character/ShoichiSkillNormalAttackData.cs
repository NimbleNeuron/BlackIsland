using System.Collections.Generic;

namespace Blis.Common
{
	
	public class ShoichiSkillNormalAttackData : Singleton<ShoichiSkillNormalAttackData>
	{
		
		public readonly int EffectAndSoundOneHandSwordType = 1018001;

		
		public readonly int EffectAndSoundRapierType = 1018002;

		
		public readonly Dictionary<MasteryType, List<int>> EffectAndSoundWeaponType =
			new Dictionary<MasteryType, List<int>>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.OneHandSword,
					new List<int>
					{
						1018001,
						1018002
					}
				},
				{
					MasteryType.Rapier,
					new List<int>
					{
						1018011,
						1018012
					}
				}
			};

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly Dictionary<MasteryType, float> NormalAttackDelay =
			new Dictionary<MasteryType, float>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.OneHandSword,
					0.19f
				},
				{
					MasteryType.Rapier,
					0.19f
				}
			};

		
		public readonly float NormalAttackNextTime = 3f;

		
		public readonly float NormalAttackOneHandSwordDelay = 0.19f;

		
		public readonly float NormalAttackRapierDelay = 0.19f;
	}
}