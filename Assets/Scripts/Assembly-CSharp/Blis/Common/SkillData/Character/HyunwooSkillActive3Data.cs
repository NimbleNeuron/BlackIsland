using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HyunwooSkillActive3Data : Singleton<HyunwooSkillActive3Data>
	{
		
		public readonly float CollisionBoxDepth = 0.5f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly float DashDistance = 5f;

		
		public readonly float DashDuration = 0.4f;

		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public readonly int EffectAndSoundCode = 1007401;

		
		public readonly float HitDashDistance = 3.5f;

		
		public readonly float HitDashDuration = 0.3f;

		
		public readonly Dictionary<int, float> HpRateByLevel = new Dictionary<int, float>();

		
		public readonly Dictionary<int, float> RateByLevel = new Dictionary<int, float>();

		
		public readonly float SkillDefCoef = 0.8f;

		
		public readonly float StunDuration = 1.5f;

		
		public HyunwooSkillActive3Data()
		{
			RateByLevel.Add(1, 0.05f);
			RateByLevel.Add(2, 0.08f);
			RateByLevel.Add(3, 0.11f);
			RateByLevel.Add(4, 0.14f);
			RateByLevel.Add(5, 0.17f);
			HpRateByLevel.Add(1, 0.1f);
			HpRateByLevel.Add(2, 0.11f);
			HpRateByLevel.Add(3, 0.12f);
			HpRateByLevel.Add(4, 0.13f);
			HpRateByLevel.Add(5, 0.14f);
			DamageByLevel.Add(1, 60);
			DamageByLevel.Add(2, 95);
			DamageByLevel.Add(3, 130);
			DamageByLevel.Add(4, 165);
			DamageByLevel.Add(5, 200);
			DebuffState.Add(1, 1007401);
			DebuffState.Add(2, 1007402);
			DebuffState.Add(3, 1007403);
			DebuffState.Add(4, 1007404);
			DebuffState.Add(5, 1007405);
		}
	}
}