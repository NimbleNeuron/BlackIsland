using System.Collections.Generic;

namespace Blis.Common
{
	
	public class IsolSkillActive2Data : Singleton<IsolSkillActive2Data>
	{
		
		public readonly Dictionary<int, int> Damage = new Dictionary<int, int>
		{
			{
				1,
				18
			},
			{
				2,
				27
			},
			{
				3,
				36
			},
			{
				4,
				45
			},
			{
				5,
				54
			}
		};

		
		public readonly int DamageCount = 4;

		
		public readonly float DamageTermTime = 0.66f;

		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>
		{
			{
				1,
				1009301
			},
			{
				2,
				1009302
			},
			{
				3,
				1009303
			},
			{
				4,
				1009304
			},
			{
				5,
				1009305
			}
		};

		
		public readonly int DurationTime = 2;

		
		public readonly int effectCode;

		
		public readonly float SkillApCoef = 0.5f;
	}
}