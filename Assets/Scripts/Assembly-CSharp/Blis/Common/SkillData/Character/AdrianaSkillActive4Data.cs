using System.Collections.Generic;

namespace Blis.Common
{
	
	public class AdrianaSkillActive4Data : Singleton<AdrianaSkillActive4Data>
	{
		
		public readonly float DamageApCoef = 0.4f;

		
		public readonly Dictionary<int, int> DamageBySkillLevel = new Dictionary<int, int>();

		
		public readonly int ExplosionEffectSound = 1017501;

		
		public readonly int FireBombPositionProjectileCode = 101742;

		
		public readonly int FireBombProjectileCode = 101741;

		
		public readonly int FireBombStateCode = 1017501;

		
		public readonly int FireFlame3ProjectileCode = 101740;

		
		public readonly float KnockBackDistance = 0.75f;

		
		public readonly float KnockBackDuration = 0.3f;

		
		public AdrianaSkillActive4Data()
		{
			DamageBySkillLevel.Add(1, 70);
			DamageBySkillLevel.Add(2, 130);
			DamageBySkillLevel.Add(3, 190);
		}
	}
}