using System.Collections.Generic;

namespace Blis.Common
{
	
	public class WhipSkillActiveData : Singleton<WhipSkillActiveData>
	{
		
		public readonly float AirborneDuration = 0.75f;

		
		public readonly float AirbornePower = 0.4f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				100
			},
			{
				2,
				150
			}
		};

		
		public readonly float DamageCoef = 0.3f;

		
		public readonly int EffectSoundCode;

		
		public readonly int HookLineProjectileCode = 303100;

		
		public readonly Dictionary<int, int> HookLineProjectileCodes = new Dictionary<int, int>
		{
			{
				20,
				303100
			}
		};

		
		public readonly float KnockBackMinDistance = 2.5f;

		
		public readonly float KnockBackMoveDuration = 0.75f;

		
		public readonly int ProjectileCode = 303100;
	}
}