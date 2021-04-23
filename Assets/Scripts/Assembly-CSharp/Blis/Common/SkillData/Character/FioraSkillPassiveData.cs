using System.Collections.Generic;

namespace Blis.Common
{
	
	public class FioraSkillPassiveData : Singleton<FioraSkillPassiveData>
	{
		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public FioraSkillPassiveData()
		{
			DebuffState.Add(1, 1003101);
			DebuffState.Add(2, 1003102);
			DebuffState.Add(3, 1003103);
		}
	}
}