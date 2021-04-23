using System.Collections.Generic;

namespace Blis.Common
{
	
	public class NadineSkillPassiveData : Singleton<NadineSkillPassiveData>
	{
		
		public readonly int BuffState = 1006101;

		
		public readonly float PassiveSightRange = 20f;

		
		public readonly Dictionary<int, int> StackCountOnKillCommon = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> StackCountOnKillEpic = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> StackCountOnKillRare = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> StackCountOnKillUncommon = new Dictionary<int, int>();

		
		public NadineSkillPassiveData()
		{
			StackCountOnKillCommon.Add(1, 1);
			StackCountOnKillCommon.Add(2, 2);
			StackCountOnKillCommon.Add(3, 3);
			StackCountOnKillUncommon.Add(1, 3);
			StackCountOnKillUncommon.Add(2, 4);
			StackCountOnKillUncommon.Add(3, 5);
			StackCountOnKillRare.Add(1, 6);
			StackCountOnKillRare.Add(2, 7);
			StackCountOnKillRare.Add(3, 8);
			StackCountOnKillEpic.Add(1, 9);
			StackCountOnKillEpic.Add(2, 11);
			StackCountOnKillEpic.Add(3, 13);
		}
	}
}