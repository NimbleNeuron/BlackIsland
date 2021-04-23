using System.Collections.Generic;

namespace Blis.Common
{
	
	public class XiukaiSkillActive3Data : Singleton<XiukaiSkillActive3Data>
	{
		
		public float AirborneDuration = 1f;

		
		public Dictionary<int, int> BaseDamage = new Dictionary<int, int>();

		
		public float DashDistance = 7f;

		
		public float DashDuration = 1.07f;

		
		public Dictionary<int, float> SkillAddDamageMaxHpRatio = new Dictionary<int, float>();

		
		public Dictionary<int, float> SkillAddDamageMaxHpRatio2 = new Dictionary<int, float>();

		
		public float SkillApCoef = 0.5f;

		
		public XiukaiSkillActive3Data()
		{
			BaseDamage.Add(1, 80);
			BaseDamage.Add(2, 110);
			BaseDamage.Add(3, 140);
			BaseDamage.Add(4, 170);
			BaseDamage.Add(5, 190);
			SkillAddDamageMaxHpRatio.Add(1, 3f);
			SkillAddDamageMaxHpRatio.Add(2, 3f);
			SkillAddDamageMaxHpRatio.Add(3, 3f);
			SkillAddDamageMaxHpRatio.Add(4, 3f);
			SkillAddDamageMaxHpRatio.Add(5, 3f);
			SkillAddDamageMaxHpRatio2.Add(1, 6f);
			SkillAddDamageMaxHpRatio2.Add(2, 6f);
			SkillAddDamageMaxHpRatio2.Add(3, 6f);
			SkillAddDamageMaxHpRatio2.Add(4, 6f);
			SkillAddDamageMaxHpRatio2.Add(5, 6f);
		}
	}
}