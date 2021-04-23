using System.Collections.Generic;

namespace Blis.Common
{
	
	public class EmmaSkillNormalAttackData : Singleton<EmmaSkillNormalAttackData>
	{
		
		public readonly int DamageBuffEffectAndSoundCode = 1019102;

		
		public readonly Dictionary<MasteryType, int> DamageEffectAndSoundCode;

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly Dictionary<MasteryType, float> NormalAttackDelay;

		
		public readonly Dictionary<MasteryType, int> ProjectileCode;

		
		public EmmaSkillNormalAttackData()
		{
			ProjectileCode =
				new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
				{
					{
						MasteryType.DirectFire,
						101901
					},
					{
						MasteryType.HighAngleFire,
						101701
					}
				};
			NormalAttackDelay =
				new Dictionary<MasteryType, float>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
				{
					{
						MasteryType.DirectFire,
						0.29f
					},
					{
						MasteryType.HighAngleFire,
						0.29f
					}
				};
			DamageEffectAndSoundCode =
				new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
				{
					{
						MasteryType.DirectFire,
						1019101
					},
					{
						MasteryType.HighAngleFire,
						0
					}
				};
		}
	}
}