using System.Collections.Generic;

namespace Blis.Common
{
	
	public class MagnusSkillActive4Data : Singleton<MagnusSkillActive4Data>
	{
		
		public readonly float BikeCurveSpeed = 60f;

		
		public readonly int BuffState = 1004501;

		
		public readonly float CheckMoveOnRide = 0.55f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly float DepthOnRide = 1f;

		
		public readonly int ProjectileCode = 5;

		
		public readonly Dictionary<int, float> SkillApCoef = new Dictionary<int, float>();

		
		public readonly float SkillAttackDelay = 0.09f;

		
		public readonly Dictionary<int, float> SkillDuration = new Dictionary<int, float>();

		
		public readonly float WidthOnRide = 0.75f;

		
		public MagnusSkillActive4Data()
		{
			SkillDuration.Add(1, 9f);
			SkillDuration.Add(2, 10f);
			SkillDuration.Add(3, 11f);
			DamageByLevel.Add(1, 200);
			DamageByLevel.Add(2, 375);
			DamageByLevel.Add(3, 550);
			SkillApCoef.Add(1, 2f);
			SkillApCoef.Add(2, 2f);
			SkillApCoef.Add(3, 2f);
		}
	}
}