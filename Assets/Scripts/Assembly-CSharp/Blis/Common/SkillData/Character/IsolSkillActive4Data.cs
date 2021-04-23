using System.Collections.Generic;

namespace Blis.Common
{
	
	public class IsolSkillActive4Data : Singleton<IsolSkillActive4Data>
	{
		
		public readonly Dictionary<int, int> Damage = new Dictionary<int, int>
		{
			{
				1,
				100
			},
			{
				2,
				150
			},
			{
				3,
				200
			}
		};

		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>
		{
			{
				1,
				1009501
			},
			{
				2,
				1009502
			},
			{
				3,
				1009503
			}
		};

		
		public readonly int MokbomeTime = 40;

		
		public readonly Dictionary<int, float> SkillApCoef = new Dictionary<int, float>
		{
			{
				1,
				0.3f
			},
			{
				2,
				0.3f
			},
			{
				3,
				0.3f
			}
		};

		
		public readonly int SummonObjectCode = 1012;
	}
}