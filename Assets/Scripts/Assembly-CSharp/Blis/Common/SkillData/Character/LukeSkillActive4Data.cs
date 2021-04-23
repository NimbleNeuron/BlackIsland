using System.Collections.Generic;

namespace Blis.Common
{
	
	public class LukeSkillActive4Data : Singleton<LukeSkillActive4Data>
	{
		
		public readonly int AfterServiceDamageEffectAndSoundCode = 1022505;

		
		public readonly int AfterServiceEvolutionProjectileCode = 102252;

		
		public readonly int AfterServiceProjectileCode = 102251;

		
		public readonly int AfterServiceStateCode = 1022501;

		
		public readonly float AttachSightDuration = 2f;

		
		public readonly float AttachSightRange = 3f;

		
		public readonly float DamageApCoef = 0.8f;

		
		public readonly Dictionary<int, int> DamageBySkillLevel = new Dictionary<int, int>();

		
		public readonly float DamageTargetLossHpCoef = 0.01f;

		
		public readonly float DistanceBackAction = 2f;

		
		public readonly float DurationBackAction = 0.3f;

		
		public LukeSkillActive4Data()
		{
			DamageBySkillLevel.Add(1, 250);
			DamageBySkillLevel.Add(2, 300);
			DamageBySkillLevel.Add(3, 350);
		}
	}
}