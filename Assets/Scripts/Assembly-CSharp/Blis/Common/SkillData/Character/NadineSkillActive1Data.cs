using System.Collections.Generic;

namespace Blis.Common
{
	
	public class NadineSkillActive1Data : Singleton<NadineSkillActive1Data>
	{
		
		public readonly int BuffState = 1006201;

		
		public readonly float ChargingTime = 2f;

		
		public readonly Dictionary<int, int> MaxDamageByLevel = new Dictionary<int, int>();

		
		public readonly float MaxSkillApCoef = 1.2f;

		
		public readonly float MaxSkillRange = 2f;

		
		public readonly float MaxSkillRange2 = 2.5f;

		
		public readonly Dictionary<int, int> MinDamageByLevel = new Dictionary<int, int>();

		
		public readonly float MinSkillApCoef = 0.6f;

		
		public readonly int PassiveBuffState = 1006101;

		
		public readonly int PassiveStackCount = 50;

		
		public readonly Dictionary<int, int> ProjectileCode = new Dictionary<int, int>();

		
		public readonly float RecoverySpRatio = 0.5f;

		
		public readonly float WaitingTime = 3f;

		
		public NadineSkillActive1Data()
		{
			MinDamageByLevel.Add(1, 70);
			MinDamageByLevel.Add(2, 115);
			MinDamageByLevel.Add(3, 160);
			MinDamageByLevel.Add(4, 205);
			MinDamageByLevel.Add(5, 250);
			MaxDamageByLevel.Add(1, 140);
			MaxDamageByLevel.Add(2, 230);
			MaxDamageByLevel.Add(3, 320);
			MaxDamageByLevel.Add(4, 410);
			MaxDamageByLevel.Add(5, 500);
			ProjectileCode.Add(7, 42);
			ProjectileCode.Add(8, 52);
		}
	}
}