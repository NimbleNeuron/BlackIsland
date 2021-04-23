using System.Collections.Generic;

namespace Blis.Common
{
	
	public class CrossBowSkillActiveData : Singleton<CrossBowSkillActiveData>
	{
		
		public readonly Dictionary<int, float> KnockBackDistance = new Dictionary<int, float>();

		
		public readonly int ProjectileCode = 66;

		
		public readonly Dictionary<int, float> SkillApCoef = new Dictionary<int, float>();

		
		public readonly Dictionary<int, float> StunDuration = new Dictionary<int, float>();

		
		public CrossBowSkillActiveData()
		{
			SkillApCoef.Add(1, 0.6f);
			SkillApCoef.Add(2, 1f);
			KnockBackDistance.Add(1, 4.75f);
			KnockBackDistance.Add(2, 4.75f);
			StunDuration.Add(1, 1.5f);
			StunDuration.Add(2, 1.5f);
		}
	}
}