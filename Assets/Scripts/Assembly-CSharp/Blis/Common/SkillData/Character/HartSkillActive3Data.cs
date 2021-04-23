using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HartSkillActive3Data : Singleton<HartSkillActive3Data>
	{
		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly float DashDuration_1 = 0.23f;

		
		public readonly float DashDuration_2 = 0.23f;

		
		public readonly float DashDuration_3 = 0.33f;

		
		public readonly int EnchantedEvolutionProjectileCode_1 = 100824;

		
		public readonly int EnchantedEvolutionProjectileCode_2 = 100825;

		
		public readonly int EnchantedEvolutionProjectileCode_3 = 100826;

		
		public readonly int EnchantedProjectileCode_1 = 100818;

		
		public readonly int EnchantedProjectileCode_2 = 100819;

		
		public readonly int EnchantedProjectileCode_3 = 100820;

		
		public readonly int EvolutionProjectileCode_1 = 100821;

		
		public readonly int EvolutionProjectileCode_2 = 100822;

		
		public readonly int EvolutionProjectileCode_3 = 100823;

		
		public readonly int ProjectileCode_1 = 100815;

		
		public readonly int ProjectileCode_2 = 100816;

		
		public readonly int ProjectileCode_3 = 100817;

		
		public readonly float SkillApCoef = 0.4f;

		
		public readonly float SkillAttackRange = 4f;

		
		public HartSkillActive3Data()
		{
			DamageByLevel.Add(1, 20);
			DamageByLevel.Add(2, 30);
			DamageByLevel.Add(3, 40);
			DamageByLevel.Add(4, 50);
			DamageByLevel.Add(5, 60);
			BuffState.Add(1, 1008401);
			BuffState.Add(2, 1008411);
		}
	}
}