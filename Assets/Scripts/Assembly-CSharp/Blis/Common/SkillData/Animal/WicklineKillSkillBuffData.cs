using System.Collections.Generic;

namespace Blis.Common
{
	
	public class WicklineKillSkillBuffData : Singleton<WicklineKillSkillBuffData>
	{
		
		public readonly Dictionary<int, int> DFS_Code = new Dictionary<int, int>
		{
			{
				15,
				5000115
			},
			{
				16,
				5000116
			},
			{
				17,
				5000117
			},
			{
				18,
				5000118
			},
			{
				19,
				5000119
			},
			{
				20,
				5000120
			},
			{
				21,
				5000121
			},
			{
				22,
				5000122
			},
			{
				23,
				5000123
			},
			{
				24,
				5000124
			},
			{
				25,
				5000125
			}
		};

		
		public readonly Dictionary<int, float> DFS_DamageApCoefByLevel = new Dictionary<int, float>
		{
			{
				15,
				0f
			},
			{
				16,
				0f
			},
			{
				17,
				0f
			},
			{
				18,
				0f
			},
			{
				19,
				0f
			},
			{
				20,
				0f
			},
			{
				21,
				0f
			},
			{
				22,
				0f
			},
			{
				23,
				0f
			},
			{
				24,
				0f
			},
			{
				25,
				0f
			}
		};

		
		public readonly Dictionary<int, int> DFS_DamageByLevel = new Dictionary<int, int>
		{
			{
				15,
				120
			},
			{
				16,
				140
			},
			{
				17,
				160
			},
			{
				18,
				180
			},
			{
				19,
				200
			},
			{
				20,
				220
			},
			{
				21,
				240
			},
			{
				22,
				260
			},
			{
				23,
				280
			},
			{
				24,
				300
			},
			{
				25,
				320
			}
		};

		
		public readonly string DFS_Effect_Target = "FX_BI_Common_Bleeding_Debuff";

		
		public readonly int DFS_EffectAndSound_Target = 1000001;

		
		public readonly int DFS_EffectCooltimeState = 5000051;

		
		public readonly int DFS_IntervalCount = 3;

		
		public readonly float DFS_IntervalTime = 1f;
	}
}