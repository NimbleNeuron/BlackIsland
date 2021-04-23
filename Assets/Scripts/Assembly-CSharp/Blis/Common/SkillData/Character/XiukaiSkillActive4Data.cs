using System.Collections.Generic;

namespace Blis.Common
{
	
	public class XiukaiSkillActive4Data : Singleton<XiukaiSkillActive4Data>
	{
		
		public float AddStackDamage = 0.9f;

		
		public int AttackCount = 6;

		
		public float AttackDelay = 0.5f;

		
		public Dictionary<int, int> BaseDamage = new Dictionary<int, int>();

		
		public Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public int PassiveBuffState = 1013101;

		
		public float SkillApCoef = 0.5f;

		
		public XiukaiSkillActive4Data()
		{
			BaseDamage.Add(1, 20);
			BaseDamage.Add(2, 65);
			BaseDamage.Add(3, 110);
			DebuffState.Add(1, 1013501);
			DebuffState.Add(2, 1013502);
			DebuffState.Add(3, 1013503);
		}
	}
}