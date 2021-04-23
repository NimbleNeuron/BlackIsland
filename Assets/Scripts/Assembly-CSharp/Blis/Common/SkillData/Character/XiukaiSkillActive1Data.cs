using System.Collections.Generic;

namespace Blis.Common
{
	
	public class XiukaiSkillActive1Data : Singleton<XiukaiSkillActive1Data>
	{
		
		public Dictionary<int, int> BaseDamage = new Dictionary<int, int>();

		
		public Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public int ProjectileCode = 101301;

		
		public float SkillApCoef = 0.5f;

		
		public XiukaiSkillActive1Data()
		{
			BaseDamage.Add(1, 80);
			BaseDamage.Add(2, 120);
			BaseDamage.Add(3, 160);
			BaseDamage.Add(4, 200);
			BaseDamage.Add(5, 240);
			DebuffState.Add(1, 1013201);
			DebuffState.Add(2, 1013202);
			DebuffState.Add(3, 1013203);
			DebuffState.Add(4, 1013204);
			DebuffState.Add(5, 1013205);
		}
	}
}