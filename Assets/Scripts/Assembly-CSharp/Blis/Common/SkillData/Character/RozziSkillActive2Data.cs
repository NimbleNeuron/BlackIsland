using System.Collections.Generic;

namespace Blis.Common
{
	
	public class RozziSkillActive2Data : Singleton<RozziSkillActive2Data>
	{
		
		public readonly float A2AttackEndTime = 0.4f;

		
		public readonly float A2AttackStartTime = 0.1f;

		
		public readonly int Active2HitEffectSound = 1021301;

		
		public readonly int Active2SpeedUpStateCode = 1021301;

		
		public readonly float DamageApCoef = 0.35f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				70
			},
			{
				2,
				110
			},
			{
				3,
				150
			},
			{
				4,
				190
			},
			{
				5,
				230
			}
		};

		
		public readonly Dictionary<int, int> DebuffStateCodeByLevel = new Dictionary<int, int>
		{
			{
				1,
				1021311
			},
			{
				2,
				1021312
			},
			{
				3,
				1021313
			},
			{
				4,
				1021314
			},
			{
				5,
				1021315
			}
		};
	}
}