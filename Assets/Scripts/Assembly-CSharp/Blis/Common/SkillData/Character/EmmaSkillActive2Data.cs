using System.Collections.Generic;

namespace Blis.Common
{
	
	public class EmmaSkillActive2Data : Singleton<EmmaSkillActive2Data>
	{
		
		public readonly float CooldownReduce = -3f;

		
		public readonly float DamageApCoef = 0.75f;

		
		public readonly Dictionary<int, int> DamageBySkillLevel = new Dictionary<int, int>();

		
		public readonly int FireworkHatDropEffectAndSoundCode = 1019302;

		
		public readonly int FireworkHatExplosionAreaProjectileCode = 101931;

		
		public readonly int FireworkHatExplosionDamageEffectAndSoundCode = 1019303;

		
		public readonly int FireworkHatExplosionEffectAndSoundCode = 1019301;

		
		public readonly int FireworkHatSummonCode = 1052;

		
		public EmmaSkillActive2Data()
		{
			DamageBySkillLevel.Add(1, 100);
			DamageBySkillLevel.Add(2, 150);
			DamageBySkillLevel.Add(3, 200);
			DamageBySkillLevel.Add(4, 250);
			DamageBySkillLevel.Add(5, 300);
		}
	}
}