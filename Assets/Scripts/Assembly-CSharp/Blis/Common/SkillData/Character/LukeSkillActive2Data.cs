using System.Collections.Generic;

namespace Blis.Common
{
	
	public class LukeSkillActive2Data : Singleton<LukeSkillActive2Data>
	{
		
		public readonly int AddAttackBuffGroupCode = 1022310;

		
		public readonly int AddAttackBuffStackCode = 1022321;

		
		public readonly Dictionary<int, int> AddAttackBuffStateCodeByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> BaseDamage = new Dictionary<int, int>();

		
		public readonly float DamageApCoef = 0.2f;

		
		public readonly int EffectAddAttackSoundCode = 1022301;

		
		public readonly int EvolutionEffectAddAttackSoundCode = 1022302;

		
		public readonly float EvolutionSkillActive1CoolDown = 0.5f;

		
		public readonly int ReduceCCBuffCode = 1022301;

		
		public readonly Dictionary<int, float> ReductionCCBuffCodeByLevel = new Dictionary<int, float>();

		
		public LukeSkillActive2Data()
		{
			BaseDamage.Add(1, 10);
			BaseDamage.Add(2, 20);
			BaseDamage.Add(3, 30);
			BaseDamage.Add(4, 40);
			BaseDamage.Add(5, 50);
			AddAttackBuffStateCodeByLevel.Add(1, 1022311);
			AddAttackBuffStateCodeByLevel.Add(2, 1022312);
			AddAttackBuffStateCodeByLevel.Add(3, 1022313);
			AddAttackBuffStateCodeByLevel.Add(4, 1022314);
			AddAttackBuffStateCodeByLevel.Add(5, 1022315);
			ReductionCCBuffCodeByLevel.Add(1, 0.2f);
			ReductionCCBuffCodeByLevel.Add(2, 0.2f);
			ReductionCCBuffCodeByLevel.Add(3, 0.2f);
			ReductionCCBuffCodeByLevel.Add(4, 0.2f);
			ReductionCCBuffCodeByLevel.Add(5, 0.2f);
		}
	}
}