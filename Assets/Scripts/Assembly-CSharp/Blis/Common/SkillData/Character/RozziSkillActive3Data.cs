using System.Collections.Generic;

namespace Blis.Common
{
	
	public class RozziSkillActive3Data : Singleton<RozziSkillActive3Data>
	{
		
		public readonly int Active3_2EnableStateCode = 1021411;

		
		public readonly int Active3_2Group = 1021410;

		
		public readonly float Active3AttackRange = 1f;

		
		public readonly int Active3BuffStateCode = 1021401;

		
		public readonly int Active3HitEffectCode = 1021401;

		
		public readonly float Active3KnockBackDistance = 2f;

		
		public readonly float Active3KnockBackSpeed = 0.2f;

		
		public readonly float Active3KnockBackStunDuration = 0.75f;

		
		public readonly float Active3Move1Duration = 0.3f;

		
		public readonly float Active3Move2Distance = 2.5f;

		
		public readonly float Active3Move2Duration = 0.35f;

		
		public readonly int Active3ProjectileCode = 102140;

		
		public readonly float Active3ShotDuration = 0.1f;

		
		public readonly float DamageActive3_1ApCoef = 0.4f;

		
		public readonly Dictionary<int, int> DamageActive3_1ByLevel = new Dictionary<int, int>
		{
			{
				1,
				50
			},
			{
				2,
				70
			},
			{
				3,
				90
			},
			{
				4,
				110
			},
			{
				5,
				130
			}
		};

		
		public readonly float DamageActive3_2ApCoef = 0.45f;

		
		public readonly Dictionary<int, int> DamageActive3_2ByLevel = new Dictionary<int, int>
		{
			{
				1,
				50
			},
			{
				2,
				70
			},
			{
				3,
				90
			},
			{
				4,
				110
			},
			{
				5,
				130
			}
		};
	}
}