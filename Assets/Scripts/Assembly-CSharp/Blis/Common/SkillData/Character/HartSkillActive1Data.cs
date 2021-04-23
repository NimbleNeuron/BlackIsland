using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HartSkillActive1Data : Singleton<HartSkillActive1Data>
	{
		
		public readonly int BuffState = 1008201;

		
		public readonly float ChargeDuration = 0.25f;

		
		public readonly int ChargeEnchantedEvolutionProjectileCode = 100814;

		
		public readonly int ChargeEnchantedProjectileCode = 100812;

		
		public readonly int ChargeEvolutionProjectileCode = 100810;

		
		public readonly int ChargeProjectileCode = 100808;

		
		public readonly Dictionary<int, int> DebuffMoveSpeedRatio = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public readonly int EnchantedEvolutionProjectileCode = 100813;

		
		public readonly int EnchantedProjectileCode = 100811;

		
		public readonly int EvolutionProjectileCode = 100809;

		
		public readonly float MaxSkillApCoef = 0.6f;

		
		public readonly Dictionary<int, int> MaxSkillDamage = new Dictionary<int, int>();

		
		public readonly float MinSkillApCoef = 0.3f;

		
		public readonly Dictionary<int, int> MinSkillDamage = new Dictionary<int, int>();

		
		public readonly int ProjectileCode = 100807;

		
		public HartSkillActive1Data()
		{
			MinSkillDamage.Add(1, 80);
			MinSkillDamage.Add(2, 100);
			MinSkillDamage.Add(3, 120);
			MinSkillDamage.Add(4, 140);
			MinSkillDamage.Add(5, 160);
			MaxSkillDamage.Add(1, 160);
			MaxSkillDamage.Add(2, 200);
			MaxSkillDamage.Add(3, 240);
			MaxSkillDamage.Add(4, 280);
			MaxSkillDamage.Add(5, 320);
			DebuffState.Add(1, 1008211);
			DebuffState.Add(2, 1008212);
			DebuffMoveSpeedRatio.Add(1, -30);
			DebuffMoveSpeedRatio.Add(2, -50);
		}
	}
}