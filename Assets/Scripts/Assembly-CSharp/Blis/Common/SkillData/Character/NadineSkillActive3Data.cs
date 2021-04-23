using System.Collections.Generic;

namespace Blis.Common
{
	
	public class NadineSkillActive3Data : Singleton<NadineSkillActive3Data>
	{
		
		public readonly Dictionary<int, int> BuffState1 = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> BuffState2 = new Dictionary<int, int>();

		
		public readonly float DashSpeed = 21f;

		
		public readonly float ExclusiveViewMaintainTime = 6f;

		
		public readonly float LineEndWidth = 0.05f;

		
		public readonly string LineMaterial = "Fx_BI_Nadine_Skill03_Line";

		
		public readonly float LineStartWidth = 0.05f;

		
		public readonly float MaxLineConnectionRange = 10f;

		
		public readonly int ProjectileCode = 63;

		
		public readonly int SummonCode = 1003;

		
		public NadineSkillActive3Data()
		{
			BuffState1.Add(1, 1006401);
			BuffState1.Add(2, 1006402);
			BuffState1.Add(3, 1006403);
			BuffState1.Add(4, 1006404);
			BuffState1.Add(5, 1006405);
			BuffState2.Add(1, 1006411);
			BuffState2.Add(2, 1006412);
			BuffState2.Add(3, 1006413);
			BuffState2.Add(4, 1006414);
			BuffState2.Add(5, 1006415);
		}
	}
}