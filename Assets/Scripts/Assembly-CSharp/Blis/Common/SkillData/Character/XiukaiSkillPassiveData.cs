using System.Collections.Generic;

namespace Blis.Common
{
	
	public class XiukaiSkillPassiveData : Singleton<XiukaiSkillPassiveData>
	{
		
		public int AddMaxHp = 7;

		
		public int AddRecoveryPercent = 20;

		
		public int BuffState = 1013101;

		
		public Dictionary<int, int> EpicStack = new Dictionary<int, int>();

		
		public Dictionary<int, int> LegendStack = new Dictionary<int, int>();

		
		public Dictionary<int, int> RareStack = new Dictionary<int, int>();

		
		public Dictionary<int, int> UncommonStack = new Dictionary<int, int>();

		
		public XiukaiSkillPassiveData()
		{
			UncommonStack.Add(1, 1);
			UncommonStack.Add(2, 2);
			UncommonStack.Add(3, 3);
			RareStack.Add(1, 1);
			RareStack.Add(2, 2);
			RareStack.Add(3, 3);
			EpicStack.Add(1, 1);
			EpicStack.Add(2, 2);
			EpicStack.Add(3, 3);
			LegendStack.Add(1, 1);
			LegendStack.Add(2, 2);
			LegendStack.Add(3, 3);
		}
	}
}