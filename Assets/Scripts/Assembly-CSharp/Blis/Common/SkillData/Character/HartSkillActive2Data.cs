using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HartSkillActive2Data : Singleton<HartSkillActive2Data>
	{
		
		public readonly int Active2BuffGroup = 1008300;

		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly int BuffState2 = 1008311;

		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public HartSkillActive2Data()
		{
			BuffState.Add(1, 1008301);
			BuffState.Add(2, 1008302);
			BuffState.Add(3, 1008303);
			BuffState.Add(4, 1008304);
			BuffState.Add(5, 1008305);
			DebuffState.Add(1, 1008321);
			DebuffState.Add(2, 1008322);
		}
	}
}