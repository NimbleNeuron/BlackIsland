using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HyunwooSkillActive2Data : Singleton<HyunwooSkillActive2Data>
	{
		
		public readonly int BuffState = 1007301;

		
		public readonly Dictionary<int, int> BuffState_2 = new Dictionary<int, int>();

		
		public HyunwooSkillActive2Data()
		{
			BuffState = 1007301;
			BuffState_2.Add(1, 1007311);
			BuffState_2.Add(2, 1007312);
			BuffState_2.Add(3, 1007313);
			BuffState_2.Add(4, 1007314);
			BuffState_2.Add(5, 1007315);
		}
	}
}