using System.Collections.Generic;

namespace Blis.Common
{
	
	public class AdrianaSkillNormalAttackData : Singleton<AdrianaSkillNormalAttackData>
	{
		
		public readonly int HighAngleFireDamageEffectAndSoundCode = 1017101;

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly Dictionary<MasteryType, float> NormalAttackDelay;

		
		public readonly Dictionary<MasteryType, int> ProjectileCode;

		
		public AdrianaSkillNormalAttackData()
		{
			ProjectileCode =
				new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
				{
					{
						MasteryType.DirectFire,
						72
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
		}
	}
}