using System.Collections.Generic;

namespace Blis.Common
{
	
	public class NunchakuSkillActiveData : Singleton<NunchakuSkillActiveData>
	{
		
		public readonly float BonusEffectChargingTime = 1f;

		
		public readonly float MaxApCoef = 1.5f;

		
		public readonly Dictionary<int, int> MaxDamageByLevel = new Dictionary<int, int>();

		
		public readonly float MaxSkillRange = 8f;

		
		public readonly float MinApCoef = 0.5f;

		
		public readonly Dictionary<int, int> MinDamageByLevel = new Dictionary<int, int>();

		
		public readonly float MinSkillRange = 4f;

		
		public readonly int ProjectileCode = 302001;

		
		public readonly int ProjectileCode2 = 302002;

		
		public readonly float StunDuration = 1.25f;

		
		public readonly float WaitingTime = 0.5f;

		
		public int EffectAndSound;

		
		public float SkillDamageDelay = 0.26f;

		
		public NunchakuSkillActiveData()
		{
			MinDamageByLevel.Add(1, 150);
			MinDamageByLevel.Add(2, 300);
			MaxDamageByLevel.Add(1, 300);
			MaxDamageByLevel.Add(2, 600);
		}
	}
}