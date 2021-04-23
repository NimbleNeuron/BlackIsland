using System.Collections.Generic;

namespace Blis.Common
{
	
	public class MagnusSkillActive1Data : Singleton<MagnusSkillActive1Data>
	{
		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public readonly int ProjectileCode = 4;

		
		public readonly float SkillApCoef = 0.6f;

		
		public readonly float SkillDefCoef = 0.5f;

		
		public MagnusSkillActive1Data()
		{
			DamageByLevel.Add(1, 40);
			DamageByLevel.Add(2, 100);
			DamageByLevel.Add(3, 160);
			DamageByLevel.Add(4, 220);
			DamageByLevel.Add(5, 280);
			DebuffState.Add(1, 1004201);
			DebuffState.Add(2, 1004202);
			DebuffState.Add(3, 1004203);
			DebuffState.Add(4, 1004204);
			DebuffState.Add(5, 1004205);
		}
	}
}