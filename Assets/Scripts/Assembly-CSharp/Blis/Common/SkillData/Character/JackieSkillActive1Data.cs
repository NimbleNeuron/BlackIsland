using System.Collections.Generic;

namespace Blis.Common
{
	
	public class JackieSkillActive1Data : Singleton<JackieSkillActive1Data>
	{
		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DamageByLevel_2 = new Dictionary<int, int>();

		
		public readonly int DebuffGroup = 1001200;

		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public readonly Dictionary<int, float> DFS_DamageApCoefByLevel = new Dictionary<int, float>();

		
		public readonly Dictionary<int, int> DFS_DamageByLevel = new Dictionary<int, int>();

		
		public readonly string DFS_Effect_Target = "FX_BI_Common_Bleeding_Debuff";

		
		public readonly int DFS_EffectAndSound_Target = 1000001;

		
		public readonly int DFS_EffectPointCheck;

		
		public readonly int DFS_IntervalCount = 5;

		
		public readonly float DFS_IntervalTime = 1f;

		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public readonly float SkillApCoef = 0.25f;

		
		public readonly float SkillApCoef_2 = 0.65f;

		
		public readonly float SkillAttackDelay = 0.16f;

		
		public readonly float SkillAttackDelay_2 = 0.23f;

		
		public JackieSkillActive1Data()
		{
			DamageByLevel.Add(1, 45);
			DamageByLevel.Add(2, 80);
			DamageByLevel.Add(3, 115);
			DamageByLevel.Add(4, 150);
			DamageByLevel.Add(5, 195);
			DamageByLevel_2.Add(1, 30);
			DamageByLevel_2.Add(2, 55);
			DamageByLevel_2.Add(3, 80);
			DamageByLevel_2.Add(4, 105);
			DamageByLevel_2.Add(5, 130);
			DebuffState.Add(1, 1001201);
			DebuffState.Add(2, 1001202);
			DebuffState.Add(3, 1001203);
			DebuffState.Add(4, 1001204);
			DebuffState.Add(5, 1001205);
			DFS_DamageByLevel.Add(1, 80);
			DFS_DamageByLevel.Add(2, 110);
			DFS_DamageByLevel.Add(3, 140);
			DFS_DamageByLevel.Add(4, 170);
			DFS_DamageByLevel.Add(5, 200);
			DFS_DamageApCoefByLevel.Add(1, 0f);
			DFS_DamageApCoefByLevel.Add(2, 0f);
			DFS_DamageApCoefByLevel.Add(3, 0f);
			DFS_DamageApCoefByLevel.Add(4, 0f);
			DFS_DamageApCoefByLevel.Add(5, 0f);
			EffectAndSoundWeaponType.Add(0, 1001201);
			EffectAndSoundWeaponType.Add(15, 1001201);
			EffectAndSoundWeaponType.Add(16, 1001202);
			EffectAndSoundWeaponType.Add(14, 1001203);
			EffectAndSoundWeaponType.Add(18, 1001204);
		}
	}
}