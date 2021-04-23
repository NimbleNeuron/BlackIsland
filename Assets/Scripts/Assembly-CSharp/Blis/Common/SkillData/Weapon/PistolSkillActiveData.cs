using System.Collections.Generic;

namespace Blis.Common
{
	
	public class PistolSkillActiveData : Singleton<PistolSkillActiveData>
	{
		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly float GunReloadTime = 1f;

		
		public readonly float SkillCooldownReduce = -6f;

		
		public PistolSkillActiveData()
		{
			BuffState.Add(1, 3009001);
			BuffState.Add(2, 3009002);
		}
	}
}