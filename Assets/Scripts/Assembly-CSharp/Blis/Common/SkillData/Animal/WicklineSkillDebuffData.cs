using System.Collections.Generic;

namespace Blis.Common
{
	
	public class WicklineSkillDebuffData : Singleton<WicklineSkillDebuffData>
	{
		
		public readonly Dictionary<int, float> DFS_DamageApCoefByLevel = new Dictionary<int, float>();

		
		public readonly Dictionary<int, int> DFS_DamageByLevel = new Dictionary<int, int>();

		
		public readonly string DFS_Effect_Target = "FX_BI_Wickline_Passive_Poisoning";

		
		public readonly int DFS_EffectAndSound_Target;

		
		public readonly int DFS_EffectPointCheck = 1;

		
		public readonly int DFS_IntervalCount = 5;

		
		public readonly float DFS_IntervalTime = 1f;

		
		public WicklineSkillDebuffData()
		{
			DFS_DamageByLevel.Add(1, 200);
			DFS_DamageApCoefByLevel.Add(1, 0f);
		}
	}
}