using System.Collections.Generic;

namespace Blis.Common
{
	
	public class YukiSkillActive2Data : Singleton<YukiSkillActive2Data>
	{
		
		public readonly float CooldownReduceActive2 = -0.5f;

		
		public readonly Dictionary<int, float> CooldownReduceActive3 = new Dictionary<int, float>();

		
		public readonly int DefenceBuffState = 1011311;

		
		public readonly float Duration = 1f;

		
		public YukiSkillActive2Data()
		{
			CooldownReduceActive3.Add(1, -6f);
			CooldownReduceActive3.Add(2, -6.5f);
			CooldownReduceActive3.Add(3, -7f);
			CooldownReduceActive3.Add(4, -7.5f);
			CooldownReduceActive3.Add(5, -8f);
		}
	}
}