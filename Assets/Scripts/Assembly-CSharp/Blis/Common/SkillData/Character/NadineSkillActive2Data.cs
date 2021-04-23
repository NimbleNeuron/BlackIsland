using System.Collections.Generic;

namespace Blis.Common
{
	
	public class NadineSkillActive2Data : Singleton<NadineSkillActive2Data>
	{
		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DamageByLevel_2 = new Dictionary<int, int>();

		
		public readonly int DebuffState = 1006301;

		
		public readonly float ExclusiveViewSkillReuseTime = 5f;

		
		public readonly float ExclusiveViewTrapConnectTime = 70f;

		
		public readonly float GainSightDuration = 5f;

		
		public readonly float LineEndWidth = 0.06f;

		
		public readonly string LineMaterial = "Fx_BI_Nadine_Skill02_Line";

		
		public readonly float LineStartWidth = 0.06f;

		
		public readonly float MaxLinkRange = 5f;

		
		public readonly int ProjectileCode1 = 61;

		
		public readonly int ProjectileCode2 = 62;

		
		public readonly float SkillApCoef = 0.6f;

		
		public readonly int SummonCode1 = 1001;

		
		public readonly int SummonCode2 = 1002;

		
		public NadineSkillActive2Data()
		{
			DamageByLevel.Add(1, 100);
			DamageByLevel.Add(2, 170);
			DamageByLevel.Add(3, 240);
			DamageByLevel.Add(4, 310);
			DamageByLevel.Add(5, 380);
			DamageByLevel_2.Add(1, 100);
			DamageByLevel_2.Add(2, 140);
			DamageByLevel_2.Add(3, 180);
			DamageByLevel_2.Add(4, 220);
			DamageByLevel_2.Add(5, 260);
		}
	}
}