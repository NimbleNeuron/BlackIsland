using System.Collections.Generic;

namespace Blis.Common
{
	
	public class EmmaSkillActive4Data : Singleton<EmmaSkillActive4Data>
	{
		
		public readonly int FireworkHatExplosionAreaProjectileCode = 101951;

		
		public readonly float FireworkHatExplosionDamageApCoef = 0.75f;

		
		public readonly Dictionary<int, int> FireworkHatExplosionDamageBySkillLevel = new Dictionary<int, int>();

		
		public readonly int FireworkHatExplosionEffectAndSoundCode = 1019503;

		
		public readonly int FireworkHatKnockbackProjectileCode = 101952;

		
		public readonly float KnockbackDistance = 2.5f;

		
		public readonly float KnockbackDuration = 0.5f;

		
		public readonly float KnockbackInnerRange = 0.3f;

		
		public readonly int MagicRabbitFetterStateCode = 1019521;

		
		public readonly int MagicRabbitStateCode = 1019511;

		
		public readonly float PigeonAttackDamageApCoef = 0.45f;

		
		public readonly Dictionary<int, int> PigeonAttackDamageBySkillLevel = new Dictionary<int, int>();

		
		public readonly int PigeonDealerFetterStateCode = 1019501;

		
		public readonly int PigeonLineProjectileCode = 101953;

		
		public readonly int WarpAfterEffectAndSoundCode = 1019506;

		
		public readonly int WarpPrevEffectAndSoundCode = 1019502;

		
		public EmmaSkillActive4Data()
		{
			PigeonAttackDamageBySkillLevel.Add(1, 175);
			PigeonAttackDamageBySkillLevel.Add(2, 200);
			PigeonAttackDamageBySkillLevel.Add(3, 225);
			FireworkHatExplosionDamageBySkillLevel.Add(1, 225);
			FireworkHatExplosionDamageBySkillLevel.Add(2, 250);
			FireworkHatExplosionDamageBySkillLevel.Add(3, 275);
		}
	}
}