using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HartSkillActive4Data : Singleton<HartSkillActive4Data>
	{
		
		public readonly int BuffState = 1008501;

		
		public readonly int DebuffEffectAndSound;

		
		public readonly Dictionary<int, int> DecSpAmount = new Dictionary<int, int>();

		
		public readonly int HealEffectAndSound;

		
		public readonly Dictionary<int, int> HpAmount = new Dictionary<int, int>();

		
		public readonly Dictionary<int, float> HpCoef = new Dictionary<int, float>();

		
		public readonly int IntervalCount = 5;

		
		public readonly float IntervalTime = 1f;

		
		public readonly int MonsterDebuffState = 1008512;

		
		public readonly int PlayerDebuffState = 1008511;

		
		public readonly float SkillUpdateTime = 0.1f;

		
		public HartSkillActive4Data()
		{
			HpCoef.Add(1, 0.02f);
			HpCoef.Add(2, 0.03f);
			HpCoef.Add(3, 0.04f);
			HpAmount.Add(1, 30);
			HpAmount.Add(2, 40);
			HpAmount.Add(3, 50);
			DecSpAmount.Add(1, 80);
			DecSpAmount.Add(2, 180);
		}
	}
}