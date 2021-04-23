using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HighAngleFireSkillActiveData : Singleton<HighAngleFireSkillActiveData>
	{
		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public readonly int Duration = 5;

		
		public readonly int ProjectileCode = 77;

		
		public HighAngleFireSkillActiveData()
		{
			DebuffState.Add(1, 3005001);
			DebuffState.Add(2, 3005002);
		}
	}
}