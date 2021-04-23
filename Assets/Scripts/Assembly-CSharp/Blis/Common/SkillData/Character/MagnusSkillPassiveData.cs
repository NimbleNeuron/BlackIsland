using System.Collections.Generic;

namespace Blis.Common
{
	
	public class MagnusSkillPassiveData : Singleton<MagnusSkillPassiveData>
	{
		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public MagnusSkillPassiveData()
		{
			BuffState.Add(1, 1004101);
			BuffState.Add(2, 1004102);
			BuffState.Add(3, 1004103);
		}
	}
}