using System.Collections.Generic;

namespace Blis.Common
{
	
	public class JackieSkillPassiveData : Singleton<JackieSkillPassiveData>
	{
		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> BuffState_2 = new Dictionary<int, int>();

		
		public readonly int LessThanLargeSizeMonsterKillCondition = 1;

		
		public readonly int PlayerOrOverLargeSizeMonsterKillCondition = 1;

		
		public JackieSkillPassiveData()
		{
			BuffState.Add(1, 1001101);
			BuffState.Add(2, 1001102);
			BuffState.Add(3, 1001103);
			BuffState_2.Add(1, 1001111);
			BuffState_2.Add(2, 1001112);
			BuffState_2.Add(3, 1001113);
		}
	}
}