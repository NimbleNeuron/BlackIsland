using System.Collections.Generic;

namespace Blis.Common
{
	
	public class MagnusSkillActive3Data : Singleton<MagnusSkillActive3Data>
	{
		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public readonly float SkillApCoef = 0.4f;

		
		public readonly int StunCode = 1004401;

		
		public readonly float TargetMoveDistance = 3.25f;

		
		public readonly float TargetMoveDuration = 0.3f;

		
		public MagnusSkillActive3Data()
		{
			DamageByLevel.Add(1, 60);
			DamageByLevel.Add(2, 115);
			DamageByLevel.Add(3, 170);
			DamageByLevel.Add(4, 225);
			DamageByLevel.Add(5, 280);
			EffectAndSoundWeaponType.Add(0, 1004401);
			EffectAndSoundWeaponType.Add(3, 1004401);
			EffectAndSoundWeaponType.Add(13, 1004402);
		}
	}
}