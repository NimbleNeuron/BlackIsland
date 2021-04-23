using System.Collections.Generic;

namespace Blis.Common
{
	
	public class IsolSkillPassiveData : Singleton<IsolSkillPassiveData>
	{
		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>
		{
			{
				1,
				1009101
			},
			{
				2,
				1009102
			},
			{
				3,
				1009103
			}
		};

		
		public readonly Dictionary<int, int> InstallTrapAdditionalStateEffect = new Dictionary<int, int>
		{
			{
				1,
				1009111
			},
			{
				2,
				1009112
			},
			{
				3,
				1009113
			}
		};

		
		public readonly Dictionary<int, float> InstallTrapAttackSpeed = new Dictionary<int, float>
		{
			{
				1,
				0.7f
			},
			{
				2,
				0.5f
			},
			{
				3,
				0.3f
			}
		};

		
		public readonly Dictionary<int, float> InstallTrapCastingTimeReduce = new Dictionary<int, float>
		{
			{
				1,
				0.3f
			},
			{
				2,
				0.4f
			},
			{
				3,
				0.5f
			}
		};

		
		public readonly Dictionary<int, float> InstallTrapCreateVisibleTime = new Dictionary<int, float>
		{
			{
				1,
				1f
			},
			{
				2,
				0.8f
			},
			{
				3,
				0.6f
			}
		};
	}
}