using System.Collections.Generic;

namespace Blis.Common
{
	
	public class IsolSkillActive3Data : Singleton<IsolSkillActive3Data>
	{
		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>
		{
			{
				1,
				1009401
			},
			{
				2,
				1009402
			},
			{
				3,
				1009403
			},
			{
				4,
				1009404
			},
			{
				5,
				1009405
			}
		};

		
		public readonly float DashDistance = 4f;

		
		public readonly float DashDuration = 0.46f;

		
		public readonly float StealthSpeedUpDelay;
	}
}