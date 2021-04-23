using System.Collections.Generic;

namespace Blis.Common
{
	
	public class IsolSkillNormalAttackData : Singleton<IsolSkillNormalAttackData>
	{
		
		public readonly float AssaultRifleApCoef = 0.5f;

		
		public readonly Dictionary<int, float> NormalAttackApCoef = new Dictionary<int, float>
		{
			{
				10,
				0.35f
			},
			{
				9,
				1f
			},
			{
				8,
				1f
			}
		};

		
		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>
		{
			{
				10,
				0.09f
			},
			{
				9,
				0.16f
			},
			{
				8,
				0.13f
			}
		};

		
		public readonly Dictionary<int, int> ProjectileCode = new Dictionary<int, int>
		{
			{
				10,
				100901
			},
			{
				9,
				100903
			},
			{
				8,
				100901
			}
		};
	}
}