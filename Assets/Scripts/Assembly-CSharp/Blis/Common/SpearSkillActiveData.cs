using System.Collections.Generic;

namespace Blis.Common
{
	
	public class SpearSkillActiveData : Singleton<SpearSkillActiveData>
	{
		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public readonly int EffectAndSound = 1000000;

		
		public readonly float FirstRange = 2f;

		
		public readonly float knockBackThreshHold = 0.01f;

		
		public readonly float SecondRange = 4f;

		
		public readonly Dictionary<int, float> SkillApCoef = new Dictionary<int, float>();

		
		public SpearSkillActiveData()
		{
			SkillApCoef.Add(1, 1f);
			SkillApCoef.Add(2, 1.5f);
			DebuffState.Add(1, 3017001);
			DebuffState.Add(2, 3017002);
		}
	}
}