using System.Collections.Generic;

namespace Blis.Common
{
	
	public class JackieSkillActive3Data : Singleton<JackieSkillActive3Data>
	{
		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly float DashDuration = 0.5f;

		
		public readonly int DebuffState = 1001411;

		
		public readonly Dictionary<int, int> ReinforcedDebuffState = new Dictionary<int, int>();

		
		public readonly Dictionary<int, float> SkillApCoefByLevel = new Dictionary<int, float>();

		
		public JackieSkillActive3Data()
		{
			DamageByLevel.Add(1, 10);
			DamageByLevel.Add(2, 70);
			DamageByLevel.Add(3, 130);
			DamageByLevel.Add(4, 190);
			DamageByLevel.Add(5, 250);
			ReinforcedDebuffState.Add(1, 1001401);
			ReinforcedDebuffState.Add(2, 1001402);
			ReinforcedDebuffState.Add(3, 1001403);
			ReinforcedDebuffState.Add(4, 1001404);
			ReinforcedDebuffState.Add(5, 1001405);
			SkillApCoefByLevel.Add(1, 0.3f);
			SkillApCoefByLevel.Add(2, 0.4f);
			SkillApCoefByLevel.Add(3, 0.5f);
			SkillApCoefByLevel.Add(4, 0.6f);
			SkillApCoefByLevel.Add(5, 0.7f);
		}
	}
}