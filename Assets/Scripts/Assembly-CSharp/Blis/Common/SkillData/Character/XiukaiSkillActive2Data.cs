using System.Collections.Generic;

namespace Blis.Common
{
	
	public class XiukaiSkillActive2Data : Singleton<XiukaiSkillActive2Data>
	{
		
		public Dictionary<int, int> BaseDamage = new Dictionary<int, int>();

		
		public Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public Dictionary<int, int> DebuffState2 = new Dictionary<int, int>();

		
		public Dictionary<int, float> SkillAddDamageMaxHpRatio = new Dictionary<int, float>();

		
		public float SkillApCoef = 0.5f;

		
		public XiukaiSkillActive2Data()
		{
			BaseDamage.Add(1, 60);
			BaseDamage.Add(2, 100);
			BaseDamage.Add(3, 140);
			BaseDamage.Add(4, 180);
			BaseDamage.Add(5, 220);
			SkillAddDamageMaxHpRatio.Add(1, 2f);
			SkillAddDamageMaxHpRatio.Add(2, 2.5f);
			SkillAddDamageMaxHpRatio.Add(3, 3f);
			SkillAddDamageMaxHpRatio.Add(4, 3.5f);
			SkillAddDamageMaxHpRatio.Add(5, 4f);
			DebuffState.Add(1, 1013301);
			DebuffState.Add(2, 1013302);
			DebuffState.Add(3, 1013303);
			DebuffState.Add(4, 1013304);
			DebuffState.Add(5, 1013305);
			DebuffState2.Add(1, 1013311);
			DebuffState2.Add(2, 1013312);
			DebuffState2.Add(3, 1013313);
			DebuffState2.Add(4, 1013314);
			DebuffState2.Add(5, 1013315);
		}
	}
}