using System.Collections.Generic;

namespace Blis.Common
{
	
	public class FioraSkillActive3Data : Singleton<FioraSkillActive3Data>
	{
		
		public readonly float BackDashDistance = 4f;

		
		public readonly float BackDashDuration = 0.3f;

		
		public readonly float CollisionBoxDepth = 1.2f;

		
		public readonly float CooldownReduce = -4f;

		
		public readonly float CooldownReduce2 = -4f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly float DashDistance = 5f;

		
		public readonly float DashDuration = 0.4f;

		
		public readonly int EffectAndSound = 1000000;

		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public readonly float NextActionWaitTime = 2f;

		
		public readonly float SkillApCoef = 0.4f;

		
		public FioraSkillActive3Data()
		{
			DamageByLevel.Add(1, 90);
			DamageByLevel.Add(2, 130);
			DamageByLevel.Add(3, 170);
			DamageByLevel.Add(4, 210);
			DamageByLevel.Add(5, 250);
			EffectAndSoundWeaponType.Add(0, 1003011);
			EffectAndSoundWeaponType.Add(21, 1003011);
			EffectAndSoundWeaponType.Add(16, 1003012);
			EffectAndSoundWeaponType.Add(19, 1003013);
		}
	}
}