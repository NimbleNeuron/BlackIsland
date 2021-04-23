using System.Collections.Generic;

namespace Blis.Common
{
	
	public class LukeSkillActive1_2Data : Singleton<LukeSkillActive1_2Data>
	{
		
		public readonly float AttackPossibleRange = 1.5f;

		
		public readonly Dictionary<int, int> BaseDamage = new Dictionary<int, int>();

		
		public readonly float DamageApCoef = 1f;

		
		public readonly int EffectSoundCode = 1022211;

		
		public readonly int EvolutionBuffEffectSoundCode = 1022221;

		
		public readonly float EvolutionBuffRatio = 0.8f;

		
		public readonly float MaxMoveDistance = 30f;

		
		public readonly float MoveToTagetSpeed = 13f;

		
		public readonly float WarpDistance = 0.4f;

		
		public LukeSkillActive1_2Data()
		{
			BaseDamage.Add(1, 50);
			BaseDamage.Add(2, 80);
			BaseDamage.Add(3, 110);
			BaseDamage.Add(4, 140);
			BaseDamage.Add(5, 170);
		}
	}
}