using System.Collections.Generic;

namespace Blis.Common
{
	
	public class LukeSkillActive3Data : Singleton<LukeSkillActive3Data>
	{
		
		public readonly float DamageApCoef = 0.4f;

		
		public readonly Dictionary<int, int> DamageBySkillLevel = new Dictionary<int, int>();

		
		public readonly int DamageEffectAndSoundCode = 1022401;

		
		public readonly int EvolutionDamageEffectAndSoundCode = 1022402;

		
		public readonly int SilentVacuumCleanerMoveSpeedDownStateCode = 1022411;

		
		public readonly int SilentVacuumCleanerStateCode = 1022401;

		
		public readonly float WarpBackDistance = 1.5f;

		
		public LukeSkillActive3Data()
		{
			DamageBySkillLevel.Add(1, 60);
			DamageBySkillLevel.Add(2, 90);
			DamageBySkillLevel.Add(3, 120);
			DamageBySkillLevel.Add(4, 150);
			DamageBySkillLevel.Add(5, 180);
		}
	}
}