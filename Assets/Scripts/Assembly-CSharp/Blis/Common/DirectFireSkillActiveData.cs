using System.Collections.Generic;

namespace Blis.Common
{
	
	public class DirectFireSkillActiveData : Singleton<DirectFireSkillActiveData>
	{
		
		public readonly Dictionary<int, int> DamageByLevel_2 = new Dictionary<int, int>();

		
		public readonly float DamageReduce_2 = 0.3f;

		
		public readonly Dictionary<int, int> DebuffState_2 = new Dictionary<int, int>();

		
		public readonly int Duration = 6;

		
		public readonly int InnerCaltropCount = 4;

		
		public readonly int OuterCaltropCount = 8;

		
		public readonly int ProjectileCode = 78;

		
		public readonly int ProjectileCode_2 = 79;

		
		public readonly float SkillApCoef_2 = 0.3f;

		
		public DirectFireSkillActiveData()
		{
			DebuffState_2.Add(1, 3006001);
			DebuffState_2.Add(2, 3006002);
			DamageByLevel_2.Add(1, 110);
			DamageByLevel_2.Add(2, 180);
		}
	}
}