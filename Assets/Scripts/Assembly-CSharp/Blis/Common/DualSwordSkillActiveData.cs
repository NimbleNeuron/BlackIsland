using System.Collections.Generic;

namespace Blis.Common
{
	
	public class DualSwordSkillActiveData : Singleton<DualSwordSkillActiveData>
	{
		
		public readonly float AttackDistance = 2f;

		
		public readonly float AttackDuration = 0.5f;

		
		public readonly float AttackRange = 2f;

		
		public readonly Dictionary<int, float> DamageApCoef = new Dictionary<int, float>();

		
		public readonly float DashDistance = 1.5f;

		
		public readonly float DashDuration = 0.25f;

		
		public readonly int EffectAndSoundCode = 1000000;

		
		public readonly float WaitingForCast = 5f;

		
		public DualSwordSkillActiveData()
		{
			DamageApCoef.Add(1, 0.3f);
			DamageApCoef.Add(2, 0.5f);
		}
	}
}