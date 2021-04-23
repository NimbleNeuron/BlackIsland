using System.Collections.Generic;

namespace Blis.Common
{
	
	public class SniperRifleSkillActiveData : Singleton<SniperRifleSkillActiveData>
	{
		
		public readonly float aimingDelay = 0.7f;

		
		public readonly Dictionary<int, float> ApCoefficient = new Dictionary<int, float>();

		
		public readonly int BuffState = 3011001;

		
		public readonly int maxBullet = 3;

		
		public readonly int ProjectileCode = 6;

		
		public readonly float shotDelay = 1f;

		
		public SniperRifleSkillActiveData()
		{
			ApCoefficient.Add(1, 2.5f);
			ApCoefficient.Add(2, 3f);
		}
	}
}