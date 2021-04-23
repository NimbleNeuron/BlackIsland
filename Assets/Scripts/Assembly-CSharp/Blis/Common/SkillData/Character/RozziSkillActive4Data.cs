using System.Collections.Generic;

namespace Blis.Common
{
	
	public class RozziSkillActive4Data : Singleton<RozziSkillActive4Data>
	{
		
		public readonly Dictionary<int, float> AdditionalDamageRatioByLevel = new Dictionary<int, float>
		{
			{
				1,
				4f
			},
			{
				2,
				8f
			},
			{
				3,
				12f
			}
		};

		
		public readonly int AttachAfterSkillDamageStack = 2;

		
		public readonly int AttachDebuffStackStateCode = 1021511;

		
		public readonly int AttachDebuffStackStateGroupId = 1021510;

		
		public readonly Dictionary<int, int> AttachDebuffStateCodeByLevel = new Dictionary<int, int>
		{
			{
				1,
				1021501
			},
			{
				2,
				1021502
			},
			{
				3,
				1021503
			}
		};

		
		public readonly int AttachDebuffStateGroupId = 1021500;

		
		public readonly int AttachFullStackBuffStateCode = 1021521;

		
		public readonly float BombTimerOnGround = 3f;

		
		public readonly float DamageActive4ApCoef = 0.45f;

		
		public readonly Dictionary<int, int> DamageActive4ByLevel = new Dictionary<int, int>
		{
			{
				1,
				100
			},
			{
				2,
				200
			},
			{
				3,
				300
			}
		};

		
		public readonly float FullStackCoolDownValue = 0.3f;

		
		public readonly int ProjectileCode = 102150;

		
		public readonly int SummonObjectCode = 1061;
	}
}