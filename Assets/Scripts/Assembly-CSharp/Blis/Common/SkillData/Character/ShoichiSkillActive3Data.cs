using System.Collections.Generic;

namespace Blis.Common
{
	
	public class ShoichiSkillActive3Data : Singleton<ShoichiSkillActive3Data>
	{
		
		public readonly int DaggerCreateEffectProjectileCode = 101840;

		
		public readonly float DaggerCreateEffectProjectileDistance = 1f;

		
		public readonly Dictionary<int, int> DaggerDebuffStateCodes = new Dictionary<int, int>
		{
			{
				1,
				1018401
			},
			{
				2,
				1018402
			},
			{
				3,
				1018403
			},
			{
				4,
				1018404
			},
			{
				5,
				1018405
			}
		};

		
		public readonly int DaggerProjectile = 101830;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				35
			},
			{
				2,
				75
			},
			{
				3,
				115
			},
			{
				4,
				155
			},
			{
				5,
				195
			}
		};

		
		public readonly float MoveDistance = 2f;

		
		public readonly float MoveDuration = 0.35f;

		
		public readonly float ProjectileSightRange = 3f;

		
		public readonly float SkillApCoef = 0.3f;
	}
}