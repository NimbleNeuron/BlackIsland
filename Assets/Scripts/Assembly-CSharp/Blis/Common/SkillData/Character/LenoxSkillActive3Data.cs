using System.Collections.Generic;

namespace Blis.Common
{
	
	public class LenoxSkillActive3Data : Singleton<LenoxSkillActive3Data>
	{
		
		public const int CollisionPartitionCount = 5;

		
		public const float Active3SkillDuration = 0.26f;

		
		public readonly int Active3KnockbackCode = 1020401;

		
		public readonly Dictionary<int, int> Active3SlowCode = new Dictionary<int, int>
		{
			{
				1,
				1020411
			},
			{
				2,
				1020412
			},
			{
				3,
				1020413
			},
			{
				4,
				1020414
			},
			{
				5,
				1020415
			}
		};

		
		public readonly float AirbornPower = 0.3f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				20
			},
			{
				2,
				80
			},
			{
				3,
				140
			},
			{
				4,
				200
			},
			{
				5,
				260
			}
		};

		
		public readonly int EffectAndSoundCode = 1020401;

		
		public readonly float KnockbackDistance = 1.5f;

		
		public readonly float SkillApCoef = 0.3f;
	}
}