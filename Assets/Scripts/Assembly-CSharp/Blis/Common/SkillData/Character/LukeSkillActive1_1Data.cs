using System.Collections.Generic;

namespace Blis.Common
{
	
	public class LukeSkillActive1_1Data : Singleton<LukeSkillActive1_1Data>
	{
		
		public readonly Dictionary<int, int> BaseDamage = new Dictionary<int, int>();

		
		public readonly int BuffCode = 1022221;

		
		public readonly int BuffGroupCode = 1022220;

		
		public readonly float CheckSkillSlotTerm = 0.09f;

		
		public readonly float DamageApCoef = 0.5f;

		
		public readonly int DebuffCode = 1022231;

		
		public readonly int DebuffGroupCode = 1022230;

		
		public readonly float DebuffSightRange = 3f;

		
		public readonly int EvolutionProjectileCode = 102211;

		
		public readonly int ProjectileCode = 102210;

		
		public LukeSkillActive1_1Data()
		{
			BaseDamage.Add(1, 30);
			BaseDamage.Add(2, 50);
			BaseDamage.Add(3, 70);
			BaseDamage.Add(4, 90);
			BaseDamage.Add(5, 110);
		}
	}
}