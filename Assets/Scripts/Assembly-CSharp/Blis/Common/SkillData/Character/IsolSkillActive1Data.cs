using System.Collections.Generic;

namespace Blis.Common
{
	
	public class IsolSkillActive1Data : Singleton<IsolSkillActive1Data>
	{
		
		public readonly Dictionary<int, int> AdditionalDamagePerHit = new Dictionary<int, int>
		{
			{
				1,
				8
			},
			{
				2,
				12
			},
			{
				3,
				16
			},
			{
				4,
				20
			},
			{
				5,
				24
			}
		};

		
		public readonly float AdditionalSkillApCoefPerHit = 0.3f;

		
		public readonly float AttachDetectTimer = 0.04f;

		
		public readonly int AttachState = 1009211;

		
		public readonly Dictionary<int, int> BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				50
			},
			{
				2,
				75
			},
			{
				3,
				100
			},
			{
				4,
				125
			},
			{
				5,
				150
			}
		};

		
		public readonly float BaseSkillApCoef = 0.5f;

		
		public readonly float BombTimerOnGround = 6f;

		
		public readonly int DebuffState = 1009201;

		
		public readonly float DebuffStateDurationIncreasePerStack = 0.1f;

		
		public readonly float DebuffStateMaxDuration = 1f;

		
		public readonly float DurationDecreasePerHit = 0.5f;

		
		public readonly int ProjectileCode = 100902;

		
		public readonly int SummonObjectCode = 1011;
	}
}