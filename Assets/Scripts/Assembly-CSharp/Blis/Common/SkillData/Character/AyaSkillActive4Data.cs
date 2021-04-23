using System.Collections.Generic;

namespace Blis.Common
{
	
	public class AyaSkillActive4Data : Singleton<AyaSkillActive4Data>
	{
		
		public readonly Dictionary<int, int> DamageByMaxLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DamageByMinLevel = new Dictionary<int, int>();

		
		public readonly int DebuffState = 1002501;

		
		public readonly Dictionary<int, float> DebuffStateMaxDuration = new Dictionary<int, float>();

		
		public readonly float DebuffStateMinDuration = 0.5f;

		
		public readonly float SkillMaxApCoef = 1.4f;

		
		public readonly float SkillMinApCoef = 0.7f;

		
		public AyaSkillActive4Data()
		{
			DamageByMinLevel.Add(1, 200);
			DamageByMinLevel.Add(2, 300);
			DamageByMinLevel.Add(3, 400);
			DamageByMaxLevel.Add(1, 400);
			DamageByMaxLevel.Add(2, 600);
			DamageByMaxLevel.Add(3, 800);
			DebuffStateMaxDuration.Add(1, 1.5f);
			DebuffStateMaxDuration.Add(2, 1.75f);
			DebuffStateMaxDuration.Add(3, 2f);
		}
	}
}