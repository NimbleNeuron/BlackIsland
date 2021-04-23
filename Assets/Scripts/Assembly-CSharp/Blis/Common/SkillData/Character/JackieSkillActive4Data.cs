using System.Collections.Generic;

namespace Blis.Common
{
	
	public class JackieSkillActive4Data : Singleton<JackieSkillActive4Data>
	{
		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly int DebuffGroup = 1001510;

		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public readonly Dictionary<int, float> DFS_DamageApCoefByLevel = new Dictionary<int, float>();

		
		public readonly Dictionary<int, int> DFS_DamageByLevel = new Dictionary<int, int>();

		
		public readonly string DFS_Effect_Target = "FX_BI_Common_Bleeding_Debuff";

		
		public readonly int DFS_EffectAndSound_Target = 1000001;

		
		public readonly int DFS_EffectPointCheck;

		
		public readonly int DFS_IntervalCount = 6;

		
		public readonly float DFS_IntervalTime = 1f;

		
		public readonly Dictionary<int, int> FinishDamageByLevel = new Dictionary<int, int>();

		
		public readonly float FinishSkillApCoef = 1f;

		
		public readonly Dictionary<int, int> FinishSkillDataCode = new Dictionary<int, int>();

		
		public JackieSkillActive4Data()
		{
			BuffState.Add(1, 1001501);
			BuffState.Add(2, 1001502);
			BuffState.Add(3, 1001503);
			DebuffState.Add(1, 1001511);
			DebuffState.Add(2, 1001512);
			DebuffState.Add(3, 1001513);
			DFS_DamageByLevel.Add(1, 10);
			DFS_DamageByLevel.Add(2, 15);
			DFS_DamageByLevel.Add(3, 20);
			DFS_DamageApCoefByLevel.Add(1, 0f);
			DFS_DamageApCoefByLevel.Add(2, 0f);
			DFS_DamageApCoefByLevel.Add(3, 0f);
			FinishSkillDataCode.Add(1, 1001511);
			FinishSkillDataCode.Add(2, 1001512);
			FinishSkillDataCode.Add(3, 1001513);
			FinishDamageByLevel.Add(1, 300);
			FinishDamageByLevel.Add(2, 500);
			FinishDamageByLevel.Add(3, 700);
		}
	}
}