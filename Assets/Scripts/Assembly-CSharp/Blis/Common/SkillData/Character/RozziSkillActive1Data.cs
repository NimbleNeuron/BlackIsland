using System.Collections.Generic;

namespace Blis.Common
{
	
	public class RozziSkillActive1Data : Singleton<RozziSkillActive1Data>
	{
		
		public readonly float Aactive1CooldownReduce = -2f;

		
		public readonly float Active1DamageLength = 5f;

		
		public readonly int Active1HitEffectCode = 1021201;

		
		public readonly int Active1MoveSkillCode = 1021211;

		
		public readonly int Active1MoveStateCode = 1021201;

		
		public readonly float DamageApCoef = 0.35f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				30
			},
			{
				2,
				70
			},
			{
				3,
				110
			},
			{
				4,
				150
			},
			{
				5,
				190
			}
		};

		
		public readonly float MoveDistance = 2.75f;

		
		public readonly float MoveDuration = 0.25f;
	}
}