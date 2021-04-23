using System.Collections.Generic;

namespace Blis.Common
{
	public class WildDogSkillData : Singleton<WildDogSkillData>
	{
		public readonly float ApCoefficient = 1f;


		public readonly float AttackDelay = 0.2f;


		public readonly int BleedState = 4004101;


		public readonly Dictionary<int, float> DFS_DamageApCoefByLevel = new Dictionary<int, float>();


		public readonly Dictionary<int, int> DFS_DamageByLevel = new Dictionary<int, int>();


		public readonly string DFS_Effect_Target = "FX_BI_Common_Bleeding_Debuff";


		public readonly int DFS_EffectAndSound_Target = 2000013;


		public readonly int DFS_EffectPointCheck;


		public readonly int DFS_IntervalCount = 3;


		public readonly float DFS_IntervalTime = 1f;

		public WildDogSkillData()
		{
			DFS_DamageByLevel.Add(1, 90);
			DFS_DamageApCoefByLevel.Add(1, 0f);
		}
	}
}